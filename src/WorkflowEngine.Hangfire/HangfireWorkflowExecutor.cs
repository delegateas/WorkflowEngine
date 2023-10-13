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
            var job = backgroundJobClient.Enqueue<IHangfireWorkflowExecutor>(
                        (executor) => executor.TriggerAsync(trigger));

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
            //TODO - avoid sending all workflow over hangfire, so we should lookup the manifest here if not set on workflow form its ID.
            workflow.Manifest ??= await workflowAccessor.GetWorkflowManifestAsync(workflow);

            runContextAccessor.RunContext = run;
            arrayContext.JobId = context.BackgroundJob.Id;

            try
            {
                var result = await actionExecutor.ExecuteAsync(run, workflow, action);
                
                await outputRepository.AddEvent(run, workflow, action, new ActionCompletedEvent
                {
                    // TODO: Add path to action in state
                    // assignees: thygesteffensen
                    JobId = context.BackgroundJob.Id,
                    ActionKey = action.Key
                });

                if (result != null)
                {
                    var next = await executor.GetNextAction(run, workflow, result);
                    

                    await hangfireActionExecutorResultHandler.InspectAsync(run, workflow, result, next);

                    if (next != null)
                    {
                        // This is the hangfire ID thingy, this we would like to save
                        var workflowRunId = backgroundJobClient.Enqueue<IHangfireActionExecutor>(
                                   (executor) => executor.ExecuteAsync(run, workflow, next, null));
                        // result.
                    }
                    else if (workflow.Manifest.Actions.FindParentAction(action.Key) is ForLoopActionMetadata scope)
                    {
                        var scopeAction = run.CopyTo(new Action { ScopeMoveNext = true, Type = scope.Type, Key = action.Key.Substring(0, action.Key.LastIndexOf('.')), ScheduledTime = DateTimeOffset.UtcNow });

                        var workflowRunId = backgroundJobClient.Enqueue<IHangfireActionExecutor>(
                                 (executor) => executor.ExecuteAsync(run, workflow, scopeAction, null));
                        //await outputRepository.EndScope(run, workflow, action);
                    }
                    else if (result.Status == "Failed" && result.ReThrow)
                    {
                        await outputRepository.AddEvent(run, workflow, action, new WorkflowFinishedEvent());
                        throw new InvalidOperationException("Action failed: " + result.FailedReason) { Data = { ["ActionResult"] = result } };
                    }
                    else
                    {
                        await outputRepository.AddEvent(run, workflow, action, new WorkflowFinishedEvent());
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
        
        /// <summary>
        /// Runs on the background process in hangfire
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<object> TriggerAsync(ITriggerContext context)
        {
            //TODO - avoid sending all workflow over hangfire,
            context.Workflow.Manifest ??= await workflowAccessor.GetWorkflowManifestAsync(context.Workflow);

            context.RunId = context.RunId == Guid.Empty ? Guid.NewGuid() : context.RunId;

            runContextAccessor.RunContext = context;
            var action = await executor.Trigger(context);

            if (action != null)
            {
                //TODO - avoid sending all workflow over hangfire, so we should wipe the workflow.manifest before scheduling and restore it after.
                context.Workflow.Manifest = null;

                var a = backgroundJobClient.Enqueue<IHangfireActionExecutor>(
                            (executor) => executor.ExecuteAsync(context, context.Workflow, action, null));
            }
            return action;
        }
    }
}

