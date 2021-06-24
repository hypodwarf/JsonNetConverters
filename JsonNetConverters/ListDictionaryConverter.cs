using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static (Type dictType, Type kvpType) GetTypeInfo(Type objectType)
        {
            var type = objectType;
            while (type.BaseType!.IsAssignableTo(typeof(IDictionary)))
            {
                type = type.BaseType;
            }
            
            var args = type?.GenericTypeArguments;
            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(args!);
            var dictType = typeof(Dictionary<,>).MakeGenericType(args!);

            return (dictType, kvpType);
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var (_, kvpType) = GetTypeInfo(value.GetType());
            
            IDictionary dict = ((IDictionary) value);
            Array arr = Array.CreateInstance(kvpType, dict.Count);
            dict.CopyTo(arr, 0);
            
            serializer.Serialize(writer, arr);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var (dictType, kvpType) = GetTypeInfo(objectType);
            
            dynamic arr = (serializer.Deserialize(reader, kvpType.MakeArrayType()));
            dynamic tempDict = Activator.CreateInstance(dictType, arr);

            try
            {
                return Activator.CreateInstance(objectType, tempDict);
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

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof(IDictionary));
        }
    }
}