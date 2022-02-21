using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public class DefaultOutputsRepository : IOutputsRepository
    {
        public ConcurrentDictionary<Guid,JToken> Runs { get; set; } = new ConcurrentDictionary<Guid, JToken>();

        public ValueTask AddScopeItem(IRunContext context, IWorkflow workflow, IAction action, IActionResult result)
        {
            return AddAsync(context,workflow,action,result);
            JToken run = GetOrCreateRun(context);

            var obj = GetOrCreateStateObject(action.Key, run);

            obj.Merge(JToken.FromObject(new { status = result.Status, body = result.Result, failedReason = result.FailedReason }));

            return new ValueTask();
        }
        public ValueTask AddAsync(IRunContext context, IWorkflow workflow, IAction action, IActionResult result)
        {
            JToken run = GetOrCreateRun(context);
          
            var obj = GetOrCreateStateObject(action.Key, run);

            obj.Merge(JToken.FromObject(new { status = result.Status, body = result.Result, failedReason = result.FailedReason }));



            return new ValueTask();
        }

        private static JObject GetOrCreateStateObject(string key, JToken run)
        {
            JToken actions =run["actions"];
           
            var idx = key.IndexOf('.');
            while (idx != -1)
            {
                actions = actions[key.Substring(0, idx)];
                key = key.Substring(idx + 1);
                idx = key.IndexOf('.');


                actions = actions["actions"] ?? (actions["actions"] = new JObject());
            }

            JObject obj = actions[key] as JObject;
            if(obj == null)
            {
                actions[key]=obj=new JObject();
            }

            return obj;
        }

        private JToken GetOrCreateRun(IRunContext context)
        {
            return Runs.GetOrAdd(context.RunId, (id) => new JObject(new JProperty("actions", new JObject()), new JProperty("triggers", new JObject())));
        }

        public ValueTask AddAsync(IRunContext context, IWorkflow workflow, ITrigger trigger)
        {
            JToken run = GetOrCreateRun(context);

            run["triggers"][trigger.Key] = JToken.FromObject( new { time=trigger.ScheduledTime, body = trigger.Inputs });

            return new ValueTask();
        }

       
        public ValueTask<object> GetTriggerData(Guid id)
        {
            var run = Runs[id];
            return new ValueTask<object>(run["triggers"].OfType<JProperty>().FirstOrDefault().Value);
        }

        public ValueTask<object> GetOutputData(Guid id, string v)
        {
            var run = Runs[id];

            var obj = GetOrCreateStateObject(v, run);

            var json = JsonConvert.SerializeObject(obj);


            return new ValueTask<object>(JToken.Parse(json));
        }

        public ValueTask AddArrayItemAsync(IRunContext context, IWorkflow workflow, string key, IActionResult result)
        {
           
            JToken run = GetOrCreateRun(context);


            var obj1 = GetOrCreateStateObject(key, run);

            obj1.Merge(JToken.FromObject(new { status = result.Status, body = result.Result, failedReason = result.FailedReason }));


          //  var obj = GetOrCreateStateObject(key.Substring(0, key.LastIndexOf('.')), run);

          //  var body = obj["items"] as JArray;
          //  if (body==null)
          //      obj["items"] =body= new JArray();

          //  var lastItem = body[body.Count-1] as JObject;
          //  var actions = lastItem["actions"] as JObject;

          //  var itteration = actions[key.Substring(key.LastIndexOf('.')+1)] as JObject;
          // // if (itteration==null)
          ////      actions[key.Substring(key.LastIndexOf('.')+1)] = itteration = new JObject();

          //  itteration.Merge(JToken.FromObject(JToken.FromObject(new { status = result.Status, body = result.Result, failedReason = result.FailedReason })));
           
           


            return new ValueTask();
        }

        public ValueTask AddArrayInput(IRunContext context, IWorkflow workflow, IAction action)
        {

             
            JToken run = GetOrCreateRun(context);

            var obj = GetOrCreateStateObject(action.Key, run);

            obj.Merge(JToken.FromObject(new { input = action.Inputs }));


            //var obj = GetOrCreateStateObject(action.Key.Substring(0, action.Key.LastIndexOf('.')), run);


            //var body = obj["items"] as JArray;

            //var lastItem = body[body.Count-1] as JObject;
            //var actions = lastItem["actions"] as JObject;

            //actions[action.Key.Substring(action.Key.LastIndexOf('.')+1)] = JToken.FromObject(new { input = action.Inputs });


            return new ValueTask();

             
        }
        public ValueTask EndScope(IRunContext context, IWorkflow workflow, IAction action)
        {
            JToken run = GetOrCreateRun(context);

            var obj = GetOrCreateStateObject(action.Key, run);

            var body = obj["items"] as JArray;
            if (body==null)
                obj["items"] =body= new JArray();
          
            var actions = obj["actions"];
            actions.Parent.Remove();
            body.Add(actions);
           

            action.Index= body.Count;

            return new ValueTask();

        }
            public ValueTask StartScope(IRunContext context, IWorkflow workflow, IAction action)
        {
            JToken run = GetOrCreateRun(context);
             
            var obj = GetOrCreateStateObject(action.Key, run);

            var body = obj["items"] as JArray;
            if (body==null)
                obj["items"] =body= new JArray();

            var lastItem = JToken.FromObject(new { actions = new JObject() });

            body.Add(lastItem);

            return new ValueTask();
        }

        public ValueTask AddInput(IRunContext context, IWorkflow workflow, IAction action)
        {
            JToken run = GetOrCreateRun(context);

            var obj = GetOrCreateStateObject(action.Key, run);

            obj.Merge(JToken.FromObject(new { input = action.Inputs }));

            return new ValueTask();
        }



    }


}
