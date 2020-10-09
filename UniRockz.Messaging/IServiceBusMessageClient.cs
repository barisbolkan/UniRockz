using System;
using System.Threading.Tasks;

namespace UniRockz.Messaging
{
    public interface IServiceBusMessageClient
    {
        Task Send<T>(T obj);

        Task Receive();
    }
}
