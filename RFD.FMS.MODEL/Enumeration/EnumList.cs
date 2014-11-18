/*
* (C)Copyright 2011-2012 如风达信息管理系统
* 
* 模块名称：公共枚举类
* 说明：订单状态、订单类型
* 作者：wangyong
* 创建日期：2011-07-19
* 修改人：何名宇
* 修改时间：2011-07-26
* 修改记录：将枚举变量独立出来
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RFD.FMS.MODEL.Enumeration
{
	/// <summary>
	/// 状态信息
	/// </summary>
	public enum StatusInfo
	{
		[Description("运单来源")]
		WaybillSource = 3,
		[Description("运单状态")]
		WaybillStatus = 1
	}

	/// <summary>
	/// 运单状态
	/// </summary>
	public enum WayBillStatus
	{
		/// <summary>
		/// 无效订单
		/// </summary>
		[Description("无效订单")]
		DisabledBill = -9,

		/// <summary>
		/// 退换货入库
		/// </summary>
		[Description("退换货入库")]
		ReturnBounded = 6,

		/// <summary>
		/// 拒收入库
		/// </summary>
		[Description("拒收入库")]
		RefusedBounded = 7,

		/// <summary>
		/// 待入分拣中心
		/// </summary>
		[Description("待入分拣中心")]
		WaitingBounded = -3,
		/// <summary>
		/// 已入分拣中心
		/// </summary>
		[Description("已入分拣中心")]
		InBounded = -2,
		/// <summary>
		/// 已分拣
		/// </summary>
		[Description("已分拣")]
		AssignedBound = -1,
		/// <summary>
		/// 运输在途
		/// </summary>
		[Description("运输在途")]
		WaitingStation = 0,
		/// <summary>
		/// 已入站
		/// </summary>
		[Description("已入站")]
		EnterStation = 1,
		/// <summary>
		/// 已分配
		/// </summary>
		[Description("已分配")]
		Assigned = 2,
		/// <summary>
		/// 配送员已出站
		/// </summary>
		[Description("配送员已出站")]
		OutStation = 2,
		/// <summary>
		/// 妥投
		/// </summary>
		[Description("妥投")]
		Success = 3,
		/// <summary>
		/// 滞留
		/// </summary>
		[Description("滞留")]
		Resort = 4,
		/// <summary>
		/// 拒收
		/// </summary>
		[Description("拒收")]
		Reject = 5,
		//6已用
		//7已用
		/// <summary>
		/// 转站中
		/// </summary>
		[Description("转站中")]
		TurnStationApply = 8,

		/// <summary>
		/// 待分配
		/// </summary>
		[Description("待分配")]
		WatingAssign = 9,

		/// <summary>
		/// 已出库
		/// </summary>
		[Description("已出库")]
		OutBounded = 10,

		/// <summary>
		/// 返货在途
		/// </summary>
		[Description("返货在途")]
		ReturnOnStation = 11,

		/// <summary>
		/// 分拣接收
		/// </summary>
		[Description("分拣接收")]
		SortingIncept = 12
	}

	/// <summary>
	/// 返货状态
	/// </summary>
	public enum BackStatus
	{
		/// <summary>
		/// 退换货入库
		/// </summary>
		[Description("退换货入库")]
		ReturnBounded = 6,

		/// <summary>
		/// 拒收入库
		/// </summary>
		[Description("拒收入库")]
		RefusedBounded = 7,

		/// <summary>
		/// 返货在途
		/// </summary>
		[Description("返货在途")]
		ReturnOnStation = 0,

		/// <summary>
		/// 返货入库
		/// </summary>
		[Description("返货入库")]
		SortingIncept = 1
	}

	/// <summary>
	/// 运单类型
	/// </summary>
	public enum WayBillType
	{
		/// <summary>
		/// 普通订单
		/// </summary>
		[Description("普通订单")]
		Common = 0,

		/// <summary>
		/// 上门换货单
		/// </summary>
		[Description("上门换货单")]
		Exchange = 1,

		/// <summary>
		/// 上门退货单
		/// </summary>
		[Description("上门退货单")]
		Returned = 2
	}

	/// <summary>
	/// 订单来源
	/// </summary>
	public enum WaybillSourse
	{
		[Description("VANCL")]
		Vancl = 0,
		[Description("VJIA")]
		Vjia = 1,
		[Description("其他")]
		Other = 2,
	}

	/// <summary>
	/// 单据号种子类型
	/// </summary>
	public enum InvoiceType
	{
		[Description("如风达运单号")]
		Waybill = 0,
		[Description("如风达批次号")]
		Batch = 1,
		[Description("入库单")]
		InBound = 2,
		[Description("出库单")]
		OutBound = 3,
		[Description("主管票证")]
		AreaRoleUserMapping = 4
	}

	/// <summary>
	/// 转站类型
	/// </summary>
	public enum TurnStationType
	{
		[Description("已申请")]
		Applied = 0,
		[Description("已审核")]
		Audited = 1,
		[Description("运输在途")]
		Transition = 2,
		[Description("已接收")]
		Accepted = 3
	}

	/// <summary>
	/// 主管映射类型
	/// </summary>
	public enum userRoleMappingTypeEnum
	{
		[Description("主管")]
		Admin = 1,
		[Description("区域主管")]
		AreaManager = 2,
		[Description("城市主管")]
		CityManager = 3,
		[Description("站点主管")]
		StationManager = 4
	}

	/// <summary>
	/// 支付方式
	/// </summary>
	public enum PaymentType
	{
		[Description("现金")]
		Cash = 0,
		[Description("POS机")]
		POS = 1
	}

	/// <summary>
	/// 结算方式
	/// </summary>
	public enum SettleAccountType
	{
		[Description("日结")]
		ByDay = 0,
		[Description("自然月结")]
		ByMonth = 1
	}

	public enum Factors
	{
		[Description("固定费用")]
		FixedFee = 0,
		[Description("重量")]
		Weight = 1,
		[Description("体积")]
		Volumn = 2,
		[Description("城际距离")]
		Distant = 3
	}

	/// <summary>
	/// 审核结果
	/// </summary>
	public enum AuditResult
	{
		[Description("驳回")]
		Reject = 0,
		[Description("审核成功")]
		Audited = 1
	}

	/// <summary>
	/// 维护状态
	/// </summary>
	public enum MaintainStatus
	{
		/// <summary>
		/// 待维护
		/// </summary>
		[Description("待维护")]
		Maintain = 0,
		/// <summary>
		/// 待审核
		/// </summary>
		[Description("待审核")]
		Auditing = 1,
		/// <summary>
		/// 已审核
		/// </summary>
		[Description("已审核")]
		Audited = 2,
		/// <summary>
		/// 已置回
		/// </summary>
		[Description("已置回")]
		Reset = 3
	}

	/// <summary>
	/// 财务收款状态
	/// </summary>
	public enum AcceptStatus
	{
		/// <summary>
		/// 未收款
		/// </summary>
		[Description("未收款")]
		UnReceived = 0,
		/// <summary>
		/// 已收款
		/// </summary>
		[Description("已收款")]
		Received = 1
	}
	/// <summary>
	/// 导出文件格式
	/// </summary>
	public enum ExportFileFormat
	{
		[Description("CSV")]
		CSV = 0,
		[Description("EXCEL")]
		Excel = 1,
		[Description("PDF")]
		PDF = 2
	}

    /// <summary>
    /// 催款类型
    /// </summary>
    public enum enumUrge
    {
        [Description("催黄联")]
        BorrowToBX = 1,
        [Description("催款")]
        BorrowToFee = 2
    }

	public enum EnumCODAudit
	{
		/// <summary>
		/// 已审核
		/// </summary>
		[Description("已审核")]
		A1 = 1,
		/// <summary>
		/// 未审核
		/// </summary>
		[Description("未审核")]
		A2 = 2,
		/// <summary>
		/// 置回
		/// </summary>
		[Description("置回")]
		A3 = 3
	}

	public enum EnumAccountAudit
	{
		/// <summary>
		/// 结算待提交
		/// </summary>
		[Description("结算待提交")]
		A1 = 1,
		/// <summary>
		/// 结算待审核
		/// </summary>
		[Description("结算待审核")]
		A2 = 2,
		/// <summary>
		/// 结算置回
		/// </summary>
		[Description("结算置回")]
		A3 = 3,
		/// <summary>
		/// 已结算
		/// </summary>
		[Description("已结算")]
		A4 = 4
	}

	/// <summary>
	/// 获取配送商类型
	/// </summary>
	public enum EnumLoadCompanyType
	{
		/// <summary>
		/// 全部可用配送商
		/// </summary>
		[Description("全部可用配送商")]
		AllCompany=0,
		/// <summary>
		/// 只是rfd站点、第三方配送商
		/// </summary>
		[Description("只是rfd站点、第三方配送商")]
		OnlySite=1,
		/// <summary>
		/// RFD下所有
		/// </summary>
		[Description("RFD下所有")]
		OnlyRFD=2,
		/// <summary>
		/// 第三方配送公司
		/// </summary>
		[Description("第三方配送公司")]
		OnlyThirdCompany = 3,
        /// <summary>
        /// 第三方配送公司和RFD
        /// </summary>
        [Description("第三方配送公司和RFD")]
        OnlyThirdRFD = 4,
        /// <summary>
        /// 配送公司站点
        /// </summary>
        [Description("配送公司站点")]
        CompanySite = 5
	}

    /// <summary>
    /// 商家结算重量取值类型
    /// </summary>
    public enum MerchantWeightType
    {
        /// <summary>
        /// 商家重量
        /// </summary>
        [Description("商家重量")]
        W1 = 0,
        /// <summary>
        /// 称重重量
        /// </summary>
        [Description("称重重量")]
        W2 = 1,
        /// <summary>
        /// 商家重量与体积重量较大者
        /// </summary>
        [Description("商家重量与体积重量较大者")]
        W3 = 2,
        /// <summary>
        /// 取件重量
        /// </summary>
        [Description("取件重量")]
        W4 = 3,
        /// <summary>
        /// 单量结算
        /// </summary>
        [Description("单量结算")]
        W5 = 4
    }

    /// <summary>
    /// 商家结算重量 取件重量 取值类型
    /// </summary>
    public enum MerchantWeightValueRule
    {
        /// <summary>
        /// 四舍五入
        /// </summary>
        [Description("原重量")]
        V9 = 9,
        /// <summary>
        /// 四舍五入
        /// </summary>
        [Description("四舍五入")]
        V0 = 0,
        /// <summary>
        /// 2下3上，7下8
        /// </summary>
        [Description("2下3上，7下8上")]
        V1 = 1
    }

    /// <summary>
    /// 区域类型 审核
    /// </summary>
    public enum AreaLevelStatus
    {
        /// <summary>
        /// 待维护
        /// </summary>
        [Description("待维护")]
        S0 = -1,
        /// <summary>
        /// 未审核
        /// </summary>
        [Description("未审核")]
        S1 = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        S2 = 1,

        /// <summary>
        /// 已同步
        /// </summary>
        [Description("已同步")]
        S3 = 2,
        /// <summary>
        /// 已置回
        /// </summary>
        [Description("已置回")]
        S4 = 3
    }

    /// <summary>
    /// 区域类型 审核
    /// </summary>
    public enum AreaLevelEnable
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        E0 = 0,
        /// <summary>
        /// 可用
        /// </summary>
        [Description("可用")]
        E1 = 1,

        /// <summary>
        /// 待删除
        /// </summary>
        [Description("待删除")]
        E2 = 2,

        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        E3 = 3
    }

    public enum SoringStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        [Description("待审核")]
        S0 = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        S1 = 1,

        /// <summary>
        /// 已置回
        /// </summary>
        [Description("置回")]
        S2 = 2,
        /// <summary>
        /// 已生效
        /// </summary>
        [Description("已生效")]
        S3 = 3,
        /// <summary>
        /// 置无效
        /// </summary>
        [Description("无效")]
        S4 = 4,

    }
    /// <summary>
    /// 账期配送公司关系运算类型
    /// </summary>
    public enum EnumPeriodExpressNexus
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        N0 = 0,

        /// <summary>
        /// 包含
        /// </summary>
        [Description("包含")]
        N1 = 1,
        /// <summary>
        /// 不包含
        /// </summary>
        [Description("不包含")]
        N2 = 2,
    }

    public enum EnumExpressParameter
    {
        /// <summary>
        /// 修改分拣中心
        /// </summary>
        [Description("修改分拣中心")]
        SortingCenterId=0,

        /// <summary>
        /// 修改站点
        /// </summary>
        [Description("修改站点")]
        ExpressCompanyid=1,

        /// <summary>
        /// 修改付款方式 现金 月结
        /// </summary>
        [Description("修改付款方式")]
        TransferPayType = 2,

        /// <summary>
        /// 修改结算方式 寄方付 到方付
        /// </summary>
        [Description("修改结算方式")]
        AccountType=3,

        /// <summary>
        /// 修改支付方式
        /// </summary>
       [Description("修改支付方式")]
        AcceptType=4,

        /// <summary>
        /// 修改运费
        /// </summary>
        [Description("修改运费")]
        DeliverFee=5,

        /// <summary>
        /// 修改商家
        /// </summary>
        [Description("修改商家")]
        MerchantId=6,

        /// <summary>
        /// 修改保价金额
        /// </summary>
        [Description("修改保价金额")]
        ProtectPrices=7,

        /// <summary>
        /// 修改保价费
        /// </summary>
        [Description("修改保价费")]
        ProtectFee=8,

         /// <summary>
        /// 修改代收货款
        /// </summary>
       [Description("修改代收货款")]
        Goodspayment=9,

        /// <summary>
        /// 修改代收货款手续费
        /// </summary>
        [Description("修改代收货款手续费")]
        Factorage=10,

        /// <summary>
        /// 修改取件重量
        /// </summary>
        [Description("修改取件重量")]
        FinancialWeight=11,

        /// <summary>
        /// 修改客户重量
        /// </summary>
        [Description("修改客户重量")]
        MerchantWeight=12,

    }
}
