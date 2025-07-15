using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints with JWT
    public class TrackController : ControllerBase
    {
        private readonly TrackService _trackService;

        public TrackController(TrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadTrack([FromForm] IFormFile file, [FromForm] string title, [FromForm] string artist)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided");

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(artist))
                    return BadRequest("Title and artist are required");

                // Create track metadata
                var trackMetadata = new Track
                {
                    Title = title,
                    Artist = artist,
                    Duration = TimeSpan.Zero
                };

                // Upload the track
                var uploadedTrack = await _trackService.UploadTrackAsync(file, trackMetadata);

                return Ok(new
                {
                    id = uploadedTrack.Id.ToString(),
                    title = uploadedTrack.Title,
                    artist = uploadedTrack.Artist,
                    fileUrl = uploadedTrack.FileUrl,
                    uploadedAt = uploadedTrack.UploadedAt
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTracks()
        {
            try
            {
                var tracks = await _trackService.GetAsync();
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
