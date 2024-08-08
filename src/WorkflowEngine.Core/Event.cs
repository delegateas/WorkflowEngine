using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace WorkflowEngine.Core
{
   public interface IHaveFinisningStatus
    {
        IActionResult Result { get; }
    }
    [JsonConverter(typeof(BaseClassConverter))]
    public abstract class Event
    {
        [JsonProperty("eventType")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public abstract EventType EventType { get; }

         
    }

    public class BaseClassConverter : CustomCreationConverter<Event>
    {
        private EventType _currentObjectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JObject.ReadFrom(reader);
            _currentObjectType = jobj["eventType"].ToObject<EventType>();
            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override Event Create(Type objectType)
        {
            switch (_currentObjectType)
            {
                case EventType.ActionCompleted:
                    return new ActionCompletedEvent();
                case EventType.WorkflowStarted:
                    return new WorkflowStarteddEvent();
                case EventType.WorkflowFinished:
                    return new WorkflowFinishedEvent();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
