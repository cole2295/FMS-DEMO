using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	[Serializable]
	public class RequisitionedFormModel
	{
		/// <summary>
		/// 订单号
		/// </summary>
		public string WaybillNO { get; set; }
		/// <summary>
		/// 配送公司
		/// </summary>
		public string CompanyName { get; set; }
		/// <summary>
		/// 发货时间
		/// </summary>
		public DateTime DeliverTime { get; set; }
		/// <summary>
		/// 重量
		/// </summary>
		public decimal Weight { get; set; }
		/// <summary>
		/// 仓库ID
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 仓库名称
		/// </summary>
		public string WarehouseName { get; set; }
		/// <summary>
		/// 运费
		/// </summary>
		public decimal DeliveryFare { get; set; }
		/// <summary>
		/// 发货地址
		/// </summary>
		public string Address { get; set; }
		/// <summary>
		/// 领用单号
		/// </summary>
		public string RequisitionedNo { get; set; }
		/// <summary>
		/// 领用人
		/// </summary>
		public string RequisitionedBy { get; set; }
		/// <summary>
		/// 部门
		/// </summary>
		public string Dept { get; set; }
		/// <summary>
		/// 制单人
		/// </summary>
		public string BuildBy { get; set; }
	}

	public class RequisitionedStatModel
	{
		public string DeptName { get; set; }
		public string CompanyName { get; set; }
		public string WarehouseName { get; set; }
		public decimal FareSum { get; set; }
		public decimal WeightSum { get; set; }
		public int CountNum { get; set; }
	}
}
