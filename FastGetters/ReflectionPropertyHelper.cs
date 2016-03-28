namespace FastGetters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Reflection;

    public class ReflectionPropertyHelper<T>
    {
        private static Type typeOfObject = typeof(object);

        private static Type typeOfOpenFunc = typeof(Func<,>);

        private static Type closedGenericFunc = typeof(Func<T, object>);

        private static readonly ConcurrentDictionary<Type, List<ReflectionPropertyHelper<T>>> cache =
            new ConcurrentDictionary<Type, List<ReflectionPropertyHelper<T>>>();

        private static MethodInfo callPropertyMethod =
            typeof(ReflectionPropertyHelper<T>).GetTypeInfo().GetDeclaredMethod(nameof(ReflectionPropertyHelper<T>.CreateFuncObject));

        public string Name { get; private set; }

        public Func<T, object> Getter { get; private set; }

        public static IDictionary<string, object> Get(T obj)
        {
            var result = new Dictionary<string, object>();

            var type = typeof(T);

            var helpers = cache.GetOrAdd(type, _ =>
            {
                var allProperties = type.GetTypeInfo().DeclaredProperties;

                var listOfHelpers = new List<ReflectionPropertyHelper<T>>();

                foreach (var property in allProperties)
                {
                    // MyClass.Integer
                    var getMethod = property.GetMethod;
                    var input = getMethod.DeclaringType; // MyClass
                    var output = getMethod.ReturnType; // int

                    // Func<MyClass, int>
                    var openGenericType = typeOfOpenFunc.MakeGenericType(input, output);
                    var getterDelegate = getMethod.CreateDelegate(openGenericType);
                    
                    var wrapperDelegate = callPropertyMethod.MakeGenericMethod(
                        input, output);

                    var specificDelegate = (Func<T, object>)
                        wrapperDelegate.CreateDelegate(closedGenericFunc, getterDelegate);

                    var propertyHelper = new ReflectionPropertyHelper<T>
                    {
                        Name = property.Name,
                        Getter = specificDelegate
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

        // Called by reflection
        private static object CreateFuncObject<TDeclaringType, TValue>(
            Func<TDeclaringType, TValue> getter,
            object target)
        {
            if (target == null)
            {
                return null;
            }

            return getter((TDeclaringType)target);
        }
    }
}
