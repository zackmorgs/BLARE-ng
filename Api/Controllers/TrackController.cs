using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;

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

        // Define your actions here
        [HttpGet]
        public async Task<IActionResult> GetTracks()
        {
            var tracks = await _trackService.GetAsync();
            return Ok(tracks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrack([FromBody] Track track)
        {
            await _trackService.CreateAsync(track);
            return CreatedAtAction(nameof(GetTracks), new { id = track.Id }, track);
        }
    }
}
