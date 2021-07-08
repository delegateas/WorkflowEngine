using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionExecutor
    {
        public ValueTask<IActionResult> ExecuteAsync(IWorkflow workflow, IAction action);

    }
    


}
