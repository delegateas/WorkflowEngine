namespace WorkflowEngine.Core
{
    public class WorkflowFinishedEvent : Event
    {
        public WorkflowFinishedEvent() : base(EventType.WorkflowFinished)
        {
        }
    }
}
