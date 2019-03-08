using System;

namespace TupacAmaru.Yacep.Exceptions
{
    public class MemberNotFoundException : Exception
    {
        public MemberNotFoundException(Type type, string memberName)
            : base($"Not found member({memberName}) in ({type.Name})") { }
    }
}
