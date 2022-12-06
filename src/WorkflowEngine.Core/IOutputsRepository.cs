using System;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IOutputsRepository
    {
        ValueTask AddAsync(IRunContext context, IWorkflow workflow, IAction action, IActionResult result);
        ValueTask AddAsync(IRunContext context, IWorkflow workflow, ITrigger trigger);
        ValueTask<object> GetTriggerData(Guid id);
        ValueTask AddInput(IRunContext context, IWorkflow workflow, IAction action);
        ValueTask<object> GetOutputData(Guid id, string v);
        ValueTask EndScope(IRunContext run, IWorkflow workflow, IAction action);
    }


}
