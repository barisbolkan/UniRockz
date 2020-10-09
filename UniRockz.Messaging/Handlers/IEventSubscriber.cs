using System.Threading.Tasks;

namespace UniRockz.Messaging.Handlers
{
    public interface IEventSubscriber<TSubscriptionEvent> where TSubscriptionEvent : class, new()
    {
        public Task HandleMessage(object @event) => Handle(@event as TSubscriptionEvent);

        Task Handle(TSubscriptionEvent @event);
    }
}
