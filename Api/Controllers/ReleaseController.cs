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
            await _releaseService.CreateAsync(release);
            return CreatedAtAction(nameof(GetReleases), new { id = release.Id }, release);
        }
    }
}
