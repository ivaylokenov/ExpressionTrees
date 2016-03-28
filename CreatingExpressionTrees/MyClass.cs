namespace CreatingExpressionTrees
{
    public class MyClass
    {
        private int? number;

        public MyClass()
        {
            this.Integer = 42;
        }

        public MyClass(int number)
        {
            this.number = number;
        }

        public int Integer { get; set; }

        public int SomeMethod()
        {
            return this.number ?? 42;
        }

        public int SomeMethodWithParams(int first, AnotherClass second)
        {
            return second.SomeValue + first;
        }
    }
}
