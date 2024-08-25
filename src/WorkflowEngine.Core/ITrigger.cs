using System;
using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public interface ITrigger
    {
        DateTimeOffset ScheduledTime { get; set; }
        string Type { get; set; }

        object Inputs { get; set; }

        string Key { get; }
    }

    public class Trigger : ITrigger
    {
        public DateTimeOffset ScheduledTime { get; set; }
        public string Type { get; set; }

        public object Inputs { get; set; } = new Dictionary<string, object>();

        public string Key { get; set; }
    }

}
