using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Core.Expressions;

namespace WorkflowEngine.Core
{
    public static class ExpressionEngineExtensions
    {
          
        public static IServiceCollection AddFunctions(this IServiceCollection services)
        {
            services.RegisterScopedFunctionAlias<TriggerBodyFunction>("triggerBody");
            services.RegisterScopedFunctionAlias<TriggerOutputsFunction>("triggerOutputs");
            services.RegisterScopedFunctionAlias<OutputsFunction>("outputs");
            services.RegisterScopedFunctionAlias<ItemsFunction>("items");
            
           
            return services;
        }
        public static async ValueTask<IDictionary<string,object>> ResolveInputs(this IExpressionEngine engine, ActionMetadata actionMetadata, ILogger logger)
        {

            var resolvedInputs = new Dictionary<string, object>();

            foreach (var input in actionMetadata.Inputs)
            {
                if (input.Value is string str && str.Contains("@"))
                {
                    resolvedInputs[input.Key] = await engine.ParseToValueContainer(str);
                }
                else
                {
                    resolvedInputs[input.Key] = input.Value;
                }
                //else
                //{

                //    logger.LogWarning("{Key}: {Type}", input, inputs[input].GetType());
                //}
                //  inputs[input] = inputs[input];
            }

            return resolvedInputs;
        }
    }
}
