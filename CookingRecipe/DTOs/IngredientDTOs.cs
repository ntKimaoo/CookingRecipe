namespace CookingRecipe.DTOs
{
    public class CreateIngredientDto
    {
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
    }

    public class UpdateIngredientDto
    {
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
    }

    public class IngredientDto
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
