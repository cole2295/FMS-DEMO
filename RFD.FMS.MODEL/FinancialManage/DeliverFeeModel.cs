using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    [Serializable]
    public class DeliverFeeModel
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public long InfoID { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public long WaybillNO { get; set; }

        /// <summary>
        /// 配送区域
        /// </summary>
        public int Area { get; set; }

        /// <summary>
        /// 结算重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 计算公式
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        public decimal DeliverFee { get; set; }

        /// <summary>
        /// 未计算原因
        /// </summary>
        public int IsFare { get; set; }
    }
}
