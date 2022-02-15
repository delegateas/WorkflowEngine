using System;

namespace WorkflowEngine.Core
{
    public class ActionImplementationMetadata<T> : IActionImplementationMetadata 
        where T: IActionImplementation
    {
        public string Type { get; set; }
        public Type Implementation => typeof(T);
    }
    


}
