using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionImplementation
    {
         
       
        ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action);


    }
    


}
