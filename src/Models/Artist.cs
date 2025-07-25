using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Artist
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; } = string.Empty;

        [BsonElement("bio")]
        public string Bio { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonRequired]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("tracks")]
        public List<ObjectId> TrackIds { get; set; } = new List<ObjectId>();

        [BsonElement("musicTagIds")]
        public List<ObjectId> MusicTagIds { get; set; } = new List<ObjectId>();

        [BsonElement("slug")]
        public string Slug { get; set; } = String.Empty;
    }
}
