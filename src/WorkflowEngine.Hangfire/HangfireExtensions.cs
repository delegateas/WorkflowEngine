using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{


    internal sealed class HangfireWorkflowManifestJobFilterAttribute : JobFilterAttribute
    {
    }
    public sealed class HangfireWorkflowManifestJobFilter :  IServerFilter, IClientFilter
    {
        private readonly IWorkflowAccessor _workflowAccessor;

        public HangfireWorkflowManifestJobFilter(IWorkflowAccessor workflowAccessor)
        {
            _workflowAccessor = workflowAccessor ?? throw new System.ArgumentNullException(nameof(workflowAccessor));
        }
        public void OnCreated(CreatedContext filterContext)
        {
            foreach (var workflow in filterContext.BackgroundJob.Job.Args.OfType<IWorkflow>())
            {
                workflow.Manifest = _workflowAccessor.GetWorkflowManifestAsync(workflow).GetAwaiter().GetResult();
            }

            foreach (var workflow in filterContext.BackgroundJob.Job.Args.OfType<ITriggerContext>())
            {

                workflow.Workflow.Manifest = _workflowAccessor.GetWorkflowManifestAsync(workflow.Workflow).GetAwaiter().GetResult();
            }
        }

        public void OnCreating(CreatingContext filterContext)
        {
           foreach(var workflow in filterContext.Job.Args.OfType<IWorkflow>())
            {
                workflow.Manifest = null;
            }

            foreach (var workflow in filterContext.Job.Args.OfType<ITriggerContext>())
            {
                workflow.Workflow.Manifest = null;
            }

            
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            
            foreach (var workflow in filterContext.BackgroundJob.Job.Args.Where(c => c is IWorkflow))
            {

            }
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            foreach (var workflow in filterContext.BackgroundJob.Job.Args.OfType<IWorkflow>())
            {
                workflow.Manifest = _workflowAccessor.GetWorkflowManifestAsync(workflow).GetAwaiter().GetResult();
            }

            foreach (var workflow in filterContext.BackgroundJob.Job.Args.OfType<ITriggerContext>())
            {
               
                workflow.Workflow.Manifest = _workflowAccessor.GetWorkflowManifestAsync(workflow.Workflow).GetAwaiter().GetResult();
            }
        }
    }
    public static class HangfireExtensions
    {


        public static void SetJobExternalKey(this IStorageConnection connection, string externalId, string jobId)
        {
            // This method can be implemented in 1.1.0
            connection.SetRangeInHash($"x-backgroundjob-keys:{externalId}", new[] { new KeyValuePair<string, string>("JobId", jobId) });
        }

        public static string GetJobIdByKey(this IStorageConnection connection, string externalId)
        {
            // This method can be implemented in 1.1.0
            var entries = connection.GetAllEntriesFromHash($"x-backgroundjob-keys:{externalId}");
            if (entries == null || !entries.ContainsKey("JobId"))
                return null;

            return entries["JobId"];
        }
    }
}
