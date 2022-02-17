using System;
using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public interface IAction : IRunContext
    {
        public DateTimeOffset ScheduledTime { get; set; }
        public string Type { get; set; }
        string Key { get; }
        IDictionary<string, object> Inputs { get; set; }

        public int Index { get; set; }
        public bool ScopeMoveNext { get; set; }
    }


}
