using MediatR;
using UniRockz.Domain.Asteroids.Responses;

namespace UniRockz.Domain.Asteroids.Queries
{
    public class GetAsteroidByIdQuery : IRequest<AsteroidInfoResponse>
    {
        public string Id { get; }

        public GetAsteroidByIdQuery(string id)
        {
            Id = id;
        }
    }
}
