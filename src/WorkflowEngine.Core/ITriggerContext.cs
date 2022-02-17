using System;

namespace WorkflowEngine.Core
{
    public interface ITriggerContext:IRunContext
    {
        IWorkflow Workflow { get; }
        ITrigger Trigger { get; set; }
       
       
    }


}
