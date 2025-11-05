using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using CookingRecipe.Services.Interfaces;
using static CookingRecipe.DTOs.Favorite;

namespace CookingRecipe.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<IEnumerable<FavoriteDto>> GetAllFavoritesAsync()
        {
            var favorites = await _favoriteRepository.GetAllFavoritesAsync();
            return favorites.Select(f => MapToDto(f));
        }

        public async Task<FavoriteDto?> GetFavoriteByIdAsync(int id)
        {
            var favorite = await _favoriteRepository.GetFavoriteByIdAsync(id);
            return favorite != null ? MapToDto(favorite) : null;
        }

        public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(int userId)
        {
            var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
            return favorites.Select(f => MapToDto(f));
        }

        public async Task<FavoriteDto> AddFavoriteAsync(CreateFavoriteDto dto)
        {
            // Kiểm tra đã tồn tại chưa
            if (await _favoriteRepository.IsFavoriteExistsAsync(dto.UserId, dto.RecipeId))
            {
                throw new InvalidOperationException("Recipe already in favorites");
            }

            var favorite = new Favorite
            {
                UserId = dto.UserId,
                RecipeId = dto.RecipeId
            };

            var result = await _favoriteRepository.AddFavoriteAsync(favorite);
            var addedFavorite = await _favoriteRepository.GetFavoriteByIdAsync(result.FavoriteId);
            return MapToDto(addedFavorite!);
        }

        public async Task<bool> RemoveFavoriteAsync(int id)
        {
            return await _favoriteRepository.RemoveFavoriteAsync(id);
        }

        public async Task<bool> ToggleFavoriteAsync(int userId, int recipeId)
        {
            var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
            var existing = favorites.FirstOrDefault(f => f.RecipeId == recipeId);

            if (existing != null)
            {
                await _favoriteRepository.RemoveFavoriteAsync(existing.FavoriteId);
                return false;
            }
            else
            {
                await AddFavoriteAsync(new CreateFavoriteDto
                {
                    UserId = userId,
                    RecipeId = recipeId
                });
                return true;
            }
        }

        private FavoriteDto MapToDto(Favorite favorite)
        {
            return new FavoriteDto
            {
                FavoriteId = favorite.FavoriteId,
                UserId = favorite.UserId,
                RecipeId = favorite.RecipeId,
                CreatedAt = favorite.CreatedAt,
                RecipeName = favorite.Recipe?.Title,
                UserName = favorite.User?.Username
            };
        }
    }
}
