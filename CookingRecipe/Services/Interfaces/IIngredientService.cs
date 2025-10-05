using CookingRecipe.DTOs;

namespace CookingRecipe.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync();
        Task<IngredientDto?> GetIngredientByIdAsync(int id);
        Task<IngredientDto> CreateIngredientAsync(CreateIngredientDto dto);
        Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientDto dto);
        Task<bool> DeleteIngredientAsync(int id);
    }
}
