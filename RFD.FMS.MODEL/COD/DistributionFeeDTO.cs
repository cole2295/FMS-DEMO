using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RFD.FMS.MODEL.COD
{
    public class DistributionFeeDTO
    {
        /// <summary>
        /// 调用方 vancl:0,vjia:1,rfd:2
        /// </summary>
        public Int32 Source { get; set; }

        /// <summary>
        /// 商家编号 vancl:8,vjia:9,其他：具体值
        /// </summary>
        public Int32 MerchantID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public Int64 FormCode { get; set; }

        /// <summary>
        /// 运单状态
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 配送公司编号
        /// </summary>
        public Int32 ExpressCompanyID { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        public Decimal Fare { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public Boolean IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public String ErrorMsg { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        public String ErrorCode { get; set; }
    }
}
