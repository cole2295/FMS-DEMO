using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// COD结算创建查询条件model
	/// </summary>
	[Serializable]
	public class CODSearchCondition
	{
		private string _accountno;
		private string _expresscompany;
		private string _expressCompanyChilds;
		private string _companyname;
		private DateTime _date_d_s;
		private DateTime _date_d_e;
		private DateTime _date_r_s;
		private DateTime _date_r_e;
		private DateTime _date_v_s;
		private DateTime _date_v_e;
		private string _housed;
		private string _houser;
		private string _housev;
		private int _accountType;
		private string _merchantid;
		private string _merchantname;
        private int _isdifference;
        private string _distributionCode;
        private bool _isAreaType;
        private bool _iscod;
        private string _countType;
        private string _displayCompanyName;
        private string _displayMetchantName;

        /// <summary>
        /// 统计的类型 D、发货，R、拒收，V、上门退
        /// </summary>
        public string CountType
        {
            get { return _countType; }
            set { _countType = value; }
        }

		public string AccountNO
		{
			get { return _accountno; }
			set { _accountno = value; }
		}

		public string ExpressCompanyID
		{
			get { return _expresscompany; }
			set { _expresscompany = value; }
		}

		/// <summary>
		/// 配送公司下面的所有子 XML存储
		/// </summary>
		public string ExpressCompanyChilds
		{
			get { return _expressCompanyChilds; }
			set { _expressCompanyChilds = value; }
		}

		public string CompanyName
		{
			get { return _companyname; }
			set { _companyname = value; }
		}

		public DateTime Date_D_S
		{
			get { return _date_d_s; }
			set { _date_d_s = value; }
		}

		public DateTime Date_D_E
		{
			get { return _date_d_e; }
			set { _date_d_e= value; }
		}

		public DateTime Date_R_S
		{
			get { return _date_r_s; }
			set {_date_r_s= value; }
		}

		public DateTime Date_R_E
		{
			get { return _date_r_e; }
			set { _date_r_e=value; }
		}

		public DateTime Date_V_S
		{
			get { return _date_v_s; }
			set { _date_v_s=value; }
		}

		public DateTime Date_V_E
		{
			get { return _date_v_e; }
			set { _date_v_e= value; }
		}

		public string HouseD
		{
			get { return _housed; }
			set { _housed= value; }
		}

		public string HouseR
		{
			get { return _houser; }
			set { _houser = value; }
		}

		public string HouseV
		{
			get { return _housev; }
			set { _housev= value; }
		}

		public int AccountType
		{
			get { return _accountType; }
			set { _accountType = value; }
		}

		public string MerchantID
		{
			get { return _merchantid; }
			set { _merchantid = value; }
		}

		public string MerchantName
		{
			get { return _merchantname; }
			set { _merchantname = value; }
		}
        /// <summary>
        /// 是否存在统计差异
        /// </summary>
        public int IsDifference
        {
            get { return _isdifference; }
            set { _isdifference = value; }
        }

        /// <summary>
        /// 配送商code
        /// </summary>
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }

        /// <summary>
        /// 是否按区域类型分组
        /// </summary>
        public bool IsAreaType
        {
            get { return _isAreaType; }
            set { _isAreaType = value; }
        }
        /// <summary>
        /// 是否按业务分组
        /// </summary>
        public bool IsCOD
        {
            get { return _iscod; }
            set { _iscod = value; }
        }

        /// <summary>
        /// 显示的公司名称
        /// </summary>
        public string DisplayCompanyName
        {
            get { return _displayCompanyName; }
            set { _displayCompanyName = value; }
        }

        /// <summary>
        /// 显示的商家名称
        /// </summary>
        public string DisplayMerchantName
        {
            get { return _displayMetchantName; }
            set { _displayMetchantName = value; }
        }
	}

	[Serializable]
	public class PageColumns
	{
		/// <summary>
		/// 显示列
		/// </summary>
		public IDictionary<string, bool> ColumnsShow { get; set; }

		public IList<string> DataColumns { get; set; }
	}
}
