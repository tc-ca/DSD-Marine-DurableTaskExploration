using Azure.Messaging.ServiceBus;
using Common.Messaging;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayApi
{
    public class EventListenerService : IHostedService, IAsyncDisposable
    {
        private readonly OrchestratorService _orchestrator;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor _vesselDetailTopicProcessor;
        private ServiceBusProcessor _vesselRegistryTopicProcessor;

        public EventListenerService(OrchestratorService orchestrator)
        {
            _orchestrator = orchestrator;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _serviceBusClient = new ServiceBusClient("fixme: SB connection string");
            _vesselDetailTopicProcessor = _serviceBusClient.CreateProcessor(
                "fixme: topic name Vessel Detail service emits its events to", 
                new ServiceBusProcessorOptions());
            _vesselRegistryTopicProcessor = _serviceBusClient.CreateProcessor(
                "fixme: topic name Vessel Registry service emits its events to", 
                new ServiceBusProcessorOptions());

            _vesselDetailTopicProcessor.ProcessMessageAsync += VesselDetailTopicProcessor_ProcessMessageAsync;
            _vesselDetailTopicProcessor.ProcessErrorAsync += VesselDetailTopicProcessor_ProcessErrorAsync;
            _vesselRegistryTopicProcessor.ProcessMessageAsync += VesselRegistryTopicProcessor_ProcessMessageAsync;
            _vesselRegistryTopicProcessor.ProcessErrorAsync += VesselRegistryTopicProcessor_ProcessErrorAsync;

            await _vesselDetailTopicProcessor.StartProcessingAsync(cancellationToken);
            await _vesselRegistryTopicProcessor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _vesselDetailTopicProcessor?.StopProcessingAsync(cancellationToken);
            await _vesselRegistryTopicProcessor?.StopProcessingAsync(cancellationToken);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_vesselDetailTopicProcessor != null)
            {
                await _vesselDetailTopicProcessor.DisposeAsync().ConfigureAwait(false);
            }

            if (_vesselRegistryTopicProcessor != null)
            {
                await _vesselRegistryTopicProcessor.DisposeAsync().ConfigureAwait(false);
            }

            if (_serviceBusClient != null)
            {
                await _serviceBusClient.DisposeAsync().ConfigureAwait(false);
            }
        }

        private async Task VesselDetailTopicProcessor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            await RaiseOrchestrationEvent(arg);
        }

        private async Task VesselRegistryTopicProcessor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            await RaiseOrchestrationEvent(arg);
        }

        private Task VesselDetailTopicProcessor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private Task VesselRegistryTopicProcessor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private async Task RaiseOrchestrationEvent(ProcessMessageEventArgs arg)
        {
            string content = arg.Message.Body.ToString();
            Event serviceEvent = MessageParser.Parse<Event>(content);
            await _orchestrator.RaiseEventAsync(serviceEvent);
            await arg.CompleteMessageAsync(arg.Message);
        }
    }
}
