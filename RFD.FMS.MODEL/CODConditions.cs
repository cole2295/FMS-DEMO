using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
    //此类不对应数据某表，逻辑使用
    public class CODConditions
    {
        public CODConditions()
        {
        }

        #region Model
        private long _waybillno;
        private string _waybilltype;       
        private string _sources; 
        private string _outwarehouseid;
        private string _inwarehouseid;
        private string _sortingCenter;
        private string _finalShipmentSortingCenter;
        private string _returnSortingCenter;
        private int? _deliverstationid; 
        private DateTime? _outcreattimebegin;
        private DateTime? _outcreattimeend;
        private DateTime? _outfinaltimebegin;
        private DateTime? _outfinaltimeend;
        private DateTime? _increattimebegin;
        private DateTime? _increattimeend;
        private string _merchantId;
        private string _reporttype;
        private string _shipmenttype;
        private int? _areaexpresslevel;
        private string _distributioncode;

        public string DistributionCode
        {
            set { this._distributioncode = value; }
            get { return this._distributioncode; }
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
        /// 运单类型 0普通订单，1上门换货，2上门退货
        /// </summary>
        public string WaybillType
        {
            set { _waybilltype = value; }
            get { return _waybilltype; }
        }
        
        /// <summary>
        /// 运单来源
        /// </summary>
        public string Sources
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
        public int? DeliverStationID
        {
            set { _deliverstationid = value; }
            get { return _deliverstationid; }
        }    
        
        /// <summary>
        /// 出库时间条件的开始时间
        /// </summary>
        public DateTime? OutCreatTimeBegin
        {
            set { _outcreattimebegin = value; }
            get { return _outcreattimebegin; }
        }


        public DateTime? OutFinalTimeBegin
        {
            set { _outfinaltimebegin = value; }
            get { return _outfinaltimebegin; }
        }
        /// <summary>
        /// 出库时间条件的结束时间
        /// </summary>
        public DateTime? OutFinalTimeEnd
        {
            set { _outfinaltimeend = value; }
            get { return _outfinaltimeend; }
        }

        public DateTime? OutCreatTimeEnd
        {
            set { _outcreattimeend = value; }
            get { return _outcreattimeend; }
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
        /// 报表类型:发货、拒收、上门退换货
        /// </summary>
        public string ReportType
        {
            set { _reporttype = value; }
            get { return _reporttype; }
        }

        /// <summary>
        /// 发货类型：普通发货、拒收发货
        /// </summary>
        public string ShipmentType
        {
            set { _shipmenttype = value; }
            get { return _shipmenttype; }
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


        public string FinalShipmentSortingCenter
        {
            get { return _finalShipmentSortingCenter; }
            set { _finalShipmentSortingCenter = value; }
        }
        public string ReturnSortingCenter
        {
            set { _returnSortingCenter = value; }
            get { return _returnSortingCenter; }
        }


        #endregion Model
    }

	[Serializable]
	public class CodSearchCondition
	{
		public String RFDType { get; set; }
		/// <summary>
		/// 配送公司
		/// </summary>
		public String ExpressCompanyID { get; set; }

		/// <summary>
		/// 分配区域
		/// </summary>
		public String AreaType { get; set; }

        /// <summary>
        /// 是否按分配区域分组
        /// </summary>
        public bool IsAreaType { get; set; }

		/// <summary>
		/// 订单号
		/// </summary>
		public String WaybillNO { get; set; }

		/// <summary>
		/// 运单来源
		/// </summary>
		public String Sources { get; set; }

		/// <summary>
		/// 报表类型
		/// </summary>
		public String ReportType { get; set; }

		/// <summary>
		/// 发货、拒收、上门退类型
		/// </summary>
		public String ShipmentType { get; set; }

		/// <summary>
		/// 日期类型
		/// </summary>
		public String DateType { get; set; }

		/// <summary>
		/// 开始时间
		/// </summary>
		public String DateStr { get; set; }

		/// <summary>
		/// 结束时间
		/// </summary>
		public String DateEnd { get; set; }

		/// <summary>
		/// 仓库、分拣类型
		/// </summary>
		public String HouseType { get; set; }

		/// <summary>
		/// 仓库、分拣编号
		/// </summary>
		public String HouseCode { get; set; }

        /// <summary>
        /// 当前登录者的所属配送公司CODE
        /// </summary>
        public String DistributionCode { get; set; }

        /// <summary>
        /// 是否COD
        /// </summary>
        public Int32 IsCod { get; set; }

        /// <summary>
        /// 是否COD汇总
        /// </summary>
        public Boolean SummaryByCod { get; set; }

        /// <summary>
        /// 总金额 开始值
        /// </summary>
        public Decimal? AmountStr { get; set; }

        /// <summary>
        /// 总金额 结束值
        /// </summary>
        public Decimal? AmountEnd { get; set; }
	}
}
