using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Bson;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class UploadController : ControllerBase
    {
        private readonly TrackService _trackService;

        public UploadController(TrackService trackService)
        {
            _trackService = trackService;
        }

        
     
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(string id)
        {
            try
            {
                var track = await _trackService.GetByIdAsync(id);
                if (track == null)
                    return NotFound($"Track with ID {id} not found");

                await _trackService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
