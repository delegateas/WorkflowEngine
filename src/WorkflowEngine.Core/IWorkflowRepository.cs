using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IWorkflowRepository
    {
        public ValueTask<IWorkflow[]> GetAllWorkflows();
    }


}
