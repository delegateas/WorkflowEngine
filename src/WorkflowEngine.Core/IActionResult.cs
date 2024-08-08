using System;

namespace WorkflowEngine.Core
{
    //Har info om hvad der skal schedules next i hangfire
    public interface IActionResult
    {
        string Key { get; }
        string Status { get; }
        object Result { get;  }
        string FailedReason { get; }
        bool ReThrow { get;  }

        TimeSpan? DelayNextAction { get; }
    }
    


}
