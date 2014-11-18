namespace RFD.FMS.AdoNet.DbBase
{
	/// <summary>
	/// ���ݿ���ʶ���
	/// </summary>
	public class DatabaseModel
	{
		/// <summary>
		/// ���ݿ�ö��ֵ
		/// </summary>
		public DatabaseSource DatabaseSource { get; set;}

		/// <summary>
		/// �ֿ�Id
		/// </summary>
		public string WarehouseId { get; set; }

		/// <summary>
		/// ���ݿ�����
		/// </summary>
		public string DatabaseName { get; set; }

		/// <summary>
		/// ���ݿ��Ӧ��Cookieֵ���ֳ�ϵͳ�ã�
		/// </summary>
		public string WebCookieValue { get; set; }

		/// <summary>
		/// �����ַ���
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// ���ݿ����ͣ�ֻ����ִ��
		/// </summary>
		public DatabaseType DatabaseType { get; set; }

		/// <summary>
		/// ��˾
		/// </summary>
		public CompanySource CompanySource { get; set; }
	}
}