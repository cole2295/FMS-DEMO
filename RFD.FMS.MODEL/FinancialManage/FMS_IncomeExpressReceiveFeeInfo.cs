using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RFD.FMS.Model.FinancialManage
{
    public class FMS_IncomeExpressReceiveFeeInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long FeeID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public long WaybillNO { get; set; }

        /// <summary>
        /// 运单来源
        /// </summary>
        public int Sources { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantID { get; set; }

        /// <summary>
        /// 配送员编号
        /// </summary>
        public int DeliverManID { get; set; }

        /// <summary>
        /// 运单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? WaybillCreatTime { get; set; }

        /// <summary>
        /// 取件站点
        /// </summary>
        public int ReceiveStationID { get; set; }

        /// <summary>
        /// 派件站点
        /// </summary>
        public int DeliverStationID { get; set; }

        /// <summary>
        /// 配送商编码
        /// </summary>
        public string DistributionCode { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal TransferFee { get; set; }

        /// <summary>
        /// 保险费
        /// </summary>
        public decimal DinsureFee { get; set; }

        /// <summary>
        /// 结算方式（1:现付;2:到付;3:月结）
        /// </summary>
        public int TransferPayType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string AcceptType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? BackStationCreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string CustomerOrder { get; set; }

        /// <summary>
        /// POS机号码
        /// </summary>
        public string POSCode { get; set; }
    }
}
