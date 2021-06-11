using System;
using JsonNetConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    public class KeyValuePairEnumTests
    {
        enum x
        {
            Test1
        }
        
        [Test]
        public void MarbleTest()
        {
            var json = JsonConvert.SerializeObject(x.Test1, new KeyValuePairEnumConverter());
            Console.WriteLine(json);
            var dx = JsonConvert.DeserializeObject<Enum>(json, new KeyValuePairEnumConverter());
            Assert.AreEqual(x.Test1, dx);
        }
    }
}