// Services/ReleaseService.cs
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
        private readonly ArtistService _artistService;

        // contructor to inject the DataContext
        public ReleaseService(
            DataContext dataContext,
            IWebHostEnvironment env,
            TrackService trackService,
            SlugService slugService,
            UserService userService,
            ArtistService artistService
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
            return await _releases
                .Find(release => release.Id == ObjectId.Parse(id))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Release>> GetRecentAsync(int limit = 10) =>
            await _releases
                .Find(_ => true)
                .SortByDescending(r => r.CreatedAt)
                .Limit(limit)
                .ToListAsync();

        public async Task CreateAsync(Release release) => await _releases.InsertOneAsync(release);

        public async Task UpdateAsync(string id, Release release)
        {
            var filter = Builders<Release>.Filter.Eq(r => r.Id, ObjectId.Parse(id));
            await _releases.ReplaceOneAsync(filter, release);
        }

        public async Task<Release> CreateReleaseAsync(
            Release release,
            IFormFile coverImage,
            IFormFile[] releaseFiles
        )
        {
            try
            {
                // Generate and assign the release ID
                var releaseId = ObjectId.GenerateNewId();
                release.Id = releaseId; // FIX: Assign the ID

                // Initialize lists if they're null
                release.TrackUrls ??= new List<string>(); // FIX: Initialize if null
                release.TrackNames ??= new List<string>(); // FIX: Initialize if null

                // Fix cover image path
                var coverImageName =
                    $"{releaseId.ToString()}{Path.GetExtension(coverImage.FileName)}";
                // This could fail if _uploadsPath is null
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

                // Set timestamps
                release.CreatedAt = DateTime.UtcNow;
                release.UpdatedAt = DateTime.UtcNow;

                // check the Artist db for this artist, if it doesnt exist, create it
                var artist = await getArtistById(artistId);
                if (artist == null)
                {
                    // If artist does not exist, create a new Artist object
                    // Ensure the user exists before creating an artist
                    var user = await _userService.GetUserByIdAsync(artistId);
                    if (user != null) // FIX: Check if user exists
                    {
                        var myArtist = new Artist
                        {
                            Id = user.Id,
                            Name = user.ArtistName,
                            Slug = artistSlug,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };
                        await _artistService.CreateAsync(myArtist);
                    }
                }

                foreach (var file in releaseFiles)
                {
                    var trackId = ObjectId.GenerateNewId();
                    var trackSlug = await _slugService.GenerateSlug(
                        Path.GetFileNameWithoutExtension(file.FileName)
                    ); // FIX: Use clean filename

                    // Generate unique filename
                    var fileName = trackId.ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_uploadsPath, "releases", fileName); // FIX: Use proper Path.Combine

                    // Save file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var track = new Track
                    {
                        Id = trackId,
                        Title = Path.GetFileNameWithoutExtension(file.FileName), // FIX: Clean title
                        ArtistId = ObjectId.Parse(release.ArtistId),
                        FileUrl = $"/uploads/releases/{fileName}", // FIX: Use correct path and generated filename
                        Duration = TimeSpan.Zero,
                        UploadedAt = DateTime.UtcNow,
                        MusicTagIds = null,
                        Slug = trackSlug.SlugValue,
                    };

                    await _trackService.CreateAsync(track);

                    // Add file URL to release (now safe because lists are initialized)
                    release.TrackUrls.Add($"/uploads/releases/{fileName}");
                    release.TrackNames.Add(Path.GetFileNameWithoutExtension(file.FileName)); // FIX: Use clean name
                }

                // Insert the release into the database
                await _releases.InsertOneAsync(release);
                return release;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating release: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}"); // FIX: Add stack trace for better debugging
                throw;
            }
        }

        public async Task<Release> GetReleaseBySlugsAsync(string artistSlug, string releaseSlug)
        {
            return await _releases
                .Find(release =>
                    release.ArtistSlug == artistSlug && release.ReleaseSlug == releaseSlug
                )
                .FirstOrDefaultAsync();
        }

        public async Task<Artist> getArtistById(string id)
        {
            var artistCollection = _releases.Database.GetCollection<Artist>("artists");
            return await artistCollection
                .Find(artist => artist.Id == ObjectId.Parse(id))
                .FirstOrDefaultAsync();
        }
    }
}
