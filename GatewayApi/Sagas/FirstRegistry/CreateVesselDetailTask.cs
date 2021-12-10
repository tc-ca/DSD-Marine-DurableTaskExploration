using Azure.Messaging.ServiceBus;
using Common.Messaging;
using Common.Messaging.Commands;
using DurableTask.Core;
using System.Threading.Tasks;

namespace GatewayApi.Sagas.FirstRegistry
{
    public class CreateVesselDetailTask : TaskActivity<VesselInput, bool>
    {
        protected override bool Execute(TaskContext context, VesselInput input)
        {
            return false;
        }

        protected override async Task<bool> ExecuteAsync(TaskContext context, VesselInput input)
        {
            MessageHeader header = new(
                context.OrchestrationInstance.InstanceId,
                nameof(CreateVesselDetailsCommand),
                nameof(CreateVesselDetailTask));

            CreateVesselDetailsCommand command = new()
            {
                Header = header,
                VesselName = input.VesselName
            };

            await using ServiceBusClient client = new("fixme: SB connection string");
            await using ServiceBusMessageSender sender = new(client);
            await sender.SendAsync(command, "fixme: queue name Vessel Detail service to listen to").ConfigureAwait(false);

            return true;
        }
    }
}
