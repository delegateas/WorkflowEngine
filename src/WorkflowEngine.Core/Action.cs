using System;
using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public class Action : IAction, IFormattable
    {
        public DateTimeOffset ScheduledTime { get; set; }

        public string Type { get; set; }
        public string Key { get; set; }
        public Guid RunId { get;  set; }
        public IDictionary<string, object> Inputs { get; set; }
        public int Index { get; set; }
        public bool ScopeMoveNext { get; set; }

        public string PrincipalId { get; set; }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "Type")
                return Type;

            return string.Empty;
        }
    }


}
