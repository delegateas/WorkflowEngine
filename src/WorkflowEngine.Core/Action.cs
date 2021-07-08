using System;

namespace WorkflowEngine.Core
{
    public class Action : IAction
    {
        public DateTimeOffset ScheduledTime { get; set; }

        public string Type { get; set; }
        public string Key { get; set; }
    }


}
