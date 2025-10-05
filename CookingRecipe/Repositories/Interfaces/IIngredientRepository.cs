using CookingRecipe.Models;

namespace CookingRecipe.Repositories.Interfaces
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(int id);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
        Task<Ingredient?> UpdateAsync(int id, Ingredient ingredient);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
