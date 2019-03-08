using System;
using TupacAmaru.Yacep.Symbols;

namespace TupacAmaru.Yacep.BuiltIn
{
    public static class DateAndTimeFunctions
    {
        private static object NowHandler(object[] arguments)
        {
            var format = arguments.Length > 0 ? arguments[0] as string : null;
            return DateTime.Now.ToString(format ?? "yyyy/MM/dd HH:mm:ss");
        }
        private static object TodayHandler(object[] arguments)
        {
            var format = arguments.Length > 0 ? arguments[0] as string : null;
            return DateTime.Today.ToString(format ?? "yyyy/MM/dd");
        }
        private static object TimeHandler(object[] arguments)
        {
            var format = arguments.Length > 0 ? arguments[0] as string : null;
            return DateTime.Now.ToString(format ?? "HH:mm:ss");
        }
        private static object YearHandler(object[] arguments) => DateTime.Now.Year;
        private static object MonthHandler(object[] arguments) => DateTime.Now.Month;
        private static object DayHandler(object[] arguments) => DateTime.Now.Day;
        private static object HourHandler(object[] arguments) => DateTime.Now.Hour;
        private static object MinuteHandler(object[] arguments) => DateTime.Now.Minute;
        private static object SecondHandler(object[] arguments) => DateTime.Now.Second;

        public static readonly NakedFunction Now = new NakedFunction("now", NowHandler);
        public static readonly NakedFunction Date = new NakedFunction("today", TodayHandler);
        public static readonly NakedFunction Time = new NakedFunction("time", TimeHandler);
        public static readonly NakedFunction Year = new NakedFunction("year", YearHandler);
        public static readonly NakedFunction Month = new NakedFunction("month", MonthHandler);
        public static readonly NakedFunction Day = new NakedFunction("day", DayHandler);
        public static readonly NakedFunction Hour = new NakedFunction("hour", HourHandler);
        public static readonly NakedFunction Minute = new NakedFunction("minute", MinuteHandler);
        public static readonly NakedFunction Second = new NakedFunction("second", SecondHandler);
    }
}
