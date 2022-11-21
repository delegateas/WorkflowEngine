using Hangfire.Client;
using Hangfire.Common;
using System;
using System.Linq;

namespace WorkflowEngine
{

    public class JobNaturalKeyAttribute : JobFilterAttribute, IClientFilter
    {
        public JobNaturalKeyAttribute(string keyFormat)
        {
            KeyFormat = keyFormat;
        }

        public string KeyFormat { get; private set; }

        public void OnCreated(CreatedContext filterContext)
        {
            var key = String.Format(KeyFormat, args: filterContext.Job.Args.ToArray());
            filterContext.Connection.SetJobExternalKey(key, filterContext.BackgroundJob.Id);

        }

        public void OnCreating(CreatingContext filterContext)
        {


        }
    }
}
