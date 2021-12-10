using Common.Messaging;
using Common.Messaging.Events;
using DurableTask.Core;
using System;
using System.Threading.Tasks;

namespace GatewayApi.Sagas.FirstRegistry
{
    public class FirstRegistryOrchestration : TaskOrchestration<SagaOutput, SagaInput, Event, string>
    {
        private TaskCompletionSource<Event> _resumeHandle;

        public override async Task<SagaOutput> RunTask(OrchestrationContext context, SagaInput input)
        {
            VesselInput vesselInput = new() 
            { 
                VesselName = input.VesselName
            };

            bool taskWasScheduled = await context.ScheduleTask<bool>(typeof(CreateVesselDetailTask), vesselInput);

            if (!taskWasScheduled)
            {
                return new SagaOutput() { Succeeded = false, IsDataConsistent = true };
            }

            Event result = await WaitForEvent();

            // It is not pretty to cust returned event type to one of its subtypes.
            // Perhaps one event with various properties would be a better choice.
            // Still the following logic should illustrate event-driven saga transitions.

            if (result is VesselDetailCreationFailed)
            {
                return new SagaOutput() { Succeeded = false, IsDataConsistent = true };
            }

            if (result is not VesselDetailCreated vesselDetailCreatedEvent)
            {
                throw new ApplicationException("Expected VesselDetailCreated or VesselDetailCreationFailed events only.");
            }

            RegistryInput registryInput = new()
            {
                OfficialNumber = vesselDetailCreatedEvent.OfficialNumber
            };

            taskWasScheduled = await context.ScheduleTask<bool>(typeof(CreateRegistryTask), registryInput);

            if (!taskWasScheduled)
            {
                return await UndoVesselDetailCreationAndExitSaga(context, vesselDetailCreatedEvent.OfficialNumber);
            }

            result = await WaitForEvent();

            if (result is VesselRegistryCreationFailed)
            {
                return await UndoVesselDetailCreationAndExitSaga(context, vesselDetailCreatedEvent.OfficialNumber);
            }

            if (result is not VesselRegistryCreated vesselRegistryCreatedEvent)
            {
                throw new ApplicationException("Expected VesselRegistryCreated or VesselRegistryCreationFailed events only.");
            }

            SagaOutput output = new() 
            { 
                IsDataConsistent = true,
                OfficialNumber = vesselDetailCreatedEvent.OfficialNumber,
                RegistryId = vesselRegistryCreatedEvent.RegistrationId,
                Succeeded = true
            };

            return output;
        }

        public override void OnEvent(OrchestrationContext context, string name, Event input)
        {
            _resumeHandle?.SetResult(input);
        }

        private async Task<Event> WaitForEvent()
        {
            _resumeHandle = new TaskCompletionSource<Event>();
            Event data = await _resumeHandle.Task;
            _resumeHandle = null;
            return data;
        }

        private async Task<SagaOutput> UndoVesselDetailCreationAndExitSaga(OrchestrationContext context, string officialNumber)
        {
            bool scheduled = await context.ScheduleTask<bool>(typeof(UndoVesselDetailTask), officialNumber);

            if (!scheduled)
            {
                return new SagaOutput() { Succeeded = false, IsDataConsistent = false };
            }

            Event result = await WaitForEvent();

            if (result is UndoVesselDetailsSucceeded)
            {
                return new SagaOutput() { Succeeded = false, IsDataConsistent = true };
            }
            else
            {
                return new SagaOutput() { Succeeded = false, IsDataConsistent = false };
            }
        }
    }
}
