# JsonNetConverters
This project provides a few helpful converters for Newtonsoft.Json (aka Json.NET).

### Enum <-> KeyValuePair : KeyValuePairEnumConverter
#### System.Enum and enum objects are stored as a KeyValuePair object. 
The base JSON.net package has trouble deserializing System.Enum objects. This converter will package Enum objects as a KeyValuePair object, where the key is the fully qualified assembly name of the enum and the value is the specific enum in that group.

### Dictionary<,> <-> List<KeyValuePair<>>
#### Dictionary objects are stored as a List of KeyValuePair objects.
JSON.net has trouble with Dictionary objects with complex keys because it wants to store the key as a string ( see: https://stackoverflow.com/questions/24504245/not-ableto-serialize-dictionary-with-complex-key-using-json-net/56351540#56351540 ). This converter will take the Dictionary object and store it as a List of KeyValuePairs.
