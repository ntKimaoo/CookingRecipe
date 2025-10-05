using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipe.Repositories.Implementations
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly CookingrecipeContext _context;
        private readonly ILogger<IngredientRepository> _logger;

        public IngredientRepository(CookingrecipeContext context, ILogger<IngredientRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            return await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                .ToListAsync();
        }

        public async Task<Ingredient?> GetByIdAsync(int id)
        {
            return await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                .FirstOrDefaultAsync(i => i.IngredientId == id);
        }

        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            ingredient.CreatedAt = DateTime.UtcNow;
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient?> UpdateAsync(int id, Ingredient ingredient)
        {
            var existingIngredient = await _context.Ingredients.FindAsync(id);
            if (existingIngredient == null)
                return null;

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Type = ingredient.Type;

            await _context.SaveChangesAsync();
            return existingIngredient;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Ingredients.AnyAsync(i => i.IngredientId == id);
        }
    }
}
