namespace WorkflowEngine.Core
{
    public class ActionCompletedEvent : Event
    {
        public ActionCompletedEvent() : base(EventType.ActionCompleted)
        {
        }
        
        public string JobId { get; set; }
        public string ActionKey { get; set; }
        public string ResultPath { get; set; }
    }
}
