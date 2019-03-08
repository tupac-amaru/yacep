using System;

namespace TupacAmaru.Yacep.Test
{
    public class Fixture
    {
        public object a;
        public object b;
        public Func<string> c;
        public object D { get; set; }
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
}
