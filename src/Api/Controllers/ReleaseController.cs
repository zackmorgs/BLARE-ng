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
            var releases = await _releaseService.GetReleasesByArtistIdAsync(id);
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

        [HttpPut("upload")]
        public async Task<IActionResult> UploadRelease(
            [FromForm] string title,
            [FromForm] string type,
            [FromForm] string artistId,
            [FromForm] string description,
            [FromForm] string musicTags,
            [FromForm] DateTime releaseDate,
            [FromForm] IFormFile coverImage,
            [FromForm] IFormFile[] releaseFiles
        )
        {
            var uploadedRelease = new Release();
            try
            {
                if (coverImage == null || coverImage.Length == 0)
                {
                    return BadRequest("Cover image is required.");
                }
                if (releaseFiles == null || releaseFiles.Length == 0)
                {
                    return BadRequest("At least one release file is required.");
                }
                uploadedRelease.Title = title;
                uploadedRelease.Type = type;
                uploadedRelease.ArtistId = artistId;
                uploadedRelease.Description = description;
                uploadedRelease.MusicTags = musicTags?
                    .Split(',')
                    .Select(static tag => tag.Trim())
                    .ToList() ?? new List<string>();
                uploadedRelease.ReleaseDate = releaseDate;
                uploadedRelease.CreatedAt = DateTime.UtcNow;
                uploadedRelease.UpdatedAt = DateTime.UtcNow;
                // Create the release with files
                uploadedRelease = await _releaseService.CreateReleaseAsync(
                    uploadedRelease,
                    coverImage,
                    releaseFiles
                );
                return Ok(uploadedRelease);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading release: {ex.Message}");
            }
        }
    }
}
