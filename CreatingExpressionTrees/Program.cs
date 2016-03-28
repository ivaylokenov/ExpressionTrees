namespace CreatingExpressionTrees
{
    using System;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Reflection;

    public class Program
    {
        public static void Main()
        {
            // var instance = New<MyClass>.Instance();

            // c => c.SomeMethod();

            //// c
            //var parameterExpression =
            //    Expression.Parameter(typeof(MyClass), "c");

            //// c.SomeMethod()
            //var methodCallExpression =
            //    Expression.Call(
            //        parameterExpression,
            //        typeof(MyClass).GetTypeInfo().GetMethod(nameof(MyClass.SomeMethod)));

            //// c => c.SomeMethod();
            //var lambdaExpression =
            //    Expression.Lambda<Func<MyClass, int>>(
            //        methodCallExpression,
            //        parameterExpression);

            //var result = lambdaExpression.Compile().Invoke(new MyClass());
            //var secondResult = lambdaExpression.Compile().Invoke(new MyClass(100));

            // c => c.SomeMethodWithParams(1, new AnotherClass { SomeValue = 1000 });

            // c
            var parameter = Expression.Parameter(typeof(MyClass), "c");

            // 1
            var constant = Expression.Constant(1);

            // new AnotherClass()
            var newExpr = Expression.New(typeof(AnotherClass));

            // 1000
            var secondConstant = Expression.Constant(1000);

            // { SomeValue = 1000 } (AnotherClass)
            var memberBinding = Expression.Bind(
                typeof(AnotherClass).GetTypeInfo().GetProperty(nameof(AnotherClass.SomeValue)),
                secondConstant);

            // new AnotherClass { SomeValue = 1000; }
            var initExpr = Expression.MemberInit(
                newExpr,
                memberBinding);

            // c.SomeMethodWithParams(1, new ...)
            var methodCall = Expression.Call(
                parameter,
                typeof(MyClass).GetTypeInfo().GetMethod(nameof(MyClass.SomeMethodWithParams)),
                constant,
                initExpr);

            var lambdaExpr = Expression.Lambda<Func<MyClass, int>>(
                methodCall,
                parameter);

            var result = lambdaExpr.Compile().Invoke(new MyClass());
        }
    }
}
