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
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

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

        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            var token = GenerateJwtToken(user.Username, user.RoleName);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(token);
        }
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
