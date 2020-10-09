using System;
using System.Threading.Tasks;
using UniRockz.Domain.Asteroids.Events;
using UniRockz.Messaging.Handlers;

namespace UniRockz.Domain.Asteroids.Publishers
{
    public class AsteroidInfoUpdatedEventPublisher : IEventPublisher<AsteroidInfoUpdatedEvent>
    {
        public AsteroidInfoUpdatedEventPublisher()
        {
        }

        public Task Send(AsteroidInfoUpdatedEvent message)
        {
            throw new NotImplementedException();
        }
    }
}
