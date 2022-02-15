using Microsoft.Extensions.DependencyInjection;

namespace WorkflowEngine.Core
{
    public static class IActionImplementationExtenssions
    {
        public static IServiceCollection AddAction<T>(this IServiceCollection services, string type)
            where T: class, IActionImplementation
        {
            return services.AddTransient<T>()
                .AddSingleton< IActionImplementationMetadata>(new ActionImplementationMetadata<T> { Type = type });
        }
    }
    


}
