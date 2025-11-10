using System.ComponentModel.DataAnnotations;

namespace CookingRecipe.DTOs
{
    public class RecipeDTOs
    {
        public class CreateRecipeDto
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(200)]
            public string Title { get; set; } = null!;

            [StringLength(1000)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Instructions are required")]
            public string Instructions { get; set; } = null!;

            [Range(0, 1440, ErrorMessage = "Prep time must be between 0 and 1440 minutes")]
            public int? PrepTime { get; set; }

            [Range(0, 1440, ErrorMessage = "Cook time must be between 0 and 1440 minutes")]
            public int? CookTime { get; set; }

            [RegularExpression("^(Easy|Medium|Hard)$", ErrorMessage = "Difficulty must be Easy, Medium, or Hard")]
            public string? Difficulty { get; set; }
            
            public int AuthorId { get; set; }
            public string? Thumbnail { get; set; }

            public List<CreateRecipeIngredientDto>? Ingredients { get; set; }
            public List<CreateRecipeStepDto>? Steps { get; set; }
            public List<int>? CategoryIds { get; set; }
        }

        // DTO cho việc cập nhật Recipe
        public class UpdateRecipeDto
        {
            [StringLength(200)]
            public string? Title { get; set; }

            [StringLength(1000)]
            public string? Description { get; set; }

            public string? Instructions { get; set; }

            [Range(0, 1440)]
            public int? PrepTime { get; set; }

            [Range(0, 1440)]
            public int? CookTime { get; set; }
            public string? Thumbnail { get; set; }

            [RegularExpression("^(easy|medium|hard)$")]
            public string? Difficulty { get; set; }
            public List<CreateRecipeIngredientDto>? Ingredients { get; set; }
            public List<CreateRecipeStepDto>? Steps { get; set; }
            public List<int>? CategoryIds { get; set; }
        }

        // DTO cho việc đọc Recipe (response)
        public class RecipeDto
        {
            public int RecipeId { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public string? Thumbnail { get; set; }
            public string Instructions { get; set; } = null!;
            public int? PrepTime { get; set; }
            public int? CookTime { get; set; }
            public int? TotalTime => (PrepTime ?? 0) + (CookTime ?? 0);
            public string? Difficulty { get; set; }
            public DateTime? CreatedAt { get; set; }

            public AuthorDto Author { get; set; } = null!;
            public List<RecipeIngredientDto> Ingredients { get; set; } = new();
            public List<RecipeStepDto> Steps { get; set; } = new();
            public List<CategoryDto> Categories { get; set; } = new();
            public int FavoriteCount { get; set; }
        }

        // DTO cho Recipe đơn giản (dùng trong list)
        public class RecipeListDto
        {
            public int RecipeId { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public string? Thumbnail { get; set; }
            public int? TotalTime { get; set; }
            public string? Difficulty { get; set; }
            public string AuthorName { get; set; } = null!;
            public DateTime? CreatedAt { get; set; }
            public int FavoriteCount { get; set; }
        }

        // DTO phụ trợ
        public class AuthorDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = null!;
            public string? Email { get; set; }
        }

        public class RecipeIngredientDto
        {
            public int IngredientId { get; set; }
            public string Name { get; set; } = null!;
            public string Quantity { get; set; } = null!;
        }

        public class CreateRecipeIngredientDto
        {
            public int IngredientId { get; set; }
            public string Quantity { get; set; } = null!;
        }

        public class RecipeStepDto
        {
            public int StepNumber { get; set; }
            public string Content { get; set; } = null!;
            public string? ImageUrl { get; set; }
            public string? VideoUrl { get; set; }
            public int? Duration { get; set; }
        }

        public class CreateRecipeStepDto
        {
            public int StepNumber { get; set; }
            public string Content { get; set; } = null!;
            public string? ImageUrl { get; set; }
            public string? VideoUrl { get; set; }
            public int? Duration { get; set; }
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string Name { get; set; } = null!;
        }
    }
}
