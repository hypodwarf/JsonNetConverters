using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonNetConverters
{
    /**
     * The base JSON.net package has trouble deserializing System.Enum objects. This converter will package Enum objects
     * as a KeyValuePair object, where the key is the fully qualified assembly name of the enum and the value is the
     * specific enum in that group.
     */
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
                    new KeyValuePair<string, string>(value.GetType().AssemblyQualifiedName, value.ToString()));
        }
    }
}