using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RFD.FMS.Model.FinancialManage
{
    public class FMS_IncomeReceiveFeeInfo
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
        /// 运单类型
        /// </summary>
        public string WaybillType { get; set; }	

        /// <summary>
        /// 配送员编号
        /// </summary>
        public int DeliverManID { get; set; }

        /// <summary>
        /// 运单来源
        /// </summary>
        public int Sources { get; set; }	

        /// <summary>
        /// 下单方式
        /// </summary>
        public int ComeFrom	{ get; set; }	

        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string CustomerOrder	{ get; set; }	

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? WaybillCreateTime { get; set; }	

        /// <summary>
        /// 配送站
        /// </summary>
        public int DeliverStationID { get; set; }	

        /// <summary>
        /// 支付方式
        /// </summary>
        public string AcceptType { get; set; }

        /// <summary>
        /// 归班时间
        /// </summary>
        public DateTime? BackStationCreateTime { get; set; }

        /// <summary>
        /// 签收状态
        /// </summary>
        public string SignStatus { get; set; }	

        /// <summary>
        /// POS机号码
        /// </summary>
        public string POSCode { get; set; }	

        /// <summary>
        /// 实退金额
        /// </summary>
        public decimal FactBackAmount { get; set; }	

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal FactAmount { get; set; }	

        /// <summary>
        /// 收款状态
        /// </summary>
        public int FinancialStatus { get; set; }	

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? SignInfoCreateTime { get; set; }	

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
        /// 配送商编码
        /// </summary>
        public string DiscributionCode { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal NeedAmount { get; set; }

        /// <summary>
        /// 应退金额
        /// </summary>
        public decimal NeedBackAmount { get; set; }

        /// <summary>
        ///1寄方付,2到方付
        /// </summary>
        public int TransferPayType { get; set; }

        /// <summary>
        ///1现结，3月结
        /// </summary>
        public int AccountType { get; set; }
    }
}
