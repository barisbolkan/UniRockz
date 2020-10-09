using UniRockz.Repository.Asteroids.Entities;

namespace UniRockz.Domain.Asteroids.Responses
{
    public class AsteroidInfoResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string JplUrl { get; set; }
        public double Magnitude { get; set; }
        public bool IsHazardous { get; set; }
    }
}