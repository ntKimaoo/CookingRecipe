using CookingRecipe.Models;
using CookingRecipe.Repositories;
using CookingRecipe.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CookingRecipe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly AIService _aiService;
        private readonly IRecipeRepository _recipeRepo;

        public ChatbotController(AIService aiService, IRecipeRepository recipeRepo)
        {
            _aiService = aiService;
            _recipeRepo = recipeRepo;
        }

        [HttpPost("advice")]
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            try
            {
                var allRecipes = await _recipeRepo.GetAllAsync();

                var mentionedRecipes = FindMentionedRecipes(request.Message, allRecipes.ToList());

                var intent = ClassifyIntent(request.Message);

                string aiResponse;
                List<int> suggestedRecipeIds = new List<int>();

                switch (intent)
                {
                    case ChatIntent.Nutrition:
                        // 🥗 Phân tích dinh dưỡng
                        aiResponse = await _aiService.AnalyzeNutritionAsync(
                            request.Message,
                            mentionedRecipes.Any() ? mentionedRecipes : allRecipes.Take(5).ToList()
                        );
                        break;

                    case ChatIntent.Suggestion:
                        // 🍽️ Gợi ý món ăn - trả về IDs
                        var suggestionResult = await _aiService.SuggestRecipesWithIdsAsync(
                            request.Message,
                            mentionedRecipes,
                            allRecipes.ToList()
                        );
                        aiResponse = suggestionResult.Response;
                        suggestedRecipeIds = suggestionResult.RecipeIds;
                        break;

                    case ChatIntent.Cooking:
                        // 👨‍🍳 Hướng dẫn nấu ăn
                        aiResponse = await _aiService.GetCookingInstructionsAsync(
                            request.Message,
                            mentionedRecipes
                        );
                        break;

                    case ChatIntent.Ingredients:
                        // 🥕 Hỏi về nguyên liệu
                        aiResponse = await _aiService.GetIngredientsAdviceAsync(
                            request.Message,
                            mentionedRecipes
                        );
                        break;

                    case ChatIntent.Time:
                        // ⏰ Hỏi về thời gian
                        aiResponse = await _aiService.GetTimeEstimateAsync(
                            request.Message,
                            mentionedRecipes
                        );
                        break;

                    case ChatIntent.Difficulty:
                        // 📊 Hỏi về độ khó
                        aiResponse = await _aiService.GetDifficultyAdviceAsync(
                            request.Message,
                            mentionedRecipes
                        );
                        break;

                    case ChatIntent.Substitution:
                        // 🔄 Thay thế nguyên liệu
                        aiResponse = await _aiService.GetSubstitutionAdviceAsync(
                            request.Message,
                            mentionedRecipes
                        );
                        break;

                    case ChatIntent.MealPlanning:
                        // 📅 Lên thực đơn
                        aiResponse = await _aiService.CreateMealPlanAsync(
                            request.Message,
                            allRecipes.ToList()
                        );
                        break;

                    default:
                        // 💬 Trả lời chung
                        aiResponse = await _aiService.GetRecipeAdviceAsync(
                            request.Message,
                            mentionedRecipes.Any() ? mentionedRecipes : allRecipes.Take(10).ToList()
                        );
                        break;
                }

                return Ok(new ChatResponse
                {
                    Reply = aiResponse,
                    Intent = intent.ToString(),
                    MentionedRecipes = mentionedRecipes.Select(r => r.Title).ToList(),
                    SuggestedRecipeIds = suggestedRecipeIds,
                    Suggestions = ExtractSuggestions(aiResponse)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private List<Recipe> FindMentionedRecipes(string message, List<Recipe> allRecipes)
        {
            var mentioned = new List<Recipe>();
            var messageLower = RemoveDiacritics(message.ToLower());

            foreach (var recipe in allRecipes)
            {
                var titleLower = RemoveDiacritics(recipe.Title.ToLower());

                // Tìm chính xác hoặc tương tự
                if (messageLower.Contains(titleLower) ||
                    titleLower.Contains(messageLower) ||
                    LevenshteinDistance(messageLower, titleLower) < 3)
                {
                    mentioned.Add(recipe);
                }
            }

            return mentioned;
        }

        private ChatIntent ClassifyIntent(string message)
        {
            var msgLower = message.ToLower();

            // Nutrition
            if (Regex.IsMatch(msgLower, @"(dinh dưỡng|calo|protein|chất béo|carb|vitamin|khoáng chất|béo|đường)"))
                return ChatIntent.Nutrition;

            // Suggestion
            if (Regex.IsMatch(msgLower, @"(gợi ý|đề xuất|thêm món|món gì|nên ăn|món khác|món nào|giới thiệu)"))
                return ChatIntent.Suggestion;

            // Cooking instructions
            if (Regex.IsMatch(msgLower, @"(nấu|làm|chế biến|hướng dẫn|cách làm|bước|quy trình|công thức)"))
                return ChatIntent.Cooking;

            // Ingredients
            if (Regex.IsMatch(msgLower, @"(nguyên liệu|thành phần|cần gì|mua gì|nguyên vật liệu)"))
                return ChatIntent.Ingredients;

            // Time
            if (Regex.IsMatch(msgLower, @"(mất bao lâu|thời gian|nhanh|lâu|phút|giờ)"))
                return ChatIntent.Time;

            // Difficulty
            if (Regex.IsMatch(msgLower, @"(khó|dễ|phức tạp|đơn giản|độ khó|người mới)"))
                return ChatIntent.Difficulty;

            // Substitution
            if (Regex.IsMatch(msgLower, @"(thay thế|thay|không có|hết|khác|thay đổi)"))
                return ChatIntent.Substitution;

            // Meal planning
            if (Regex.IsMatch(msgLower, @"(thực đơn|kế hoạch|tuần|ngày|bữa|lên món)"))
                return ChatIntent.MealPlanning;

            return ChatIntent.General;
        }

        private List<string> ExtractSuggestions(string aiResponse)
        {
            return aiResponse
                .Split('\n')
                .Where(line => line.TrimStart().StartsWith("-") ||
                              line.TrimStart().StartsWith("•") ||
                              line.TrimStart().StartsWith("*") ||
                              Regex.IsMatch(line.TrimStart(), @"^\d+\."))
                .Select(line => Regex.Replace(line.TrimStart(), @"^[-•*\d\.]+\s*", ""))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        private string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var result = new System.Text.StringBuilder();

            foreach (var c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }
    }

    public enum ChatIntent
    {
        General,
        Nutrition,
        Suggestion,
        Cooking,
        Ingredients,
        Time,
        Difficulty,
        Substitution,
        MealPlanning
    }
}