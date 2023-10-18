using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
        public static async ValueTask<IDictionary<string, object>> ResolveInputs(this IExpressionEngine engine, IDictionary<string, object> inputs, ILogger logger)
        {

            var resolvedInputs = new Dictionary<string, object>();

            foreach (var input in inputs)
            {
                if (input.Value is string str && str.Contains("@"))
                {
                    resolvedInputs[input.Key] = await engine.ParseToValueContainer(input.Value.ToString());
                }
                else
                {
                    if (input.Value is IDictionary<string, object> obj)
                    {
                        resolvedInputs[input.Key] = await engine.ResolveInputs(obj, logger);
                    }
                    else
                    {
                        resolvedInputs[input.Key] = input.Value;
                    }

                }
                 
            }

            return resolvedInputs;

        }
        public static  ValueTask<IDictionary<string,object>> ResolveInputs(this IExpressionEngine engine, ActionMetadata actionMetadata, ILogger logger)
        {
            return engine.ResolveInputs(actionMetadata.Inputs, logger);
             
        }
    }
}
