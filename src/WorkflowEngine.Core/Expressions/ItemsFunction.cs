using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Expressions
{
   // [FunctionName("items")]
    public class ItemsFunction : IFunction
    {
        private readonly IOutputsRepository outputsRepository;
        private readonly IRunContextAccessor runContextFactory;

        public ItemsFunction(IOutputsRepository outputsRepository, IRunContextAccessor runContextFactory) //: base("items")
        {
            this.outputsRepository=outputsRepository;
            this.runContextFactory=runContextFactory;
        }



        public async ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var run = runContextFactory.RunContext;
            var id = run.RunId;
         
            var triggerData = JToken.FromObject(await outputsRepository.GetOutputData(id, parameters[0].ToString()));

            var parsed = await ValueContainerExtension.CreateValueContainerFromJToken(triggerData);

            var body = parsed["body"];
            var item = body["item"];

            return item;
        }
    }
}
