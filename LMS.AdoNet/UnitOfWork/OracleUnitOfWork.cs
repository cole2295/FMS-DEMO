using System.Transactions;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Oracle.DataAccess.Client;
using RFD.FMS.Util.Security;
using System.Configuration;
using RFD.FMS.Util.Context;
using System.Data;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	/// <summary>
	/// ���ӱ��ӳٵ���һ�η���CurrentConnectionʱ��򿪣����Ǳ�������Ƕ��
	/// <see cref="AdoNetUnitOfWork"/>�е�<see cref="TransactionScope"/>������
	/// ��ǰ�򿪣���Ȼ�ͻ��������״̬��Ч��
	/// </summary>
    public sealed class OracleUnitOfWork : UnitOfWorkBase<OracleConnection>
	{
		private readonly TransactionScope scope;

		/// <summary>
		/// ���캯����ʹ�ù�������<see cref="TransactionScope"/>��Ŀ����Ϊ�˹���
		/// ������ʱ����
		/// </summary>
		/// <remarks>��UnitOfWork�����������ʹ�õ������ַ�������<see cref="ConnectionStrings.ConnStringOfSCM"/>ȡ�õġ�</remarks>
		/// <param name="factory">���ڴ���<see cref="TransactionScope"/>�Ĺ���</param>
		public OracleUnitOfWork(ITransactionScopeFactory factory)
		{
            Init(new UnitOfWorkDefinition
                 {
                     Exclude = false,
                     Name = "OracleUnitOfWork",
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

        #region Dao

        ///<summary>
        /// RFD���������ַ���
        ///</summary>
        public override string ConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["OracleExecuteConnString"].ToString().Trim()); }
        }

        ///<summary>
        /// RFDֻ�������ַ���
        ///</summary>
        public override string ReadOnlyConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["OracleRedOnlyConnString"].ToString().Trim()); }
        }

        public override OracleConnection GetConnection(string connstr)
        {
            OracleConnection connection = NeutralContext.Get(RFD.FMS.AdoNet.Constants.NcUnitofworkTransactionConnectionThread) as OracleConnection;

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = new OracleConnection(connstr);
            }

            return connection;
        }

        #endregion
    }
}