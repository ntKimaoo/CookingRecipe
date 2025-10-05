using System.ComponentModel.DataAnnotations;

namespace CookingRecipe.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }   
    }

    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class UserUpdateDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string? FullName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;
    }
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public UserResponseDto User { get; set; } = null!;
    }
}
