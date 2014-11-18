using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.SqlSecurity
{
    public static class SqlInjectionExtension
    {
        private static readonly Dictionary<string, string> BlackWordList = new Dictionary<string, string>
                                                                 {
                                                                     {"'","‘"},
                                                                     {"-","——"},
                                                                     {";",""}
                                                                 };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SqlInjectionFilter(this string s)
        {
            try
            {
                BlackWordList.ToList().ForEach(blackWord => s = s.Replace(blackWord.Key, blackWord.Value));
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
            return s;
        }
    }
}
