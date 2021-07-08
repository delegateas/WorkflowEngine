using System;

namespace WorkflowEngine.Core
{
    public class Workflow : IWorkflow
    {
        public Guid Id { get; set; }
        public string Version { get; set; }
        public WorkflowManifest Manifest { get; set; }
    }


}
