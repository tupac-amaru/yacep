using System;
using TupacAmaru.Yacep.Exceptions;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object FunctionEvaluator(object function, object[] argumnets);
    public static class SimpleFunctionEvaluator
    {
        public static object CallFunction(object function, object[] argumnets)
        {
            if (function is Func<object[], object> func)
                return func(argumnets);
            throw new UnsupportedFunctionException();
        }
    }
}
