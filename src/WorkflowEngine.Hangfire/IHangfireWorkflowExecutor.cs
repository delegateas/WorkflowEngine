using Hangfire;
using System.ComponentModel;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{
    public interface IHangfireWorkflowExecutor
    {
        [JobDisplayName("Trigger: {0:WorkFlow}")]
        public ValueTask<object> TriggerAsync(ITriggerContext context);
    }

}
