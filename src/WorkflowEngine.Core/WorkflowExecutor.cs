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
        private readonly IOutputsRepository outputsRepository;

        public WorkflowExecutor(ILogger<WorkflowExecutor> logger, IOutputsRepository  outputsRepository)
        {
            this.logger = logger??throw new ArgumentNullException(nameof(logger));
            this.outputsRepository=outputsRepository??throw new ArgumentNullException(nameof(outputsRepository));
        }
        public ValueTask<IAction> GetNextAction(IRunContext context, IWorkflow workflow, IAction action, IActionResult priorResult)
        {
            logger.LogInformation("Finding Next Action for {WorkflowId} and prior {@result} ", workflow.Id, priorResult);
             
            var next = workflow.Manifest.Actions.FindNextAction(priorResult.Key);
             
            if (next.IsDefault())
                return new ValueTask<IAction>();

            if (next.Value.ShouldRun(priorResult.Key,priorResult.Status)) // .RunAfter[priorResult.Key].Contains(priorResult.Status))
            {
                return new ValueTask<IAction>(context.CopyTo(new Action { Type = next.Value.Type, Key=next.Key, ScheduledTime=DateTimeOffset.UtcNow, Index = action.Index }));
            }

            return new ValueTask<IAction>();
        }

       

        public async ValueTask<IAction> Trigger(ITriggerContext context)
        {
            
            await outputsRepository.AddTrigger(context,context.Workflow, context.Trigger);
           
            var action = context.Workflow.Manifest.Actions.SingleOrDefault(c => c.Value.RunAfter?.Count == 0);


            if (action.IsDefault())
                return null;

            return context.CopyTo(
                new Action { Type = action.Value.Type, Key=action.Key, ScheduledTime = DateTimeOffset.UtcNow }
                );
        }
    }


}
