using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonNetConverters
{
    /**
     * JSON.net has trouble with Dictionary objects with complex keys because it wants to store the key as a string
     * ( see: https://stackoverflow.com/questions/24504245/not-ableto-serialize-dictionary-with-complex-key-using-json-net/56351540#56351540 ).
     * This converter will take the Dictionary object and store it as a List of KeyValuePairs.
     */
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