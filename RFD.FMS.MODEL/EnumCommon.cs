/*
* (C)Copyright 2011-2012 如风达信息管理系统
* 
* 模块名称：公共枚举类
* 说明：订单状态、订单类型
* 作者：wangxue
* 创建日期：2011-03-03
* 修改人：
* 修改时间：
* 修改记录：
*/
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data;


namespace RFD.FMS.MODEL
{
    [Serializable]
    public partial class EnumCommon
    {

        public enum FormCodeType
        {
            [Description("运单号")]
            WaybillNo = 0,
            [Description("订单号")]
            CustomOrder = 1,
            [Description("空号")]
            WithoutNo = -1,
            /// <summary>
            /// 按配送单号
            /// </summary>
            [Description("按配送单号")]
            DeliverCode = 2
        }
        public enum StatusInfoEnum
        {
            [Description("来源")]
            source = 3,
            [Description("运单状态")]
            WaybillStatus = 1,
            [Description("运单提成标准")]
            Commission = 304
        }

        /// <summary>
        /// 退货入库异常状态
        /// </summary>
        public enum ErrorWaybillDealStatus
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Description("未处理")]
            NoDeal = 0,
            /// <summary>
            /// 处理中
            /// </summary>
            [Description("处理中")]
            Dealing = 1,
            /// <summary>
            /// 已处理
            /// </summary>
            [Description("已处理")]
            Dealed = 2
        }

        public enum WaybillBoxStatus
        {
            /// <summary>
            /// 未装箱
            /// </summary>
            [Description("未装箱")]
            NoBox = 0,
            /// <summary>
            /// 已装箱
            /// </summary>
            [Description("已装箱")]
            Boxed = 1,
            /// <summary>
            /// 已拆箱
            /// </summary>
            [Description("已拆箱")]
            UnBox = 2
        }

        /// <summary>
        /// 运单状态
        /// </summary>
        [Serializable]
        public enum WayBillStatusEnum
        {
            /// <summary>
            /// 待取件
            /// </summary>
            [Description("待取件")]
            WaitingFetch = -6,

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
            /// 有效待分配
            /// </summary>
            [Description("有效待分配")]
            WaitingGisAssign = -5,

            /// <summary>
            /// 已分配站点
            /// </summary>
            [Description("已分配站点")]
            GisAssigned = -4,

            /// <summary>
            /// 待入分拣中心
            /// </summary>
            [Description("待入分拣中心")]
            WaitingBounded = -3,
            /// <summary>
            /// 已入分拣中心
            /// </summary>
            [Description("已入分拣中心")]//未使用
            [Obsolete]
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
            /// 返货入库
            /// </summary>
            [Description("返货入库")]
            ReturnInBound = 12
        }

        /// <summary>
        /// 运单状态排序
        /// </summary>
        public enum WayBillStatusSortingEnum
        {
            /// <summary>
            /// 有效待分配
            /// </summary>
            [Description("有效待分配")]
            WaitingGisAssign = 1,

            /// <summary>
            /// 已分配站点
            /// </summary>
            [Description("已分配站点")]
            GisAssigned = 2,

            /// <summary>
            /// 待入分拣中心
            /// </summary>
            [Description("待入分拣中心")]
            WaitingBounded = 3,
            /// <summary>
            /// 已入分拣中心
            /// </summary>
            [Description("已入分拣中心")]
            AssignedBound = 4,

            /// <summary>
            /// 已出库
            /// </summary>
            [Description("已出库")]
            OutBounded = 5,

            /// <summary>
            /// 运输在途
            /// </summary>
            [Description("运输在途")]
            WaitingStation = 6,
            /// <summary>
            /// 已入站
            /// </summary>
            [Description("已入站")]
            EnterStation = 7,
            /// <summary>
            /// 已分配
            /// </summary>
            [Description("已分配")]
            Assigned = 8,
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
            ReturnOnStation = -1,

            /// <summary>
            /// 返货入库
            /// </summary>
            [Description("返货入库")]
            SortingIncept = 1
        }

        /// <summary>
        /// 运单分拣类型
        /// </summary>
        public enum WaybillSortingType
        {
            /// <summary>
            /// 发货分拣
            /// </summary>
            [Description("发货分拣")]
            SimpleSorting = 0,
            /// <summary>
            /// 转站分拣
            /// </summary>
            [Description("转站分拣")]
            TurnSorting = 1,
            /// <summary>
            /// 二级分拣
            /// </summary>
            [Description("二级分拣")]
            SecondSorting = 2,
            /// <summary>
            /// 分拣至商家
            /// </summary>
            [Description("分拣至商家")]
            DistributionSorting = 3
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
        /// 支付方式
        /// </summary>
        public enum PaymentType
        {
            [Description("现金")]
            Cash = 0,
            [Description("POS机")]
            POS = 1
        }

        public enum WaybillTypeStrEnum
        {
            [Description("普通订单")]
            NomalWaybill = 0,
            [Description("上门换货")]
            ChangeWaybill = 1,
            [Description("上门退货")]
            BackWaybill = 2,
        }

        /// <summary>
        /// 运单来源
        /// </summary>
        public enum WaybillSourse
        {
            [Description("VANCL")]
            VANCL = 0,
            [Description("VJIA")]
            VJIA = 1,
            [Description("其他")]
            Other = 2,
        }

        /// <summary>
        /// 单据号种子类型
        /// </summary>
        public enum NoTypeEnum
        {
            /// <summary>
            /// 如风达运单号
            /// </summary>
            [Description("如风达运单号")]
            Waybill = 0,
            /// <summary>
            /// 如风达批次号
            /// </summary>
            [Description("如风达批次号")]
            Batch = 1,
            /// <summary>
            /// 入库单
            /// </summary>
            [Description("入库单")]
            InBound = 2,
            /// <summary>
            /// 出库单
            /// </summary>
            [Description("出库单")]
            OutBound = 3,
            /// <summary>
            /// 主管票证
            /// </summary>
            [Description("主管票证")]
            AreaRoleUserMapping = 4,
            /// <summary>
            /// 预留运单号
            /// </summary>
            [Description("预留运单号")]
            ReserveWaybill = 9,
            /// <summary>
            /// 入库箱号
            /// </summary>
            [Description("入库箱号")]
            InboundBox = 11,
            /// <summary>
            /// 取件单号
            /// </summary>
            [Description("取件单号")]
            FetchOrder = 12,
            /// <summary>
            /// 考核信息编号
            /// </summary>
            [Description("考核信息编号")]
            KpiCode = 13
        }

        /// <summary>
        /// 单据号种子类型
        /// </summary>
        public enum TurnStationEnum
        {
            [Description("已申请")]
            apply = 0,
            [Description("已审核")]
            author = 1,
            [Description("运输在途")]
            print = 2,
            [Description("已接收")]
            accept = 3
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

        public static String GetEnumDesc(Enum e)
        {
            FieldInfo EnumInfo = e.GetType().GetField(e.ToString());
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.
            GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
            {
                return EnumAttributes[0].Description;
            }
            return e.ToString();
        }
        /// <summary>
        /// 系统类型
        /// </summary>
        public enum SystemType
        {
            /// <summary>
            /// 权限系统
            /// </summary>
            [Description("权限系统")]
            Permission_Sys = 0,

            /// <summary>
            /// 如风达配送管理系统
            /// </summary>
            [Description("如风达配送管理系统")]
            LMS_RFD_Manager = 1,

            /// <summary>
            /// 如风达财务结算系统
            /// </summary>
            [Description("如风达财务结算系统")]
            LMS_RFD_FMS = 2,

            /// <summary>
            /// 如风达财务审批系统
            /// </summary>
            [Description("如风达财务审批系统")]
            LMS_RFD_FMS_FLOW = 3,

            /// <summary>
            /// 如风达运输管理系统
            /// </summary>
            [Description("如风达运输管理系统")]
            LMS_RFD_TMS = 4
        }

        /// <summary>
        /// GIS分单状态
        /// </summary>
        public enum GisStatusEnum
        {
            /// <summary>
            /// Gis待分配
            /// </summary>
            [Description("Gis已分配")]
            GisWaitAssign = 0,

            /// <summary>
            /// Gis已分配
            /// </summary>
            [Description("人工已确认")]
            GisAssigned = 1,

            /// <summary>
            /// Gis已打印
            /// </summary>
            [Description("已打印")]
            GisPrinted = 2,
        }

        /// <summary>
        /// 投诉订单状态--add by wangxue
        /// </summary>
        public enum ComplaintStatusEnum
        {
            /// <summary>
            /// 已导入
            /// </summary>
            [Description("已导入")]
            Import = 1,
            /// <summary>
            /// 已分配
            /// </summary>
            [Description("已分配")]
            Assigned = 2,
            /// <summary>
            /// 站点已回复
            /// </summary>
            [Description("站点已回复")]
            Replied = 3,
            /// <summary>
            /// 客服已界定
            /// </summary>
            [Description("客服已界定")]
            Deceded = 4
        }

        /// <summary>
        /// 客服投诉订单类型（投诉订单、查询订单）
        /// </summary>
        public enum ComplaintTypeEnum
        {
            /// <summary>
            /// 投诉订单
            /// </summary>
            [Description("投诉订单")]
            Complaint = 1,
            /// <summary>
            /// 查询订单
            /// </summary>
            [Description("查询订单")]
            Search = 2,


        }




        /// <summary>
        /// 部门类型
        /// </summary>
        [Serializable]
        public enum CompanyType
        {
            /// <summary>
            /// 行政部门
            /// </summary>
            [Description("行政部门")]
            Administration = 0,
            /// <summary>
            /// 分拣中心
            /// </summary>
            [Description("分拣中心")]
            SortingCenter = 1,
            /// <summary>
            /// 配送站
            /// </summary>
            [Description("配送站")]
            DeliverStation = 2,
            /// <summary>
            /// 配送商
            /// </summary>
            [Description("配送商")]
            Franchiser = 3,
            /// <summary>
            /// 运输中心
            /// </summary>
            [Description("运输中心")]
            TransportCenter = 4,
            /// <summary>
            /// 商家
            /// </summary>
            [Description("商家")]
            Merchant = 5
        }

        /// <summary>
        /// 取货模式
        /// </summary>
        public enum FetchFreightMode
        {
            /// <summary>
            /// 商家送货上门
            /// </summary>
            [Description("商家送货上门")]
            HomeDeliver = 0,
            /// <summary>
            /// 如风达自提
            /// </summary>
            [Description("如风达自提")]
            RuFengDa = 1,
            /// <summary>
            /// 其他
            /// </summary>
            [Description("其他")]
            Other = 2
        }

        /// <summary>
        /// 启用状态
        /// </summary>
        public enum ActiveMode
        {
            /// <summary>
            /// 启用
            /// </summary>
            [Description("启用")]
            Actived = 0,
            /// <summary>
            /// 停用
            /// </summary>
            [Description("停用")]
            InActived = 1
        }

        /// <summary>
        /// 短信模板标签
        /// [配送员]、[配送员手机号]、[客户电子邮箱]、[是否支持POS机]、[配送站点]、[站点电话]
        /// </summary>
        public enum SmsTemplateLabel
        {
            /// <summary>
            /// [配送员]
            /// </summary>
            [Description("[配送员]")]
            DeliverManName,

            /// <summary>
            /// [配送员手机号]
            /// </summary>
            [Description("[配送员手机号]")]
            DeliverManPhone,

            /// <summary>
            /// [客户电子邮箱]
            /// </summary>
            [Description("[客户电子邮箱]")]
            CustomerEmail,

            /// <summary>
            /// [是否支持POS机]
            /// </summary>
            [Description("[是否支持POS机]")]
            IsSupportPos,

            /// <summary>
            /// [配送站点]
            /// </summary>
            [Description("[配送站点]")]
            DeliverStationName,

            /// <summary>
            /// [站点电话]
            /// </summary>
            [Description("[站点电话]")]
            DeliverStationTel,

            /// <summary>
            /// [当前城市]
            /// </summary>
            [Description("[当前城市]")]
            CurrentCity,

            /// <summary>
            /// [当前部门]
            /// </summary>
            [Description("[当前部门]")]
            CurrentSortingCenter,

            /// <summary>
            /// [目标城市]
            /// </summary>
            [Description("[目标城市]")]
            TargetCity,

            /// <summary>
            /// [目标部门]
            /// </summary>
            [Description("[目标部门]")]
            TargetSortingCenter,

            /// <summary>
            /// [签收人]
            /// </summary>
            [Description("[签收人]")]
            SignCustomer,

            /// <summary>
            /// [签收时间]
            /// </summary>
            [Description("[签收时间]")]
            SignTime,

            /// <summary>
            /// [运单号]
            /// </summary>
            [Description("[运单号]")]
            WaybillNo,

            /// <summary>
            /// [收件人城市]
            /// </summary>
            [Description("[收件城市]")]
            ReceiveCity,

            /// <summary>
            /// [收件人]
            /// </summary>
            [Description("[收件人]")]
            ReceiveBy,

            /// <summary>
            /// [发件人城市]
            /// </summary>
            [Description("[发件城市]")]
            SendCity,

            /// <summary>
            /// [发件人]
            /// </summary>
            [Description("[发件人]")]
            SendBy
        }

        public enum SmsTemplateType
        {
            /// <summary>
            /// 运单出站模板
            /// </summary>
            [Description("运单出站短信模板")]
            OutStation = 0,
            /// <summary>
            /// 职能短信模板
            /// </summary>
            [Description("职能短信模板")]
            FunctionType = 1,
            /// <summary>
            /// 一级分拣出库模板（发往分拣中心）
            /// </summary>
            [Description("一级分拣出库短信模板（发往分拣中心）")]
            LevelOneOutBound = 2,
            /// <summary>
            /// 二级分拣出库模板（发往站点）
            /// </summary>
            [Description("二级分拣出库短信模板（发往站点）")]
            LevelTwoOutBound = 3,
            /// <summary>
            /// 退换货（拒收）入库模板
            /// </summary>
            [Description("退换货（拒收）入库短信模板")]
            ReturnInBound = 4,
            /// <summary>
            /// 发往配送商出库模板（发往配送商）
            /// </summary>
            [Description("发往配送商出库短信模板（发往配送商）")]
            LevelThereOutBound = 5,
            /// <summary>
            /// 运单归班模板
            /// </summary>
            [Description("运单归班短信模板--发寄件人")]
            BackStation = 6,

            /// <summary>
            /// 运单归班模板
            /// </summary>
            [Description("运单归班短信模板--代收发收件人")]
            BackStationSender = 7
        }

        /// <summary>
        /// 6.6	异常处理状态
        /// </summary>
        public enum VanclSynAbnoramlStatus
        {
            /// <summary>
            /// 滞留超期
            /// </summary>
            [Description("滞留超期")]
            H1,
            /// <summary>
            /// 超范围（停用）
            /// </summary>
            [Description("超范围（停用）")]
            H2,
            /// <summary>
            /// 拒收
            /// </summary>
            [Description("拒收")]
            H3,
            /// <summary>
            /// 其他（停用）
            /// </summary>
            [Description("其他（停用）")]
            H4
        }

        /// <summary>
        /// 6.8	系统错误信息
        /// </summary>
        public enum VanclSynExceptionStatus
        {
            /// <summary>
            /// 非法的XML格式
            /// </summary>
            [Description("非法的XML格式")]
            S1,
            /// <summary>
            /// 非法的数字签名
            /// </summary>
            [Description("非法的数字签名")]
            S2,
            /// <summary>
            /// 非法的LC
            /// </summary>
            [Description("非法的LC")]
            S3,
            /// <summary>
            /// 非法的通知类型
            /// </summary>
            [Description("非法的通知类型")]
            S4,
            /// <summary>
            /// 非法的通知内容
            /// </summary>
            [Description("非法的通知内容")]
            S5,
            /// <summary>
            /// 服务器处理错误
            /// </summary>
            [Description("服务器处理错误")]
            S6,
            /// <summary>
            /// 字符串长度超出规定长度
            /// </summary>
            [Description("字符串长度超出规定长度")]
            S7
        }

        /// <summary>
        /// 6.6	异常处理状态
        /// </summary>
        public enum VanclAbnormalDisposalStatus
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Description("未处理")]
            L1,
            /// <summary>
            /// 处理中
            /// </summary>
            [Description("处理中")]
            L2,
            /// <summary>
            /// 已置回
            /// </summary>
            [Description("已置回")]
            L5,
            /// <summary>
            /// 已处理
            /// </summary>
            [Description("已处理")]
            L10
        }

        /// <summary>
        /// 区域类型 审核
        /// </summary>
        public enum AreaLevelStatus
        {
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
            S3 = 2
        }

        public enum LimitPermissionType
        {
            /// <summary>
            /// 
            /// </summary>
            [Description("综合查询受限用户翻页不可用")]
            SearchPager = 100
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
            TruckIn = 8
        }
        /// <summary>
        /// COD操作类型
        /// </summary>
        public enum InefficacyStatus
        {
            [Description("丢失待赔付")]
            LostWaitintPay = 1,
            [Description("丢失已赔付")]
            LostPay = 2,
            [Description("破损")]
            Break = 3,
            [Description("取消")]
            Cancel = 4,
            [Description("作废")]
            Cancellation = 5,
            [Description("城际丢失待赔付")]
            IntercityLostWaitPay = 6,
            [Description("城际丢失已赔付")]
            IntercityLostPay = 7,
            [Description("快递丢失待赔付")]
            ExpressLostWaitPay = 8,
            [Description("快递丢失已赔付")]
            ExpressLostPay = 9
        }

        /// <summary>
        /// 快递单状态
        /// </summary>
        public enum ExpressOrderStatus
        {
            [Description("已入站")]
            IntoStation = 0,
            [Description("已出站")]
            OutStation = 1
        }

        /// <summary>
        /// 快递单状态
        /// </summary>
        public enum ExpressOrderDeliveryFeeType
        {
            [Description("现结")]
            NowPay = 1,
            [Description("到付")]
            ArrivedPay = 2,
            [Description("月结")]
            MonthlyPay = 3
        }

        /// <summary>
        /// 取件单状态
        /// </summary>
        public enum FetchOrderStatus
        {
            /// <summary>
            /// 待分配站点
            /// </summary>
            [Description("待分配站点")]
            WaitingAssignStation = 0,
            /// <summary>
            /// 取件待分配
            /// </summary>
            [Description("取件待分配")]
            WaitingDeliver = 1,
            /// <summary>
            /// 取件已分配
            /// </summary>
            [Description("取件已分配")]
            Assigned = 2
        }

        /// <summary>
        /// 货品属性
        /// </summary>
        public enum WaybillProperty
        {
            /// <summary>
            /// 普货
            /// </summary>
            [Description("普货")]
            CommonGoods = 0,
            /// <summary>
            /// 禁航
            /// </summary>
            [Description("禁航")]
            Nofly = 1,
            /// <summary>
            /// 易碎
            /// </summary>
            [Description("易碎")]
            Frangible = 2
        }

        /// <summary>
        /// 运单子状态
        /// </summary>
        public enum SubStatusType
        {
            [Description("二级分拣")]
            SecondDeliver = 1
        }

        /// <summary>
        /// 运单状态变更发生环节
        /// </summary>
        public enum StatusChangeNodeType
        {
            [Description("商家")]
            Merchant = 1,
            [Description("分拣中心")]
            DeliverCenter = 5,
            [Description("运输中心")]
            TransportCenter = 10,
            [Description("配送站")]
            Station = 15,
            [Description("第三方配送")]
            ThirdPartStation = 20,
            [Description("退货仓库")]
            ReturnWareHouse = 25,
            [Description("未知")]
            NotType = 30
        }

        public enum StatusChangeNode
        {
            [Description("订单导入")]
            OrderImport = 1,
            [Description("商家订单同步")]
            MerchantOrderSyn = 2,
            [Description("上门退货同步")]
            GoReturnGoodsSyn = 3,
            [Description("取件服务同步")]
            PickupSyn = 4,
            [Description("分配站点")]
            AllotStation = 5,
            [Description("打印面单")]
            PrintFace = 6,
            [Description("转站打印")]
            ChangeStationPrint = 7,
            [Description("二级分拣")]
            SecondDeliver = 8,
            [Description("分拣入库")]
            DeliverInBound = 9,
            [Description("转站入库")]
            ChangeStationInBound = 10,
            [Description("扫描出库")]
            ScanOutBound = 11,
            [Description("分拣出库")]
            DeliverOutBound = 12,
            [Description("查询出库")]
            SearchOutBound = 13,
            [Description("订单装车")]
            OrderLoading = 14,
            [Description("运单入站")]
            WaybillInStation = 15,
            [Description("退货单入站")]
            BackGoodsInStation = 16,
            [Description("批量接收入站")]
            BatchReceiveInStation = 17,
            [Description("入站转站申请")]
            RZChangeStationApply = 18,
            [Description("滞留转站申请")]
            ZLChangeStationApply = 19,
            [Description("运单分配")]
            WaybillAllot = 20,
            [Description("归班")]
            BackStation = 21,
            [Description("取消转站")]
            ChanclChangeStation = 22,
            [Description("妥投退库")]
            DeliveryBackWareHouse = 23,
            [Description("拒收退库")]
            RefuseReceiveBackWareHouse = 24,
            [Description("退库分拣")]
            BackWareHouseDeliver = 25,
            [Description("返货入库")]
            BackGoodsToWareHouse = 26,
            [Description("运输入库")]
            TransportToWareHouse = 27,
            [Description("服务同步入库")]
            ServiceToWareHouseSyn = 28
        }

        /// <summary>
        /// window服务类型
        /// </summary>
        public enum ServiceNameEnum
        {
            /// <summary>
            /// 配送报表服务
            /// </summary>
            [Description("配送报表服务")]
            ServiceForStationDaily = 1,
            /// <summary>
            /// 配送报表服务
            /// </summary>
            [Description("物流报表服务")]
            ServiceForLMSReport = 2
        }

        /// <summary>
        /// 分拣服务化操作类型
        /// </summary>
        [Description("分拣服务化操作类型")]
        public enum SortCenterSequenceOpType
        {
            /// <summary>
            /// 分拣入库
            /// </summary>
            [Description("分拣入库")]
            InBound = 1,

            /// <summary>
            /// 分拣出库
            /// </summary>
            [Description("分拣出库")]
            OutBound = 2
        }

        /// <summary>
        /// 分拣入库短信处理状态
        /// </summary>
        [Description("分拣入库短信处理状态")]
        public enum SortCenterSMSSeqStatus
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Description("未处理")]
            NoHand = 0,
            /// <summary>
            /// 已处理
            /// </summary>
            [Description("已处理")]
            Handed = 1,
            /// <summary>
            /// 错误
            /// </summary>
            [Description("错误")]
            Error = 2
        }


        /// <summary>
        /// 分拣服务化处理状态
        /// </summary>
        [Description("分拣服务化处理状态")]
        public enum SortCenterSequenceSeqStatus
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Description("未处理")]
            NoHand = 0,
            /// <summary>
            /// 已处理
            /// </summary>
            [Description("已处理")]
            Handed = 1,
            /// <summary>
            /// 错误
            /// </summary>
            [Description("错误")]
            Error = 2
        }

        /// <summary>
        /// 分拣出入库操作单类型
        /// </summary>
        [Description("分拣出入库操作单类型")]
        [Serializable]
        public enum SortCenterIOFormType
        {
            /// <summary>
            /// 按运单
            /// </summary>
            [Description("按运单")]
            Waybill = 0,
            /// <summary>
            /// 按订单
            /// </summary>
            [Description("按订单")]
            Order = 1,
            /// <summary>
            /// 按配送单号
            /// </summary>
            [Description("按配送单号")]
            DeliverCode = 2
        }


        /// <summary>
        /// 订单状态枚举
        /// </summary>
        public enum OrderStatus
        {
            /// <summary>
            /// 等待系统处理订单-7
            /// </summary>
            [Description("等待系统处理")]
            WaitSys = -7,
            /// <summary>
            /// 拒收-6
            /// </summary>
            [Description("拒收已入库")]
            Refuse = -6,
            /// <summary>
            /// 已作废-5
            /// </summary>
            [Description("已作废")]
            Recycle = -5,
            /// <summary>
            /// 缺库存-4
            /// </summary>
            [Description("缺库存")]
            NoStock = -4,
            /// <summary>
            /// 待核订单-2
            /// </summary>
            [Description("待核")]
            Wait = -2,
            /// <summary>
            /// 无效订单-1
            /// </summary>
            [Description("无效")]
            Cancel = -1,
            /// <summary>
            /// 等待客服处理订单0
            /// </summary>
            [Description("等待客服处理")]
            Unsure = 0,
            /// <summary>
            /// 有效订单1
            /// </summary>
            [Description("有效待分配")]
            Sure = 1,
            /// <summary>
            /// 有效等待支付10
            /// </summary>
            [Description("有效待支付")]
            WaitPay = 10,
            /// <summary>
            /// 有效等待调拨11
            /// </summary>
            [Description("有效待调拨")]
            WaitRedeploy = 11,
            /// <summary>
            /// 已分配配送站12
            /// </summary>
            [Description("已分配配送站")]
            SetExpress = 12,
            /// <summary>
            /// 已打印2
            /// </summary>
            [Description("已打印")]
            Printed = 2,
            /// <summary>
            /// 已配货3
            /// </summary>
            [Description("已配货")]
            Picked = 3,
            /// <summary>
            /// 已打包4
            /// </summary>
            [Description("已打包")]
            Packed = 4,
            /// <summary>
            /// 已出库5
            /// </summary>
            [Description("已出库")]
            OutStock = 5,
            /// <summary>
            /// 已入配送站20
            /// </summary>
            [Description("已入站")]
            EnterStation = 20,
            /// <summary>
            /// 已分配配送员21
            /// </summary>
            [Description("已分配配送员")]
            SetDeliverMan = 21,
            /// <summary>
            /// 已出配送站22
            /// </summary>
            [Description("已出站")]
            OutStation = 22,
            /// <summary>
            /// 送达失败23
            /// </summary>
            [Description("送达失败")]
            SendFailed = 23,
            /// <summary>
            /// 拒收24
            /// </summary>
            [Description("拒收")]
            CustomerRefuse = 24,
            /// <summary>
            /// 送达成功25
            /// </summary>
            [Description("送达成功")]
            SendSuccess = 25
        }
        public enum TMallOrderStatus
        {
            /// <summary>
            /// 已揽收
            /// </summary>
            [Description("TMS_ACCEPT")]
            TMS_ACCEPT,
            /// <summary>
            /// 派送中
            /// </summary>
            [Description("TMS_DELIVERING")]
            TMS_DELIVERING,
            /// <summary>
            /// 支付成功(COD订单)
            /// </summary>
            [Description("TMS_PAY")]
            TMS_PAY,
            /// <summary>
            /// 签收成功
            /// </summary>
            [Description("TMS_SIGN")]
            TMS_SIGN,
            /// <summary>
            /// 拒签
            /// </summary>
            [Description("TMS_FAILED")]
            TMS_FAILED,
            /// <summary>
            /// 异常
            /// </summary>
            [Description("TMS_ERROR")]
            TMS_ERROR,
            /// <summary>
            /// 分站进
            /// </summary>
            [Description("TMS_STATION_IN")]
            TMS_STATION_IN,
            /// <summary>
            /// 分站出
            /// </summary>
            [Description("TMS_STATION_OUT")]
            TMS_STATION_OUT
        }


        public enum TMallDeliverType
        {
            /// <summary>
            /// 工作日
            /// </summary>
            [Description("工作日")]
            WorKingDay = 1,
            /// <summary>
            /// 节假日
            /// </summary>
            [Description("节假日")]
            Holiday = 2,
            /// <summary>
            /// 当日达
            /// </summary>
            [Description("当日达")]
            CurrentDay = 101,
            /// <summary>
            /// 次晨达
            /// </summary>
            [Description("次晨达")]
            NextMorning = 102,
            /// <summary>
            /// 次日达
            /// </summary>
            [Description("次日达")]
            NextDay = 103
        }
        /// <summary>
        /// 支付方
        /// </summary>
        public enum AccountType
        {
            /// <summary>
            /// 寄方付
            /// </summary>
            [Description("寄方付")]
            SendPartyPay = 1,
            /// <summary>
            /// 到方付
            /// </summary>
            [Description("到方付")]
            ReceivePartyPay = 2

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
        /// 配送单号类型
        /// </summary>
        [Serializable]
        public enum DeliverCodeType
        {
            /// <summary>
            /// 系统运单号
            /// </summary>
            [Description("系统运单号")]
            SystemWaybillNo = 0,
            /// <summary>
            /// 客户运单号
            /// </summary>
            [Description("客户运单号")]
            CustomerWaybillNo = 1,
            /// <summary>
            /// 客户订单号
            /// </summary>
            [Description("客户订单号")]
            CustomerOrder = 2
        }

        [Serializable]
        public class SearchCondition
        {
            public int ID { get; set; }//主键
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
            public string DistributionCode { get; set; }
            public string ExportPath;//导出路径
            public ExportFileFormat ExportFormat;//导出格式
            public int POSType;//POS机类型 add by wangyongc 2012-03-05
            public int TransferPayType;//配送费结算方式 add by wangyongc 2012-03-05
            public int SearchWaybillType { get; set; }//查询类型：0运单号查询，1订单号查询
        }

        public enum SearchType
        {
            Total,
            Details,
            AllDetails,
            Success,
            Delay,
            Reload
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

        public enum LoadScriptType
        {
            Reload,
            Init,
            Search,
            All
        }
    }
}
