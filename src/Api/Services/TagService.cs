using Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using MongoDB.Driver;

namespace Services
{
    public class TagService
    {
        private readonly IMongoCollection<MusicTag> _musicTags;

        // This service handles operations related to tags, such as creating, retrieving, and managing tags.

        public TagService(DataContext dataContext)
        {
            _musicTags = dataContext.MusicTags;
        }

        // get all tags
        [HttpGet("all")]
        public async Task<IEnumerable<MusicTag>> GetAsync()
        {
            // Logic to retrieve all tags from the database
            return await _musicTags.Find(tag => true).ToListAsync();
        }

        // create a new tag
        public async Task CreateAsync(MusicTag tag)
        {
            if (_musicTags.Find(t => t.Name == tag.Name).Any() || tag == null)
            {
                throw new Exception("Tag already exists or is null");
            }
            else
            {
                await _musicTags.InsertOneAsync(tag);
            }
        }

        // look for tags by name
        public async Task<List<MusicTag>> SearchTagsAsync(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
                return new List<MusicTag>();

            var formattedName = tagName.Replace(" ", "-").ToLower();

            // Use a case-insensitive regex filter for partial matches
            var filter = Builders<MusicTag>.Filter.Regex(
                t => t.Name, new MongoDB.Bson.BsonRegularExpression(formattedName, "i")
            );

            return await _musicTags.Find(filter).ToListAsync();
        }

        public async Task<List<MusicTag>> GetSome()
        {
            // Logic to retrieve a subset of tags from the database
            return await _musicTags.Find(_ => true).Limit(25).ToListAsync();
        }
    }
}
