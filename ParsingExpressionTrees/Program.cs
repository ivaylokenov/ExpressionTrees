namespace ParsingExpressionTrees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    public class Program
    {
        public static void Main()
        {
            // c => c.SomeMethod()
            // c.SomeMethod()
            // SomeMethod()
            // SomeMethod
            //PrintExpression<MyClass>(c => c.SomeMethod());

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //PrintExpression(() => MyClass.StaticMethod());

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //PrintExpression(() => 1);

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //Expression<Func<object>> objectFunc = () => 100;

            //Expression<Func<int, int, int>> func = (x, y) => x + y;

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //ParseExpression(func);

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //var id = 15;
            //PrintExpression(() => id);

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //PrintExpression(objectFunc);

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //var first = 1;
            //var second = 2;
            //// () => first + second;
            //// first + second
            //// +(first + second;)
            //// first
            //// second
            //PrintExpression(() => first + second);

            //Console.WriteLine(new string('-', 10));
            //Console.WriteLine();

            //var text = "test";
            //// c => c.SomeMethodWithMultipleParameters(1, text, null)
            //// c.SomeMethodWithMultipleParameters(1, text, null)
            //// SomeMethodWithMultipleParameters(1, text, null)
            //// SomeMethodWithMultipleParameters
            //// 1 (Constant)
            //// text ("test") (Variable)
            //// null (Constant)
            //PrintExpression<MyClass>(c => c.SomeMethodWithMultipleParameters(
            //    1,
            //    text,
            //    null));

            //Expression<Func<MyClass, int>> funcGetter = c => c.Integer;
            //ParseExpression(funcGetter);

            //Expression<Func<MyClass, string>> funcNestedGetter = c => c.Another.AnotherString;
            //ParseExpression(funcNestedGetter);

            // new[5] { 1,2,3,4,5 }
            // new List<int> { 1,2,3,4,5 }
            // new Object { Propert = "FOO" }

            PrintExpression(() => new MyClass());

            PrintExpression(() => new MyClass(100, "test"));

            //PrintExpression(() => new MyClass
            //{
            //    Integer = 200,
            //    String = "Pesho"
            //});

            // c => c...
            // c.SomeMethodWithMultipleParameters(...)
            // SomeMethodWithMultipleParameters(...)
            // SomeMethodWithMultipleParameters
            // 1
            // text - field - "PESHO!"
            // new MyClass(...) { ... }
            // new MyClass(...)
            // 12
            // "GOSHO"
            // { ... }
            // Integer = 500
            // Integer
            // 500
            // String = "10000"
            // String
            // "10000"
            var text = "PESHO!";
            PrintExpression<MyClass>(c => c.SomeMethodWithMultipleParameters(
                1,
                text,
                new MyClass(13, "GOSHO!")
                {
                    Integer = 500,
                    String = "10000"
                }));
        }

        public static void PrintExpression(Expression<Action> expr)
        {
            var body = expr.Body;
            ParseExpression(body);
        }

        public static void PrintExpression<T>(Expression<Func<T>> expr)
        {
            var body = expr.Body;
            ParseExpression(body);
        }

        public static void PrintExpression<T>(Expression<Action<T>> expr)
        {
            var body = expr.Body;
            ParseExpression(body);
        }

        public static void ParseExpression(Expression expr)
        {
            if (expr is LambdaExpression)
            {
                var exprAsLambda = (LambdaExpression)expr;
                foreach (var parameter in exprAsLambda.Parameters)
                {
                    Console.Write("Parameter: ");
                    Console.WriteLine(parameter.Name);
                    Console.WriteLine(parameter.Type.Name);
                }

                ParseExpression(exprAsLambda.Body);
            }

            if (expr.NodeType == ExpressionType.Convert)
            {
                var convertExpr = (UnaryExpression)expr;
                var operand = convertExpr.Operand;
                Console.Write("Converting ");
                ParseExpression(operand);
                return;
            }
            if (expr.NodeType == ExpressionType.Add)
            {
                var binaryExpression = (BinaryExpression)expr;
                Console.Write("Binary: ");
                Console.WriteLine("Left: ");
                var left = binaryExpression.Left;
                ParseExpression(left);
                Console.WriteLine("Right");
                var right = binaryExpression.Right;
                ParseExpression(right);

                return;
            }
            if (expr.NodeType == ExpressionType.Constant)
            {
                var contsExpr = (ConstantExpression)expr;
                Console.Write("Constant: ");
                Console.WriteLine(contsExpr.Value);
            }
            if (expr.NodeType == ExpressionType.MemberAccess)
            {
                var memberAccessExpr = (MemberExpression)expr;
                
                if (memberAccessExpr.Member is FieldInfo)
                {
                    var constantExpression = (ConstantExpression)memberAccessExpr.Expression;
                    var compiledLambdaScopeField =
                        constantExpression.Value.GetType().GetField(memberAccessExpr.Member.Name);
                    var realValue = compiledLambdaScopeField.GetValue(constantExpression.Value);
                    Console.WriteLine(realValue);
                }

                if (memberAccessExpr.Member is PropertyInfo)
                {
                    var propertyInfo = (PropertyInfo)memberAccessExpr.Member;
                    Console.WriteLine(propertyInfo.Name);

                    if (memberAccessExpr.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        ParseExpression(memberAccessExpr.Expression);
                    }
                }
                
                return;
            }
            else if (expr.NodeType == ExpressionType.Call)
            {
                var methodCallExpr = (MethodCallExpression)expr;
                var methodTarget = methodCallExpr.Object;
                var name = methodCallExpr.Method.Name;
                Console.Write("Method Call: ");
                Console.WriteLine(name);
                Console.WriteLine($"Instance: {methodTarget?.Type.Name}");
                Console.WriteLine($"Static: {methodCallExpr.Method.DeclaringType.Name}");

                foreach (var arg in methodCallExpr.Arguments)
                {
                    ParseExpression(arg);
                }

                return;
            }
            else if (expr.NodeType == ExpressionType.New)
            {
                var newExpression = (NewExpression)expr;
                Console.Write(newExpression.Constructor.Name);
                Console.Write("(");
                foreach (var arg in newExpression.Arguments)
                {
                    Console.Write(arg.Type.Name + " ");
                }

                Console.WriteLine(")");
            }
            else if (expr.NodeType == ExpressionType.MemberInit)
            {
                var memberInitExpression = (MemberInitExpression)expr;

                // constructor information
                ParseExpression(memberInitExpression.NewExpression);

                var bindings = memberInitExpression.Bindings;
                foreach (var binding in bindings)
                {
                    if (binding.BindingType == MemberBindingType.Assignment)
                    {
                        var assignment = (MemberAssignment)binding;
                        Console.WriteLine(assignment.Member.Name);
                        ParseExpression(assignment.Expression);
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                var value = Expression.Lambda(expr).Compile().DynamicInvoke();
                Console.Write("Not Supported! Compiled value: ");
                Console.WriteLine(value);

                return;
            }
        }
    }
}
