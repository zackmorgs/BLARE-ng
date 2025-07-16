// Services/ReleaseService.cs
using Data;
using Models;
using MongoDB.Driver;

namespace Services
{
    public class ReleaseService
    {
        private readonly IMongoCollection<Release> _releases;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsPath;

        // contructor to inject the DataContext
        public ReleaseService(DataContext dataContext, IWebHostEnvironment env)
        {
            _releases = dataContext.Releases;
            _env = env;
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Ensure upload directories exist
            Directory.CreateDirectory(Path.Combine(_uploadsPath, "albumart"));
            Directory.CreateDirectory(Path.Combine(_uploadsPath, "releases"));
        }

        // gets an artist via its Id
        public async Task<List<Release>> GetReleasesByArtistIdAsync(string artistId)
        {
            return await _releases.Find(release => release.ArtistId == artistId).ToListAsync();
        }

        // gets all releases that exist
        public async Task<List<Release>> GetAsync() =>
            await _releases.Find(_ => true).ToListAsync();

        //
        public async Task<Release> GetByIdAsync(string id)
        {
            return await _releases.Find(release => release.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Release>> GetRecentAsync(int limit = 10) =>
            await _releases
                .Find(_ => true)
                .SortByDescending(r => r.CreatedAt)
                .Limit(limit)
                .ToListAsync();

        public async Task CreateAsync(Release release) => await _releases.InsertOneAsync(release);

        public async Task<Release> CreateReleaseAsync(
            Release release,
            IFormFile coverImage,
            IFormFile[] releaseFiles
        )
        {
            // Validate, process, and upload cover image
            // Save cover image and set URL
            var coverImageName = $"{Guid.NewGuid()}{Path.GetExtension(coverImage.FileName)}";
            var coverImagePath = Path.Combine(_uploadsPath + "/albumart", coverImageName);
            using (var stream = new FileStream(coverImagePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }
            var coverImageUrl = $"/uploads/albumart/{coverImageName}";
            release.CoverImageUrl = coverImageUrl;

            List<string> trackUrls = new List<string>();

            foreach (var file in releaseFiles)
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(_uploadsPath, "releases", fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Add file URL to release
                release.TrackUrls.Add($"/uploads/releases/{fileName}");
            }

            // Insert the release into the database
            await _releases.InsertOneAsync(release);
            return release;
        }
    }
}
