namespace WorkflowEngine.Core
{
    public class TriggerContext : ITriggerContext
    {
        public IWorkflow Workflow { get; set; }
    }


}
