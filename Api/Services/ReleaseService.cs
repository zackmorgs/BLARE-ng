// Services/TrackService.cs
using MongoDB.Driver;

using Models;

public class ReleaseService
{
    private readonly IMongoCollection<Release> _releases;

    public ReleaseService(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
        var database = client.GetDatabase("blare");
        _releases = database.GetCollection<Release>("releases");
    }

    public async Task<List<Release>> GetByArtistIdAsync(string artistId)
    {
        return await _releases.Find(release => release.ArtistId == artistId).ToListAsync();
    }

    public async Task<List<Release>> GetAsync() => await _releases.Find(_ => true).ToListAsync();

    public async Task<Release?> GetByIdAsync(string id) => 
        await _releases.Find(release => release.Id == id).FirstOrDefaultAsync();

    public async Task<List<Release>> GetRecentAsync(int limit = 10) =>
        await _releases.Find(_ => true)
            .SortByDescending(r => r.CreatedAt)
            .Limit(limit)
            .ToListAsync();

    public async Task CreateAsync(Release release) => await _releases.InsertOneAsync(release);
}
