using Hangfire;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Actions;
using WorkflowEngine.Core.Expressions;
using Action = WorkflowEngine.Core.Action;

namespace WorkflowEngine
{

    public interface IWorkflowAccessor
    {
        ValueTask<WorkflowManifest> GetWorkflowManifestAsync(IWorkflow workflow);
    }
    public class DefaultWorkflowAccessor : IWorkflowAccessor
    {
        private readonly IEnumerable<IWorkflow> workflows;

        public DefaultWorkflowAccessor(IEnumerable<IWorkflow> workflows)
        {
            this.workflows = workflows ?? throw new ArgumentNullException(nameof(workflows));
        }
        public ValueTask<WorkflowManifest> GetWorkflowManifestAsync(IWorkflow workflow)
        {
            return new ValueTask<WorkflowManifest>(workflows.FirstOrDefault(x => x.Id == workflow.Id && workflow.Version == x.Version).Manifest);
        }
    }

    public static class BackgroundClientExtensions
    {
        public static string TriggerAsync<TTriggerContext>(this IBackgroundJobClient backgroundJobClient, TTriggerContext trigger)
            where TTriggerContext : TriggerContext
        {
            var job = backgroundJobClient.Enqueue<IHangfireWorkflowExecutor>(trigger.Queue ?? "default",
                        (executor) => executor.TriggerAsync(trigger, null));

            return job;

        }
    }
    public interface IHangfireActionExecutorResultInspector
    {
        Task InspectAsync(IRunContext run, IWorkflow workflow, IActionResult result, IAction next);
    }
    public class DefaultHangfireActionExecutorResultInspector : IHangfireActionExecutorResultInspector
    {
        public Task InspectAsync(IRunContext run, IWorkflow workflow, IActionResult result, IAction next)
        {
            return Task.CompletedTask;
        }
    }
    public class HangfireWorkflowExecutor : IHangfireWorkflowExecutor, IHangfireActionExecutor
    {
        private readonly IWorkflowAccessor workflowAccessor;
        private readonly IHangfireActionExecutorResultInspector hangfireActionExecutorResultHandler;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IRunContextAccessor runContextAccessor;
        private readonly IWorkflowExecutor executor;
        private readonly IActionExecutor actionExecutor;
        private readonly IOutputsRepository outputRepository;
        private readonly IArrayContext arrayContext;

        public HangfireWorkflowExecutor(IWorkflowAccessor workflowAccessor, IHangfireActionExecutorResultInspector hangfireActionExecutorResultHandler, IBackgroundJobClient backgroundJobClient, IArrayContext arrayContext, IRunContextAccessor runContextAccessor, IWorkflowExecutor executor, IActionExecutor actionExecutor, IOutputsRepository actionResultRepository)
        {
            this.workflowAccessor = workflowAccessor ?? throw new ArgumentNullException(nameof(workflowAccessor));
            this.hangfireActionExecutorResultHandler = hangfireActionExecutorResultHandler ?? throw new ArgumentNullException(nameof(hangfireActionExecutorResultHandler));
            this.backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
            this.arrayContext = arrayContext ?? throw new ArgumentNullException(nameof(arrayContext));
            this.runContextAccessor = runContextAccessor;
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this.actionExecutor = actionExecutor ?? throw new ArgumentNullException(nameof(actionExecutor));
            this.outputRepository = actionResultRepository;
        }

        /// <summary>
        /// Runs on the background process in hangfire
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="action"></param>
        /// <returns></returns>

