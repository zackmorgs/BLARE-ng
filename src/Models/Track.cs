using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Track
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("title")]
        [BsonRequired]
        public string Title { get; set; } = string.Empty;

        [BsonElement("artistId")]
        [BsonRequired]
        public ObjectId ArtistId { get; set; }

        [BsonElement("fileUrl")]
        [BsonRequired]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("duration")]
        [BsonRequired]
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        [BsonElement("uploadedAt")]
        [BsonRequired]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("musicTagIds")]
        public List<ObjectId> MusicTagIds { get; set; } = new List<ObjectId>();

        [BsonElement("slug")]
        public string Slug { get; set; } = string.Empty;
    }
}
