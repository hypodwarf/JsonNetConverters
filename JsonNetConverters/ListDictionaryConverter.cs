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
    public class ListDictionaryConverter : JsonConverter
    {
        private static Type GetKvpType(Type objectType)
        {
            var type = objectType;
            while (type != null)
            {
                if(IsDictionary(type)) break;
                type = type.BaseType;
            }
            
            var args = type?.GenericTypeArguments;
            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(args!);

            return kvpType;
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var kvpType = GetKvpType(value.GetType());
            
            IDictionary dict = ((IDictionary) value);
            Array arr = Array.CreateInstance(kvpType, dict.Count);
            dict.CopyTo(arr, 0);
            
            serializer.Serialize(writer, arr);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var kvpType = GetKvpType(objectType);
            
            dynamic arr = (serializer.Deserialize(reader, kvpType.MakeArrayType()));

            try
            {
                return Activator.CreateInstance(objectType, arr);
            }
            catch (MissingMethodException)
            {
                IDictionary dict = (IDictionary)Activator.CreateInstance(objectType);
                if (arr != null)
                    foreach (dynamic kvp in arr)
                    {
                        dict?.Add(kvp.Key, kvp.Value);
                    }

                return dict;
            }
        }

        private static bool IsDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().IsEquivalentTo(typeof(Dictionary<,>));
        }
        
        public override bool CanConvert(Type objectType)
        {
            bool isDict = false;
            var type = objectType;
            while (!isDict && type != null)
            {
                isDict = IsDictionary(type);
                type = type.BaseType;
            }

            return isDict;
        }
    }
}