using System;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IOutputsRepository
    {
        ValueTask AddAsync(IRunContext context, IWorkflow workflow, IAction action, IActionResult result);
        ValueTask AddTrigger(ITriggerContext context, IWorkflow workflow, ITrigger trigger);
        ValueTask<object> GetTriggerData(Guid id);
        ValueTask AddInput(IRunContext context, IWorkflow workflow, IAction action);
        ValueTask<object> GetOutputData(Guid id, string v);
        ValueTask AddArrayItemAsync(IRunContext run, IWorkflow workflow, string key, IActionResult result);
        ValueTask AddArrayInput(IRunContext context, IWorkflow workflow, IAction action);
      //  ValueTask StartScope(IRunContext context, IWorkflow workflow, IAction action);
        ValueTask AddScopeItem(IRunContext context, IWorkflow workflow, IAction action, IActionResult result);
        ValueTask EndScope(IRunContext run, IWorkflow workflow, IAction action);
        ValueTask AddEvent(IRunContext run, IWorkflow workflow, IAction action, Event @event);
    }
}
