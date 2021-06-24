using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JsonNetConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    public class ListDictionaryTests
    {
        public class TestObj
        {
            public readonly string name;
            public readonly int value;

            public TestObj(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            protected bool Equals(TestObj other)
            {
                return name == other.name && value == other.value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TestObj) obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(name, value);
            }
        }

        public struct TestStruct
        {
            public string name;
            public int value;
        }
        
        public class Specialized : Dictionary<TestObj, TestObj>
        {
        }
        
        private JsonSerializerSettings jsonSerializeSettings_None = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>(){new ListDictionaryConverter()},
            TypeNameHandling = TypeNameHandling.None
        };
        
        private JsonSerializerSettings jsonSerializeSettings_All = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>(){new ListDictionaryConverter()},
            TypeNameHandling = TypeNameHandling.All
        };

        private void Marble<T>(T obj) where T : IDictionary
        {
            var jsonAll = JsonConvert.SerializeObject(obj, jsonSerializeSettings_All);
            var deObjAll = JsonConvert.DeserializeObject<T>(jsonAll, jsonSerializeSettings_All);
            Assert.AreEqual(obj, deObjAll);
            
            var json = JsonConvert.SerializeObject(obj, jsonSerializeSettings_None);
            Console.WriteLine(json);
            var deObj = JsonConvert.DeserializeObject<T>(json, jsonSerializeSettings_None);
            Assert.AreEqual(obj, deObj);
            
            //Can also be deserialized as an array of key value pairs
            var (_, kvpType) = ListDictionaryConverter.GetTypeInfo(typeof(T));
            dynamic arr = JsonConvert.DeserializeObject(json, kvpType.MakeArrayType() ,jsonSerializeSettings_None);
            Assert.AreEqual(obj.Count, arr!.Length);
        }

        [Test]
        public void MarbleTest()
        {
            var dictSpecialized = new Specialized {
                {new TestObj("One", 1), new TestObj("Two", 2)}, 
                {new TestObj("Three", 3), new TestObj("Four", 4)}
            };
            var dictEmpty = new Dictionary<int, int>();
            var dictInt = new Dictionary<int, int> { {1,2}, {3,4} };
            var dictString = new Dictionary<string, string> { {"One","Two"}, {"Three","Four"} };
            var dictObj = new Dictionary<TestObj, TestObj>
            {
                {new TestObj("One", 1), new TestObj("Two", 2)}, 
                {new TestObj("Three", 3), new TestObj("Four", 4)}
            };
            var dictStruct = new Dictionary<TestStruct, TestStruct>
            {
                {new TestStruct{name = "One", value = 1}, new TestStruct{name = "Two", value = 2}}, 
                {new TestStruct{name = "Three", value = 3}, new TestStruct{name = "Four", value = 4}}
            };
            var dictReadOnly = new ReadOnlyDictionary<TestStruct, TestStruct>(dictStruct);
            var dictSorted = new SortedDictionary<string, string>(dictString);
            
            Marble(dictEmpty);
            
            var jsonITest = JsonConvert.SerializeObject(dictInt);
            Console.WriteLine(jsonITest);
            var deInterface = JsonConvert.DeserializeObject<IDictionary<int, int>>(jsonITest);
            Assert.AreEqual(dictInt, deInterface);
            
            Marble(dictInt);
            Marble(dictString);
            Marble(dictObj);
            Marble(dictStruct);
            Marble(dictSpecialized);
            Marble(dictReadOnly);
            Marble(dictSorted);
        }
    }
}