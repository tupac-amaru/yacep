namespace TupacAmaru.Yacep.Utils
{
    internal static class CharHelper
    {
        internal static bool IsSpace(this char chr)
            => chr == 32 || chr == 8 || chr == 9 || chr == 10 || chr == 13 || chr == 12288;
    }
}
