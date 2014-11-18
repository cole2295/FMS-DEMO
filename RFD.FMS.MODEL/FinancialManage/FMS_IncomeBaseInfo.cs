using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace RFD.FMS.Model.FinancialManage
{
    /// <summary>
    /// 收入结算财务静态表
    /// </summary>
    [Serializable]
    public partial class FMS_IncomeBaseInfo
    {
        private Int64 _incomeid;
        /// <summary>
        /// 主键ID
        /// </summary>		
        public Int64 IncomeID
        {
            get { return _incomeid; }
            set { _incomeid = value; }
        }
        private Int64 _waybillno;
        /// <summary>
        /// 运单号
        /// </summary>		
        public Int64 WaybillNo
        {
            get { return _waybillno; }
            set { _waybillno = value; }
        }
        private String _waybilltype;
        /// <summary>
        /// 运单类型
        /// </summary>		
        public String WaybillType
        {
            get { return _waybilltype; }
            set { _waybilltype = value; }
        }
        private Int32? _merchantid;
        /// <summary>
        /// 商家
        /// </summary>		
        public Int32? MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
        }
        private Int32? _expresscompanyid;
        /// <summary>
        /// 分拣中心
        /// </summary>		
        public Int32? ExpressCompanyID
        {
            get { return _expresscompanyid; }
            set { _expresscompanyid = value; }
        }
        private Int32? _finalexpresscompanyid;
        /// <summary>
        /// 末级发货分拣中心
        /// </summary>		
        public Int32? FinalExpressCompanyID
        {
            get { return _finalexpresscompanyid; }
            set { _finalexpresscompanyid = value; }
        }
        private Int32? _deliverstationid;
        /// <summary>
        /// 配送站
        /// </summary>		
        public Int32? DeliverStationID
        {
            get { return _deliverstationid; }
            set { _deliverstationid = value; }
        }
        private Int32? _topcodcompanyid;
        /// <summary>
        /// 结算主体
        /// </summary>		
        public Int32? TopCODCompanyID
        {
            get { return _topcodcompanyid; }
            set { _topcodcompanyid = value; }
        }
        private System.DateTime? _rfdaccepttime;
        /// <summary>
        /// 如风达接单时间
        /// </summary>		
        public System.DateTime? RfdAcceptTime
        {
            get { return _rfdaccepttime; }
            set { _rfdaccepttime = value; }
        }
        private System.DateTime? _delivertime;
        /// <summary>
        /// 发货时间
        /// </summary>		
        public System.DateTime? DeliverTime
        {
            get { return _delivertime; }
            set { _delivertime = value; }
        }
        private System.DateTime? _returntime;
        /// <summary>
        /// 返货入库时间
        /// </summary>		
        public System.DateTime? ReturnTime
        {
            get { return _returntime; }
            set { _returntime = value; }
        }
        private Int32? _returnexpresscompanyid;
        /// <summary>
        /// 返货入分拣中心
        /// </summary>		
        public Int32? ReturnExpressCompanyID
        {
            get { return _returnexpresscompanyid; }
            set { _returnexpresscompanyid = value; }
        }
        private System.DateTime? _backstationtime;
        /// <summary>
        /// 归班时间
        /// </summary>		
        public System.DateTime? BackStationTime
        {
            get { return _backstationtime; }
            set { _backstationtime = value; }
        }
        private String _backstationstatus;
        /// <summary>
        /// 归班状态
        /// </summary>		
        public String BackStationStatus
        {
            get { return _backstationstatus; }
            set { _backstationstatus = value; }
        }
        private System.Decimal? _protectedamount;
        /// <summary>
        /// 保价金额
        /// </summary>		
        public System.Decimal? ProtectedAmount
        {
            get { return _protectedamount; }
            set { _protectedamount = value; }
        }
        private System.Decimal? _totalamount;
        /// <summary>
        /// 总价
        /// </summary>		
        public System.Decimal? TotalAmount
        {
            get { return _totalamount; }
            set { _totalamount = value; }
        }
        private System.Decimal? _paidamount;
        /// <summary>
        /// 已收
        /// </summary>		
        public System.Decimal? PaidAmount
        {
            get { return _paidamount; }
            set { _paidamount = value; }
        }
        private System.Decimal? _needpayamount;
        /// <summary>
        /// 应收
        /// </summary>		
        public System.Decimal? NeedPayAmount
        {
            get { return _needpayamount; }
            set { _needpayamount = value; }
        }
        private System.Decimal? _backamount;
        /// <summary>
        /// 已退
        /// </summary>		
        public System.Decimal? BackAmount
        {
            get { return _backamount; }
            set { _backamount = value; }
        }
        private System.Decimal? _needbackamount;
        /// <summary>
        /// 应退
        /// </summary>		
        public System.Decimal? NeedBackAmount
        {
            get { return _needbackamount; }
            set { _needbackamount = value; }
        }
        private System.Decimal? _accountweight;
        /// <summary>
        /// 结算重量
        /// </summary>		
        public System.Decimal? AccountWeight
        {
            get { return _accountweight; }
            set { _accountweight = value; }
        }
        private String _areaid;
        /// <summary>
        /// 地区ID
        /// </summary>		
        public String AreaID
        {
            get { return _areaid; }
            set { _areaid = value; }
        }
        private String _receiveaddress;
        /// <summary>
        /// 收货地址
        /// </summary>		
        public String ReceiveAddress
        {
            get { return _receiveaddress; }
            set { _receiveaddress = value; }
        }
        private Int32? _signtype;
        /// <summary>
        /// 支付类型
        /// </summary>		
        public Int32? SignType
        {
            get { return _signtype; }
            set { _signtype = value; }
        }
        private Int32? _inefficacystatus;
        /// <summary>
        /// 无效状态
        /// </summary>		
        public Int32? InefficacyStatus
        {
            get { return _inefficacystatus; }
            set { _inefficacystatus = value; }
        }
        private System.DateTime? _createtime;
        /// <summary>
        /// 创建时间
        /// </summary>		
        public System.DateTime? createtime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        private System.DateTime? _updatetime;
        /// <summary>
        /// 更新时间
        /// </summary>		
        public System.DateTime? updatetime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
        private Int32? _receivestationid;
        /// <summary>
        /// 接单站点
        /// </summary>		
        public Int32? ReceiveStationID
        {
            get { return _receivestationid; }
            set { _receivestationid = value; }
        }
        private Int32? _receivedelivermanid;
        /// <summary>
        /// 接单配送员
        /// </summary>		
        public Int32? ReceiveDeliverManID
        {
            get { return _receivedelivermanid; }
            set { _receivedelivermanid = value; }
        }
        private String _distributioncode;
        /// <summary>
        /// 录入配送商Code
        /// </summary>		
        public String DistributionCode
        {
            get { return _distributioncode; }
            set { _distributioncode = value; }
        }
        private String _currentdistributioncode;
        /// <summary>
        /// 当前配送商Code
        /// </summary>		
        public String CurrentDistributionCode
        {
            get { return _currentdistributioncode; }
            set { _currentdistributioncode = value; }
        }


        private Decimal? _wayBillInfoWeight;
        /// <summary>
        /// 重量
        /// </summary>		
        public Decimal? WayBillInfoWeight
        {
            get { return _wayBillInfoWeight; }
            set { _wayBillInfoWeight = value; }
        }

        private System.Int32? _subStatus;
        /// <summary>
        /// 返货状态
        /// </summary>		
        public System.Int32? SubStatus
        {
            get { return _subStatus; }
            set { _subStatus = value; }
        }

        private string _acceptType;
        /// <summary>
        /// 付款方式
        /// </summary>		
        public string AcceptType
        {
            get { return _acceptType; }
            set { _acceptType = value; }
        }

        private string _customerOrder;
        /// <summary>
        /// 商家订单号
        /// </summary>		
        public string CustomerOrder
        {
            get { return _customerOrder; }
            set { _customerOrder = value; }
        }

        private string _originDepotNo;
        /// <summary>
        /// 子公司编号
        /// </summary>		
        public string OriginDepotNo
        {
            get { return _originDepotNo; }
            set { _originDepotNo = value; }
        }

        private string _periodAccountCode;
        /// <summary>
        /// 到付月结编号
        /// </summary>
        public string PeriodAccountCode
        {
            get { return _periodAccountCode; }
            set { _periodAccountCode = value; }
        }

        private string _waybillCategory;
        /// <summary>
        /// 货物品类
        /// </summary>
        public string WaybillCategory
        {
            get { return _waybillCategory; }
            set { _waybillCategory = value; }
        }

        /// <summary>
        /// 配送单号
        /// </summary>
        [OptionalField]
        public string DeliverCode;

        /// <summary>
        /// 是否按货物品类结算
        /// </summary>
        [OptionalField]
        public int IsCategory;

        /// <summary>
        /// waybill状态
        /// </summary>
        [OptionalField]
        public string Status;
       
    }
}
