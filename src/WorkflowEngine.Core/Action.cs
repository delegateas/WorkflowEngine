using System;

namespace WorkflowEngine.Core
{
    public class Action : IAction, IFormattable
    {
        public DateTimeOffset ScheduledTime { get; set; }

        public string Type { get; set; }
        public string Key { get; set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "Type")
                return Type;

            return string.Empty;
        }
    }


}
