﻿using System.Collections;
using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public class ActionMetadata
    {
        public WorkflowRunAfterActions RunAfter { get; set; } = new WorkflowRunAfterActions();
        public string Type { get;  set; }
        public IDictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();

    }


}
