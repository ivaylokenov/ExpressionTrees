namespace CreatingExpressionTrees
{
    using System;
    using System.Linq.Expressions;

    public static class New<T>
    {
        public static Func<T> Instance
            = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
    }
}
