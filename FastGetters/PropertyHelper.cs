namespace FastGetters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Linq.Expressions;

    public class PropertyHelper<T>
    {
        private static Type typeOfObject = typeof(object);

        private static readonly ConcurrentDictionary<Type, List<PropertyHelper<T>>> cache =
            new ConcurrentDictionary<Type, List<PropertyHelper<T>>>();
        
        public string Name { get; private set; }

        public Func<T, object> Getter { get; private set; }

        public static IDictionary<string, object> Get(T obj)
        {
            var result = new Dictionary<string, object>();

            var type = typeof(T);

            var helpers = cache.GetOrAdd(type, _ =>
            {
                var allProperties = type.GetTypeInfo().DeclaredProperties;

                var listOfHelpers = new List<PropertyHelper<T>>();

                foreach (var property in allProperties)
                {
                    // c
                    var parameter = Expression.Parameter(type, "c");

                    // c.Integer
                    var memberAccess = Expression.MakeMemberAccess(
                        parameter,
                        property);

                    // (object)c.Integer
                    var cast = Expression.Convert(
                        memberAccess,
                        typeOfObject);

                    // c => (object)c.Integer
                    var lambda = Expression.Lambda<Func<T, object>>(cast, parameter);

                    var propertyHelper = new PropertyHelper<T>
                    {
                        Name = property.Name,
                        Getter = lambda.Compile()
                    };

                    listOfHelpers.Add(propertyHelper);
                }

                return listOfHelpers;
            });
            
            foreach (var helper in helpers)
            {
                result.Add(helper.Name, helper.Getter(obj));
            }

            return result;
        }
    }
}
