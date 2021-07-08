using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{
    public interface IHangfireActionExecutor
    {
        [DisplayName("Action: {0}")]
        public ValueTask<object> ExecuteAsync(string type, IWorkflow workflow, IAction action);
    }
    public interface IHangfireWorkflowExecutor
    {
        [DisplayName("Trigger: {0}")]
        public ValueTask<object> TriggerAsync(ITriggerContext context);
    }

    public class HangfireWorkflowExecutor : IHangfireWorkflowExecutor, IHangfireActionExecutor
    {
        private readonly IWorkflowExecutor executor;
        private readonly IActionExecutor actionExecutor;

        public HangfireWorkflowExecutor(IWorkflowExecutor executor, IActionExecutor actionExecutor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this.actionExecutor = actionExecutor ?? throw new ArgumentNullException(nameof(actionExecutor));
        }

        /// <summary>
        /// Runs on the background process in hangfire
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="action"></param>
        /// <returns></returns>

        public async ValueTask<object> ExecuteAsync(string type, IWorkflow workflow, IAction action)
        {
            var result = await actionExecutor.ExecuteAsync(workflow, action);

            if (result != null)
            {
                var next = await executor.GetNextAction(workflow, result);

                if (next != null)
                {
                    var a = Hangfire.BackgroundJob.Enqueue<IHangfireActionExecutor>(
                               (executor) => executor.ExecuteAsync(next.Type, workflow, next));
                }
            }

            return result;
        }
        /// <summary>
        /// Runs on the background process in hangfire
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<object> TriggerAsync(ITriggerContext context)
        {
            var action = await executor.Trigger(context);

            if (action != null)
            {

                var a = Hangfire.BackgroundJob.Enqueue<IHangfireActionExecutor>(
                            (executor) => executor.ExecuteAsync(action.Type, context.Workflow, action));
            }
            return action;
        }
    }

}
