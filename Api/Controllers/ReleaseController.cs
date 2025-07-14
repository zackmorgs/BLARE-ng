using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services; 

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReleaseController : ControllerBase
    {
        private readonly ReleaseService _releaseService;

        public ReleaseController(ReleaseService releaseService)
        {
            _releaseService = releaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReleases()
        {
            var releases = await _releaseService.GetAsync();
            return Ok(releases);
        }

        [HttpGet("artist/{id}")]
        public async Task<IActionResult> GetArtistReleases(string id)
        {
            var releases = await _releaseService.GetByArtistIdAsync(id);
            return Ok(releases);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRelease(Release release)
        {
            // Set timestamps
            release.CreatedAt = DateTime.UtcNow;
            release.UpdatedAt = DateTime.UtcNow;
            
            await _releaseService.CreateAsync(release);
            return CreatedAtAction(nameof(GetReleases), new { id = release.Id }, release);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRelease(string id)
        {
            var release = await _releaseService.GetByIdAsync(id);
            if (release == null)
            {
                return NotFound();
            }
            return Ok(release);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentReleases([FromQuery] int limit = 10)
        {
            var releases = await _releaseService.GetRecentAsync(limit);
            return Ok(releases);
        }
    }
}
