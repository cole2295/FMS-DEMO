using System;
using System.Collections.Generic;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class ThirdPartyWaybillSearchConditons
    {
        //此类不对应数据某表，逻辑使用
        public ThirdPartyWaybillSearchConditons()
        {
        }
        #region Model
        private long _waybillno;
        private string _waybilltype;
        private string _waybillstatus;
        private string _backstatus;
        private int? _sources;
        private string _outwarehouseid;
        private string _inwarehouseid;
        private string _sortingCenter;
        private string _returnSortingCenter;
        private string _deliverstationid;
        private int _timetype;
        private DateTime? _outcreattimebegin;
        private DateTime? _outcreattimeend;
        private DateTime? _increattimebegin;
        private DateTime? _increattimeend;
        private DateTime? _backstationtimebegin;
        private DateTime? _backstationtimeend;
        private string _merchantId;
        private int? _areaexpresslevel;
        private string _distributioncode;
        private string _customerorder;
        private int _isdelete;
        private string _paymenttype;
        private string _rfdtype;
        private int? _inefficacyStatus;
        private bool _isQucikQuery;
        private int _dateType;

        public bool IsQucikQuery
        {
            set { _isQucikQuery = value; }
            get { return _isQucikQuery; }
        }

        public string RFDType
        {
            set { _rfdtype = value; }
            get { return _rfdtype; }
        }

        public string Customerorder
        {
            set { _customerorder = value; }
            get { return _customerorder; }
        }

        public int IsDelete
        {
            set { _isdelete = value; }
            get { return _isdelete; }
        }
        public string DistributionCode
        {
            set { this._distributioncode = value; }
            get { return this._distributioncode; }
        }


        public string PaymentType
        {
            set { _paymenttype = value; }
            get { return _paymenttype; }
        }
        /// <summary>
        /// 运单编码
        /// </summary>
        public long WaybillNO
        {
            set { _waybillno = value; }
            get { return _waybillno; }
        }

        /// <summary>
        /// 运单类型 0普通订单，1上门换货，2上门退货,3 签单返回
        /// </summary>
        public string WaybillType
        {
            set { _waybilltype = value; }
            get { return _waybilltype; }
        }

        public string BackStatus
        {
            set { _backstatus = value; }
            get { return _backstatus; }
        }

        public string WaybillStatus
        {
            set { _waybillstatus = value; }
            get { return _waybillstatus; }
        }


        /// <summary>
        /// 运单来源
        /// </summary>
        public int? Sources
        {
            set { _sources = value; }
            get { return _sources; }
        }

        /// <summary>
        /// 出库仓库ID
        /// </summary>
        public string OutWarehouseId
        {
            set { _outwarehouseid = value; }
            get { return _outwarehouseid; }
        }

        /// <summary>
        /// 入库仓库ID
        /// </summary>
        public string InWarehouseId
        {
            set { _inwarehouseid = value; }
            get { return _inwarehouseid; }
        }

        /// <summary>
        /// 配送站点
        /// </summary>
        public string DeliverStationID
        {
            set { _deliverstationid = value; }
            get { return _deliverstationid; }
        }

        /// <summary>
        /// 查询时间类型
        /// </summary>
        public int TimeType
        {
            set { _timetype = value; }
            get { return _timetype; }
        }

        /// <summary>
        /// 出库时间条件的开始时间
        /// </summary>
        public DateTime? OutCreatTimeBegin
        {
            set { _outcreattimebegin = value; }
            get { return _outcreattimebegin; }
        }

        /// <summary>
        /// 出库时间条件的结束时间
        /// </summary>
        public DateTime? OutCreatTimeEnd
        {
            set { _outcreattimeend = value; }
            get { return _outcreattimeend; }
        }

        public DateTime? BackStationTimeBegin
        {
            set { _backstationtimebegin = value; }
            get { return _backstationtimebegin; }
        }

        public DateTime? BackStationTimeEnd
        {
            set { _backstationtimeend = value; }
            get { return _backstationtimeend; }
        }

        /// <summary>
        /// 入库时间条件的开始时间
        /// </summary>
        public DateTime? InCreatTimeBegin
        {
            set { _increattimebegin = value; }
            get { return _increattimebegin; }
        }

        /// <summary>
        /// 入库时间条件的结束时间
        /// </summary>
        public DateTime? InCreatTimeEnd
        {
            set { _increattimeend = value; }
            get { return _increattimeend; }
        }

        /// <summary>
        /// 商家信息
        /// </summary>
        public string MerchantID
        {
            set { _merchantId = value; }
            get { return _merchantId; }
        }

        /// <summary>
        ///  区域类型：1，2，3，4
        /// </summary>
        public int? AreaExpressLevel
        {
            set { _areaexpresslevel = value; }
            get { return _areaexpresslevel; }
        }

        public string SortingCenter
        {
            get { return _sortingCenter; }
            set { _sortingCenter = value; }
        }

        public string ReturnSortingCenter
        {
            set { _returnSortingCenter = value; }
            get { return _returnSortingCenter; }
        }

        public int? InefficacyStatus
        {
            set { _inefficacyStatus = value; }
            get { return _inefficacyStatus; }
        }

        public int DateType
        {
            set { _dateType = value; }
            get { return _dateType; }
        }

        /// <summary>
        /// 获取时间段,而不管是哪个时间段
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<DateTime, DateTime> GetTimeRange()
        {
            if (BackStationTimeBegin.HasValue && BackStationTimeEnd.HasValue)
            {
                return new KeyValuePair<DateTime, DateTime>(BackStationTimeBegin.Value,
                                                            BackStationTimeEnd.Value);
            }

            if (InCreatTimeBegin.HasValue && InCreatTimeEnd.HasValue)
            {
                return new KeyValuePair<DateTime, DateTime>(InCreatTimeBegin.Value,
                                                            InCreatTimeEnd.Value);
            }

            if (OutCreatTimeBegin.HasValue && OutCreatTimeEnd.HasValue)
            {
                return new KeyValuePair<DateTime, DateTime>(OutCreatTimeBegin.Value,
                                                            OutCreatTimeEnd.Value);
            }

            throw new Exception("请选择日期");
        }

        /// <summary>
        /// 设定时间段,而不管是哪个时间段
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="End"></param>
        public void SetTimeRange(DateTime begin, DateTime end, DateTime endTime)
        {
            if (BackStationTimeBegin.HasValue && BackStationTimeEnd.HasValue)
            {
                BackStationTimeBegin = begin;
                if (end < endTime)
                    BackStationTimeEnd = end;
                else
                    BackStationTimeEnd = endTime;
                return;
            }

            if (InCreatTimeBegin.HasValue && InCreatTimeEnd.HasValue)
            {
                InCreatTimeBegin = begin;
                if (end < endTime)
                    InCreatTimeEnd = end;
                else
                    InCreatTimeEnd = endTime;
                return;
            }

            if (OutCreatTimeBegin.HasValue && OutCreatTimeEnd.HasValue)
            {
                OutCreatTimeBegin = begin;
                if (end < endTime)
                    OutCreatTimeEnd = end;
                else
                    OutCreatTimeEnd = endTime;
                return;
            }

            throw new Exception("请选择日期");
        }

        #endregion Model
    }

}
