using Common.Messaging;
using DurableTask.Core;
using DurableTask.Core.Tracking;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Settings;
using DurableTask.ServiceBus.Tracking;
using GatewayApi.Configuration;
using GatewayApi.Sagas.FirstRegistry;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayApi
{
    public class OrchestratorService : IHostedService, IDisposable
    {
        private readonly TaskHubWorker _worker;
        private readonly TaskHubClient _client;

        private bool _disposed;
        
        public OrchestratorService()
        {
            IOrchestrationServiceInstanceStore instanceStore = new AzureTableInstanceStore(HubConfig.HubName, HubConfig.StorageConnectionString);
            IOrchestrationServiceBlobStore blobStore = new AzureStorageBlobStore(HubConfig.HubName, HubConfig.StorageConnectionString);
            ServiceBusOrchestrationServiceSettings settings = new();

            ServiceBusOrchestrationService service = new(
                HubConfig.ServiceBusConnectionString,
                HubConfig.HubName,
                instanceStore,
                blobStore,
                settings);
            
            _worker = new TaskHubWorker(service);
            _client = new TaskHubClient(service);
            _disposed = false;

            // Register specific saga members.
            _worker.AddFirstRegistrySaga();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _worker.StartAsync();
        }

        public async Task<OrchestrationInstance> RunOrchestrationAsync(Type orchestrationType, object orchestrationInput)
        {
            OrchestrationInstance instance = await _client.CreateOrchestrationInstanceAsync(orchestrationType, orchestrationInput);
            return instance;
        }

        public async Task<OrchestrationState> GetOrchestrationStatusAsync(string instanceId)
        {
            OrchestrationState state = await _client.GetOrchestrationStateAsync(instanceId);
            return state;
        }

        public async Task RaiseEventAsync(Event externalEvent)
        {
            OrchestrationState state = await _client.GetOrchestrationStateAsync(externalEvent.Header.TransactionId);
            await _client.RaiseEventAsync(
                state.OrchestrationInstance, 
                externalEvent.Header.MessageType, 
                externalEvent);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _worker.StopAsync();
        }

        protected virtual void Dispose (bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                if (_worker != null)
                {
                    _worker.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
