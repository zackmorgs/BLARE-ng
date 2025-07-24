using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Release
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("type")]
        [BsonRequired]
        public string Type { get; set; } = string.Empty;

        [BsonElement("title")]
        [BsonRequired]
        public string Title { get; set; } = string.Empty;

        [BsonElement("artistId")]
        [BsonRequired]
        public string ArtistId { get; set; } = string.Empty;

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

        [BsonElement("musicTags")]
        public List<string> MusicTags { get; set; } = new List<string>();

        [BsonElement("trackUrls")]
        public List<string> TrackUrls { get; set; } = new List<string>();
        
        [BsonElement("trackNames")]
        public List<string> TrackNames { get; set; } = new List<string>();

        [BsonElement("releaseSlug")]
        public string ReleaseSlug { get; set; } = string.Empty;

        [BsonElement("artistSlug")]
        public string ArtistSlug { get; set; } = string.Empty;

        [BsonElement("isPublic")]
        public bool IsPublic { get; set; } = false;
    }
}
