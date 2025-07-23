namespace Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Services;

    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistService _playlistService;

        public PlaylistController(PlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        // [HttpGet("{userSlug}/playlists")]
        // public async Task<IActionResult> GetUserPlaylist(string userSlug)
        // {
        //     var playlists = await _playlistService.GetUserPlaylists(userSlug);
        //     if (playlists == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(playlists);
        // }
    }
}