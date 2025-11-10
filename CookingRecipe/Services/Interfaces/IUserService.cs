using CookingRecipe.DTOs;
using CookingRecipe.Models;

namespace CookingRecipe.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync(string username);
        Task<UserResponseDto?> GetByIdAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<UserResponseDto?> LoginAsync(UserLoginDto dto);
        Task<UserResponseDto?> UpdateAsync(int userId, UserUpdateDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<bool> DeleteAsync(int userId);
    }
}
