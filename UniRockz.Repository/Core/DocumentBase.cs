/*using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniRockz.Repository.Core
{
    public abstract class DocumentBase : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public ObjectId Id { get; private set; }

        public DateTime CreatedAt => Id.CreationTime;

        public DocumentBase(object id)
        {
            Id = id;
            ObjectId.GenerateNewId()
        }
    }
}*/
