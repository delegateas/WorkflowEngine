using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionImplementation
    {
         
       
        ValueTask<object> ExecuteAsync(IWorkflow workflow, IAction action);


    }
    


}
