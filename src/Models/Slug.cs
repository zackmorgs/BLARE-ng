using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Slug
    {   
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("slug")]
        [BsonRequired]
        public string SlugValue { get; set; } = string.Empty;

        // Additional properties can be added as needed
    }
}