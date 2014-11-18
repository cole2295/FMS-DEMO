using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util
{
    public static class BaseTypeExtension
    {
        public static bool CanGetDecimal(this string t)
        {
            var value = default(decimal);
            return decimal.TryParse(t, out value);
        }


        public static decimal TryGetDecimal(this string t)
        {
            var value = default(decimal);
            decimal.TryParse(t, out value);
            return value;
        }

        public static bool CanGetInt(this string t)
        {
            var value = default(int);
            return int.TryParse(t, out value);
        }

        public static int TryGetInt(this string t)
        {
            var value = default(int);
            int.TryParse(t, out value);
            return value;
        }

        public static bool CanGetLong(this string t)
        {
            var value = default(long);
            return long.TryParse(t, out value);
        }

        public static long TryGetLong(this string t)
        {
            var value = default(long);
            long.TryParse(t, out value);
            return value;
        }

        public static bool CanGetDateTime(this string t)
        {
            var value = DateTime.MinValue;
            return DateTime.TryParse(t, out value);
        }

        public static DateTime TryGetDateTime(this string t)
        {
            var value = DateTime.MinValue;
            DateTime.TryParse(t, out value);
            return value;
        }


        /// <summary>   
        /// datetime 转成Unix时间戳   
        /// </summary>   
        /// <param name="dt"></param>   
        /// <returns></returns>   
        public static long GetUnixTimeStamp(this DateTime dt)
        {
            DateTime unixStartTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan timeSpan = dt.Subtract(unixStartTime);
            return timeSpan.Ticks / 10000000;
        }

    }
}
