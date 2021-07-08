using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public class DefaultWorkflowRepository : IWorkflowRepository
    {
        private readonly IWorkflow[] workflows;

        public DefaultWorkflowRepository(IEnumerable<IWorkflow> workflows)
        {
            this.workflows = workflows?.ToArray() ?? throw new ArgumentNullException(nameof(workflows));
        }

        public ValueTask<IWorkflow[]> GetAllWorkflows()
        {
            return new ValueTask<IWorkflow[]>(  workflows);
        }
    }


}
