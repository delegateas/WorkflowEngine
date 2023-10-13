namespace WorkflowEngine.Core
{
    public abstract class Event
    {
        public EventType EventType { get; }

        protected Event(EventType eventType)
        {
            EventType = eventType;
        }
    }
}
