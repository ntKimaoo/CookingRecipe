using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipe.Repositories.Implementations
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly CookingrecipeContext _context;

        public FavoriteRepository(CookingrecipeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorite>> GetAllFavoritesAsync()
        {
            return await _context.Favorites
                .Include(f => f.Recipe)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<Favorite?> GetFavoriteByIdAsync(int id)
        {
            return await _context.Favorites
                .Include(f => f.Recipe)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FavoriteId == id);
        }

        public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _context.Favorites
                .Include(f => f.Recipe)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<Favorite> AddFavoriteAsync(Favorite favorite)
        {
            favorite.CreatedAt = DateTime.Now;
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

        public async Task<bool> RemoveFavoriteAsync(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoriteExistsAsync(int userId, int recipeId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        }
    }
}
