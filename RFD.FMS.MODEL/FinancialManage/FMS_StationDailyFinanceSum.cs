using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RFD.FMS.Model.UnionPay
{
    /// <summary>
    /// FMS_StationDailyFinanceSum
    /// </summary>
    [Serializable]
    public partial class FMS_StationDailyFinanceSum
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
        /// 当日接单量
        /// </summary>		
        private Int32 _dayreceiveordercount;
        public Int32 DayReceiveOrderCount
        {
            get { return _dayreceiveordercount; }
            set { _dayreceiveordercount = value; }
        }
        /// <summary>
        /// 当日接货应收金额
        /// </summary>		
        private System.Decimal? _dayreceiveneedinsum;
        public System.Decimal? DayReceiveNeedInSum
        {
            get { return _dayreceiveneedinsum; }
            set { _dayreceiveneedinsum = value; }
        }
        /// <summary>
        /// 当日接货应退金额
        /// </summary>		
        private System.Decimal? _dayreceiveneedoutsum;
        public System.Decimal? DayReceiveNeedOutSum
        {
            get { return _dayreceiveneedoutsum; }
            set { _dayreceiveneedoutsum = value; }
        }
        /// <summary>
        /// 当日取货量
        /// </summary>		
        private Int32? _dayreceivegoodscount;
        public Int32? DayReceiveGoodsCount
        {
            get { return _dayreceivegoodscount; }
            set { _dayreceivegoodscount = value; }
        }
        /// <summary>
        /// 当日上门退款单量
        /// </summary>		
        private Int32? _dayoutordercount;
        public Int32? DayOutOrderCount
        {
            get { return _dayoutordercount; }
            set { _dayoutordercount = value; }
        }
        /// <summary>
        /// 前日滞留量
        /// </summary>		
        private Int32? _predayresortcount;
        public Int32? PreDayResortCount
        {
            get { return _predayresortcount; }
            set { _predayresortcount = value; }
        }
        /// <summary>
        /// 前日滞留应收金额
        /// </summary>		
        private System.Decimal? _predayresortneedinsum;
        public System.Decimal? PreDayResortNeedInSum
        {
            get { return _predayresortneedinsum; }
            set { _predayresortneedinsum = value; }
        }
        /// <summary>
        /// PreDayResortNeedOutSum
        /// </summary>		
        private System.Decimal? _predayresortneedoutsum;
        public System.Decimal? PreDayResortNeedOutSum
        {
            get { return _predayresortneedoutsum; }
            set { _predayresortneedoutsum = value; }
        }
        /// <summary>
        /// 当日转站入站订单量
        /// </summary>		
        private Int32? _daytransferordercount;
        public Int32? DayTransferOrderCount
        {
            get { return _daytransferordercount; }
            set { _daytransferordercount = value; }
        }
        /// <summary>
        /// 当日转站入站应收金额
        /// </summary>		
        private System.Decimal? _dayinstationneedinsum;
        public System.Decimal? DayInStationNeedInSum
        {
            get { return _dayinstationneedinsum; }
            set { _dayinstationneedinsum = value; }
        }
        /// <summary>
        /// 当日转站入站应退金额
        /// </summary>		
        private System.Decimal? _dayinstationneedoutsum;
        public System.Decimal? DayInStationNeedOutSum
        {
            get { return _dayinstationneedoutsum; }
            set { _dayinstationneedoutsum = value; }
        }
        /// <summary>
        /// 当日应配送总单量
        /// </summary>		
        private Int32? _dayneeddeliverordercount;
        public Int32? DayNeedDeliverOrderCount
        {
            get { return _dayneeddeliverordercount; }
            set { _dayneeddeliverordercount = value; }
        }
        /// <summary>
        /// 当日应配送总金额(应收）
        /// </summary>		
        private System.Decimal? _dayneeddeliverinsum;
        public System.Decimal? DayNeedDeliverInSum
        {
            get { return _dayneeddeliverinsum; }
            set { _dayneeddeliverinsum = value; }
        }
        /// <summary>
        /// 当日应配送总金额（应退）
        /// </summary>		
        private System.Decimal? _dayneeddeliveroutsum;
        public System.Decimal? DayNeedDeliverOutSum
        {
            get { return _dayneeddeliveroutsum; }
            set { _dayneeddeliveroutsum = value; }
        }
        /// <summary>
        /// 现金成功单量
        /// </summary>		
        private Int32? _cashsuccordercount;
        public Int32? CashSuccOrderCount
        {
            get { return _cashsuccordercount; }
            set { _cashsuccordercount = value; }
        }
        /// <summary>
        /// 现金实收金额
        /// </summary>		
        private System.Decimal? _cashrealinsum;
        public System.Decimal? CashRealInSum
        {
            get { return _cashrealinsum; }
            set { _cashrealinsum = value; }
        }
        /// <summary>
        /// 现金实退金额
        /// </summary>		
        private System.Decimal? _cashrealoutsum;
        public System.Decimal? CashRealOutSum
        {
            get { return _cashrealoutsum; }
            set { _cashrealoutsum = value; }
        }
        /// <summary>
        /// POS成功单量
        /// </summary>		
        private Int32? _possuccordercount;
        public Int32? PosSuccOrderCount
        {
            get { return _possuccordercount; }
            set { _possuccordercount = value; }
        }
        /// <summary>
        /// POS实收金额
        /// </summary>		
        private System.Decimal? _posrealinsum;
        public System.Decimal? PosRealInSum
        {
            get { return _posrealinsum; }
            set { _posrealinsum = value; }
        }
        /// <summary>
        /// 配送成功率
        /// </summary>		
        private String _deliversuccrate;
        public String DeliverSuccRate
        {
            get { return _deliversuccrate; }
            set { _deliversuccrate = value; }
        }
        /// <summary>
        /// 拒收单量
        /// </summary>		
        private Int32? _rejectordercount;
        public Int32? RejectOrderCount
        {
            get { return _rejectordercount; }
            set { _rejectordercount = value; }
        }
        /// <summary>
        /// 全部拒收金额(应收）
        /// </summary>		
        private System.Decimal? _allrejectneedinsum;
        public System.Decimal? AllRejectNeedInSum
        {
            get { return _allrejectneedinsum; }
            set { _allrejectneedinsum = value; }
        }
        /// <summary>
        /// 全部拒收金额(应退）
        /// </summary>		
        private System.Decimal? _allrejectneedoutsum;
        public System.Decimal? AllRejectNeedOutSum
        {
            get { return _allrejectneedoutsum; }
            set { _allrejectneedoutsum = value; }
        }
        /// <summary>
        /// 当日滞留量
        /// </summary>		
        private Int32? _dayresortcount;
        public Int32? DayResortCount
        {
            get { return _dayresortcount; }
            set { _dayresortcount = value; }
        }
        /// <summary>
        /// 当日滞留金额（应收）
        /// </summary>		
        private System.Decimal? _dayresortneedinsum;
        public System.Decimal? DayResortNeedInSum
        {
            get { return _dayresortneedinsum; }
            set { _dayresortneedinsum = value; }
        }
        /// <summary>
        /// 当日滞留金额（应退）
        /// </summary>		
        private System.Decimal? _dayresortneedoutsum;
        public System.Decimal? DayResortNeedOutSum
        {
            get { return _dayresortneedoutsum; }
            set { _dayresortneedoutsum = value; }
        }
        /// <summary>
        /// 当日转出订单量
        /// </summary>		
        private Int32? _dayoutstationordercount;
        public Int32? DayOutStationOrderCount
        {
            get { return _dayoutstationordercount; }
            set { _dayoutstationordercount = value; }
        }
        /// <summary>
        /// 当日转出订单金额（应收）
        /// </summary>		
        private System.Decimal? _dayoutstationneedinsum;
        public System.Decimal? DayOutStationNeedInSum
        {
            get { return _dayoutstationneedinsum; }
            set { _dayoutstationneedinsum = value; }
        }
        /// <summary>
        /// 当日转出订单金额（应退）
        /// </summary>		
        private System.Decimal? _dayoutstationneedoutsum;
        public System.Decimal? DayOutStationNeedOutSum
        {
            get { return _dayoutstationneedoutsum; }
            set { _dayoutstationneedoutsum = value; }
        }
        /// <summary>
        /// 滞留率
        /// </summary>		
        private String _resortrate;
        public String ResortRate
        {
            get { return _resortrate; }
            set { _resortrate = value; }
        }
        /// <summary>
        /// 站点
        /// </summary>		
        private Int32? _stationid;
        public Int32? StationID
        {
            get { return _stationid; }
            set { _stationid = value; }
        }
        /// <summary>
        /// 订单来源（0：Vancl 1：Vjia 2：其他）
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
        /// 报表时间
        /// </summary>		
        private System.DateTime? _dailytime;
        public System.DateTime? DailyTime
        {
            get { return _dailytime; }
            set { _dailytime = value; }
        }
        /// <summary>
        /// 生成时间
        /// </summary>		
        private System.DateTime? _createtime;
        public System.DateTime? CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        /// <summary>
        /// UpdateTime
        /// </summary>		
        private System.DateTime? _updatetime;
        public System.DateTime? UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
        /// <summary>
        /// 财务收款状态1：已收
        /// </summary>		
        private Int32? _financestatus;
        public Int32? FinanceStatus
        {
            get { return _financestatus; }
            set { _financestatus = value; }
        }
        /// <summary>
        /// 财务实收金额
        /// </summary>		
        private System.Decimal? _realincome;
        public System.Decimal? RealInCome
        {
            get { return _realincome; }
            set { _realincome = value; }
        }
        /// <summary>
        /// 财务收款状态pos确认；1已收
        /// </summary>		
        private Int32? _poschecked;
        public Int32? POSChecked
        {
            get { return _poschecked; }
            set { _poschecked = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>		
        private String _remark;
        public String Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
        /// <summary>
        /// 当日上门退货金额
        /// </summary>		
        private Decimal? _dayoutordersum;
        public Decimal? DayOutOrderSum
        {
            get { return _dayoutordersum; }
            set { _dayoutordersum = value; }
        }

        private Decimal? _exchangeOrderSum;
        /// <summary>
        /// 换货总金额
        /// </summary>
        public Decimal? ExchangeOrderSum
        {
            get { return _exchangeOrderSum; }
            set { _exchangeOrderSum = value; }
        }

        private int? _exchangeOrderCount;
        /// <summary>
        /// 换货单量
        /// </summary>
        public int? ExchangeOrderCount
        {
            get { return _exchangeOrderCount; }
            set { _exchangeOrderCount = value; }
        }
    }
}
