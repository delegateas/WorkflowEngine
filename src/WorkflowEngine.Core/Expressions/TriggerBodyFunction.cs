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
        private readonly TriggerOutputsFunction _triggerOutputsFunction;

        public TriggerBodyFunction(TriggerOutputsFunction triggerOutputsFunction) 
        {
            this._triggerOutputsFunction=triggerOutputsFunction;
        }
        public async ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var trigger = await _triggerOutputsFunction.ExecuteFunction();

            return trigger["body"];

           
        }
    }
}
