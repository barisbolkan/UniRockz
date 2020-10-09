using System;
using System.Threading.Tasks;
using MediatR;
using UniRockz.Domain.Asteroids.Events;
using UniRockz.Messaging.Handlers;

namespace UniRockz.Domain.Asteroids.Subscribers
{
    public class AsteroidRetrievedEventSubscriber : IEventSubscriber<AsteroidRetrievedEvent>
    {
        private IMediator Mediator { get; }

        public AsteroidRetrievedEventSubscriber(IMediator mediator)
        {
            Mediator = mediator;
        }

        public async Task Handle(AsteroidRetrievedEvent @event)
        {
            await Mediator.Send(/* TODO: Convert event to command*/@event);
        }
    }
}