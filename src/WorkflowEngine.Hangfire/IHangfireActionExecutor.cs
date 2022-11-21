using Hangfire;
using Hangfire.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{
    public interface IHangfireActionExecutor
    {
        [JobDisplayName("Action: {2:Type}, RunId={0:Id} workflow={1:Id}")]
        public ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action, PerformContext hangfireContext);
    }

}
