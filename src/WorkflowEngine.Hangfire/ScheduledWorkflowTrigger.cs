using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{

    public class ScheduledWorkflowTrigger<TWorkflow> where TWorkflow : IWorkflow, new()
    {
        private readonly ILogger _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IStorageConnection _storageConnection;

        public ScheduledWorkflowTrigger(ILogger<ScheduledWorkflowTrigger<TWorkflow>> logger, IBackgroundJobClient backgroundJobClient, JobStorage storageConnection)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _storageConnection = storageConnection.GetConnection();
        }

        [JobNaturalKey("{0}")]
        public Task<string> Trigger(string externalid, bool create, DateTimeOffset time, Dictionary<string, object> inputs)
        {
            var existingJob = _storageConnection.GetJobIdByKey(externalid);
            if (!string.IsNullOrEmpty(existingJob))
            {
                _logger.LogInformation("Cleaning up existing Hangfire Job {JobID}", existingJob);
                _backgroundJobClient.Delete(existingJob);
            }

            if (create)
            {
                var workflow = new TWorkflow();
                workflow.Manifest.Triggers.First().Value.Inputs = inputs;

                var job = _backgroundJobClient.Schedule<IHangfireWorkflowExecutor>((executor) => executor.TriggerAsync(
                    new TriggerContext { Workflow = workflow, }), time);

                _logger.LogInformation("Created scheduled workflow job {JobID}", job);

                return Task.FromResult(job);
            }

            return Task.FromResult<string>(null);
        }

    }
}
