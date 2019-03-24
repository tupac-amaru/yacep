using System;

namespace TupacAmaru.Yacep.Test
{
    public class A
    {
        public class B
        {
            public class C
            {
                public class D
                {
                    public string e;
                    public int DFunction(string input) => input.Length;
                }
                public D d;
                public string CFunction(string input) => $"c{input}";
            }
            public C c;
            public string BFunction(string input) => $"b{input}";
        }
        public B b;
    }
    public class Fixture
    {
        public static string staticField;
        public static string StaticProperty { get; set; }
        public object a;
        public object b;
        public Func<string> c;
        public object D { get; set; }
        public int E { get; set; }
        public int x;
        public string y;
        public string Y
        {
            private get => y;
            set => y = value;
        }
        public override string ToString() => $"x:{x},y:{Y}";
        public string GetString(string z) => $"x:{x},y:{Y},z:{z}";
        public int Add(int z) => x + z;
        public void DoSomething() => y = "function called";

        public static object ReturnMe(object me) => me;
        public static void DoEmpty() { }
    }

    public class IndexerObject
    {
        public int this[string x] => x.Length;
    }
}
