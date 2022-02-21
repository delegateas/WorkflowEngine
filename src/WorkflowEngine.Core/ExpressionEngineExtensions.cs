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
        internal static IServiceCollection AddFunction<T>(this IServiceCollection serviceCollection)
           where T : class, IFunction

        {
            serviceCollection.AddScoped<T>();
            serviceCollection.AddScoped<IFunction, T>(x => x.GetRequiredService<T>());
            return serviceCollection;
        }

        public static IServiceCollection AddFunctions(this IServiceCollection services)
        {
            services.AddFunction<TriggerBodyFunction>();
            services.AddFunction<TriggerOutputsFunction>();
            services.AddFunction<OutputsFunction>();
            services.AddFunction<ItemsFunction>();
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
