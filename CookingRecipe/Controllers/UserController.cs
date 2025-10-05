using CookingRecipe.DTOs;
using CookingRecipe.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CookingRecipe.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations such as registration, login, update, password change, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">Service for user operations.</param>
        /// <param name="config">Application configuration.</param>
        public UserController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        /// <summary>
        /// Gets all users. Admin only.
        /// </summary>
        /// <returns>A list of user response DTOs.</returns>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Gets a user by ID. Admin only.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>User response DTO if found; otherwise, NotFound.</returns>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="dto">User registration DTO.</param>
        /// <returns>Created user response DTO.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                var user = await _userService.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="dto">User login DTO.</param>
        /// <returns>JWT token if successful; otherwise, Unauthorized.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            var token = GenerateJwtToken(user.Username, user.RoleName);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(token);
        }

        /// <summary>
        /// Generates a JWT token for the specified username and role.
        /// </summary>
        /// <param name="username">Username for the token.</param>
        /// <param name="role">Role for the token.</param>
        /// <returns>JWT token string.</returns>
        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="dto">User update DTO.</param>
        /// <returns>Updated user response DTO if successful; otherwise, NotFound or BadRequest.</returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var user = await _userService.UpdateAsync(id, dto);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="dto">Change password DTO.</param>
        /// <returns>Ok if successful; otherwise, NotFound or BadRequest.</returns>
        [Authorize]
        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(id, dto);
                if (!result)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "Password changed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user by ID. Admin only.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>NoContent if successful; otherwise, NotFound.</returns>
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "User not found" });

            return NoContent();
        }
    }
}
