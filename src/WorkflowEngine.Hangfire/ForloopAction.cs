using ExpressionEngine;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Actions
{
    public interface IArrayContext
    {
        string JobId { get; set; }
        string Queue { get; set; }
        bool HasMore { get; set; }
    }
    public class ArrayContext : IArrayContext
    {
        public string JobId { get; set; }
        public string Queue { get; set; }
        public bool HasMore { get; set; }
    }

    public class ForeachAction : IActionImplementation
    {
        private readonly IExpressionEngine expressionEngine;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IArrayContext arrayContext;
        private readonly IWorkflowAccessor _workflowAccessor;

        public ForeachAction(IExpressionEngine expressionEngine, IBackgroundJobClient backgroundJobClient, IArrayContext arrayContext, IWorkflowAccessor workflowAccessor)
        {
            this.expressionEngine = expressionEngine ?? throw new ArgumentNullException(nameof(expressionEngine));
            this.backgroundJobClient = backgroundJobClient;
            this.arrayContext = arrayContext ?? throw new ArgumentNullException(nameof(arrayContext));
            _workflowAccessor = workflowAccessor ?? throw new ArgumentNullException(nameof(workflowAccessor));
        }
        public async ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
           
            var loop = workflow.Manifest.Actions.FindAction(action.Key) as ForLoopActionMetadata;
            
            arrayContext.HasMore = true;
           
            if (loop.ForEach is string str && str.Contains("@"))
            {
                var items = await expressionEngine.ParseToValueContainer(str);

                if (items.Type() == ExpressionEngine.ValueType.Array)
                {
                    var itemsToRunover = items.GetValue<List<ValueContainer>>();

                    if (loop.ConcurrentCount > 1)
                    {
                        var pageSize = itemsToRunover.Count / loop.ConcurrentCount;

                        foreach (var child in Enumerable.Range(0, loop.ConcurrentCount))
                        {
                            var nextactionmetadata = loop.Actions.SingleOrDefault(c => c.Value.RunAfter?.Count == 0);
                            if (nextactionmetadata.Equals(default(KeyValuePair<string, ActionMetadata>)))
                                throw new Exception("No action with no runafter could be found");

                            var nextaction = context.CopyTo(new Action { Type = nextactionmetadata.Value.Type, Key = $"{action.Key}.{nextactionmetadata.Key}", ScheduledTime = DateTimeOffset.UtcNow, Index = action.Index+1 });
                            
                            var a = backgroundJobClient.ContinueJobWith<IHangfireActionExecutor>(arrayContext.JobId,
                                    (executor) => executor.ExecuteAsync(context, workflow, nextaction, null));

                            return new { item = itemsToRunover[action.Index] };
                        }
                    }
                    else if (action.Index < itemsToRunover.Count)
                    {
                        
                        // var nextAction = new Action { Type = action.Type, Key=action.Key, ScheduledTime = DateTimeOffset.UtcNow, RunId = context.RunId, Index = action.Index+1 };

                        var nextactionmetadata = loop.Actions.SingleOrDefault(c => c.Value.RunAfter?.Count == 0);
                        if (nextactionmetadata.Equals(default(KeyValuePair<string, ActionMetadata>)))
                            throw new Exception("No action with no runafter could be found");

                        var nextaction = context.CopyTo(new Action { Type = nextactionmetadata.Value.Type, Key = $"{action.Key}.{nextactionmetadata.Key}", ScheduledTime = DateTimeOffset.UtcNow, Index = action.Index+1 });

                       
                        var a = backgroundJobClient.ContinueJobWith<IHangfireActionExecutor>(arrayContext.JobId, arrayContext.Queue,
                                (executor) => executor.ExecuteAsync(context, workflow, nextaction, null));

                        return new { item = itemsToRunover[action.Index] };
                    } 

                }
            }



            arrayContext.HasMore = false;

            return new { };
        }
    }
}
