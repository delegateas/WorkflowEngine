using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public class ActionImplementationMetadata 
    {
        public string Type { get; set; }
        public Type Implementation { get; protected set; }

        public static IActionImplementationMetadata FromType(Type type, string name)
        {
            var metadata = Activator.CreateInstance(typeof(ActionImplementationMetadata<>).MakeGenericType(type)) as ActionImplementationMetadata;
            metadata.Type = name ?? type.Name;

            return metadata as IActionImplementationMetadata;
        }
        public static IActionImplementationMetadata FromType(Type type,Type inputType, string name)
        {
            var metadata = Activator.CreateInstance(typeof(ActionImplementationMetadata<,>).MakeGenericType(type,inputType)) as ActionImplementationMetadata;
            metadata.Type = name ?? type.Name;

            return metadata as IActionImplementationMetadata;
        }
    }
    public class ActionImplementationMetadata<T> : ActionImplementationMetadata,IActionImplementationMetadata
        where T: IActionImplementation
    {
        public ActionImplementationMetadata()
        {
            Implementation = typeof(T); 
        }
       
        public async ValueTask<ActionResult> ExecuteAsync(IServiceProvider services, IRunContext context, IWorkflow workflow, IAction action)
        {
            var implementation = services.GetRequiredService(Implementation) as IActionImplementation;

            var actionResult = await implementation.ExecuteAsync(context, workflow, action);

            var result = new ActionResult
            {
                Key = action.Key,
                Status = "Succeded",
                Result = actionResult,
                DelayNextAction = (implementation is IWaitAction) ? (TimeSpan) actionResult : null
            };
            return result;
        }
    }

    public class ActionImplementationMetadata<T,TInput> : ActionImplementationMetadata, IActionImplementationMetadata
        where T : IActionImplementation<TInput>
    {
        public ActionImplementationMetadata()
        {
            Implementation = typeof(T);
        }


        public async ValueTask<ActionResult> ExecuteAsync(IServiceProvider services, IRunContext context,IWorkflow workflow,IAction action )
        {

            var implementation = services.GetRequiredService(Implementation) as IActionImplementation<TInput>;

            var payload = JsonConvert.SerializeObject(action);
            var typedAction = JsonConvert.DeserializeObject<Action<TInput>>(payload);

            var actionResult= await implementation.ExecuteAsync(context, workflow, typedAction);

            var result = new ActionResult
            {
                Key = action.Key,
                Status = "Succeded",
                Result = actionResult,
                DelayNextAction = (implementation is IWaitAction) ? (TimeSpan) actionResult : null
            };
            return result;
        }
    }



}
