using Microsoft.Extensions.DependencyInjection;

namespace WorkflowEngine.Core
{
    public static class IActionImplementationExtenssions
    {
        public static IServiceCollection AddAction<T>(this IServiceCollection services, string type = null)
            where T: class, IActionImplementation
        {
            return services.AddTransient<T>()
                .AddSingleton< IActionImplementationMetadata>(new ActionImplementationMetadata<T> { Type = type ?? typeof(T).Name });
        }
        public static IServiceCollection AddAction<T,TInput>(this IServiceCollection services, string type = null)
           where T : class, IActionImplementation<TInput>
        {
            return services.AddTransient<T>()
                .AddSingleton<IActionImplementationMetadata>(new ActionImplementationMetadata<T,TInput> { Type = type ?? typeof(T).Name });
        }

        public static IServiceCollection AddWorkflow<T>(this IServiceCollection services) where T :class, IWorkflow
        {
            return services.AddTransient<IWorkflow, T>();
        }
    }
    


}
