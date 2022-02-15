using System;
using System.Linq;

namespace WorkflowEngine.Core
{
    public class Workflow : IWorkflow, IFormattable
    {
        public Guid Id { get; set; }
        public string Version { get; set; }
        public WorkflowManifest Manifest { get; set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "Id")
                return Id.ToString();

            if (format == "Version")
                return Version;

            if (format == "Trigger")
                return Manifest.Triggers.FirstOrDefault().Value?.Type;

            return string.Empty;
        }
    }


}
