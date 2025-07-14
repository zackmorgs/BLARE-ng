using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Release
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("type")]
        [BsonRequired]
        public string Type { get; set; } = string.Empty;

        [BsonElement("title")]
        [BsonRequired]
        public string Title { get; set; } = string.Empty;

        [BsonElement("artistId")]
        [BsonRequired]
        public ObjectId ArtistId { get; set; }

        [BsonElement("releaseDate")]
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

        [BsonElement("createdAt")]
        [BsonRequired]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("tracks")]
        public List<ObjectId> TrackIds { get; set; } = new List<ObjectId>();

        [BsonElement("coverImageUrl")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("musicTagIds")]
        public List<ObjectId> MusicTagIds { get; set; } = new List<ObjectId>();
    }
}
