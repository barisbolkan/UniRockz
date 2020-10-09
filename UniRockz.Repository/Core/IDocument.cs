using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniRockz.Repository.Core
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; }

        [BsonElement(elementName: "createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        DateTime CreatedAt { get; }
    }
}
