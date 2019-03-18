using System;
using TupacAmaru.Yacep.Exceptions;
using TupacAmaru.Yacep.Utils;

namespace TupacAmaru.Yacep.Evaluators
{
    public delegate object FunctionEvaluator(object function, object[] argumnets);
    public static class SimpleFunctionEvaluator
    {
        public static object CallFunction(object function, object[] argumnets)
        {
            if (function is Func<object> justReturn)
                return justReturn();
            if (function is Func<object, object> oneArgument)
                return oneArgument.Method.AsFunction()(oneArgument.Target, argumnets);
            if (function is Func<object[], object> arrayArgument)
                return arrayArgument(argumnets);
            if (function is Delegate @delegate)
                return @delegate.Method.AsFunction()(@delegate.Target, argumnets);
            throw new UnsupportedFunctionException();
        }
    }
}
