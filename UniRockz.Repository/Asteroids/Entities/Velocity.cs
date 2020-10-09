using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniRockz.Repository.Asteroids.Entities
{
    public class Velocity
    {
        [BsonElement(elementName: "velocity")]
        public double? Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        [BsonElement(elementName: "unit")]
        public VelocityUnits Unit { get; set; }
    }
}
