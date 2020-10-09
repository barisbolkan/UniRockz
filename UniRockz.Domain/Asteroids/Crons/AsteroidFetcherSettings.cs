namespace UniRockz.Domain.Asteroids.Crons
{
    public class AsteroidFetcherSettings
    {
        public string BaseUrl { get; set; }
        public string StartPage { get; set; }
        public string Schedule { get; set; }
        public bool Enabled { get; set; }
    }
}
