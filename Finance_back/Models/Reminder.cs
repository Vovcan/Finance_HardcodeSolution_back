using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Finance_back.Models
{
    public class Reminder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        [BsonElement("DueDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DueDate { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ExpenseCategoryId { get; set; }
        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? CreatedAt { get; set; }
        [BsonElement("LastUpdatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LastUpdatedAt { get; set; }
    }
}
