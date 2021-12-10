using DurableTask.Core;

namespace GatewayApi.Sagas.FirstRegistry
{
    public static class FirstRegistryTaskHubWorkerExtensions
    {
        public static void AddFirstRegistrySaga(this TaskHubWorker worker)
        {
            worker.AddTaskOrchestrations(typeof(FirstRegistryOrchestration));
            worker.AddTaskActivities(
                typeof(CreateRegistryTask), 
                typeof(CreateVesselDetailTask), 
                typeof(UndoVesselDetailTask));
        }
    }
}
