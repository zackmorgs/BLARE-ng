using Data;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Slugify;

namespace Services
{
    public class SlugService
    {
        private readonly SlugHelper _slugHelper;
        private readonly IMongoCollection<Slug> _slugs;

        public SlugService(DataContext dataContext)
        {
            _slugHelper = new SlugHelper();
            _slugs = dataContext.Slugs;
        }

        public async Task<string> MakeUniqueSlug(string name)
        {
            string slug = _slugHelper.GenerateSlug(name);
            var existingSlug = await _slugs.Find(s => s.SlugValue == slug).FirstOrDefaultAsync();

            if (existingSlug != null)
            {
                // If the slug already exists, append a number to make it unique
                int counter = 1;
                string newSlug;
                do
                {
                    newSlug = $"{slug}-{counter}";
                    counter++;
                    existingSlug = await _slugs
                        .Find(s => s.SlugValue == newSlug)
                        .FirstOrDefaultAsync();
                } while (existingSlug != null);

                return newSlug;
            }

            return slug;
        }

        public async Task<Slug> GenerateSlug(string name)
        {
            var newSlug = await this.MakeUniqueSlug(name);
            var slug = new Slug { Id = ObjectId.GenerateNewId(), SlugValue = newSlug };

            await _slugs.InsertOneAsync(slug);
            return slug;
        }

        public async Task<Slug?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null;
            }

            return await _slugs.Find(s => s.Id == objectId).FirstOrDefaultAsync();
        }
    }
}
