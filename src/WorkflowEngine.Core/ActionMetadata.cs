using System.Collections;
using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public class ActionMetadata
    {
        public WorkflowRunAfterActions RunAfter { get; set; } = new WorkflowRunAfterActions();
        public string Type { get;  set; }
        public IDictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();

    }


}
