using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace WorkflowEngine.Core
{
    public enum EventType
    {
        [EnumMember(Value = "workflow_started")]
        WorkflowStarted = 0,
        [EnumMember(Value = "workflow_finished")]
        WorkflowFinished = 1,
        [EnumMember(Value = "action_completed")]
        ActionCompleted = 2
    }
}
