// Services/TrackService.cs
using MongoDB.Driver;

using Models;

public class TrackService
{
    private readonly IMongoCollection<Track> _tracks;

    public TrackService(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
        var database = client.GetDatabase("blare");
        _tracks = database.GetCollection<Track>("tracks");
    }

    public async Task<List<Track>> GetAsync() => await _tracks.Find(_ => true).ToListAsync();

    public async Task CreateAsync(Track track) => await _tracks.InsertOneAsync(track);

}