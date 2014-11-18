using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	public class BizEnums
	{
		/// <summary>
		/// 业务单据状态
		/// </summary>
		public enum BizOrderStatus
		{
			[Description("新建")]
			Create = 1,
			[Description("审核中")]
			Audi = 2,
			[Description("审核通过")]
			Pass = 3,
			[Description("审核未通过")]
			notPass = 4,
			[Description("已结算")]
			settlement = 5,
			[Description("无效")]
			Invalid=6,
            [Description("已关闭")]
            Close=7
		}
		/// <summary>
		/// 业务单据提交状态
		/// </summary>
		public enum FeeCommitStatus
		{
			[Description("未提交")]
			Uncommitted=0,
			[Description("已提交")]
			Submitted=1,
			[Description("打回")]
			Refuse=2,
			[Description("重新提交")]
			Resubmit=3
		}

		/// <summary>
		/// 环节类型
		/// </summary>
		public enum PointType
		{
			[Description("开始")]
			begin = 1,
			[Description("中间")]
			normal = 2,
			[Description("结束")]
			end = 3,
			[Description("逻辑")]
			Logic=4

		}
		/// <summary>
		/// 审核实例状态
		/// </summary>
		public enum ProccessStatus
		{
			[Description("新分配")]
			create = 0,
			[Description("审核中")]
			auding = 1,
			[Description("处理完成")]
			done = 2,
			[Description("暂缓处理")]
			waiting = 3,
			[Description("驳回")]
			back = 4
		}
		/// <summary>
		/// 操作日志类型
		/// </summary>
		public enum OperatorlogType
		{
            [Description("商家配送费")]
            L2 = 2,
            [Description("商家基础信息")]
            L3 = 3,
            [Description("商家基础信息待生效")]
            L4 = 4,
            [Description("商家配送费待生效")]
            L5 = 5,
            [Description("商家区域类型")]
            L6 = 6,
            [Description("修改配送费(收入)")]
            ChangeIncomeFee = 7,
            [Description("修改配送费(支出)")]
            ChangeCODFee = 8,
            [Description("拣运基础信息")]
            SoringFee =9
            
		}

		/// <summary>
		/// 单据号种子类型
		/// </summary>
		public enum NoTypeEnum
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

        public enum BizOrderType
        {
            [Description("请款单")]
            BorrowFee = 1,
            [Description("报销单")]
            BXFee = 2,
            [Description("替代报销单")]
            BXFeeTD = 14
        }

        public enum BizCreaterNo
        { 
            [Description("单据编号")]
            BizOrderNo = 5
        }

        public enum RelationType
        { 
            /// <summary>
            /// 没有关联
            /// </summary>
            NoRelation = 0,

            /// <summary>
            /// 关联报销
            /// </summary>
            BX = 2,

            /// <summary>
            /// 替代单据
            /// </summary>
            TD = 3
        }

        public enum FeeBillPrintType
        {
            /// <summary>
            /// 带签名栏
            /// </summary>
            WithSignatureColumn = 0,

            /// <summary>
            /// 不带签名栏
            /// </summary>
            WithoutSignatureColumn = 1
        }

        /// <summary>
        /// 单据子类型
        /// </summary>
        public enum enumSubOrderType
        {
            [Description("普通类")]
            Normal = 0,

            [Description("合同类")]
            Contract = 1
        }

        /// <summary>
        /// 发送邮件的类型
        /// </summary>
        public enum enumMailType
        {
            [Description("催黄联")]
            BX = 1,
            [Description("催还款")]
            HK = 2,
            [Description("催审批")]
            SP = 3
        }

        public enum enumBusinessDataType
        {
            [Description("用户审批单量")]
            UserSPApplyCount = 1
        }

        /// <summary>
        /// 系统类型
        /// </summary>
        public enum SystemType
        {
            /// <summary>
            /// 如风达配送管理系统
            /// </summary>
            [Description("配送管理系统")]
            LMS_RFD_Manager = 1,

            /// <summary>
            /// 如风达财务系统
            /// </summary>
            [Description("财务系统")]
            LMS_RFD_FMS = 2,

            /// <summary>
            /// 如风达财务系统(审批流程)
            /// </summary>
            [Description("财务系统(审批流程)")]
            LMS_RFD_FMS_FLOW = 3
        }

        /// <summary>
        /// 配送员提成计算错误枚举
        /// </summary>
        public enum DeductResult
        {
            [Description("待计算")]
            Wait = 0,
            [Description("计算成功")]
            Success = 1
        }

        /// <summary>
        /// 提成类型(1快递取件，2快递派件，3项目取件，4项目派件)
        /// </summary>
        public enum enumDeductType
        {
            [Description("快递取件")]
            ExpressReceive = 1,
            [Description("快递派件")]
            ExpressSend = 2,
            [Description("项目取件")]
            ProjectReceive = 3,
            [Description("项目派件")]
            ProjectSend = 4
        }

        /// <summary>
        /// COD操作类型
        /// </summary>
        public enum CODOperateType
        {
            [Description("发货")]
            Delivery = 1,
            [Description("转站转配送商（出）")]
            TransferOUT = 2,
            [Description("转站转配送商（入）")]
            TransferIN = 3,
            [Description("拒收")]
            Rejection = 4,
            [Description("置运单无效")]
            Invalid = 5,
            [Description("逆向返货入库")]
            ReverseInbound = 6,
            [Description("出库")]
            OutBound = 7,
            [Description("装车")]
            TruckIn = 8,
          

        }

        public enum EnumIsFareType
        {
            /// <summary>
            /// 未计算
            /// </summary>
            [Description("未计算")]
            NotEval = 0,
            /// <summary>
            /// 已计算
            /// </summary>
            [Description("已计算")]
            Eval = 1,
            /// <summary>
            /// 区域类型未取到
            /// </summary>
            [Description("区域类型未取到")]
            FailseGetAreaType = 2,
            /// <summary>
            /// 计算公式未取到
            /// </summary>
            [Description("计算公式未取到")]
            NoFormula = 3,
            /// <summary>
            /// 计算费用失败
            /// </summary>
            [Description("计算费用失败")]
            EvalFailse = 4,
            /// <summary>
            /// 信息配送商未取到
            /// </summary>
            [Description("信息配送商未取到")]
            GetDistributionFailse = 5,
            /// <summary>
            /// 不存在配送商或仓库或商家
            /// </summary>
            [Description("不存在配送商或仓库或商家")]
            InfoError = 6,
            /// <summary>
            /// 区域ID为空
            /// </summary>
            [Description("区域ID为空")]
            AreaIDIsNull = 7,
            /// <summary>
            /// 重量小于等于0
            /// </summary>
            [Description("重量小于等于0")]
            WeightEqualZero = 8,
            /// <summary>
            /// 如风达配送第三方商家不计算运费
            /// </summary>
            [Description("如风达配送第三方商家不计算运费")]
            NotNeedEval = 10,
            /// <summary>
            /// 作废
            /// </summary>
            [Description("作废")]
            Deleted = 11
        }

        public enum IncomeFeeAccountType
        {
            /// <summary>
            /// 未计算
            /// </summary>
            [Description("未计算")]
            T0 = 0,
            /// <summary>
            /// 已计算
            /// </summary>
            [Description("已计算")]
            T1 = 1,
            /// <summary>
            /// 不计算
            /// </summary>
            [Description("不计算")]
            T2 = 2,
            /// <summary>
            /// 未维护区域类型
            /// </summary>
            [Description("未维护区域类型")]
            T3 = 3,
            /// <summary>
            /// 未维护正向配送费计算公式
            /// </summary>
            [Description("未维护正向配送费计算公式")]
            T4 = 4,
            /// <summary>
            /// 没有维护配送费结算标准
            /// </summary>
            [Description("没有维护配送费结算标准")]
            T5 = 5,
            /// <summary>
            /// 重量不能为0
            /// </summary>
            [Description("重量不能为0")]
            T6 = 6,
            /// <summary>
            /// 配送费计算失败
            /// </summary>
            [Description("配送费计算失败")]
            T9 = 9,
            /// <summary>
            /// 
            /// </summary>
            [Description("按配送商进行核算")]
            T10=10
        }

        public enum AccountPeriodType
        {
            /// <summary>
            /// 按周设置
            /// </summary>
            [Description("按周设置")]
            T1 = 1,
            /// <summary>
            /// 按月设置
            /// </summary>
            [Description("按月设置")]
            T2 = 2,
            /// <summary>
            /// 自定义设置
            /// </summary>
            [Description("自定义设置")]
            T3 = 3
        }

        public enum AccountPeriodChildType
        {
            /// <summary>
            /// 一周N次
            /// </summary>
            [Description("一周N次")]
            T1 = 1,
            /// <summary>
            /// 本周结上周
            /// </summary>
            [Description("本周结上周")]
            T2 = 2
        }
	}
}
