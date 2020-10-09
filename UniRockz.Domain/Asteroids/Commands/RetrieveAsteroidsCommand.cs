using MediatR;

namespace UniRockz.Domain.Asteroids.Commands
{
    public class RetrieveAsteroidsCommand : IRequest
    {
        public string BaseUrl { get; }
        public string StartPage { get; }

        public RetrieveAsteroidsCommand(string baseUrl, string startPage)
        {
            BaseUrl = baseUrl;
            StartPage = startPage;
        }
    }
}
