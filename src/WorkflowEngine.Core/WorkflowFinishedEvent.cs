using Newtonsoft.Json;

namespace WorkflowEngine.Core
{
    public abstract class WorkflowEvent : Event
    { 
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("jobId")]
        public string JobId { get; set; }
         

        public static WorkflowFinishedEvent CreateFinishedEvent(string jobid, IActionResult result)
        {
            return new WorkflowFinishedEvent() { JobId = jobid, Status = result.Status, Result = result };
        }
        public static WorkflowFinishedEvent CreateStartedEvent(string jobid, string status)
        {
            return new WorkflowFinishedEvent() { JobId = jobid, Status = status };
        }
    }
    public class WorkflowFinishedEvent : WorkflowEvent, IHaveFinisningStatus
    {
        public override EventType EventType => EventType.WorkflowFinished;


        [JsonIgnore]
        public IActionResult Result { get; set; }
        

    }
    public class WorkflowStarteddEvent : WorkflowEvent
    {
        public override EventType EventType => EventType.WorkflowStarted;
        

    }
}
