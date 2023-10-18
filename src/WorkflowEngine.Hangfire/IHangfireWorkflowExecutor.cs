using Hangfire;
using Hangfire.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{
    public interface IHangfireWorkflowExecutor
    {
        [JobDisplayName("Trigger: {0:Workflow} RunId={0:Id}")]
        public ValueTask<object> TriggerAsync(ITriggerContext triggercontext, PerformContext context);
    }

}
