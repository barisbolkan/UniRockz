using System;
using System.Threading.Tasks;
using UniRockz.Domain.Asteroids.Events;
using UniRockz.Messaging.Handlers;

namespace UniRockz.Domain.Asteroids.Publishers
{
    public class AsteroidRetrievedEventPublisher : IEventPublisher<AsteroidRetrievedEvent>
    {
        public AsteroidRetrievedEventPublisher()
        {
        }

        public Task Send(AsteroidRetrievedEvent message)
        {
            throw new NotImplementedException();
        }
    }
}
