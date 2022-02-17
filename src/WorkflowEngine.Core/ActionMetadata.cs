using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowEngine.Core
{
    /// <summary>
    /// Represent the metadata for a action
    /// </summary>
    [JsonConverter(typeof(ActionMetadataJsonConvert))]
    public class ActionMetadata
    {
        public WorkflowRunAfterActions RunAfter { get; set; } = new WorkflowRunAfterActions();
        public string Type { get;  set; }
        public IDictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();

        public bool ShouldRun(string key, string status)
        {
            if (key.Contains("."))
            {
                key = key.Substring(key.LastIndexOf(".")+1);
            }
             
            return RunAfter[key].Contains(status);
        }
    }

    public class ActionMetadataJsonConvert : JsonConverter
    {
        public override bool CanRead => true;
        override public bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(ActionMetadata).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
           
            var jtoken = JToken.ReadFrom(reader) as JObject;
            var type = jtoken.GetValue("type", StringComparison.OrdinalIgnoreCase).Value<string>();

            if (type=="Foreach")
            {
                var a = new ForLoopActionMetadata();
                serializer.Populate(jtoken.CreateReader(), a);
                return a;
            }
            var val = new ActionMetadata();
            serializer.Populate(jtoken.CreateReader(), val);
            return val;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
