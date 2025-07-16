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
            // Console.WriteLine("=== UPLOAD REQUEST RECEIVED ===");
            // Console.WriteLine($"Title: {title}");
            // Console.WriteLine($"Type: {type}");
            // Console.WriteLine($"ArtistId: {artistId}");
            // Console.WriteLine($"Cover image: {coverImage?.FileName} ({coverImage?.Length} bytes)");
            // Console.WriteLine($"Release files count: {releaseFiles?.Length}");
            
            var uploadedRelease = new Release();
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(title))
                {
                    Console.WriteLine("ERROR: Title is missing");
                    return BadRequest("Title is required.");
                }
                if (string.IsNullOrEmpty(type))
                {
                    Console.WriteLine("ERROR: Type is missing");
                    return BadRequest("Type is required.");
                }
                if (string.IsNullOrEmpty(artistId))
                {
                    Console.WriteLine("ERROR: ArtistId is missing");
                    return BadRequest("Artist ID is required.");
                }
                if (coverImage == null || coverImage.Length == 0)
                {
                    Console.WriteLine("ERROR: Cover image is missing");
                    return BadRequest("Cover image is required.");
                }
                if (releaseFiles == null || releaseFiles.Length == 0)
                {
                    Console.WriteLine("ERROR: Release files are missing");
                    return BadRequest("At least one release file is required.");
                }

                Console.WriteLine("All validations passed, creating release...");
                
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
                
                Console.WriteLine("Calling CreateReleaseAsync...");
                // Create the release with files
                uploadedRelease = await _releaseService.CreateReleaseAsync(
                    uploadedRelease,
                    coverImage,
                    releaseFiles
                );
                
                Console.WriteLine("Release created successfully!");
                return Ok(uploadedRelease);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in UploadRelease: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest($"Error uploading release: {ex.Message}");
            }
        }
    }
}
