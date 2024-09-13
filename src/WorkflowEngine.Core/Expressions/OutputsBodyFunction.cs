using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Expressions
{
    public class OutputsBodyFunction : IFunction
    {
        private readonly OutputsFunction _outputsFunction;

        public OutputsBodyFunction(OutputsFunction outputsFunction)
        {
            _outputsFunction = outputsFunction;
        }
        public async ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var output = await _outputsFunction.ExecuteFunction(parameters);

            return output?["body"];
        }
    }
}
