using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public class TriggerMetadata
    {
      
        public string Type { get; set; }

        public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();
    }


}
