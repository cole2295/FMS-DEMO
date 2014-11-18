using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.Enumeration;
using System.Data;
using RFD.FMS.Model;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// 商家配送费
	/// </summary>
	public class FMS_MerchantDeliverFee : BaseDataModel<int>
	{
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

		/// <summary>
		/// 来源商家
		/// </summary>
		public int MerchantID { get; set; }
		/// <summary>
		/// 基本配送费
		/// </summary>
		public decimal? BasicDeliverFee { get; set; }
		/// <summary>
		/// 货款结算方式
		/// </summary>
		public SettleAccountType PaymentType { get; set; }
		/// <summary>
		/// 货款结算周期
		/// </summary>
		public int? PaymentPeriod { get; set; }
		/// <summary>
		/// 货款结算周期开始日期
		/// </summary>
		public DateTime? PaymentPeriodDate { get; set; }
		/// <summary>
		/// 配送费结算方式
		/// </summary>
		public SettleAccountType DeliverFeeType { get; set; }
		/// <summary>
		/// 配送费结算周期
		/// </summary>
		public int? DeliverFeePeriod { get; set; }
		/// <summary>
		/// 配送费结算周期开始日期
		/// </summary>
		public DateTime? DeliverFeePeriodDate { get; set; }
		/// <summary>
		/// 计费因素
		/// </summary>
		public string FeeFactors { get; set; }
		/// <summary>
		/// 基本配送费是否一致(0:不一致,1:一致)
		/// </summary>
		public bool IsUniformedFee { get; set; }
		/// <summary>
		/// 拒收订单配送费
		/// </summary>
		public decimal RefuseFeeRate { get; set; }
        /// <summary>
        /// 拒收订单配送附加费
        /// </summary>
        public decimal ExtraRefuseFeeRate { get; set; }

	    /// <summary>
		/// 上门退订单配送费
		/// </summary>
		public decimal VisitReturnsFeeRate { get; set; }
        /// <summary>
        /// 上门退订附加费
        /// </summary>
        public decimal ExtraVisitReturnsFeeRate { get; set; }
	    /// <summary>
		/// 上门退拒收订单配送费
		/// </summary>
		public decimal VisitReturnsVFeeRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ExtraVisitReturnsVFeeRate { get; set; }
		/// <summary>
		/// 上门换订单配送费
		/// </summary>
		public decimal VisitChangeFeeRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ExtraVisitChangeFeeRate { get; set; }
		/// <summary>
		/// 代收货款现金 手续费 结算类型 0 周期结，1月结
		/// </summary>
		public int ReceiveFeeType { get; set; }
		/// <summary>
		/// 代收货款现金 手续费
		/// </summary>
		public decimal ReceiveFeeRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ExtraReceiveFeeRate { get; set; }
		/// <summary>
		/// 代收货款现金 服务费 结算类型 0 周期结，1月结
		/// </summary>
		public int CashServiceType { get; set; }
		/// <summary>
		/// 代收货款现金 服务费
		/// </summary>
		public decimal CashServiceFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ExtraCashServiceFee { get; set; }
		/// <summary>
		/// 代收货款POS 手续费 结算类型 0 周期结，1月结
		/// </summary>
		public int ReceivePOSFeeType { get; set; }
		/// <summary>
		/// 代收货款POS 手续费
		/// </summary>
		public decimal ReceivePosFeeRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ExtraReceivePosFeeRate { get; set; }
		/// <summary>
		/// 代收货款POS手续费 服务费 结算类型 0 周期结，1月结
		/// </summary>
		public int POSServiceType { get; set; }
		/// <summary>
		/// 代收货款POS手续费 服务费
		/// </summary>
		public decimal POSServiceFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ExtraPOSServiceFee { get; set; }
		/// <summary>
		/// 计费公式
		/// </summary>
		public int? FormulaID { get; set; }
		/// <summary>
		/// 公式参数列表
		/// </summary>
		public string FormulaParamters { get; set; }
        /// <summary>
        /// 首重
        /// </summary>
        public decimal FirstWeight { get; set; }
        /// <summary>
        /// 计算参数1
        /// </summary>
        public decimal StatPramer { get; set; }
        /// <summary>
        /// 续重价格
        /// </summary>
        public decimal AddWeightPrice { get; set; }
        /// <summary>
        /// 首重价格
        /// </summary>
        public decimal FirstWeightPrice { get; set; }
        /// <summary>
        /// 体积计算参数
        /// </summary>
        public decimal VolumeParmer { get; set; }
        /// <summary>
        /// 保价费率
        /// </summary>
        public decimal ProtectedParmer { get; set; }
        /// <summary>
        /// 保价附加费
        /// </summary>
        public decimal ExtraProtected { get; set; }

	    /// <summary>
		/// 最后审核人
		/// </summary>
		public int? AuditBy { get; set; }
		/// <summary>
		/// 审核时间
		/// </summary>
		public DateTime? AuditTime { get; set; }
		/// <summary>
		/// 审核人编号
		/// </summary>
		public string AuditCode { get; set; }
		/// <summary>
		/// 审核结果
		/// </summary>
		public AuditResult? AuditResult { get; set; }
		/// <summary>
		/// 维护状态
		/// </summary>
		public MaintainStatus Status { get; set; }

		/// <summary>
		/// 修改批次号
		/// </summary>
		public string UpdateBatchNo { get; set; }

        /// <summary>
        /// 商家重量类型
        /// </summary>
        public int? WeightType { get; set; }

        /// <summary>
        /// 重量数值规则
        /// </summary>
        public int? WeightValueRule { get; set; }

        /// <summary>
        /// 所属配送商编码
        /// </summary>
        public string DistributionCode { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? EffectDate { get; set; }

        /// <summary>
        /// 生效的唯一ID
        /// </summary>
        public Int64 EffectID { get; set; }
        /// <summary>
        /// 货物品类结算
        /// </summary>
        public int IsCategory { get; set; }
	}
}
