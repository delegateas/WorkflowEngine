using Hangfire;
using Hangfire.Server;
using System;
using System.Threading.Tasks;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Actions;
using WorkflowEngine.Core.Expressions;
using Action = WorkflowEngine.Core.Action;

namespace WorkflowEngine
{
   
    public class HangfireWorkflowExecutor : IHangfireWorkflowExecutor, IHangfireActionExecutor
    {
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IRunContextAccessor runContextAccessor;
        private readonly IWorkflowExecutor executor;
        private readonly IActionExecutor actionExecutor;
        private readonly IOutputsRepository outputRepository;
        private readonly IArrayContext arrayContext;

        public HangfireWorkflowExecutor(IBackgroundJobClient backgroundJobClient, IArrayContext arrayContext, IRunContextAccessor runContextAccessor, IWorkflowExecutor executor, IActionExecutor actionExecutor, IOutputsRepository actionResultRepository)
        {
            this.backgroundJobClient=backgroundJobClient??throw new ArgumentNullException(nameof(backgroundJobClient));
            this.arrayContext=arrayContext??throw new ArgumentNullException(nameof(arrayContext));
            this.runContextAccessor=runContextAccessor;
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this.actionExecutor = actionExecutor ?? throw new ArgumentNullException(nameof(actionExecutor));
            this.outputRepository=actionResultRepository;
        }

        /// <summary>
        /// Runs on the background process in hangfire
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="action"></param>
        /// <returns></returns>

        public async ValueTask<object> ExecuteAsync(IRunContext run, IWorkflow workflow, IAction action, PerformContext context)
        {
            runContextAccessor.RunContext = run;
            arrayContext.JobId=context.BackgroundJob.Id;


            var result = await actionExecutor.ExecuteAsync(run, workflow, action);
            
           

            if (result != null)
            {
                var next = await executor.GetNextAction(run, workflow, result);

                if (next != null)
                {
                    var a = backgroundJobClient.Enqueue<IHangfireActionExecutor>(
                               (executor) => executor.ExecuteAsync(run, workflow, next,null));
                }else if(workflow.Manifest.Actions.FindParentAction(action.Key) is ForLoopActionMetadata scope)
                {

                    var scopeaction= new Action {ScopeMoveNext=true, RunId=run.RunId, Type = scope.Type, Key=action.Key.Substring(0, action.Key.LastIndexOf('.')), ScheduledTime=DateTimeOffset.UtcNow };


                    var a = backgroundJobClient.Enqueue<IHangfireActionExecutor>(
                             (executor) => executor.ExecuteAsync(run, workflow, scopeaction, null));

                    //await outputRepository.EndScope(run, workflow, action);
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
            runContextAccessor.RunContext = context;
            var action = await executor.Trigger(context);

            if (action != null)
            {

                var a = backgroundJobClient.Enqueue<IHangfireActionExecutor>(
                            (executor) => executor.ExecuteAsync(context, context.Workflow, action,null));
            }
            return action;
        }
    }

}
