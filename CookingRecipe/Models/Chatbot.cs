namespace CookingRecipe.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;

        // Optional: Context từ conversation trước
        public List<ChatMessage>? ConversationHistory { get; set; }
    }

    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;

        // Intent được nhận diện
        public string Intent { get; set; } = "General";

        // Các món được đề cập trong câu hỏi
        public List<string> MentionedRecipes { get; set; } = new();

        // IDs của món được gợi ý (nếu là suggestion intent)
        public List<int> SuggestedRecipeIds { get; set; } = new();

        // Các gợi ý nhanh cho user
        public List<string> Suggestions { get; set; } = new();

        // Quick reply buttons (optional)
        public List<QuickReply>? QuickReplies { get; set; }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = "user"; // "user" or "assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class QuickReply
    {
        public string Label { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // "suggest", "nutrition", etc.
    }

    public class SuggestionResult
    {
        public string Response { get; set; } = string.Empty;
        public List<int> RecipeIds { get; set; } = new();
    }
}