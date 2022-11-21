using System.Collections.Generic;
using System.Linq;

namespace WorkflowEngine.Core
{
    public class ForLoopActionMetadata : ActionMetadata, IScopedActionMetadata
    {
        public object ForEach { get; set; }
        public int ConcurrentCount { get; set; } = 1;

        public WorkflowActions Actions { get; set; } = new WorkflowActions();

    }
    public interface IScopedActionMetadata
    {
        WorkflowActions Actions { get; set; }
    }
    public class WorkflowActions : Dictionary<string, ActionMetadata>
    {
        public ActionMetadata FindAction(string key)
        {
            if (key.Contains("."))
            {
                var leg = key.Substring(0, key.IndexOf('.'));
                var child = this[leg];
                if(child is IScopedActionMetadata childactions)
                {
                    return childactions.Actions.FindAction(key.Substring(leg.Length+1));
                }
            }

            if(ContainsKey(key))
                return this[key];

            return null;
        }
        public KeyValuePair<string,ActionMetadata> FindNextAction(string key)
        {
            if (key.Contains("."))
            {
                var leg = key.Substring(0, key.IndexOf('.'));
                var child = this[leg];
                if (child is IScopedActionMetadata childactions)
                {
                    var a= childactions.Actions.FindNextAction(key.Substring(leg.Length+1));
                    if (a.IsDefault())
                        return a;

                    
                    return new KeyValuePair<string, ActionMetadata>(leg+"."+a.Key, a.Value);
                }
            }

            var next = this.SingleOrDefault(c => c.Value.RunAfter.ContainsKey(key));

            return next;
        }

        public ActionMetadata FindParentAction(string key)
        {
            if (key.Contains("."))
            {
                return FindAction(key.Substring(0, key.LastIndexOf('.')));
            }

            return null;
        }
    }


}
