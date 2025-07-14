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
        return await _releases.Find(release => release.ArtistId.ToString() == artistId).ToListAsync();
    }

    public async Task<List<Release>> GetAsync() => await _releases.Find(_ => true).ToListAsync();

    public async Task CreateAsync(Release release) => await _releases.InsertOneAsync(release);
}
