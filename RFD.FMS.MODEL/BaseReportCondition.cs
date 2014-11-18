using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
    /*
     * ***********************
     * 通用报表条件基类
     * Terry
     * 2011-09-08
     * ***********************
     */

    [Serializable]
    public class BaseReportCondition
    {
        /// <summary>
        /// 报表开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 报表结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get;
            set;
        }




    }
}
