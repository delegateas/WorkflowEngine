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

                    foreach (var trigger in workflow.Manifest.Triggers.Where(t => t.Value.Type == "TimerTrigger"))
                    {

                        if (!trigger.Equals(default(KeyValuePair<string, TriggerMetadata>)))
                        {

                            workflow.Manifest = null;

                            jobs.AddOrUpdate(workflow.Id.ToString() + trigger.Key,
                                (System.Linq.Expressions.Expression<System.Action<IHangfireWorkflowExecutor>>)((executor) => executor.TriggerAsync(new TriggerContext
                                {
                                    Workflow = workflow,
                                    Trigger = new Trigger
                                    {
                                        Inputs = trigger.Value.Inputs,
                                        ScheduledTime = DateTimeOffset.UtcNow,
                                        Type = trigger.Value.Type,
                                        Key = trigger.Key
                                    },
                                }, null)), trigger.Value.Inputs["cronExpression"] as string,GetTimeZone(trigger) );

                            if (first && trigger.Value.Inputs.ContainsKey("runAtStartup") && (bool)trigger.Value.Inputs["runAtStartup"])
                                jobs.Trigger(workflow.Id.ToString() + trigger.Key);
                        }
                    }
                }



                first = false;
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);



            }

            static TimeZoneInfo GetTimeZone(KeyValuePair<string, TriggerMetadata> trigger)
            {
               
                if (trigger.Value.Inputs.ContainsKey("timezone") && trigger.Value.Inputs["timezone"] is string zone && !string.IsNullOrWhiteSpace(zone))
                    return TimeZoneInfo.FindSystemTimeZoneById(zone) ?? TimeZoneInfo.Utc;
                return TimeZoneInfo.Utc;
               
            }
        }
    }
}
