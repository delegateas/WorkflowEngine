using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;

namespace WorkflowEngine.Core
{
    public static class IActionImplementationExtenssions
    {
        public static IServiceCollection AddAction<T>(this IServiceCollection services, string type = null)
            where T: class, IActionForRegistration
        {
             
            // Check if T implements IActionImplementation<TInput>
            var interfaceType = typeof(T).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IActionImplementation<>));
            if (interfaceType != null)
            {
                // Extract the TInput type argument
                var inputType = interfaceType.GetGenericArguments()[0];

                // Get the method definition for AddAction<T, TInput>
                var method = typeof(IActionImplementationExtenssions).GetMethod(nameof(AddAction), 2, 
                    new Type[] { typeof(IServiceCollection), typeof(string) }).MakeGenericMethod(new Type[] { typeof(T), inputType });

                // Invoke AddAction<T, TInput> using reflection
                return (IServiceCollection) method.Invoke(null, new object[] { services, type });
            }

            
            return services.AddTransient<T>()
                .AddSingleton<IActionImplementationMetadata>(ActionImplementationMetadata.FromType(typeof(T),type));
        }

        


        public static IServiceCollection AddAction<T,TInput>(this IServiceCollection services, string type = null)
           where T : class, IActionImplementation<TInput>
        {
            return services.AddTransient<T>()
                .AddSingleton<IActionImplementationMetadata>(ActionImplementationMetadata.FromType(typeof(T),typeof(TInput), type));
        }

        public static IServiceCollection AddWorkflow<T>(this IServiceCollection services) where T :class, IWorkflow
        {
            return services.AddTransient<IWorkflow, T>();
        }
    }
    


}
