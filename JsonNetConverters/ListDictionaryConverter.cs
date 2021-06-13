using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace JsonNetConverters
{
    /**
     * JSON.net has trouble with Dictionary objects with complex keys because it wants to store the key as a string
     * ( see: https://stackoverflow.com/questions/24504245/not-ableto-serialize-dictionary-with-complex-key-using-json-net/56351540#56351540 ).
     * This converter will take the Dictionary object and store it as a List of KeyValuePairs.
     */
    public class ListDictionaryConverter : JsonConverter
    {
        private (Type kvp, Type list, Type enumerable, Type[] args) GetTypes(Type objectType)
        {
            var args = objectType.GenericTypeArguments;
            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(args);
            var listType = typeof(List<>).MakeGenericType(kvpType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(kvpType);

            return (kvpType, listType, enumerableType, args);
        }
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var (kvpType, listType, enumerableType, args) = GetTypes(value.GetType());
            
            var keys = ((IDictionary)value)?.Keys.GetEnumerator();
            var values = ((IDictionary)value)?.Values.GetEnumerator();
            var cl = listType.GetConstructor(Array.Empty<Type>());
            var ckvp = kvpType.GetConstructor(args);
            // IList list = new List<KeyValuePair<object, object>>();
            var list = (IList)cl.Invoke(Array.Empty<object?>());
            while (keys != null && keys.MoveNext() && values.MoveNext())
            {
                list.Add(ckvp.Invoke(new []{keys.Current, values.Current}));
            }
            
            serializer.Serialize(writer, list);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var (kvpType, listType, enumerableType, args) = GetTypes(objectType);
            
            var list = ((IList)(serializer.Deserialize(reader, listType)));
            
            Type ciType = objectType;
            if (objectType.IsAbstract || objectType.IsInterface)
            {
                ciType = typeof(Dictionary<,>).MakeGenericType(args);
            }
  
            var ci = ciType.GetConstructor(new[] {enumerableType});

            var dict = (IDictionary) ci?.Invoke(new object?[]{ list });

            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            if (!objectType.IsGenericType) return objectType.IsAssignableTo(typeof(IDictionary));
            
            var args = objectType.GenericTypeArguments;
            return args.Length == 2 && objectType.IsAssignableTo(typeof(IDictionary<,>).MakeGenericType(args));
        }
    }
}