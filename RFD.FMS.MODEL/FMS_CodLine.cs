using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// COD线路价格
	/// </summary>
    [Serializable]
	public partial class FMS_CODLine
	{
		public FMS_CODLine()
		{ }
		#region Model
		private long _lineid;
		private string _codlineno;
		private int _expresscompanyid;
		private string _companyname;
		private int _isechelon = 0;
		private string _warehouseid;
		private int _areatype;
		private string _priceformula = "";
		private int _linestatus = 0;
		private int _auditstatus = 0;
		private string _auditby = "";
		private DateTime _audittime = DateTime.Now;
		private string _createby = "";
		private DateTime _createtime = DateTime.Now;
		private string _updateby = "";
		private DateTime _updatetime = DateTime.Now;
		private bool _deleteflag = false;
		private DateTime _effectdate;
		private int _warehousetype;
		private int _merchantid;
		private int _productid;
        private string _distributionCode;
        private int _isCod;
        private string _formula;
        private string _merchantName;

		/// <summary>
		/// 自增编号
		/// </summary>
		public long LineID
		{
			set { _lineid = value; }
			get { return _lineid; }
		}
		/// <summary>
		/// 线路编号
		/// </summary>
		public string CODLineNO
		{
			set { _codlineno = value; }
			get { return _codlineno; }
		}
		/// <summary>
		/// COD承运商编号，外键
		/// </summary>
		public int ExpressCompanyID
		{
			set { _expresscompanyid = value; }
			get { return _expresscompanyid; }
		}
		/// <summary>
		/// COD承运商名称
		/// </summary>
		public string CompanyName
		{
			set { _companyname = value; }
			get { return _companyname; }
		}
		/// <summary>
		/// 是否按发货地梯次收费,默认0,1、是，2、否
		/// </summary>
		public int IsEchelon
		{
			set { _isechelon = value; }
			get { return _isechelon; }
		}
		/// <summary>
		/// 发货仓库
		/// </summary>
		public string WareHouseID
		{
			set { _warehouseid = value; }
			get { return _warehouseid; }
		}
		/// <summary>
		/// 区域类型，默认0，只存在1,2,3,4
		/// </summary>
		public int AreaType
		{
			set { _areatype = value; }
			get { return _areatype; }
		}
		/// <summary>
		/// 价格公式，只存在两种情况，1、没有公式、只记录价格，2、没有价格只记录公式
		/// </summary>
		public string PriceFormula
		{
			set { _priceformula = value; }
			get { return _priceformula; }
		}
		/// <summary>
		/// 线路状态，默认0，1、可用，2、暂停
		/// </summary>
		public int LineStatus
		{
			set { _linestatus = value; }
			get { return _linestatus; }
		}
		/// <summary>
		/// 审核状态，默认0,1、未审核，2、已审核，3、置回
		/// </summary>
		public int AuditStatus
		{
			set { _auditstatus = value; }
			get { return _auditstatus; }
		}
		/// <summary>
		/// 最后确认人
		/// </summary>
		public string AuditBy
		{
			set { _auditby = value; }
			get { return _auditby; }
		}
		/// <summary>
		/// 确认时间
		/// </summary>
		public DateTime AuditTime
		{
			set { _audittime = value; }
			get { return _audittime; }
		}
		/// <summary>
		/// 创建人
		/// </summary>
		public string CreateBy
		{
			set { _createby = value; }
			get { return _createby; }
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			set { _createtime = value; }
			get { return _createtime; }
		}
		/// <summary>
		/// 最后更改人
		/// </summary>
		public string UpdateBy
		{
			set { _updateby = value; }
			get { return _updateby; }
		}
		/// <summary>
		/// 更改时间
		/// </summary>
		public DateTime UpdateTime
		{
			set { _updatetime = value; }
			get { return _updatetime; }
		}
		/// <summary>
		/// 删除表示，当1是表示删除
		/// </summary>
		public bool DeleteFlag
		{
			set { _deleteflag = value; }
			get { return _deleteflag; }
		}

		/// <summary>
		/// 待生效时间
		/// </summary>
		public DateTime EffectDate
		{
			get { return _effectdate; }
			set { _effectdate = value; }
		}

		/// <summary>
		/// 0、无，1、仓库，2、分拣中心
		/// </summary>
		public int WareHouseType
		{
			set { _warehousetype = value; }
			get { return _warehousetype; }
		}

		/// <summary>
		/// 商家ID
		/// </summary>
		public int MerchantID
		{
			set { _merchantid = value; }
			get { return _merchantid; }
		}

		/// <summary>
		/// 产品ID
		/// </summary>
		public int ProductID
		{
			set { _productid = value; }
			get { return _productid; }
		}

        /// <summary>
        /// 配送商编码
        /// </summary>
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }

        /// <summary>
        /// 是否区分COD
        /// </summary>
        public int IsCOD
        {
            get { return _isCod; }
            set { _isCod = value; }
        }

        /// <summary>
        /// 非COD公式
        /// </summary>
        public string Formula
        {
            get { return _formula; }
            set { _formula = value; }
        }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName
        {
            get { return _merchantName; }
            set { _merchantName = value; }
        }
		#endregion Model
	}
}