        public async ValueTask<object> ExecuteAsync(IRunContext run, IWorkflow workflow, IAction action, PerformContext context)
        {
            try
            {
                runContextAccessor.RunContext = run;
              
                var queue = context.BackgroundJob.Job.Queue ?? "default";
                arrayContext.JobId = context.BackgroundJob.Id;
                arrayContext.Queue = queue;

                var result = await actionExecutor.ExecuteAsync(run, workflow, action);


                await outputRepository.AddEvent(run, workflow, action, ActionCompletedEvent.FromAction(result, action, context.BackgroundJob.Id));

                if (result != null)
                {
                    var next = await executor.GetNextAction(run, workflow, action, result);
                   

                    await hangfireActionExecutorResultHandler.InspectAsync(run, workflow, result, next);

                    if (next != null)
                    {
                      
                        if (result.DelayNextAction.HasValue)
                        {
                            
                            var workflowRunId = backgroundJobClient.Schedule<IHangfireActionExecutor>(queue,
                                     (executor) => executor.ExecuteAsync(run, workflow, next, null),result.DelayNextAction.Value);
                        }
                        else
                        {
                            var workflowRunId = backgroundJobClient.Enqueue<IHangfireActionExecutor>(queue,
                                       (executor) => executor.ExecuteAsync(run, workflow, next, null));
                        }


                         
                    }
                    else if (workflow.Manifest.Actions.FindParentAction(action.Key) is IScopedActionMetadata scope)
                    {
                        var scopeAction = run.CopyTo(new Action { 
                            Index = action.Index,
                            ScopeMoveNext = true,
                            Type = scope.Type,
                            Key = action.Key.Substring(0, action.Key.LastIndexOf('.')),
                            ScheduledTime = DateTimeOffset.UtcNow +(result.DelayNextAction ?? TimeSpan.Zero) });

                        if (result.DelayNextAction != null)
                        {

                            var workflowRunId = backgroundJobClient.Schedule<IHangfireActionExecutor>(queue,
                                     (executor) => executor.ExecuteAsync(run, workflow, scopeAction, null),result.DelayNextAction.Value);
                        }
                        else
                        {


                            var workflowRunId = backgroundJobClient.Enqueue<IHangfireActionExecutor>(queue,
                                     (executor) => executor.ExecuteAsync(run, workflow, scopeAction, null));
                        }
                        //await outputRepository.EndScope(run, workflow, action);
                    }
                    else if (result.Status == "Failed" && result.ReThrow)
                    {
                        await outputRepository.AddEvent(run, workflow, action, WorkflowEvent.CreateFinishedEvent(context.BackgroundJob.Id, result));
                        throw new InvalidOperationException("Action failed: " + result.FailedReason) { Data = { ["ActionResult"] = result } };
                    }
                    else if ( workflow.Manifest.Actions.FindAction(action.Key) is IScopedActionMetadata scopedAction && arrayContext.HasMore)
                    {
                        
                    }
                    else
                    {
                        await outputRepository.AddEvent(run, workflow, action, WorkflowEvent.CreateFinishedEvent(context.BackgroundJob.Id, result));
                    }
                }

                return result;
            }
            catch (InvalidOperationException ex)
            {
                context.SetJobParameter("RetryCount", 999);
                throw;
            }
        }

        public async ValueTask<object> TriggerAsync(ITriggerContext triggerContext)
        {
            return await TriggerAsync(triggerContext, null);

        }

        /// <summary>
        /// Runs on the background process in hangfire
        /// </summary>
        /// <param name="triggerContext"></param>
        /// <returns></returns>
        public async ValueTask<object> TriggerAsync(ITriggerContext triggerContext, PerformContext context = null)
        {
            var queue = context.BackgroundJob.Job.Queue ?? "default";
            triggerContext.RunId = triggerContext.RunId == Guid.Empty ? Guid.NewGuid() : triggerContext.RunId;
            triggerContext.JobId = context?.BackgroundJob.Id;

            runContextAccessor.RunContext = triggerContext;
            var action = await executor.Trigger(triggerContext);

            if (action != null)
            {   
                var a = backgroundJobClient.Enqueue<IHangfireActionExecutor>(queue,
                            (executor) => executor.ExecuteAsync(triggerContext, triggerContext.Workflow, action, null));
            }
            return action;
        }
    }
}

