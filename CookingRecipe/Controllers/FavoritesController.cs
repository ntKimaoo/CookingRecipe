using CookingRecipe.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static CookingRecipe.DTOs.Favorite;

namespace CookingRecipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // GET: api/favorites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetAllFavorites()
        {
            var favorites = await _favoriteService.GetAllFavoritesAsync();
            return Ok(favorites);
        }

        // GET: api/favorites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FavoriteDto>> GetFavorite(int id)
        {
            var favorite = await _favoriteService.GetFavoriteByIdAsync(id);

            if (favorite == null)
                return NotFound(new { message = "Favorite not found" });

            return Ok(favorite);
        }

        // GET: api/favorites/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetUserFavorites(int userId)
        {
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }

        // POST: api/favorites
        [HttpPost]
        public async Task<ActionResult<FavoriteDto>> AddFavorite(CreateFavoriteDto dto)
        {
            try
            {
                var favorite = await _favoriteService.AddFavoriteAsync(dto);
                return CreatedAtAction(nameof(GetFavorite),
                    new { id = favorite.FavoriteId }, favorite);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/favorites/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveFavorite(int id)
        {
            var result = await _favoriteService.RemoveFavoriteAsync(id);

            if (!result)
                return NotFound(new { message = "Favorite not found" });

            return NoContent();
        }

        // POST: api/favorites/toggle
        [HttpPost("toggle")]
        public async Task<ActionResult<object>> ToggleFavorite(CreateFavoriteDto dto)
        {
            var isAdded = await _favoriteService.ToggleFavoriteAsync(dto.UserId, dto.RecipeId);
            return Ok(new
            {
                isFavorite = isAdded,
                message = isAdded ? "Added to favorites" : "Removed from favorites"
            });
        }
    }
}
