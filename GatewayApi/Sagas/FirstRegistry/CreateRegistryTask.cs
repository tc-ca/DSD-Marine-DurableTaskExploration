using Azure.Messaging.ServiceBus;
using Common.Messaging;
using Common.Messaging.Commands;
using DurableTask.Core;
using System.Threading.Tasks;

namespace GatewayApi.Sagas.FirstRegistry
{
    public class CreateRegistryTask : TaskActivity<RegistryInput, bool>
    {
        protected override bool Execute(TaskContext context, RegistryInput input)
        {
            return false;
        }

        protected override async Task<bool> ExecuteAsync(TaskContext context, RegistryInput input)
        {
            MessageHeader header = new(
                context.OrchestrationInstance.InstanceId,
                nameof(CreateVesselRegistryCommand),
                nameof(CreateRegistryTask));

            CreateVesselRegistryCommand command = new()
            {
                Header = header,
                OfficialNumber = input.OfficialNumber
            };

            await using ServiceBusClient client = new("fixme: SB connection string");
            await using ServiceBusMessageSender sender = new(client);
            await sender.SendAsync(command, "fixme: queue name Vessel Registry service to listen to").ConfigureAwait(false);

            return true;
        }
    }
}
