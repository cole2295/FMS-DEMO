using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util
{
    public class DateHelper
    {
        //public static 

        /// <summary>根据日期，获得星期几</summary>
        /// <param name="y">年</param> 
        /// <param name="m">月</param> 
        /// <param name="d">日</param> 
        /// <returns>星期几，1代表星期一；7代表星期日</returns>
        public static int getWeekDay(int y, int m, int d)
        {
            if (m == 1) m = 13;
            if (m == 2) m = 14;
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7 + 1;
            return week;
        }

        ///   <summary>   
        ///   取指定日期是一年中的第几周   
        ///   </summary>   
        ///   <param   name="dateTime">给定的日期</param>   
        ///   <returns>返回 该日期所在一年中的周数</returns>   
        public static int WeekOfYear(DateTime dateTime)
        {
            int firstdayofweek = System.Convert.ToDateTime(dateTime.Year.ToString() + "- " + "1-1 ").DayOfWeek.GetHashCode();
            int days = dateTime.DayOfYear;
            int daysOutOneWeek = days - (7 - firstdayofweek);
            if (daysOutOneWeek <= 0)
            {
                return 1;
            }
            else
            {
                int weeks = daysOutOneWeek / 7;
                if (daysOutOneWeek % 7 != 0)
                {
                    weeks++;
                }
                return weeks + 1;
            }
        }

        ///// <summary> 
        ///// 取指定日期是一年中的第几周 
        ///// </summary> 
        ///// <param name="dtime">给定的日期</param> 
        ///// <returns>数字 一年中的第几周</returns> 
        //public static int WeekOfYear(DateTime dtime)
        //{
        //    try
        //    {
        //        //确定此时间在一年中的位置
        //        int dayOfYear = dtime.DayOfYear;
        //        //当年第一天
        //        DateTime tempDate = new DateTime(dtime.Year, 1, 1);
        //        //确定当年第一天
        //        int tempDayOfWeek = (int)tempDate.DayOfWeek;
        //        tempDayOfWeek = tempDayOfWeek == 0 ? 7 : tempDayOfWeek;
        //        ////确定星期几
        //        int index = (int)dtime.DayOfWeek;
        //        index = index == 0 ? 7 : index;


        //        //当前周的范围
        //        DateTime retStartDay = dtime.AddDays(-(index - 1));
        //        DateTime retEndDay = dtime.AddDays(7 - index);


        //        //确定当前是第几周
        //        int weekIndex = (int)Math.Ceiling(((double)dayOfYear + tempDayOfWeek - 1) / 7);


        //        if (retStartDay.Year < retEndDay.Year)
        //        {
        //            weekIndex = 1;
        //        }


        //        return weekIndex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }


        //}

        /// <summary>
        /// 求某年有多少周
        /// </summary>
        /// <param name="dtime"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(DateTime dtime)
        {
            int countDay = DateTime.Parse(dtime.Year + "-12-31").DayOfYear;
            int countWeek = countDay / 7;
            return countWeek;
        }

        //根据年月日获得星期几
        public static string CaculateWeekDay(int month, int day)
        {
            string weekstr = string.Empty;
            int year = DateTime.Now.Year;
            //把一月和二月看成是上一年的十三月和十四月
            if (month == 1) { month = 13; year--; }
            if (month == 2) { month = 14; year--; }
            int week = (day + 2 * month + 3 * (month + 1) / 5 + year + year / 4 - year / 100 + year / 400) % 7;
            switch (week)
            {
                case 0: weekstr = "1"; break;
                case 1: weekstr = "2"; break;
                case 2: weekstr = "3"; break;
                case 3: weekstr = "4"; break;
                case 4: weekstr = "5"; break;
                case 5: weekstr = "6"; break;
                case 6: weekstr = "7"; break;
            }
            return weekstr;
        }

        /// <summary>
        /// 根据2个时间段获得相应的周数
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int WeekOfDate(DateTime beginDate, DateTime endDate)
        {
            TimeSpan ts1 = new TimeSpan(beginDate.Ticks);
            TimeSpan ts2 = new TimeSpan(endDate.Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration();
            int weeks = ts.Days / 7;

            //确定此时间在一年中的位置
            int dayOfYear = beginDate.DayOfYear;
            //当年第一天
            DateTime tempDate = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day);
            //最后一天
            DateTime tempendDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);
            int tempDayOfWeek = (int)tempDate.DayOfWeek;
            tempDayOfWeek = tempDayOfWeek == 0 ? 7 : tempDayOfWeek;

            ////确定星期几
            int startindex = (int)beginDate.DayOfWeek;
            startindex = startindex == 0 ? 7 : startindex;
            //当前周的范围
            DateTime retStartDay = beginDate.AddDays(-(startindex - 1));
            DateTime retEndDay = beginDate.AddDays(7 - startindex);
            //确定当前是第几周
            int weekIndex = (int)Math.Ceiling(((double)dayOfYear + tempDayOfWeek - 1) / 7);

            return weeks;
        }

        /// <summary>
        /// 根据起始时间，获取第几周
        /// </summary>
        /// <param name="dtime">当前时间</param>
        /// <returns></returns>
        public static int WeekOfTermDate(DateTime dtime)
        {
            string datetime = "2011-3-1";

            TimeSpan ts1 = new TimeSpan(dtime.Ticks);
            TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(datetime).Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration();

            //确定此时间在一年中的位置
            int dayOfYear = ts.Days;
            //当年第一天
            DateTime tempDate = new DateTime(Convert.ToDateTime(datetime).Year, Convert.ToDateTime(datetime).Month, Convert.ToDateTime(datetime).Day);

            int tempDayOfWeek = (int)tempDate.DayOfWeek;
            tempDayOfWeek = tempDayOfWeek == 0 ? 7 : tempDayOfWeek;
            ////确定星期几
            int index = (int)dtime.DayOfWeek;
            index = index == 0 ? 7 : index;

            //当前周的范围
            DateTime retStartDay = dtime.AddDays(-(index - 1));
            DateTime retEndDay = dtime.AddDays(7 - index);

            //确定当前是第几周
            int weekIndex = (int)Math.Ceiling(((double)dayOfYear + tempDayOfWeek) / 7);
            return weekIndex;
        }

        /// <summary>
        /// 根据哪年的第N周的星期N获得具体年月日
        /// </summary>
        /// <param name="week">第几周</param>
        /// <param name="day">星期几</param>
        /// <returns></returns>
        public static DateTime DateTimeByYearAndWeekAndDay(int year,int week, int day)
        {
            DateTime mDatetime = new DateTime(year, 1, 1);//year为要求的那一年
            int firstDayOfWeek = mDatetime.DayOfWeek.GetHashCode();
            int allWeekDay = ((week - 2) * 7);//整周数*7
            int firstWeekDays = 7 - firstDayOfWeek;//第一个周剩余天数
            return mDatetime.AddDays(allWeekDay + firstWeekDays + day);//第N周第day天 
        }

        public static KeyValuePair<DateTime, DateTime> GetDateRangeByWeekAndYear(int year,int week)
        {
            DateTime mDatetime = new DateTime(year, 1, 1);//year为要求的那一年
            //int firstweekfirstday = Convert.ToInt32(mDatetime.DayOfWeek);//一年中第一天是周几
            //var days = (double)(7 - firstweekfirstday);
            //DateTime secondweekfisrtday = mDatetime.AddDays(days); //第二周一

            int firstDayOfWeek = mDatetime.DayOfWeek.GetHashCode();
            int allWeekDay = ((week - 2) * 7);//整周数*7
            int firstWeekDays = 7 - firstDayOfWeek;//第一个周剩余天数

            var fisrtday = mDatetime.AddDays(allWeekDay + firstWeekDays + 1);//第N周第一天
            var lastday = mDatetime.AddDays(allWeekDay + firstWeekDays + 7);//第N周最后一天
            return new KeyValuePair<DateTime, DateTime>(fisrtday, lastday);
        }
    }
}
