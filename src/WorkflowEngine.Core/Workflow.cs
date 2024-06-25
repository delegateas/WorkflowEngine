using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WorkflowEngine.Core
{
    public class Workflow : IWorkflow, IFormattable
    {
        public Guid Id { get; set; }
        public string Version { get; set; }
        public WorkflowManifest Manifest { get; set; }
        
      
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "Id")
                return Id.ToString();

            if (format == "Version")
                return Version;

            if (format == "Trigger")
                return Manifest.Triggers.FirstOrDefault().Value?.Type;

            return string.Empty;
        }
    }
    public class Workflow<TInput> : Workflow, IWorkflowInputs<TInput>
   where TInput : class
    {

        public static IDictionary<string, object> ForwardInputs<TTarget>()
        {
            var targetkeys = typeof(TTarget).GetProperties().Select(x => x.GetCustomAttribute<JsonPropertyAttribute>().PropertyName).ToHashSet();
            return typeof(TInput).GetProperties()
                .Where(x => targetkeys.Contains(x.GetCustomAttribute<JsonPropertyAttribute>().PropertyName))
                .ToDictionary(x => x.GetCustomAttribute<JsonPropertyAttribute>().PropertyName,
                v => $"@triggerBody()?.payload?.values?.{v.GetCustomAttribute<JsonPropertyAttribute>().PropertyName}" as object);
        }

        public static T WorkflowInput<T>(Expression<Func<TInput,object>> selector)
        {
            throw new NotImplementedException();
           // return "@triggerBody()?.data?.values?.name";
        }
        public static T Expr<T>(Expression<Func<TInput, T>> selector)
        {
            throw new NotImplementedException();
            // return "@triggerBody()?.data?.values?.name";
        }
        public static TOut Fn<TFunc,TOut>()
        {
            throw new NotImplementedException();
            // return "@triggerBody()?.data?.values?.name";
        }


    }


}
