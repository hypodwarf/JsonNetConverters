using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonNetConverters
{
    public class ListDictionaryConverter : JsonConverter<IDictionary>
        {
            public override void WriteJson(JsonWriter writer, IDictionary? value, JsonSerializer serializer)
            {
                var keys = value.Keys.GetEnumerator();
                var values = value.Values.GetEnumerator();
                IList list = new List<KeyValuePair<object, object>>();
                while (keys.MoveNext() && values.MoveNext())
                {
                    list.Add(KeyValuePair.Create(keys.Current, values.Current));
                }
                
                serializer.Serialize(writer, list);
            }

            public override IDictionary? ReadJson(JsonReader reader, Type objectType, IDictionary? existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                var kvpGeneric = typeof(KeyValuePair<,>);
                var kvpSpecific = kvpGeneric.MakeGenericType(objectType.GenericTypeArguments);

                var listGeneric = typeof(List<>);
                var listSpecific = listGeneric.MakeGenericType(kvpSpecific);

                var enumerableGeneric = typeof(IEnumerable<>);
                var enumerableSpecific = enumerableGeneric.MakeGenericType(kvpSpecific);
                
                var list = ((IList)(serializer.Deserialize(reader, listSpecific)));
                
                var ci = objectType.GetConstructor(new []{enumerableSpecific});
                var dict = (IDictionary) ci.Invoke(new object?[]{ list });

                return dict;
            }
        
    }
}