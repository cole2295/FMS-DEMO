using System.ComponentModel;
using RFD.FMS.AdoNet.UnitOfWork;

namespace RFD.FMS.AdoNet
{
	/// <summary>
	/// 公司
	/// </summary>
	public enum CompanySource
	{
		/// <summary>
		/// 凡客
		/// </summary>
		Vancl = 1,
		/// <summary>
		/// VJIA
		/// </summary>
		Vjia = 2,
	}

	/// <summary>
	/// 数据库类型
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// 执行权限
		/// </summary>
		Execute = 1,
		/// <summary>
		/// 只读权限
		/// </summary>
		Readonly = 2,
	}

	/// <summary>
	/// 数据库源，用于指定一个<see cref="IUnitOfWork"/>的数据库。
	/// </summary>
	public enum DatabaseSource
	{
		/// <summary>
		/// 未指定，根据当前登录员工决定连接的数据库。
		/// </summary>
		[Description("Unspecified")]
		Unspecified,
		/// <summary>
		/// SCM库
		/// </summary>
		SCM,
		/// <summary>
		/// 使用Vancl的数据库
		/// </summary>
		[Description("SCM")]
		Vancl,
		/// <summary>
		/// 使用Vjia的数据库
		/// </summary>
		[Description("SCM_VJIA")]
		Vjia,
		/// <summary>
		/// 订单数据库
		/// </summary>
		[Description("Order")]
		Order,
		/// <summary>
		/// 客户数据库
		/// </summary>
		[Description("Customer")]
		Customer,
		/// <summary>
		/// 凡库数据库
		/// </summary>
		[Description("FanKu")]
		FanKu,
		/// <summary>
		/// 使用当前登陆仓库的数据库
		/// </summary>
		[Description("WMS")]
		WMS,
		/// <summary>
		/// 广州仓库数据库
		/// </summary>
		[Description("WMS_GZ")]
		WMS_GZ,
		/// <summary>
		/// 广州化妆品库
		/// </summary>
		[Description("WMS_GZC")]
		WMS_GZC,
		/// <summary>
		/// 北京仓库数据库
		/// </summary>
		[Description("WMS_BJ")]
		WMS_BJ,
		/// <summary>
		/// 北京化妆品库
		/// </summary>
		[Description("WMS_BJC")]
		WMS_BJC,
		///<summary>
		/// 上海仓库数据库
		///</summary>
		[Description("WMS_SH")]
		WMS_SH,
		/// <summary>
		/// 上海化妆品库
		/// </summary>
		[Description("WMS_SHC")]
		WMS_SHC,
		/// <summary>
		/// 武汉仓库数据库
		/// </summary>
		[Description("WMS_WH")]
		WMS_WH,
		/// <summary>
		/// 成都仓库数据库
		/// </summary>
		[Description("WMS_CD")]
		WMS_CD,
		/// <summary>
		/// 西安数据库
		/// </summary>
		[Description("WMS_XA")]
		WMS_XA,
		/// <summary>
		/// WMS Vjia 实际是WMS vjia bj
		/// </summary>
		[Description("WMS_Vjia")]
		WMS_Vjia,
		/// <summary>
		/// 未指定，根据当前登录员工决定连接的只读数据库。
		/// </summary>
		[Description("UnspecifiedReadOnly")]
		UnspecifiedReadOnly,
        /// <summary>
        /// 如风达系统库
        /// </summary>
        [Description("LMS_RFD")]
        LMS_RFD
	}

	///<summary>
	/// 数据库源，用于指定当前登陆的仓库
	///</summary>
	public enum WMSDatabaseSource
	{
		///<summary>
		/// 未指定，根据当前登录员工决定连接的WMS数据库。
		///</summary>
		[Description("Unspecified")]
		Unspecified,
		///<summary>
		/// 使用广州的数据库
		///</summary>
		[Description("WMS_GZ")]
		WMS_GZ = 1,
		/// <summary>
		/// 广州化妆品库
		/// </summary>
		[Description("WMS_GZC")]
		WMS_GZC,
		///<summary>
		/// 使用北京的WMS数据库
		///</summary>
		[Description("WMS_BJ")]
		WMS_BJ,
		/// <summary>
		/// 北京化妆品库
		/// </summary>
		[Description("WMS_BJC")]
		WMS_BJC,
		///<summary>
		/// 使用上海的WMS数据库
		///</summary>
		[Description("WMS_SH")]
		WMS_SH,
		/// <summary>
		/// 上海化妆品库
		/// </summary>
		[Description("WMS_SHC")]
		WMS_SHC,
		///<summary>
		/// 武汉仓库
		///</summary>
		[Description("WMS_WH")]
		WMS_WH,
		///<summary>
		/// 成都仓库
		///</summary>
		[Description("WMS_CD")]
		WMS_CD,
		/// <summary>
		/// 西安数据库
		/// </summary>
		[Description("WMS_XA")]
		WMS_XA,
		/// <summary>
		/// WMS Vjia
		/// </summary>
		[Description("WMS_Vjia")]
		WMS_Vjia,
		/// <summary>
		/// 所有可卖的仓库
		/// </summary>
		WMS_ForOrder,
	}
}