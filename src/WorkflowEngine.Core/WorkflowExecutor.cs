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
        public ValueTask<IAction> GetNextAction(IRunContext context, IWorkflow workflow, IActionResult priorResult)
        {
            logger.LogInformation("Finding Next Action for {WorkflowId} and prior {@result} ", workflow.Id, priorResult);
            //var action = workflow.Manifest.Actions.Single(c => c.Key == priorResult.Key);

            
            var next = workflow.Manifest.Actions.FindNextAction(priorResult.Key);
            //var parent = workflow.Manifest.Actions.FindParentAction(priorResult.Key) is ForLoopActionMetadata;

            if (next.IsDefault())
                return new ValueTask<IAction>();

            if (next.Value.ShouldRun(priorResult.Key,priorResult.Status)) // .RunAfter[priorResult.Key].Contains(priorResult.Status))
            {
                return new ValueTask<IAction>(new Action { RunId=context.RunId, Type = next.Value.Type, Key=next.Key, ScheduledTime=DateTimeOffset.UtcNow });
            }

            return new ValueTask<IAction>();
        }

       

        public async ValueTask<IAction> Trigger(ITriggerContext context)
        {

            await outputsRepository.AddAsync(context,context.Workflow, context.Trigger);
           
            var action = context.Workflow.Manifest.Actions.SingleOrDefault(c => c.Value.RunAfter?.Count == 0);


            if (action.IsDefault())
                return null;

            return new Action { Type = action.Value.Type, Key=action.Key, ScheduledTime = DateTimeOffset.UtcNow, RunId = context.RunId };
        }
    }


}
