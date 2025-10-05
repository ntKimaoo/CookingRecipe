using CookingRecipe.DTOs;

namespace CookingRecipe.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetByIdAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<UserResponseDto?> LoginAsync(UserLoginDto dto);
        Task<UserResponseDto?> UpdateAsync(int userId, UserUpdateDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<bool> DeleteAsync(int userId);
    }
}
