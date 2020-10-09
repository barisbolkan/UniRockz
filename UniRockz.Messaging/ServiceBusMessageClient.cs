using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace UniRockz.Messaging
{
    internal class ServiceBusMessageClient : IServiceBusMessageClient
    {
        private IQueueClient Client { get; }

        public ServiceBusMessageClient(IQueueClient client)
        {
            Client = client;
        }

        public Task Send<T>(T obj)
        {
            return Client.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(obj))));
        }

        public Task Receive()
        {
            throw new NotImplementedException();
        }
    }
}
