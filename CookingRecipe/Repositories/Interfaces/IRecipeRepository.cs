using CookingRecipe.Models;

namespace CookingRecipe.Repositories;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<IEnumerable<Recipe>> GetRecommendRecipeAsync();
    Task<Recipe?> GetByIdAsync(int id);
    Task<IEnumerable<Recipe>> GetByAuthorIdAsync(int authorId);
    Task<Recipe> CreateAsync(Recipe recipe);
    Task<Recipe> UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Recipe>> SearchAsync(string keyword);
    Task<IEnumerable<Recipe>> GetByCategoryAsync(int categoryId);
}