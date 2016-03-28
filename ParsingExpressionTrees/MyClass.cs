namespace ParsingExpressionTrees
{
    public class MyClass
    {
        public MyClass()
        {
            this.Another = new AnotherClass { AnotherString = "I am nested!" };
        }

        public MyClass(int a, string b)
        {
            this.Integer = a;
            this.String = b;
        }

        public static void StaticMethod()
        {

        }

        public AnotherClass Another { get; set; }

        public int Integer { get; set; }

        public string String { get; set; }

        public int SomeMethod()
        {
            return 42;
        }

        public int SomeMethodWithParam(int id)
        {
            return 42;
        }

        public int SomeMethodWithClass(MyClass cl)
        {
            return 42;
        }

        public int SomeMethodWithMultipleParameters(int a, string b, MyClass cl)
        {
            return 42;
        }
    }
}
