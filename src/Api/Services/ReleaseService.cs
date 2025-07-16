// Services/ReleaseService.cs
using Controllers;
using Data;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Services
{
    public class ReleaseService
    {
        private readonly IMongoCollection<Release> _releases;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsPath;
        private readonly TrackService _trackService;
        private readonly UserService _userService;
        private readonly SlugService _slugService;
        // contructor to inject the DataContext
        public ReleaseService(
            DataContext dataContext,
            IWebHostEnvironment env,
            TrackService trackService,
            SlugService slugService,
            UserService userService
        )
        {
            _releases = dataContext.Releases;
            _env = env;
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _userService = userService;

            _trackService = trackService;
            _slugService = slugService;

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
            // if my understanding is correct, this should be a unique ID for the release
            var releaseId = ObjectId.GenerateNewId();

            var coverImageName = $"{releaseId.ToString()}{Path.GetExtension(coverImage.FileName)}";
            var coverImagePath = Path.Combine(_uploadsPath + "/albumart", coverImageName);
            using (var stream = new FileStream(coverImagePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }
            var coverImageUrl = $"/uploads/albumart/{coverImageName}";
            release.CoverImageUrl = coverImageUrl;

            // Initialize slug
            var releaseSlug = await _slugService.GenerateSlug(release.Title);
            release.ReleaseSlug = releaseSlug.SlugValue;

            // get artist slug
            var artistId = release.ArtistId;
            string artistSlug = await _userService.GetUserSlugAsync(artistId);
            release.ArtistSlug = artistSlug;

            List<string> trackUrls = new List<string>();

            foreach (var file in releaseFiles)
            {

                var id = ObjectId.GenerateNewId();
                var trackSlug = await _slugService.GenerateSlug(file.FileName);

                // Generate unique filename
                var fileName = id.ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_uploadsPath + "/releases", fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var track = new Track
                {
                    Id = id,
                    Title = file.FileName,
                    ArtistId = ObjectId.Parse(release.ArtistId),
                    FileUrl = $"/uploads/tracks/{file.FileName}",
                    Duration = TimeSpan.Zero, // Placeholder
                    UploadedAt = DateTime.UtcNow,
                    MusicTagIds = null,
                    Slug = trackSlug.SlugValue,
                };

                await _trackService.CreateAsync(track);

                // Add file URL to release
                release.TrackUrls.Add($"/uploads/releases/{fileName}");
            }

            // Insert the release into the database
            await _releases.InsertOneAsync(release);
            return release;
        }
    }
}
