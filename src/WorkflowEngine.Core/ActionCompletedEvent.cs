using Newtonsoft.Json;

namespace WorkflowEngine.Core
{
    public class ActionCompletedEvent : Event
    {
        public override EventType EventType => EventType.ActionCompleted;

        [JsonProperty("jobId")]
        public string JobId { get; set; }
        [JsonProperty("actionKey")]
        public string ActionKey { get; set; }
        [JsonProperty("resultPath")]
        public string ResultPath { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public static ActionCompletedEvent FromAction(IActionResult result,IAction action,string jobId)
        {

            return new ActionCompletedEvent
            {
                
                JobId = jobId,
                ActionKey = action.Key,
                Status = result.Status,
            };
        }
    }
}
