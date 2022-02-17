using System;

namespace WorkflowEngine.Core
{
    public class TriggerContext : ITriggerContext, IFormattable
    {
        public IWorkflow Workflow { get; set; }
        public ITrigger Trigger { get; set; }

        public Guid RunId { get; set; }
        

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if(format =="Workflow")
            {   
                return $"{Workflow?.GetType().Name} workflow={Workflow?.Id} version={Workflow?.Version} trigger={Trigger?.Key}";
            }
            
            if (format == "Id")
                return RunId.ToString();

            return string.Empty;
        }
    }


}
