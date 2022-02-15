using System;

namespace WorkflowEngine.Core
{
    public interface IActionImplementationMetadata{
         string Type { get;  }
        Type Implementation { get;  }
    }
    


}
