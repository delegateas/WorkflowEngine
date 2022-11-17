using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Expressions
{
    public interface IRunContextAccessor
    {
        IRunContext RunContext { get; set; }
    }
    public class RunContextFactory : IRunContextAccessor
    {
        public IRunContext RunContext { get; set; }
    }
    
    public class TriggerOutputsFunction : IFunction
    {
        private readonly IOutputsRepository outputsRepository;
        private readonly IRunContextAccessor runContextFactory;

        public TriggerOutputsFunction(IOutputsRepository outputsRepository,IRunContextAccessor runContextFactory)
        {
            this.outputsRepository=outputsRepository;
            this.runContextFactory=runContextFactory;
        }
        public async ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var run = runContextFactory.RunContext;
            var id = run.RunId;
            var triggerData = await outputsRepository.GetTriggerData(id);
            
            return await ValueContainerExtension.CreateValueContainerFromJToken(JToken.FromObject(triggerData));
        }
    }
}
