using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonNetConverters
{
    public class KeyValuePairEnumConverter: JsonConverter<Enum>
    {
        public override Enum ReadJson(JsonReader reader, Type objectType, Enum existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var (key, value) = serializer.Deserialize<KeyValuePair<string, string>>(reader);
            var type = Type.GetType(key);
            if (type == null) return null;
            return (Enum)Enum.Parse(type, value);
        }
            
        public override void WriteJson(JsonWriter writer, Enum value, JsonSerializer serializer)
        {
            if (value != null)
                serializer.Serialize(writer,
                    new KeyValuePair<string, string>(value.GetType().FullName, value.ToString()));
        }
    }
}