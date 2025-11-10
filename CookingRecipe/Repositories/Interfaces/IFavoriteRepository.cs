using CookingRecipe.Models;

namespace CookingRecipe.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Favorite>> GetAllFavoritesAsync();
        Task<Favorite?> GetFavoriteByIdAsync(int id);
        Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId);
        Task<Favorite> AddFavoriteAsync(Favorite favorite);
        Task<bool> RemoveFavoriteAsync(int id);
        Task<bool> IsFavoriteExistsAsync(int userId, int recipeId);
    }
}
