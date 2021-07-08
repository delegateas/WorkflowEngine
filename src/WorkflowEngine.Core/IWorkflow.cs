using System;

namespace WorkflowEngine.Core
{
    public interface IWorkflow // Documentet
    {
        public Guid Id { get;  }
        public string Version { get;  }
        public WorkflowManifest Manifest { get; }

    }


}
