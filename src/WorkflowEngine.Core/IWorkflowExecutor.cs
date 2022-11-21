using System;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IRunContext
    {
        Guid RunId { get; set; }
        
        string PrincipalId { get; set; }

        public T CopyTo<T>(T other)
            where T : IRunContext
        {
            other.RunId = RunId;
            other.PrincipalId = PrincipalId;
            return other;
        }
    }
    public interface IWorkflowExecutor
    {
        public ValueTask<IAction> Trigger(ITriggerContext context);
        public ValueTask<IAction> GetNextAction(IRunContext context, IWorkflow workflow, IActionResult priorResult);

    }


}
