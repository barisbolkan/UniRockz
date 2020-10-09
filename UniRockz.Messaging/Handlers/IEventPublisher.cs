using System.Threading.Tasks;

namespace UniRockz.Messaging.Handlers
{
    public interface IEventPublisher<TPublishEvent> where TPublishEvent : class, new()
    {
        Task Send(TPublishEvent message);
    }
}
