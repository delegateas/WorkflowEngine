using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public static class Extensions
    {
        public static bool IsDefault<T>(this T value) where T : struct
        {
            bool isDefault = value.Equals(default(T));

            return isDefault;
        }
    }

    public class WorkflowExecutor : IWorkflowExecutor
    {
        private readonly ILogger<WorkflowExecutor> logger;

        public WorkflowExecutor(ILogger<WorkflowExecutor> logger)
        {
            this.logger = logger;
        }
        public ValueTask<IAction> GetNextAction(IWorkflow workflow, IActionResult priorResult)
        {
            logger.LogInformation("Finding Next Action for {WorkflowId} and prior {@result}", workflow.Id, priorResult);
            //var action = workflow.Manifest.Actions.Single(c => c.Key == priorResult.Key);
            var next = workflow.Manifest.Actions.SingleOrDefault(c => c.Value.RunAfter.ContainsKey(priorResult.Key));
            
            if (next.IsDefault())
                return new ValueTask<IAction>();

            if (next.Value.RunAfter[priorResult.Key].Contains(priorResult.Status))
            {
                return new ValueTask<IAction>(new Action { Type = next.Value.Type, Key=next.Key, ScheduledTime=DateTimeOffset.UtcNow });
            }

            return new ValueTask<IAction>();
        }

        public ValueTask<IAction> Trigger(ITriggerContext context)
        {
            var action = context.Workflow.Manifest.Actions.SingleOrDefault(c => c.Value.RunAfter?.Count == 0);
            if (action.IsDefault())
                return new ValueTask<IAction>();

            return new ValueTask<IAction>(new Action { Type = action.Value.Type, Key=action.Key, ScheduledTime = DateTimeOffset.UtcNow });
        }
    }


}
