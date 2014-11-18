using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RFD.FMS.Model.UnionPay
{
    /// <summary>
    /// FMS_StationDailyFinanceDetails
    /// </summary>
    [Serializable]
    public partial class FMS_StationDailyFinanceDetails
    {
        /// <summary>
        /// ID
        /// </summary>		
        private Int32 _id;
        public Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 接货时间
        /// </summary>		
        private System.DateTime? _entertime;
        public System.DateTime? EnterTime
        {
            get { return _entertime; }
            set { _entertime = value; }
        }
        /// <summary>
        /// 订单号
        /// </summary>		
        private Int64 _waybillno;
        public Int64 WaybillNO
        {
            get { return _waybillno; }
            set { _waybillno = value; }
        }
        /// <summary>
        /// 记录原因,0：入站；：转站
        /// </summary>		
        private String _replacetypename;
        public String ReplaceTypeName
        {
            get { return _replacetypename; }
            set { _replacetypename = value; }
        }
        /// <summary>
        /// 应收金额
        /// </summary>		
        private System.Decimal? _needprice;
        public System.Decimal? NeedPrice
        {
            get { return _needprice; }
            set { _needprice = value; }
        }
        /// <summary>
        /// 应退金额
        /// </summary>		
        private System.Decimal? _needreturnprice;
        public System.Decimal? NeedReturnPrice
        {
            get { return _needreturnprice; }
            set { _needreturnprice = value; }
        }
        /// <summary>
        /// 实收金额
        /// </summary>		
        private System.Decimal? _pricediff;
        public System.Decimal? PriceDiff
        {
            get { return _pricediff; }
            set { _pricediff = value; }
        }
        /// <summary>
        /// 实退金额
        /// </summary>		
        private System.Decimal? _pricereturncash;
        public System.Decimal? PriceReturnCash
        {
            get { return _pricereturncash; }
            set { _pricereturncash = value; }
        }
        /// <summary>
        /// 重量
        /// </summary>		
        private System.Decimal? _weight;
        public System.Decimal? Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        /// <summary>
        /// 订单类型
        /// </summary>		
        private String _waybilltype;
        public String WaybillType
        {
            get { return _waybilltype; }
            set { _waybilltype = value; }
        }
        /// <summary>
        /// 状态名称
        /// </summary>		
        private String _statusname;
        public String StatusName
        {
            get { return _statusname; }
            set { _statusname = value; }
        }
        /// <summary>
        /// 付款方式
        /// </summary>		
        private String _accepttype;
        public String AcceptType
        {
            get { return _accepttype; }
            set { _accepttype = value; }
        }
        /// <summary>
        /// 拒收理由
        /// </summary>		
        private String _rejectreason;
        public String RejectReason
        {
            get { return _rejectreason; }
            set { _rejectreason = value; }
        }
        /// <summary>
        /// 滞留理由
        /// </summary>		
        private String _resortreason;
        public String ResortReason
        {
            get { return _resortreason; }
            set { _resortreason = value; }
        }
        /// <summary>
        /// 提交时间
        /// </summary>		
        private System.DateTime? _posttime;
        public System.DateTime? PostTime
        {
            get { return _posttime; }
            set { _posttime = value; }
        }
        /// <summary>
        /// 快递员名
        /// </summary>		
        private String _delivermanname;
        public String DeliverManName
        {
            get { return _delivermanname; }
            set { _delivermanname = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>		
        private String _comment;
        public String Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }
        /// <summary>
        /// 状态
        /// </summary>		
        private String _status;
        public String Status
        {
            get { return _status; }
            set { _status = value; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>		
        private System.DateTime? _createtime;
        public System.DateTime? CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        /// <summary>
        /// 站点ID
        /// </summary>		
        private Int32? _stationid;
        public Int32? StationID
        {
            get { return _stationid; }
            set { _stationid = value; }
        }
        /// <summary>
        /// 订单来源
        /// </summary>		
        private Int32? _sources;
        public Int32? Sources
        {
            get { return _sources; }
            set { _sources = value; }
        }
        /// <summary>
        /// 商家ID
        /// </summary>		
        private Int32? _merchantid;
        public Int32? MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
        }
        /// <summary>
        /// 1：转出；：转入
        /// </summary>		
        private System.Byte? _optype;
        public System.Byte? OPType
        {
            get { return _optype; }
            set { _optype = value; }
        }
        /// <summary>
        /// POS机终端号
        /// </summary>		
        private String _posnum;
        public String PosNum
        {
            get { return _posnum; }
            set { _posnum = value; }
        }
        /// <summary>
        /// LmsId
        /// </summary>		
        private Int32? _lmsid;
        public Int32? LmsId
        {
            get { return _lmsid; }
            set { _lmsid = value; }
        }
        /// <summary>
        /// 提成标准
        /// </summary>		
        private System.Decimal? _deductmoney;
        public System.Decimal? DeductMoney
        {
            get { return _deductmoney; }
            set { _deductmoney = value; }
        }

        private string customerOrder;
        /// <summary>
        /// 订单号
        /// </summary>
        public string CustomerOrder
        {
            get { return customerOrder; }
            set { customerOrder = value; }
        }

        private DateTime? _DailyTime;
        /// <summary>
        /// 报表日期
        /// </summary>
        public DateTime? DailyTime
        {
            get { return _DailyTime; }
            set { _DailyTime = value; }
        }

        //add by wangyongc 2011-10-31 配送费
        private System.Decimal? _deliverfee;
        /// <summary>
        /// 配送费
        /// </summary>	
        public System.Decimal? DeliverFee
        {
            get { return _deliverfee; }
            set { _deliverfee = value; }
        }

        private System.Decimal? _protectedprice;
        /// <summary>
        /// 报价费
        /// </summary>	
        public System.Decimal? Protectedprice
        {
            get { return _protectedprice; }
            set { _protectedprice = value; }
        }

        public int? DeliverManID
        {
            get;
            set;
        }
    }
}
