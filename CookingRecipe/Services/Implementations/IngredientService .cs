using CookingRecipe.DTOs;
using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using CookingRecipe.Services.Interfaces;

namespace CookingRecipe.Services.Implementations
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;

        public IngredientService(IIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync()
        {
            var ingredients = await _repository.GetAllAsync();
            return ingredients.Select(MapToDto);
        }

        public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
        {
            var ingredient = await _repository.GetByIdAsync(id);
            return ingredient == null ? null : MapToDto(ingredient);
        }

        public async Task<IngredientDto> CreateIngredientAsync(CreateIngredientDto dto)
        {
            var ingredient = new Ingredient
            {
                Name = dto.Name,
                Type = dto.Type
            };

            var created = await _repository.CreateAsync(ingredient);
            return MapToDto(created);
        }

        public async Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientDto dto)
        {
            var ingredient = new Ingredient
            {
                Name = dto.Name,
                Type = dto.Type
            };

            var updated = await _repository.UpdateAsync(id, ingredient);
            return updated == null ? null : MapToDto(updated);
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // Manual mapping method
        private IngredientDto MapToDto(Ingredient ingredient)
        {
            return new IngredientDto
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Type = ingredient.Type,
                CreatedAt = ingredient.CreatedAt
            };
        }
    }
}

