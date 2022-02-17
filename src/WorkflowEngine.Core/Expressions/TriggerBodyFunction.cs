using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Expressions
{
    public class TriggerBodyFunction : Function
    {
        private readonly TriggerOutputsFunction triggerOutputsFunction;

        public TriggerBodyFunction(TriggerOutputsFunction triggerOutputsFunction) : base("triggerBody")
        {
            this.triggerOutputsFunction=triggerOutputsFunction;
        }
        public async override ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var trigger = await triggerOutputsFunction.ExecuteFunction();

            return trigger["body"];

           
        }
    }
}
