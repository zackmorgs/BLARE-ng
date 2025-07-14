using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly UserService _userService;

        public ArtistController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("updateUserRole")]
        public async Task<IActionResult> UpdateUserRoleAsync(string userId, string newRole)
        {
            var result = await _userService.UpdateUserRoleAsync(userId, newRole);
            if (result)
            {
                return Ok("User role updated successfully.");
            }
            return BadRequest("Failed to update user role.");
        }
    }
}
