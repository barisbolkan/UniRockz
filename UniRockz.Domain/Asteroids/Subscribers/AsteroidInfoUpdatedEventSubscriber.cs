using System;
using System.Threading.Tasks;
using MediatR;
using UniRockz.Domain.Asteroids.Events;
using UniRockz.Messaging.Handlers;

namespace UniRockz.Domain.Asteroids.Subscribers
{
    public class AsteroidInfoUpdatedEventSubscriber : IEventSubscriber<AsteroidInfoUpdatedEvent>
    {
        private IMediator Mediator { get; }

        public AsteroidInfoUpdatedEventSubscriber(IMediator mediator)
        {
            Mediator = mediator;
        }

        public async Task Handle(AsteroidInfoUpdatedEvent @event)
        {
            await Mediator.Send(/* TODO: Convert event to command*/ @event);
        }
    }
}
