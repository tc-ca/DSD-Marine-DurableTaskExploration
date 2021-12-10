using Azure.Messaging.ServiceBus;
using Common.Messaging;
using Common.Messaging.Commands;
using DurableTask.Core;
using System.Threading.Tasks;

namespace GatewayApi.Sagas.FirstRegistry
{
    public class UndoVesselDetailTask : TaskActivity<string, bool>
    {
        protected override bool Execute(TaskContext context, string input)
        {
            return false;
        }

        protected override async Task<bool> ExecuteAsync(TaskContext context, string input)
        {
            MessageHeader header = new(
                context.OrchestrationInstance.InstanceId,
                nameof(UndoVesselDetailCommand),
                nameof(UndoVesselDetailTask));

            UndoVesselDetailCommand command = new()
            {
                Header = header,
                OfficialNumber = input
            };

            await using ServiceBusClient client = new("fixme: SB connection string");
            await using ServiceBusMessageSender sender = new(client);
            await sender.SendAsync(command, "fixme: queue name Vessel Detail service to listen to").ConfigureAwait(false);

            return true;
        }
    }
}
