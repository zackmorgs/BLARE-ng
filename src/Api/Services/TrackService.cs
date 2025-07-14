// Services/TrackService.cs
using MongoDB.Driver;
using Data;
using Models;

namespace Services
{
    public class TrackService
    {
        private readonly IMongoCollection<Track> _tracks;

        public TrackService(DataContext dataContext)
        {
            _tracks = dataContext.Tracks;
        }

        public async Task<List<Track>> GetAsync() => await _tracks.Find(_ => true).ToListAsync();

        public async Task CreateAsync(Track track) => await _tracks.InsertOneAsync(track);

    }
}