using System;
using System.Collections.Generic;
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
        
        private JsonSerializerSettings jsonSerializeSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>(){new ListDictionaryConverter()},
            TypeNameHandling = TypeNameHandling.None
        };

        [Test]
        public void MarbleTest()
        {
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

            var jsonEmpty = JsonConvert.SerializeObject(dictEmpty, jsonSerializeSettings);
            Console.WriteLine(jsonEmpty);
            var deEmpty = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonEmpty, jsonSerializeSettings);
            Assert.AreEqual(dictEmpty, deEmpty);
            
            var jsonITest = JsonConvert.SerializeObject(dictInt);
            Console.WriteLine(jsonITest);
            var deInterface = JsonConvert.DeserializeObject<IDictionary<int, int>>(jsonITest);
            Assert.AreEqual(dictInt, deInterface);
            
            var jsonInt = JsonConvert.SerializeObject(dictInt, jsonSerializeSettings);
            Console.WriteLine(jsonInt);
            var deInt = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonInt, jsonSerializeSettings);
            Assert.AreEqual(dictInt, deInt);
            
            var jsonString = JsonConvert.SerializeObject(dictString, jsonSerializeSettings);
            Console.WriteLine(jsonString);
            var deString = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString, jsonSerializeSettings);
            Assert.AreEqual(dictString, deString);
            
            var jsonObj = JsonConvert.SerializeObject(dictObj, jsonSerializeSettings);
            Console.WriteLine(jsonObj);
            var deObj = JsonConvert.DeserializeObject<Dictionary<TestObj, TestObj>>(jsonObj, jsonSerializeSettings);
            Assert.AreEqual(dictObj, deObj);
            
            var jsonStruct = JsonConvert.SerializeObject(dictStruct, jsonSerializeSettings);
            Console.WriteLine(jsonStruct);
            var deStruct = JsonConvert.DeserializeObject<Dictionary<TestStruct, TestStruct>>(jsonStruct, jsonSerializeSettings);
            Assert.AreEqual(dictStruct, deStruct);
            
        }
    }
}