namespace WorkflowEngine.Core
{
    public class WorkflowManifest
    {
        public WorkflowTriggers Triggers { get; set; } = new WorkflowTriggers();
        public WorkflowActions Actions { get; set; } = new WorkflowActions();
    }


}
