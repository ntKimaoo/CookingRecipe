using CookingRecipe.DTOs;
using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using CookingRecipe.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CookingRecipe.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        public async Task<UserResponseDto?> GetByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }
        
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }
        public async Task<User?> GetCurrentUserAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }
        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
        {
            // Kiểm tra username đã tồn tại
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                throw new InvalidOperationException("Username already exists");

            // Kiểm tra email đã tồn tại
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email already exists");
            var role = await _roleRepository.GetByNameAsync("User");
            if (role == null)
                throw new InvalidOperationException("Default role 'User' not found");
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = HashPassword(dto.Password),
                RoleId = role.RoleId,
                CreatedAt = DateTime.Now
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return MapToDto(createdUser);
        }

        public async Task<UserResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                return null;

            return MapToDto(user);
        }
        
        public async Task<UserResponseDto?> UpdateAsync(int userId, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // Kiểm tra email mới nếu có thay đổi
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                if (await _userRepository.EmailExistsAsync(dto.Email))
                    throw new InvalidOperationException("Email already exists");

                user.Email = dto.Email;
            }

            if (dto.FullName != null)
                user.FullName = dto.FullName;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect");

            user.PasswordHash = HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }

        private UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                RoleName = user.Role?.RoleName ?? "Unknown",
                CreatedAt = user.CreatedAt
            };
        }

        private string HashPassword(string password)
        {
            // Sử dụng BCrypt hoặc ASP.NET Core Identity
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
