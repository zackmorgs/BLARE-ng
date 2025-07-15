// Services/TrackService.cs
using MongoDB.Driver;
using MongoDB.Bson;
using Data;
using Models;

namespace Services
{
    public class TrackService
    {
        // the collection of tracks fro MongoDB
        private readonly IMongoCollection<Track> _tracks;
        private readonly string _uploadsPath;

        // contructor to inject the DataContext
        public TrackService(DataContext dataContext)
        {
            _tracks = dataContext.Tracks;
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tracks");
            
            // Ensure the uploads directory exists
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        // upload a track file and save metadata to database
        public async Task<Track> UploadTrackAsync(IFormFile file, Track trackToAdd)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            // Validate file type (only audio files)
            var allowedExtensions = new[] { ".wav", ".mp3", ".flac" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Only audio files (.wav, .mp3, .flac) are allowed");

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update track metadata with file information
            trackToAdd.FileUrl = $"/uploads/tracks/{fileName}";
            trackToAdd.UploadedAt = DateTime.UtcNow;

            // Save track metadata to database
            await _tracks.InsertOneAsync(trackToAdd);

            return trackToAdd;
        }

        // get all tracks
        public async Task<List<Track>> GetAsync() => await _tracks.Find(_ => true).ToListAsync();

        // get track by ID
        public async Task<Track?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return null;
            return await _tracks.Find(track => track.Id == objectId).FirstOrDefaultAsync();
        }

        // delete track
        public async Task DeleteAsync(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId objectId))
                await _tracks.DeleteOneAsync(track => track.Id == objectId);
        }
    }
}