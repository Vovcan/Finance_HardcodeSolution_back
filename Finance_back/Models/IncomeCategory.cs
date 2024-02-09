using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Finance_back.Models
{
    public class IncomeCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public int? Sum { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

    }
}
