namespace WorkflowEngine.Core
{
    public class ActionResult : IActionResult
    {
        public string Key { get; set; }

        public string Status { get; set; }
        public object Result { get;  set; }
        public string FailedReason { get;  set; }
    }



}
