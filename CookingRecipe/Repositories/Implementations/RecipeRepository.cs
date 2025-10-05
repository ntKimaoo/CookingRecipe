using CookingRecipe.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipe.Repositories.Implementations;

public class RecipeRepository : IRecipeRepository
{
    private readonly CookingrecipeContext _context;

    public RecipeRepository(CookingrecipeContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Recipesteps)
            .Include(r => r.Favorites)
            .ToListAsync();
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Recipesteps)
            .Include(r => r.Favorites)
            .FirstOrDefaultAsync(r => r.RecipeId == id);
    }

    public async Task<IEnumerable<Recipe>> GetByAuthorIdAsync(int authorId)
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.Favorites)
            .Where(r => r.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<Recipe> CreateAsync(Recipe recipe)
    {
        recipe.CreatedAt = DateTime.UtcNow;
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe> UpdateAsync(Recipe recipe)
    {
        _context.Entry(recipe).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null) return false;

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Recipes.AnyAsync(r => r.RecipeId == id);
    }

    public async Task<IEnumerable<Recipe>> SearchAsync(string keyword)
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.Favorites)
            .Where(r => r.Title.Contains(keyword) ||
                       (r.Description != null && r.Description.Contains(keyword)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.RecipeCategories)
            .Include(r => r.Favorites)
            .Where(r => r.RecipeCategories.Any(rc => rc.CategoryId == categoryId))
            .ToListAsync();
    }
}

