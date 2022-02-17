using System;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IRunContext
    {
        Guid RunId { get; set; }
    }
    public interface IWorkflowExecutor
    {
        public ValueTask<IAction> Trigger(ITriggerContext context);
        public ValueTask<IAction> GetNextAction(IRunContext context, IWorkflow workflow, IActionResult priorResult);

    }


}
