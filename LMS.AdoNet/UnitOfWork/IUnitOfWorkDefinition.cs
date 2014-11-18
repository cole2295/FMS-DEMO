using System.Transactions;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	/// <summary>
	/// Much based on TransactionDefinition of Spring.NET. But add <see cref="DatabaseSource"/>
	/// and <see cref="Exclude"/> support.
	/// </summary>
	public interface IUnitOfWorkDefinition
	{
		/// <summary>
		/// ������뼶��
		/// </summary>
		IsolationLevel TransactionIsolationLevel { get; }

		/// <summary>
		/// ����ʱʱ��
		/// </summary>
		int TransactionTimeout { get; }

		/// <summary>
		/// ��֪Nhibernate��sessionΪReadOnly
		/// </summary>
		bool ReadOnly { get; }

		/// <summary>
		/// ��������
		/// </summary>
		string Name { get; }

		/// <summary>
		/// ���ݿ�Դ
		/// </summary>
		DatabaseSource? System { get; set; }

		/// <summary>
		/// ��������Ƕ����ʽ�����Ƿ��ų��������Զ���ֱ��Using��������
		/// </summary>
		bool Exclude { get; }
	}
}