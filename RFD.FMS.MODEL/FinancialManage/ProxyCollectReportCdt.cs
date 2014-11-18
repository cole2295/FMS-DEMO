using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RFD.FMS.MODEL.FinancialManage
{
    /*
 * ***********************
 * 代收款报表查询条件类
 * Terry
 * 2011-09-08
 * ***********************
 */

    public class ProxyCollectReportCdt :  PCSReportcdt
    {

        [Description("报表类型")]
        public enum ReportType
        {
            /// <summary>
            /// 日报表
            /// </summary>
            [Description("日报表")]
            DailyReport = 0,
            /// <summary>
            /// 周期报表
            /// </summary>
            [Description("周期报表")]
            WeeklyReport = 1
        }

        private ReportType _reporttype;

        private string _merchantid;

        /// <summary>
        /// 商家ID
        /// </summary>
        public string MerchantID
        {
            get
            {
                return _merchantid;
            }
            set
            {
                if (value != null)
                {
                    _merchantid = value.Trim();
                }
            }
        }

        /// <summary>
        /// 报表类型
        /// </summary>
        public ReportType Type
        {
            get
            {
                return _reporttype;
            }
            set
            {
                _reporttype = value;
            }
        }


    }


}
