using System.Configuration;

namespace RFD.FMS.Util
{
	/// <summary>
	/// ȫ��
	/// </summary>
	public static class Constants
	{
		// �������
		/// <summary>
		/// BUG�ʼ������key
		/// </summary>
		public const string BUG_MAILSUBJECT_KEY = "MailSubject";

		/// <summary>
		/// �ֽ��POS��ˢ��֧������
		/// </summary>
		public const string CanPosText = "�ֽ��POS��ˢ��֧������";

		public const string CmbName = "����";
		public const string CommodityCode = "��Ʒ����";
		public const string CommodityName = "��Ʒ����";

		/// <summary>
		/// Ĭ�ϵ��˷�
		/// </summary>
		public const decimal DEFAULT_FREIGHT = 15.00M;

		public const string districtCodeBeijing = "01"; //��������
		public const string districtCodeGuangzhou = "03";
		public const string districtCodeShanghai = "02";

		/// <summary>
		/// ��Ʒ�����
		/// </summary>
		public const string HOUSE_CODE_SAMPLE = "0902";

		public const string HouseCode_Beijing = "0101"; //����
		public const string HouseCode_Guangzhou = "0301"; //����

        public const string HouseCode_WuHan = "0401"; //�人

		/// <summary>
		/// ���Ͽ����
		/// </summary>
		public const string HouseCode_Material = "0901";

		public const string HouseCode_Shanghai = "0201"; //�Ϻ�

		/// <summary>
		/// ָ����Ʒ�޹��Ĳ�Ʒ���ĺ�׺
		/// </summary>
		public const string LimitPurchaseSuffix = "(�޹�Ʒ)";

		/// <summary>
		///������
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
		/// ����
		/// </summary>
		public const string password = "ck~2009";

		public const string PayPhoneCCB = "21"; //�绰����֧��-����
		public const string PayPhoneCmbChina = "20"; //�绰����֧��-����
		public const string PhoneCallInCmbChina = "1083633519"; //����Ψһ�绰
		public const string ProductCode = "��Ʒ����";
		public const string ProductName = "��Ʒ����";

		public const string SelfSuitProduct = "����Ʒ";
		public const string SelfSuitWeb = "����վ";

		/// <summary>
		/// socket����ip
		/// </summary>
		public const string SocketListenIP = "127.0.0.1";

		/// <summary>
		/// ί�мӹ���ת�ֿ�
		/// </summary>
		public const string SUBCONTRACT_TRANSFER_HOUSE_CODE = "0903";

		public const decimal SWAP_PRODUCT_COST_HIGH_RATE = 1.05M;
		public const decimal SWAP_PRODUCT_COST_LOW_RATE = 0.95M;
		public const string System = "ϵͳ";

		//������������ϸ�ʼ��Զ����� �������� �ʼ���Ϣ
		/// <summary>
		/// �û���
		/// </summary>
		public const string user = "cangchu@vancloa.cn";

		public const string Vancl = "Vancl";
		///<summary>
		/// ���ݲֿ�
		///</summary>
		public const string WMS_GZ = "WMS_GZ";

		/// <summary>
		/// Vancl�ʼ����ͷ�����
		/// </summary>
        public const string VANCL_SMTP = "smtpsrv02.vancloa.cn";

		/// <summary>
		/// vancl socket�����˿�
		/// </summary>
		public const int VanclSocketPort = 9999;

		public const string VJia = "VJia";

		/// <summary>
		/// vjia socket�����˿�
		/// </summary>
		public const int VJiaSocketPort = 9998;

		public const string wareHouseBig = "00"; //����
		public const string wareHouseSmall = "01"; //���

		///<summary>
		/// ��������Id
		///</summary>
		public const string BEI_JING_DISTRICT_ID = "01";//��������Id
		///<summary>
		/// �Ϻ�����Id
		///</summary>
		public const string SHANG_HAI_DISTRICT_ID = "02";//�Ϻ�����Id
		///<summary>
		/// ���ݵ���Id
		///</summary>
		public const string GUANG_ZHOU_DISTRICT_ID = "03";//���ݵ���Id
		/// <summary>
		/// ��������Excelģ��URL
		/// </summary>
		public const string ExcelIdAmountUrl = "http://crm.vancl.com:8001/public/excel/idamount.xls";
		/// <summary>
		/// ������������Excelģ��URL
		/// </summary>
		public const string ExcelIdAmountPriceUrl = "http://crm.vancl.com:8001/public/excel/idamountprice.xls";

		///<summary>
		/// ��ǰ��½����
		///</summary>
		private static string CurrentServerConnection
		{
			get { return ConfigurationManager.AppSettings["serverConnection"].Trim(); }
		}

		///<summary>
		/// ��ǰ��½����
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
        /// ��ǰ��½Vancl����Vancljia�ľ���ֿ�
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