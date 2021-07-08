using System;

namespace WorkflowEngine.Core
{
    public interface ITrigger
    {
        public DateTimeOffset ScheduledTime { get; set; }
        public string Type { get; set; }

    }


}
