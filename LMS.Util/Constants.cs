using System.Configuration;

namespace RFD.FMS.Util
{
	/// <summary>
	/// 全局
	/// </summary>
	public static class Constants
	{
		// 各地零库
		/// <summary>
		/// BUG邮件主题的key
		/// </summary>
		public const string BUG_MAILSUBJECT_KEY = "MailSubject";

		/// <summary>
		/// 现金和POS机刷卡支付均可
		/// </summary>
		public const string CanPosText = "现金和POS机刷卡支付均可";

		public const string CmbName = "招行";
		public const string CommodityCode = "商品编码";
		public const string CommodityName = "商品名称";

		/// <summary>
		/// 默认的运费
		/// </summary>
		public const decimal DEFAULT_FREIGHT = 15.00M;

		public const string districtCodeBeijing = "01"; //北京地区
		public const string districtCodeGuangzhou = "03";
		public const string districtCodeShanghai = "02";

		/// <summary>
		/// 样品库代码
		/// </summary>
		public const string HOUSE_CODE_SAMPLE = "0902";

		public const string HouseCode_Beijing = "0101"; //北京
		public const string HouseCode_Guangzhou = "0301"; //广州

        public const string HouseCode_WuHan = "0401"; //武汉

		/// <summary>
		/// 辅料库代码
		/// </summary>
		public const string HouseCode_Material = "0901";

		public const string HouseCode_Shanghai = "0201"; //上海

		/// <summary>
		/// 指定产品限购的产品名的后缀
		/// </summary>
		public const string LimitPurchaseSuffix = "(限购品)";

		/// <summary>
		///发件人
		/// </summary>
		public const string mailfrom = "fms.wuliusys.com@vancl.cn";

		public const string numDMSSubmit = "100";

		public const string numEight = "8";
		public const string numFifty = "50";
		public const string numFive = "5";
		public const string numFour = "4";
		public const string numNine = "9";
		public const string numOne = "1";
		public const string numOneHundred = "100";
		public const string numSeven = "7";
		public const string numSix = "6";
		public const string numThree = "3";
		public const string numTwo = "2";
		public const string numTwoHundred = "200";
		public const string numZero = "0";

		/// <summary>
		/// 密码
		/// </summary>
		public const string password = "ck~2009";

		public const string PayPhoneCCB = "21"; //电话银行支付-建行
		public const string PayPhoneCmbChina = "20"; //电话银行支付-招行
		public const string PhoneCallInCmbChina = "1083633519"; //招行唯一电话
		public const string ProductCode = "产品编码";
		public const string ProductName = "产品名称";

		public const string SelfSuitProduct = "按产品";
		public const string SelfSuitWeb = "按网站";

		/// <summary>
		/// socket监听ip
		/// </summary>
		public const string SocketListenIP = "127.0.0.1";

		/// <summary>
		/// 委托加工中转仓库
		/// </summary>
		public const string SUBCONTRACT_TRANSFER_HOUSE_CODE = "0903";

		public const decimal SWAP_PRODUCT_COST_HIGH_RATE = 1.05M;
		public const decimal SWAP_PRODUCT_COST_LOW_RATE = 0.95M;
		public const string System = "系统";

		//将发货订单明细邮件自动发送 给配送商 邮件信息
		/// <summary>
		/// 用户名
		/// </summary>
		public const string user = "cangchu@vancloa.cn";

		public const string Vancl = "Vancl";
		///<summary>
		/// 广州仓库
		///</summary>
		public const string WMS_GZ = "WMS_GZ";

		/// <summary>
		/// Vancl邮件发送服务器
		/// </summary>
        public const string VANCL_SMTP = "smtpsrv02.vancloa.cn";

		/// <summary>
		/// vancl socket监听端口
		/// </summary>
		public const int VanclSocketPort = 9999;

		public const string VJia = "VJia";

		/// <summary>
		/// vjia socket监听端口
		/// </summary>
		public const int VJiaSocketPort = 9998;

		public const string wareHouseBig = "00"; //整库
		public const string wareHouseSmall = "01"; //零库

		///<summary>
		/// 北京地区Id
		///</summary>
		public const string BEI_JING_DISTRICT_ID = "01";//北京地区Id
		///<summary>
		/// 上海地区Id
		///</summary>
		public const string SHANG_HAI_DISTRICT_ID = "02";//上海地区Id
		///<summary>
		/// 广州地区Id
		///</summary>
		public const string GUANG_ZHOU_DISTRICT_ID = "03";//广州地区Id
		/// <summary>
		/// 编码数量Excel模板URL
		/// </summary>
		public const string ExcelIdAmountUrl = "http://crm.vancl.com:8001/public/excel/idamount.xls";
		/// <summary>
		/// 编码数量单价Excel模板URL
		/// </summary>
		public const string ExcelIdAmountPriceUrl = "http://crm.vancl.com:8001/public/excel/idamountprice.xls";

		///<summary>
		/// 当前登陆主库
		///</summary>
		private static string CurrentServerConnection
		{
			get { return ConfigurationManager.AppSettings["serverConnection"].Trim(); }
		}

		///<summary>
		/// 当前登陆主库
		///</summary>
		public static string CurrentServer
		{
			get
			{
				if (CurrentServerConnection == 1.ToString())
				{
					return Vancl;
				}
				if (CurrentServerConnection == 2.ToString())
				{
					return VJia;
				}
				return string.Empty;
			}
		}

        ///<summary>
        /// 当前登陆Vancl或是Vancljia的具体仓库
        ///</summary>
        public static string CurrentDatabaseHouse
        {
            get;
            set;
        }

        /// <summary>
        /// rufengdaCode
        /// </summary>
	    public const string RfdDistributionCode = "rfd";
	}
}