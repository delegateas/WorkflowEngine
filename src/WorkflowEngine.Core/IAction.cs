using System;

namespace WorkflowEngine.Core
{
    public interface IAction
    {
        public DateTimeOffset ScheduledTime { get; set; }
        public string Type { get; set; }
        string Key { get; }
    }


}
