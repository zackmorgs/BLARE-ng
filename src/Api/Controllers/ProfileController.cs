using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserService _userService;

        public ProfileController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Return public profile data (exclude sensitive info)
            var publicProfile = new
            {
                id = user.Id,
                username = user.Username,
                avatar = user.Avatar,
                role = user.Role,
                firstName = user.FirstName,
                lastName = user.LastName,
                bio = user.Bio,
                createdAt = user.CreatedAt
            };

            return Ok(publicProfile);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update profile fields
            var updated = await _userService.UpdateUserProfileAsync(userId, request);
            if (!updated)
            {
                return BadRequest(new { message = "Failed to update profile" });
            }

            // Get updated user and return
            var updatedUser = await _userService.GetUserByIdAsync(userId);
            var response = new
            {
                id = updatedUser.Id,
                username = updatedUser.Username,
                email = updatedUser.Email,
                avatar = updatedUser.Avatar,
                role = updatedUser.Role,
                firstName = updatedUser.FirstName,
                lastName = updatedUser.LastName,
                bio = updatedUser.Bio
            };

            return Ok(response);
        }
    }

    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? Avatar { get; set; }
    }
}
