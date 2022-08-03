using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Expressions
{
    public class OutputsFunction : Function
    {
        private readonly IOutputsRepository outputsRepository;
        private readonly IRunContextAccessor runContextFactory;
        private readonly IScopeContext scopeContext;

        public OutputsFunction(IOutputsRepository outputsRepository, IRunContextAccessor runContextFactory, IScopeContext scopeContext) : base("outputs")
        {
            this.outputsRepository=outputsRepository;
            this.runContextFactory=runContextFactory;
            this.scopeContext=scopeContext;
        }
        public override async ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var run = runContextFactory.RunContext;
            var id = run.RunId;

            var key = parameters[0].ToString();
            if (scopeContext.Scope.Contains("."))
            {   
                key = $"{scopeContext.Scope.Substring(0, scopeContext.Scope.LastIndexOf('.'))}.{key}";
            }

            var triggerData = JToken.FromObject(await outputsRepository.GetOutputData(id, key));

            var parsed = await ValueContainerExtension.CreateValueContainerFromJToken(triggerData);

            return parsed;
        }
    }
}
