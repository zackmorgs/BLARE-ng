using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
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

        // [HttpPost("release")]
        // public async Task<IActionResult> UploadRelease(
        //     [FromForm] IFormFile coverImage,
        //     [FromForm] FileStream[] audioFiles,
        //     [FromForm] string title,
        //     [FromForm] string artist,
        //     [FromForm] DateTime releaseDate,
        //     [FromForm] string musicTags
        // )
        // {
        //     try
        //     {
        //         if (coverImage == null || coverImage.Length == 0)
        //             return BadRequest("No cover image provided");

        //         if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(artist))
        //             return BadRequest("Title and artist are required");

        //         // Create release metadata
        //         var releaseMetadata = new Release
        //         {
        //             Title = title,
        //             Artist = artist,
        //             ReleaseDate = releaseDate,
        //             MusicTags = musicTags,
        //         };

        //         // Validate audio files
        //         if (audioFiles == null || audioFiles.Length == 0)
        //             return BadRequest("No audio files provided");
        //         foreach (var file in audioFiles)
        //         {
        //             if (file == null || file.Length == 0)
        //                 return BadRequest("One or more audio files are empty");
        //         }

        //         // Upload audio files
        //         foreach (var file in audioFiles)
        //         {
        //             var uploadedTrack = await _trackService.UploadTrackAsync(file, releaseMetadata);
        //         }

        //         // Upload the cover image and save metadata
        //         var uploadedRelease = await _trackService.UploadTrackAsync(
        //             coverImage,
        //             releaseMetadata
        //         );

        //         return Ok(
        //             new
        //             {
        //                 id = uploadedRelease.Id.ToString(),
        //                 title = uploadedRelease.Title,
        //                 artist = uploadedRelease.Artist,
        //                 fileUrl = uploadedRelease.FileUrl,
        //                 uploadedAt = uploadedRelease.UploadedAt,
        //             }
        //         );
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }

        [HttpPost("track")]
        public async Task<IActionResult> UploadTrack(
            [FromForm] IFormFile file,
            [FromForm] string title,
            [FromForm] string artist
        )
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
                    Duration = TimeSpan.Zero, // You might want to extract this from the audio file
                };

                // Upload the track
                var uploadedTrack = await _trackService.UploadTrackAsync(file, trackMetadata);

                return Ok(
                    new
                    {
                        id = uploadedTrack.Id.ToString(),
                        title = uploadedTrack.Title,
                        artist = uploadedTrack.Artist,
                        fileUrl = uploadedTrack.FileUrl,
                        uploadedAt = uploadedTrack.UploadedAt,
                    }
                );
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
