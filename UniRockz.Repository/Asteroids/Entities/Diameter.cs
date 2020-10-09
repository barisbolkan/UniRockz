using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniRockz.Repository.Asteroids.Entities
{
    public class Diameter
    {
        [BsonElement(elementName: "min")]
        public double Min { get; set; }

        [BsonElement(elementName: "max")]
        public double Max { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        [BsonElement(elementName: "unit")]
        public DiameterUnits Unit { get; set; }
    }
}
