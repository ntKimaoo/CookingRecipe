using CookingRecipe.Models;
using static CookingRecipe.DTOs.RecipeDTOs;

namespace CookingRecipe.Services.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListDto>> GetAllRecipesAsync();
    Task<IEnumerable<RecipeListDto>> GetRecommendRecipeAsync();
    Task<RecipeDto?> GetRecipeByIdAsync(int id);
    Task<IEnumerable<RecipeListDto>> GetRecipesByAuthorAsync(int authorId);
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto);
    Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto dto);
    Task<bool> DeleteRecipeAsync(int id);
    Task<IEnumerable<RecipeListDto>> SearchRecipesAsync(string keyword);
    Task<IEnumerable<RecipeListDto>> GetRecipesByCategoryAsync(int categoryId);
}