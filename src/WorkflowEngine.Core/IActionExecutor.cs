using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionExecutor
    {
        public ValueTask<IActionResult> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action);

    }
    


}
