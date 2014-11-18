using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{

    [Serializable]
    /// <summary>
    /// 搜索实体类(可自己添加参数)
    /// </summary>
    public class SearchModel
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 报表日期
        /// </summary>
        public DateTime dtDailyDate { get; set; }

        /// <summary>
        /// 商家ID
        /// </summary>
        public string MerchantId
        {
            get;
            set;
        }

        /// <summary>
        ///  站点ID
        /// </summary>
        public string StationId
        {
            get;
            set;
        }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string Sources
        {
            get;
            set;
        }
    }
}
