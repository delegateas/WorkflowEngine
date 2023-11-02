using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine;
using WorkflowEngine.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public class WorkflowStarterBackgroundJob : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkflowStarterBackgroundJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var first = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var sp = scope.ServiceProvider;

                var workflows = sp.GetRequiredService<IWorkflowRepository>();
                var jobs = sp.GetRequiredService<IRecurringJobManager>();

                foreach (var workflow in await workflows.GetAllWorkflows())
                {
                    var trigger = workflow.Manifest.Triggers.FirstOrDefault(t => t.Value.Type == "TimerTrigger");

                    if (!trigger.Equals(default(KeyValuePair<string, TriggerMetadata>)))
                    {
                         
                        

                        jobs.AddOrUpdate<IHangfireWorkflowExecutor>(workflow.Id.ToString(),
                            (executor) => executor.TriggerAsync(new TriggerContext
                            {
                                Workflow = workflow,
                                Trigger = new Trigger
                                {
                                    Inputs = trigger.Value.Inputs,
                                    ScheduledTime = DateTimeOffset.UtcNow,
                                    Type = trigger.Value.Type,
                                    Key = trigger.Key
                                },
                            },null), trigger.Value.Inputs["cronExpression"] as string);

                        if (first && trigger.Value.Inputs.ContainsKey("runAtStartup") && (bool)trigger.Value.Inputs["runAtStartup"])
                            jobs.Trigger(workflow.Id.ToString());
                    }
                }



                first = false;
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);



            }


        }
    }
}
