using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Model;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// 配送费实体
	/// </summary>
	public class FMS_DeliverFee : BaseDataModel<int>
	{
		/// <summary>
		/// 当前操作的ID
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// 来源商家
		/// </summary>
		public int MerchantID { get; set; }
        /// <summary>
        /// 来源商家名称
        /// </summary>
        public string MerchantName { get; set; }
		/// <summary>
		/// 基本配送费
		/// </summary>
		public string BasicDeliverFee { get; set; }
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
		public EnumCODAudit Status { get; set; }
		/// <summary>
		/// 修改批次号
		/// </summary>
		public string UpdateBatchNo { get; set; }
	}
}
