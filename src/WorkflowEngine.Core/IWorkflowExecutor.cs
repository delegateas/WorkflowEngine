using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IWorkflowExecutor
    {
        public ValueTask<IAction> Trigger(ITriggerContext context);
        public ValueTask<IAction> GetNextAction(IWorkflow workflow, IActionResult priorResult);

    }


}
