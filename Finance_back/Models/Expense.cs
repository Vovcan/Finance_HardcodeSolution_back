using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Expense
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int? Amount { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ExpenseCategory { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }
}
