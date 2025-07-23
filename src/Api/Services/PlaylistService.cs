namespace Services
{
    public class PlaylistService
    {
        private readonly IMongoCollection<Playlist> _playlists;

        public PlaylistService(IMongoCollection<Playlist> playlists)
        {
            _playlists = playlists;
        }

        // public async Task<bool> ValidatePlaylistOwnership(string playlistId, string userId)
        // {
        //     var playlist = await _playlists.Find(p => p.Id == playlistId).FirstOrDefaultAsync();
        //     return playlist?.UserId == userId || playlist?.Collaborators?.Contains(userId) == true;
        // }

        // [Authorize]
        // public async Task<List<Playlist>> GetUserPlaylistsFromId(string userId)
        // {
        //     return await _playlists.Find(playlist => playlist.UserId == userId).ToListAsync();
        // }
    }
}
