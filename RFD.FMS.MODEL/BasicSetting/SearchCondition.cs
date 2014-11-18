using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.Enumeration;
using System.Data;

namespace RFD.FMS.MODEL.BasicSetting
{
	/*
	* (C)Copyright 2011-2012 如风达信息管理系统
	* 
	* 模块名称：查询条件实体
	* 说明：指定查询的条件进行查询
	* 作者：何名宇
	* 创建日期：2011/07/13
	* 修改人：
	* 修改时间：
	* 修改记录：
	*/
	[Serializable]
	public class SearchCondition
	{
        public int ID {get;set;}//主键
		public int Source { get; set; }//订单来源(vancl,vjia,其他)
		public int DeliverStation { get; set; }//配送站点
		public string StationName { get; set; }//配送站点名称
		public DateTime BeginTime { get; set; }//开始时间
		public DateTime EndTime { get; set; }//结束时间
		public DateTime SearchDate { get; set; }//搜索日期
		public int PayType { get; set; }//付款方式
		public DataTable OrderNoList { get; set; }//订单号列表
		public bool IsRawData { get; set; }//是否处理已获得的数据
		public int MerchantID { get; set; }//商家ID
        public string MerchantIDs { get; set; }//商家ID 集合，逗号分割，add by zengwei 20120509
		public string MerchantName { get; set; }//商家名称
		public string SimpleSpell { get; set; }//拼音简写
		public string StatusList { get; set; }//维护状态列表 
		public string OrderBy;//排序列
		public string Direction;//排序方向
	    public string DistributionCode{ get; set; }
		public string ExportPath;//导出路径
		public ExportFileFormat ExportFormat;//导出格式
	    public int POSType;//POS机类型 add by wangyongc 2012-03-05
        public int TransferPayType;//配送费结算方式 add by wangyongc 2012-03-05
        public int SearchWaybillType { get; set; }//查询类型：0运单号查询，1订单号查询
        public bool IsEffect { get; set; }//是否待生效
        public string PeriodExpressIds { get; set; }//账期配送公司ID
        public string PeriodExpressNexus { get; set; }//账期配送公司运算关系符
	}
	public enum SearchType
	{
		Total,
		Details,
		AllDetails,
		Success,
		Delay,
		Reload,
        /// <summary>
        /// 账期汇总
        /// </summary>
        AccountPeriodTotal,
        /// <summary>
        /// 账期明细
        /// </summary>
        AccountPeriodDetails,
        /// <summary>
        /// 账期所有明细
        /// </summary>
        AccountPeriodAllDetails
	}
	public enum StatusType
	{
		WaybillStatus = 1,
		WaybillType = 2
	}
	/// <summary>
	/// 报表对碰方向
	/// </summary>
	public enum CompareDirection
	{
		/// <summary>
		/// 系统报表对碰手工报表
		/// </summary>
		SystemToManual = 1,
		/// <summary>
		/// 手工报表对碰系统报表
		/// </summary>
		ManualToSystem = 2
	}
	public enum LoadScriptType
	{
		Reload,
		Init,
		Search,
		All
	}
}
