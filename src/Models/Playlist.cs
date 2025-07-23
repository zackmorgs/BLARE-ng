using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Models
{
    public class Playlist
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        [BsonRequired]
        public string Title { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRequired]
        public ObjectId UserId { get; set; } = ObjectId.Empty;

        [BsonElement("collaborators")]
        public List<ObjectId> Collaborators { get; set; } = new List<ObjectId>();

        [BsonElement("createdAt")]
        [BsonRequired]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("trackIds")]
        public List<string> TrackIds { get; set; } = new List<string>();

        [BsonElement("coverImageUrl")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("isPublic")]
        public bool IsPublic { get; set; } = false;
    }
}
