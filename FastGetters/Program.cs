using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastGetters
{
    public class Program
    {
        public static void Main()
        {
            // return RedirectToAction("Index", "Home", new { id = 1, query = "age" })

            var myClas = new MyClass
            {
                Integer = 1000
            };

            var obj = new { id = 1, query = "age" };
            var obj2 = new { id = 1, query = "age" };

            var result = obj.ToDictionary();
            var result2 = obj2.ToDictionary();
        }

        private static IDictionary<string, object> SlowDictionary(object obj)
        {
            var properties = obj.GetType().GetTypeInfo().DeclaredProperties;

            var result = new Dictionary<string, object>();

            foreach (var prop in properties)
            {
                result.Add(prop.Name, prop.GetValue(obj));
            }

            return result;
        }
    }
}
