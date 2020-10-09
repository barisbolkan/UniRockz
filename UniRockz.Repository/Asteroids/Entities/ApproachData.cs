using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace UniRockz.Repository.Asteroids.Entities
{
    public class ApproachData
    {
        [BsonElement(elementName: "date")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public DateTimeOffset Date { get; set; }

        [BsonElement(elementName: "relativeVelocities")]
        public IEnumerable<Velocity> RelativeVelocities { get; set; }

        [BsonElement(elementName: "missDistances")]
        public IEnumerable<Distance> MissDistances { get; set; }

        [BsonElement(elementName: "orbitingBody")]
        public string OrbitingBody { get; set; }
    }
}
