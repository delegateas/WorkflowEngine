using Hangfire;
using System.ComponentModel;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine
{
    public interface IHangfireActionExecutor
    {
        [JobDisplayName("Action: {0}, workflow={1:Id}")]
        public ValueTask<object> ExecuteAsync(string type, IWorkflow workflow, IAction action);
    }

}
