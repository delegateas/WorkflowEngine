using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowEngine.Core
{
    public static class TriggerContextExtensions
    {
        public static TriggerContext CreateTrigger(this IWorkflow workflow, Dictionary<string, object> inputs)
        {
            var trigger = new TriggerContext
            {
                Workflow = workflow,
                //  PrincipalId = httpcontext.User.FindFirstValue("sub"),
                Trigger = new Trigger
                {

                    Inputs = inputs,
                    ScheduledTime = DateTimeOffset.UtcNow,
                    Type = workflow.Manifest.Triggers.FirstOrDefault().Value.Type,
                    Key = workflow.Manifest.Triggers.FirstOrDefault().Key
                },
            };
            
            return trigger;
        }
    }
    public class TriggerContext : ITriggerContext, IFormattable
    {
        public IWorkflow Workflow { get; set; }
        public ITrigger Trigger { get; set; }
        public string PrincipalId { get; set; }

        public Guid RunId { get; set; }
        public string JobId { get; set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if(format =="Workflow")
            {   
                return $"{Workflow?.GetType().Name} workflow={Workflow?.Id} version={Workflow?.Version} trigger={Trigger?.Key}";
            }
            
            if (format == "Id" && RunId != Guid.Empty)
                return RunId.ToString();

            return string.Empty;
        }

      
    }


}
