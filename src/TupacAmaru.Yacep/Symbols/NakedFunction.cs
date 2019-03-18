using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Symbols
{
    public delegate object NakedFunctionHandler(object[] arguments);

    public class NakedFunction
    {
        public NakedFunction(string name, NakedFunctionHandler handler, bool cacheable = false)
        {
            if (name.ContainsSpace())
                throw new CannotContainsSpacesException("Naked function name");
            Name = name;
            Handler = handler;
            Cacheable = cacheable;
        }

        public string Name { get; }
        public NakedFunctionHandler Handler { get; }
        public bool Cacheable { get; }
    }
}