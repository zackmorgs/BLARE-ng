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

        [BsonElement("artist")]
        [BsonRequired]
        public string Artist { get; set; } = string.Empty;

        [BsonElement("fileUrl")]
        [BsonRequired]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("duration")]
        [BsonRequired]
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        [BsonElement("uploadedAt")]
        [BsonRequired]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
