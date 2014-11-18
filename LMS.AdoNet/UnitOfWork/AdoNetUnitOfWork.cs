using System.Transactions;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using System.Data.SqlClient;
using RFD.FMS.Util.Context;
using System.Data;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	/// <summary>
	/// ���ӱ��ӳٵ���һ�η���CurrentConnectionʱ��򿪣����Ǳ�������Ƕ��
	/// <see cref="AdoNetUnitOfWork"/>�е�<see cref="TransactionScope"/>������
	/// ��ǰ�򿪣���Ȼ�ͻ��������״̬��Ч��
	/// </summary>
    public sealed class AdoNetUnitOfWork : UnitOfWorkBase<SqlConnection>
	{
		private readonly TransactionScope scope;

		/// <summary>
		/// ���캯����ʹ�ù�������<see cref="TransactionScope"/>��Ŀ����Ϊ�˹���
		/// ������ʱ����
		/// </summary>
		/// <remarks>��UnitOfWork�����������ʹ�õ������ַ�������<see cref="ConnectionStrings.ConnStringOfSCM"/>ȡ�õġ�</remarks>
		/// <param name="factory">���ڴ���<see cref="TransactionScope"/>�Ĺ���</param>
		public AdoNetUnitOfWork(ITransactionScopeFactory factory)
		{
            Init(new UnitOfWorkDefinition
                 {
                     Exclude = false,
                     Name = "AdoNetUnitOfWork",
                     ReadOnly = false,
                     TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                     TransactionTimeout = 60
                 });

            scope = factory.GetInstance();

            Transaction.Current.TransactionCompleted +=
                (o, args) => Logger.Debug(m => m("Current transaction completed with status {0}.",
                                                 args.Transaction.TransactionInformation.Status));
		}

		public override void Dispose()
		{
			base.Dispose();
			scope.Dispose();
		}

		/// <summary>
		/// ������е������ύ���еĽ��
		/// </summary>
		public override void Complete()
		{
			base.Complete();
			scope.Complete();
		}

        public override SqlConnection GetConnection(string connstr)
        {
            var connection = NeutralContext.Get(Constants.NcUnitofworkTransactionConnectionThread) as SqlConnection;

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = new SqlConnection(connstr);
            }

            return connection;
        }
	}


	public static class TranScopeFactory
	{
		public static IUnitOfWork CreateUnit()
		{
			return ServiceLocator.GetObject<IUnitOfWorkFactory>("AdoNetUnitOfWorkFactory").GetInstance(
					UnitOfWorkDefinition.DefaultDefinition);
		}

        public static IUnitOfWork CreateOracleUnit()
        {
            return ServiceLocator.GetObject<IUnitOfWorkFactory>("OracleUnitOfWorkFactory").GetInstance(
                UnitOfWorkDefinition.DefaultDefinition);
        }
	}
}