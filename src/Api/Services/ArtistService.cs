// Services/ReleaseService.cs
using Controllers;
using Data;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Services
{
    public class ArtistService
    {
        private readonly IMongoCollection<Artist> _artists;

        public ArtistService(DataContext dataContext)
        {
            _artists = dataContext.Artists;
        }

        public async Task<List<Artist>> GetAllAsync() =>
            await _artists.Find(_ => true).ToListAsync();

        public async Task<Artist> GetByIdAsync(string id) =>
            await _artists.Find(artist => artist.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();

        public async Task CreateAsync(Artist artist) => await _artists.InsertOneAsync(artist);

        public async Task UpdateAsync(string id, Artist artist) =>
            await _artists.ReplaceOneAsync(a => a.Id == ObjectId.Parse(id), artist);

        public async Task DeleteAsync(string id) =>
            await _artists.DeleteOneAsync(a => a.Id == ObjectId.Parse(id));
    }
}
