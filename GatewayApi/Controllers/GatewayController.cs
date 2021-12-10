using DurableTask.Core;
using DurableTask.Core.Tracking;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Settings;
using DurableTask.ServiceBus.Tracking;
using GatewayApi.Sagas.FirstRegistry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly OrchestratorService _orchestrator;

        public GatewayController(OrchestratorService orchestrator, ILogger<GatewayController> logger)
        {
            _logger = logger;
            _orchestrator = orchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok("OK"));
        }

        [HttpGet]
        [Route("{instanceId}/status", Name = nameof(GetStatus))]
        public async Task<IActionResult> GetStatus(string instanceId)
        {
            OrchestrationState state = await _orchestrator.GetOrchestrationStatusAsync(instanceId);         
            return Ok(new { state.Status, state.Output });
        }

        [HttpPost]
        [Route("firstregistry")]
        public async Task<ActionResult<string>> StartFirstRegistryTransaction(SagaInput input)
        {
            OrchestrationInstance instance = await _orchestrator.RunOrchestrationAsync(typeof(FirstRegistryOrchestration), input);

            return AcceptedAtRoute(
                nameof(GetStatus), 
                new { instance.InstanceId, instance.ExecutionId });
        }
    }
}
