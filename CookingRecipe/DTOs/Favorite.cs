namespace CookingRecipe.DTOs
{
    public class Favorite
    {
        public class FavoriteDto
        {
            public int FavoriteId { get; set; }
            public int UserId { get; set; }
            public int RecipeId { get; set; }
            public DateTime? CreatedAt { get; set; }
            public string? RecipeName { get; set; }
            public string? UserName { get; set; }
        }

        public class CreateFavoriteDto
        {
            public int UserId { get; set; }
            public int RecipeId { get; set; }
        }

    }
}
