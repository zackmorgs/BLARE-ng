using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(
            UserService userService,
            JwtService jwtService,
            SlugService slugService
        )
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Check if user already exists
            var existingUser = await _userService.GetUserByUsernameAsync(request.Username);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            var existingEmail = await _userService.GetUserByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }
            // Validate role
            if (string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { message = "Invalid role specified" });
            }

            if (request.Role == "artist" && string.IsNullOrWhiteSpace(request.ArtistName))
            {
                return BadRequest(new { message = "Artist name is required for artist role" });
            }

            // Create new user
            var user = new User();

            if (request.Role == "artist")
            {
                user = await _userService.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.Role,
                    request.ArtistName
                );
            }
            else
            {
                user = await _userService.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.Role,
                    ""
                );
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(
                new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role,
                    },
                }
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(user);
            return Ok(
                new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role,
                        user.ArtistName,
                    },
                }
            );
        }

        [Authorize]
        [HttpPut("update/{username}")]
        public async Task<IActionResult> UpdateAccount(string username, [FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Invalid user data" });
            }

            var updated = await _userService.UpdateUserInfoAsync(user);
            if (!updated)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = "User updated successfully" });
        }
    }

    public class RegisterRequest
    {
        public string ArtistName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
