namespace FastGetters
{
    using System.Collections.Generic;

    public static class Extensions
    {
        public static IDictionary<string, object> ToDictionary<T>(this T obj)
        {
            return ReflectionPropertyHelper<T>.Get(obj);
        }
    }
}
