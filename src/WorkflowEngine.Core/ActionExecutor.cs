using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IActionImplementationMetadata{
         string Type { get;  }
        Type Implementation { get;  }
    }
    public class ActionImplementationMetadata<T> : IActionImplementationMetadata 
        where T: IActionImplementation
    {
        public string Type { get; set; }
        public Type Implementation => typeof(T);
    }
    public static class IActionImplementationExtenssions
    {
        public static IServiceCollection AddAction<T>(this IServiceCollection services, string type)
            where T: class, IActionImplementation
        {
            return services.AddTransient<T>()
                .AddSingleton< IActionImplementationMetadata>(new ActionImplementationMetadata<T> { Type = type });
        }
    }
    public interface IActionImplementation
    {
         
       
        ValueTask<object> ExecuteAsync(IWorkflow workflow, IAction action);


    }
    public class ActionExecutor : IActionExecutor
    {
        private readonly IServiceProvider serviceProvider;
        private Dictionary<string, IActionImplementationMetadata> _implementations;

        public ActionExecutor(IEnumerable<IActionImplementationMetadata> implementations, IServiceProvider serviceProvider)
        {
           

            _implementations = implementations?.ToDictionary(k => k.Type) ?? throw new ArgumentNullException(nameof(implementations));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        public async ValueTask<IActionResult> ExecuteAsync(IWorkflow workflow, IAction action)
        {
            try
            {
                var actionMetadata = workflow.Manifest.Actions.Single(k => k.Key == action.Key).Value;

                var actionImplementation = serviceProvider.GetRequiredService(_implementations[actionMetadata.Type].Implementation) as IActionImplementation;
                 

                return  new ActionResult { 
                    Key = action.Key, 
                    Status = "Succeded", 
                    Result = await actionImplementation.ExecuteAsync(workflow,action) 
                };
           
            
            }catch(Exception ex)
            {
                return new ActionResult { Key = action.Key, Status = "Failed", FailedReason=ex.ToString() };
            }
        }
    }
    


}
