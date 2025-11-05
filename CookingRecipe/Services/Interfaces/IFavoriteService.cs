using static CookingRecipe.DTOs.Favorite;

namespace CookingRecipe.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteDto>> GetAllFavoritesAsync();
        Task<FavoriteDto?> GetFavoriteByIdAsync(int id);
        Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(int userId);
        Task<FavoriteDto> AddFavoriteAsync(CreateFavoriteDto dto);
        Task<bool> RemoveFavoriteAsync(int id);
        Task<bool> ToggleFavoriteAsync(int userId, int recipeId);
    }
}
