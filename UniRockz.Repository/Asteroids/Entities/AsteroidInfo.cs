using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UniRockz.Repository.Attributes;
using UniRockz.Repository.Core;

namespace UniRockz.Repository.Asteroids.Entities
{
    [CollectionInfo("mystroid", "asteroid-info")]
    public class AsteroidInfo : IDocument
    {
        public string Id { get; private set; }

        [BsonElement(elementName: "neoRefId")]
        public string NeoRefId { get; private set; }

        [BsonElement(elementName: "name")]
        public string Name { get; private set; }

        [BsonElement(elementName: "jplUrl")]
        [BsonIgnoreIfNull]
        public string JplUrl { get; set; }

        [BsonElement(elementName: "absoluteMagnitude")]
        public double? AbsoluteMagnitude { get; set; }

        [BsonElement(elementName: "estimatedDiameters")]
        public IEnumerable<Diameter> EstimatedDiameters { get; set; }

        [BsonElement(elementName: "isHazardous")]
        public bool? IsHazardous { get; set; }

        [BsonElement(elementName: "closeApproachData")]
        public IEnumerable<ApproachData> CloseApproachData { get; set; }

        public DateTime CreatedAt => DateTime.Now;

        [BsonConstructor]
        public AsteroidInfo(string id, string neoRefId, string name)
        {
            Id = id;
            NeoRefId = neoRefId;
            Name = name;
        }
    }

    public static class StringExtensions
    {
        public static ObjectId ToObjectId(this string id)
        {
            var arr = Encoding.UTF8.GetBytes(id).ToList();
            if (arr.Count < 12)
            {
                arr.AddRange(Enumerable.Repeat<byte>(0, 12 - arr.Count));
            }

            return new ObjectId(BitConverter.ToString(arr.ToArray())
                .Replace("-", ""));
        }
    }
}
