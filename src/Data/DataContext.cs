using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Models;

namespace Data
{
    public class DataContext
    {
        private readonly IMongoDatabase _database;

        public DataContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
            _database = client.GetDatabase("blare-cluster");
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Artist> Artists => _database.GetCollection<Artist>("artists");
        public IMongoCollection<Track> Tracks => _database.GetCollection<Track>("tracks");
        public IMongoCollection<Release> Releases => _database.GetCollection<Release>("releases");
        public IMongoCollection<MusicTag> MusicTags => _database.GetCollection<MusicTag>("tags");
        public IMongoCollection<Slug> Slugs => _database.GetCollection<Slug>("slugs");
    }
}
