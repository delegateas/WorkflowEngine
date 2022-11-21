using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Expressions
{
    public class TriggerBodyFunction : IFunction
    {
        private readonly TriggerOutputsFunction triggerOutputsFunction;

        public TriggerBodyFunction(TriggerOutputsFunction triggerOutputsFunction) 
        {
            this.triggerOutputsFunction=triggerOutputsFunction;
        }
        public async ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var trigger = await triggerOutputsFunction.ExecuteFunction();

            return trigger["body"];

           
        }
    }
}
