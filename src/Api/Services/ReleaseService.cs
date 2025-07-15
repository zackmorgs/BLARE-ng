// Services/ReleaseService.cs
using MongoDB.Driver;
using Data;
using Models;

namespace Services
{
    public class ReleaseService
    {
        private readonly IMongoCollection<Release> _releases;

        // contructor to inject the DataContext
        public ReleaseService(DataContext dataContext)
        {
            _releases = dataContext.Releases;
        }

        // gets an artist via its Id
        public async Task<List<Release>> GetReleasesByArtistIdAsync(string artistId)
        {
            return await _releases.Find(release => release.ArtistId == artistId).ToListAsync();
        }

        // gets all releases that exist
        public async Task<List<Release>> GetAsync() => await _releases.Find(_ => true).ToListAsync();

    
        //
        public async Task<Release> GetByIdAsync(string id)
        {
            return await _releases.Find(release => release.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Release>> GetRecentAsync(int limit = 10) =>
            await _releases.Find(_ => true)
                .SortByDescending(r => r.CreatedAt)
                .Limit(limit)
                .ToListAsync();

        public async Task CreateAsync(Release release) => await _releases.InsertOneAsync(release);
    }
}
