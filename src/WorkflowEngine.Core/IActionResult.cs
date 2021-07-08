namespace WorkflowEngine.Core
{
    //Har info om hvad der skal schedules next i hangfire
    public interface IActionResult
    {
         string Key { get; }
        string Status { get; }
    }
    


}
