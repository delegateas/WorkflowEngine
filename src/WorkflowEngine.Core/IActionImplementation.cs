using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionForRegistration
    {
        
    }
    public interface IActionImplementation : IActionForRegistration
    {
         
       
        ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action);


    }
    public interface IActionImplementation<TInput>  : IActionForRegistration
    {


        ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction<TInput> action);


    }


}
