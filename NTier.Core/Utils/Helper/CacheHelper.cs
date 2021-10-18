using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NTier.Core.Utils.Helper
{
    public class CacheHelper
    {
        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        protected virtual T Deserialize<T>(string[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            string jsonString = "[";
            foreach (var item in serializedObject)
                jsonString += item + ",";
            jsonString += "]";
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        protected virtual IList<T> DeserializeList<T> (List<object> redisValues)
        {

            var result = new List<T>();
           // var deserialized = Deserialize<T>(redisValues.ToArray());
            //foreach (var item in redisValues)
            //    // HOW DO I FIND OUT IF T IS A LIST HERE?
            //    // Resharper says: the given expression is never of the provided type
            //    if (typeof(T) is List<object>)
            //    {

            //        result.AddRange(deserialized);
            //    }
            //    // Resharper says: Suspicious type check: there is not type in the solution
            //    // which is inherited from both 'System.Type' and 'System.Collections.IList'
            //    else if (typeof(T) is IList)
            //    {
            //        // deserializing json file with an array of objects
            //        var deserialized = Deserialize<List<T>>(item);
            //        result.AddRange(deserialized);
            //    }
            //    else
            //    {
            //        // deserializing a single object json file
            //        var deserialized = Deserialize<T>(file);
            //        result.Add(deserialized);
            //    }
            return result;
        }

        
    }
}
