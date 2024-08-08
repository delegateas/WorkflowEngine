using System;
using System.Collections.Generic;

namespace WorkflowEngine.Core
{
    public interface IAction<TInput>: IRunContext
    {
        public DateTimeOffset ScheduledTime { get; set; }
        public string Type { get; set; }
        string Key { get; }
        TInput Inputs { get; set; }

        public int Index { get; set; }
        public bool ScopeMoveNext { get; set; }
    }
    public interface IAction : IAction<IDictionary<string,object>>
    {
        
       
    }


}
