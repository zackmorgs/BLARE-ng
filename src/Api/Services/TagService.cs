
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
            return await _musicTags.Find(_ => true).ToListAsync();
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
            // Logic to search for a tag by its name
            var tags = await _musicTags.Find(t => t.Name == tagName).ToListAsync();
            return tags;
        }
    }
}