using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.QueryStatistics
{
    public class FMSCodDaily
    {
        
        /// <summary>
        /// 配送公司编号
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 配送公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal AllFee { get; set; }

        /// <summary>
        /// 是否核对过
        /// </summary>
        /// 

        private bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }

            set { _isChecked = value; }
        }

    }

    public class FMSCODDetails
    {
       
            /// <summary>
            /// 运单号
            /// </summary>
            public long WaybillNo { get; set; }
            /// <summary>
            /// 配送公司编号
            /// </summary>
            public int CompanyId { get; set; }

            /// <summary>
            /// 配送公司名称
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public decimal Fee { get; set; }
       
    }
}
