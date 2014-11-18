using System.ComponentModel;
using RFD.FMS.AdoNet.UnitOfWork;

namespace RFD.FMS.AdoNet
{
	/// <summary>
	/// ��˾
	/// </summary>
	public enum CompanySource
	{
		/// <summary>
		/// ����
		/// </summary>
		Vancl = 1,
		/// <summary>
		/// VJIA
		/// </summary>
		Vjia = 2,
	}

	/// <summary>
	/// ���ݿ�����
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// ִ��Ȩ��
		/// </summary>
		Execute = 1,
		/// <summary>
		/// ֻ��Ȩ��
		/// </summary>
		Readonly = 2,
	}

	/// <summary>
	/// ���ݿ�Դ������ָ��һ��<see cref="IUnitOfWork"/>�����ݿ⡣
	/// </summary>
	public enum DatabaseSource
	{
		/// <summary>
		/// δָ�������ݵ�ǰ��¼Ա���������ӵ����ݿ⡣
		/// </summary>
		[Description("Unspecified")]
		Unspecified,
		/// <summary>
		/// SCM��
		/// </summary>
		SCM,
		/// <summary>
		/// ʹ��Vancl�����ݿ�
		/// </summary>
		[Description("SCM")]
		Vancl,
		/// <summary>
		/// ʹ��Vjia�����ݿ�
		/// </summary>
		[Description("SCM_VJIA")]
		Vjia,
		/// <summary>
		/// �������ݿ�
		/// </summary>
		[Description("Order")]
		Order,
		/// <summary>
		/// �ͻ����ݿ�
		/// </summary>
		[Description("Customer")]
		Customer,
		/// <summary>
		/// �������ݿ�
		/// </summary>
		[Description("FanKu")]
		FanKu,
		/// <summary>
		/// ʹ�õ�ǰ��½�ֿ�����ݿ�
		/// </summary>
		[Description("WMS")]
		WMS,
		/// <summary>
		/// ���ݲֿ����ݿ�
		/// </summary>
		[Description("WMS_GZ")]
		WMS_GZ,
		/// <summary>
		/// ���ݻ�ױƷ��
		/// </summary>
		[Description("WMS_GZC")]
		WMS_GZC,
		/// <summary>
		/// �����ֿ����ݿ�
		/// </summary>
		[Description("WMS_BJ")]
		WMS_BJ,
		/// <summary>
		/// ������ױƷ��
		/// </summary>
		[Description("WMS_BJC")]
		WMS_BJC,
		///<summary>
		/// �Ϻ��ֿ����ݿ�
		///</summary>
		[Description("WMS_SH")]
		WMS_SH,
		/// <summary>
		/// �Ϻ���ױƷ��
		/// </summary>
		[Description("WMS_SHC")]
		WMS_SHC,
		/// <summary>
		/// �人�ֿ����ݿ�
		/// </summary>
		[Description("WMS_WH")]
		WMS_WH,
		/// <summary>
		/// �ɶ��ֿ����ݿ�
		/// </summary>
		[Description("WMS_CD")]
		WMS_CD,
		/// <summary>
		/// �������ݿ�
		/// </summary>
		[Description("WMS_XA")]
		WMS_XA,
		/// <summary>
		/// WMS Vjia ʵ����WMS vjia bj
		/// </summary>
		[Description("WMS_Vjia")]
		WMS_Vjia,
		/// <summary>
		/// δָ�������ݵ�ǰ��¼Ա���������ӵ�ֻ�����ݿ⡣
		/// </summary>
		[Description("UnspecifiedReadOnly")]
		UnspecifiedReadOnly,
        /// <summary>
        /// ����ϵͳ��
        /// </summary>
        [Description("LMS_RFD")]
        LMS_RFD
	}

	///<summary>
	/// ���ݿ�Դ������ָ����ǰ��½�Ĳֿ�
	///</summary>
	public enum WMSDatabaseSource
	{
		///<summary>
		/// δָ�������ݵ�ǰ��¼Ա���������ӵ�WMS���ݿ⡣
		///</summary>
		[Description("Unspecified")]
		Unspecified,
		///<summary>
		/// ʹ�ù��ݵ����ݿ�
		///</summary>
		[Description("WMS_GZ")]
		WMS_GZ = 1,
		/// <summary>
		/// ���ݻ�ױƷ��
		/// </summary>
		[Description("WMS_GZC")]
		WMS_GZC,
		///<summary>
		/// ʹ�ñ�����WMS���ݿ�
		///</summary>
		[Description("WMS_BJ")]
		WMS_BJ,
		/// <summary>
		/// ������ױƷ��
		/// </summary>
		[Description("WMS_BJC")]
		WMS_BJC,
		///<summary>
		/// ʹ���Ϻ���WMS���ݿ�
		///</summary>
		[Description("WMS_SH")]
		WMS_SH,
		/// <summary>
		/// �Ϻ���ױƷ��
		/// </summary>
		[Description("WMS_SHC")]
		WMS_SHC,
		///<summary>
		/// �人�ֿ�
		///</summary>
		[Description("WMS_WH")]
		WMS_WH,
		///<summary>
		/// �ɶ��ֿ�
		///</summary>
		[Description("WMS_CD")]
		WMS_CD,
		/// <summary>
		/// �������ݿ�
		/// </summary>
		[Description("WMS_XA")]
		WMS_XA,
		/// <summary>
		/// WMS Vjia
		/// </summary>
		[Description("WMS_Vjia")]
		WMS_Vjia,
		/// <summary>
		/// ���п����Ĳֿ�
		/// </summary>
		WMS_ForOrder,
	}
}