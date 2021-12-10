using Azure.Messaging.ServiceBus;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Messaging
{
    public class ServiceBusMessageSender : IMessageSender, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        public ServiceBusMessageSender(ServiceBusClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        public async Task SendAsync(object message, string queueOrTopicName)
        {
            string contents = JsonSerializer.Serialize(message, message.GetType());
            ServiceBusMessage sbMessage = new(contents);

            await using ServiceBusSender sender = _client.CreateSender(queueOrTopicName);            
            await sender.SendMessageAsync(sbMessage);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if(_client != null)
            {
                await _client.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
