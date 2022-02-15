using System;

namespace WorkflowEngine.Core
{
    public class TriggerContext : ITriggerContext, IFormattable
    {
        public IWorkflow Workflow { get; set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if(format =="WorkFlow")
            {   
                return Workflow?.GetType().Name.ToString();
            }
            return string.Empty;
        }
    }


}
