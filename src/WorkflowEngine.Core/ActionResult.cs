using System;
using static System.Collections.Specialized.BitVector32;

namespace WorkflowEngine.Core
{
    public class ActionResult : IActionResult
    {
        public string Key { get; set; }

        public string Status { get; set; }
        public object Result { get;  set; }
        public string FailedReason { get;  set; }
        public bool ReThrow { get;  set; }
        public TimeSpan? DelayNextAction { get; set; }

        public static ActionResult Success(IAction action, object actionResult, object implementation)
        {
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
