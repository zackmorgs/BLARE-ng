using MongoDB.Driver;

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

        public IMongoCollection<Artist> Artists => _database.GetCollection<Artist>("Artists");
        public IMongoCollection<Track> Tracks => _database.GetCollection<Track>("Tracks");
        public IMongoCollection<Release> Releases => _database.GetCollection<Release>("Releases");
        public IMongoCollection<MusicTag> MusicTags => _database.GetCollection<MusicTag>("Tags");

    }
}
