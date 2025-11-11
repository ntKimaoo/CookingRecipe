using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CookingRecipe.Models;

namespace CookingRecipe.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public async Task<string> GetRecipeAdviceAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = FormatRecipeList(recipes);

            var prompt = $@"
Bạn là trợ lý nấu ăn thông minh và thân thiện.
Người dùng hỏi: ""{userMessage}""

{(recipes.Any() ? $"Các món được đề cập:\n{recipeList}" : "Không có món cụ thể được đề cập.")}

Hãy trả lời ngắn gọn trong vài dòng, thân thiện bằng tiếng Việt. Tập trung vào câu hỏi của người dùng.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<string> AnalyzeNutritionAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = FormatRecipeList(recipes);

            var prompt = $@"
Bạn là chuyên gia dinh dưỡng.
Người dùng hỏi: ""{userMessage}""

Các món cần phân tích:
{recipeList}

Hãy phân tích:
1. Giá trị dinh dưỡng tổng quan (ước tính calo, protein, carb, chất béo)
2. Ưu điểm và hạn chế về dinh dưỡng
3. Gợi ý cân bằng nếu cần
4. Phù hợp với ai (người ăn kiêng, tăng cân, giảm cân, người tập thể hình...)

Trả lời bằng tiếng Việt, ngắn gọn, dễ hiểu, không quá chuyên môn.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<SuggestionResult> SuggestRecipesWithIdsAsync(
            string userMessage,
            List<Recipe> currentRecipes,
            List<Recipe> allRecipes)
        {
            var current = currentRecipes.Any()
                ? string.Join("\n", currentRecipes.Select(r => $"- {r.Title} (ID: {r.RecipeId})"))
                : "Chưa có món nào được chọn";

            var available = string.Join("\n", allRecipes
                .Where(r => !currentRecipes.Any(c => c.RecipeId == r.RecipeId))
                .Take(20)
                .Select(r => $"- {r.Title} (ID: {r.RecipeId}) - {r.Difficulty} - {r.Description}"));

            var prompt = $@"
Bạn là chuyên gia gợi ý món ăn.
Người dùng hỏi: ""{userMessage}""

Món hiện tại:
{current}

Món có thể chọn thêm:
{available}

Hãy đề xuất 3-5 món phù hợp nhất, mỗi món phải có:
- Tên món
- ID (số trong ngoặc)
- Lý do chọn (ngắn gọn)

Format: ""- [Tên món] (ID: XX) - [Lý do]""
Trả lời bằng tiếng Việt.";

            var response = await SendPromptToGemini(prompt);
            var recipeIds = ExtractRecipeIds(response);

            return new SuggestionResult
            {
                Response = response,
                RecipeIds = recipeIds
            };
        }

        public async Task<string> GetCookingInstructionsAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = FormatRecipeList(recipes);

            var prompt = $@"
Bạn là đầu bếp chuyên nghiệp.
Người dùng hỏi: ""{userMessage}""

Món cần hướng dẫn:
{recipeList}

Hãy cung cấp:
1. Các bước nấu chi tiết, rõ ràng
2. Mẹo để món ngon hơn
3. Lưu ý quan trọng (nhiệt độ, thời gian, kỹ thuật)
4. Cách biết món đã chín/đạt

Trả lời bằng tiếng Việt ngắn gọn, dễ hiểu cho người mới.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<string> GetIngredientsAdviceAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = FormatRecipeList(recipes);

            var prompt = $@"
Bạn là chuyên gia về nguyên liệu.
Người dùng hỏi: ""{userMessage}""

Món liên quan:
{recipeList}

Hãy tư vấn:
1. Danh sách nguyên liệu cần mua
2. Số lượng ước tính cho X người
3. Mẹo chọn nguyên liệu tươi ngon
4. Nơi mua và giá tham khảo (nếu biết)

