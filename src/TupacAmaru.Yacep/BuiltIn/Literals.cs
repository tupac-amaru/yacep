using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.BuiltIn
{
    public static class Literals
    {
        public static readonly LiteralValue True = new LiteralValue("true", true);
        public static readonly LiteralValue False = new LiteralValue("false", false);
        public static readonly LiteralValue Null = new LiteralValue("null", null);
    }
}
