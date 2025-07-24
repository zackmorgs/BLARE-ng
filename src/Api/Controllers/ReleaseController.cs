using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Controllers
{
    public class UpdateReleaseMetadataRequest
    {
        public List<string>? TrackNames { get; set; }
        public bool? IsPublic { get; set; }
    }

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

        [HttpGet("artist/{id}/all")]
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

        [HttpGet("artist/{artistSlug}/title/{releaseSlug}")]
        public async Task<IActionResult> GetReleaseBySlugs(string artistSlug, string releaseSlug)
        {
            var release = await _releaseService.GetReleaseBySlugsAsync(artistSlug, releaseSlug);
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
                uploadedRelease.MusicTags =
                    musicTags?.Split(',').Select(static tag => tag.Trim()).ToList()
                    ?? new List<string>();
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

        [HttpGet("artist/{id}")]
        public async Task<IActionResult> GetArtistById(string id)
        {
            var artist = await _releaseService.getArtistById(id);
            if (artist == null)
            {
                return NotFound();
            }
            return Ok(artist);
        }

        [HttpPut("{id}/metadata")]
        public async Task<IActionResult> UpdateReleaseMetadata(
            string id,
            [FromBody] UpdateReleaseMetadataRequest request
        )
        {
            try
            {
                var release = await _releaseService.GetByIdAsync(id);
                if (release == null)
                {
                    return NotFound();
                }

                // Update track names if provided
                if (request.TrackNames != null)
                {
                    release.TrackNames = request.TrackNames;
                }

                // Update public status if provided
                if (request.IsPublic.HasValue)
                {
                    release.IsPublic = request.IsPublic.Value;
                }

                release.UpdatedAt = DateTime.UtcNow;
                await _releaseService.UpdateAsync(id, release);

                return Ok(release);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating release metadata: {ex.Message}");
            }
        }

        // get artist name by release ID
        [HttpGet("artist-name/album/{id}")]
        public async Task<IActionResult> GetArtistNameByReleaseId(string id)
        {
            var artistName = await _releaseService.GetArtistNameByReleaseIdAsync(id);
            if (artistName == null)
            {
                return NotFound();
            }

            return Ok(artistName);
        }

        // delete track via ID from release
        [HttpDelete("{releaseId}/track/{trackId}")]
        public async Task<IActionResult> DeleteTrackFromRelease(string releaseId, ObjectId trackId)
        {
            try
            {
                var release = await _releaseService.GetByIdAsync(releaseId);
                if (release == null)
                {
                    return NotFound($"Release with ID {releaseId} not found.");
                }
                ObjectId track = release.TrackIds.FirstOrDefault(t => t == trackId);
                if (track == ObjectId.Empty)
                {
                    return NotFound($"Track with ID {trackId} not found in release {releaseId}.");
                }
                release.TrackIds.Remove(track);
                await _releaseService.UpdateAsync(releaseId, release);
                return Ok($"Track {trackId} removed from release {releaseId}.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting track from release: {ex.Message}");
            }
        }
    }
}
