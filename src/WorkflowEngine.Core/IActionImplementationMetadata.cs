using System;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionImplementationMetadata{
        
        string Type { get;  }
        Type Implementation { get;  }

        ValueTask<ActionResult> ExecuteAsync(IServiceProvider services, IRunContext context, IWorkflow workflow, IAction action);
    }
    
    

}