Trả lời bằng tiếng Việt ngắn gọn, thực tế.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<string> GetTimeEstimateAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = recipes.Any()
                ? string.Join("\n", recipes.Select(r =>
                    $"- {r.Title}: Chuẩn bị {r.PrepTime ?? 0}p, Nấu {r.CookTime ?? 0}p"))
                : "Không có món cụ thể";

            var prompt = $@"
Bạn là chuyên gia về thời gian nấu nướng.
Người dùng hỏi: ""{userMessage}""

Thông tin món:
{recipeList}

Hãy:
1. Ước tính tổng thời gian thực tế (kể cả nghỉ, ủ...)
2. Chia nhỏ từng giai đoạn
3. Mẹo tiết kiệm thời gian
4. Gợi ý món nhanh hơn nếu người dùng vội

Trả lời bằng tiếng Việt ngắn gọn.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<string> GetDifficultyAdviceAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = FormatRecipeList(recipes);

            var prompt = $@"
Bạn là chuyên gia đào tạo nấu ăn.
Người dùng hỏi: ""{userMessage}""

Các món:
{recipeList}

Hãy đánh giá:
1. Độ khó thực tế (Dễ/Trung bình/Khó)
2. Kỹ năng cần có
3. Phù hợp với người mới không
4. Món thay thế đơn giản hơn (nếu cần)

Trả lời bằng tiếng Việt ngắn gọn, khích lệ người mới.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<string> GetSubstitutionAdviceAsync(string userMessage, List<Recipe> recipes)
        {
            var recipeList = FormatRecipeList(recipes);

            var prompt = $@"
Bạn là chuyên gia thay thế nguyên liệu.
Người dùng hỏi: ""{userMessage}""

Món liên quan:
{recipeList}

Hãy gợi ý:
1. Nguyên liệu có thể thay thế
2. Tỷ lệ thay thế
3. Ảnh hưởng đến hương vị
4. Phương án tốt nhất

Trả lời bằng tiếng Việt ngắn gọn, linh hoạt.";

            return await SendPromptToGemini(prompt);
        }

        public async Task<string> CreateMealPlanAsync(string userMessage, List<Recipe> allRecipes)
        {
            var available = string.Join("\n", allRecipes.Take(30).Select(r =>
                $"- {r.Title} (ID: {r.RecipeId}) - {r.Difficulty}"));

            var prompt = $@"
Bạn là chuyên gia dinh dưỡng và lập thực đơn.
Người dùng muốn: ""{userMessage}""

Các món có sẵn:
{available}

Hãy tạo thực đơn:
1. Phân bổ món theo bữa sáng/trưa/tối
2. Cân đối dinh dưỡng
3. Đa dạng món ăn
4. Kèm ID món để dễ tra cứu

Format: ""**[Bữa]**: [Tên món] (ID: XX) - [Lý do]""
Trả lời bằng tiếng Việt ngắng gọn.";

            return await SendPromptToGemini(prompt);
        }

        private string FormatRecipeList(List<Recipe> recipes)
        {
            if (!recipes.Any())
                return "Không có món nào được chọn";

            return string.Join("\n", recipes.Select(r =>
                $"- {r.Title} (ID: {r.RecipeId}, {r.Difficulty ?? "Không rõ"}): " +
                $"Chuẩn bị {r.PrepTime ?? 0}p, nấu {r.CookTime ?? 0}p. " +
                $"{r.Description ?? "Không có mô tả"}"));
        }

        private List<int> ExtractRecipeIds(string text)
        {
            var ids = new List<int>();
            var matches = Regex.Matches(text, @"ID:\s*(\d+)");

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int id))
                {
                    ids.Add(id);
                }
            }

            return ids.Distinct().ToList();
        }

        private async Task<string> SendPromptToGemini(string prompt)
        {
            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("Gemini API key chưa được cấu hình.");

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = prompt } }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 1000
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={_apiKey}",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error: {response.StatusCode} - {error}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);

            return jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "Không có phản hồi từ Gemini.";
        }
    }
}