using System.Transactions;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using System.Data.SqlClient;
using RFD.FMS.Util.Context;
using System.Data;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	/// <summary>
	/// 连接被延迟到第一次访问CurrentConnection时候打开，但是必须在内嵌的
	/// <see cref="AdoNetUnitOfWork"/>中的<see cref="TransactionScope"/>被创建
	/// 以前打开，不然就会造成事务状态无效。
	/// </summary>
    public sealed class AdoNetUnitOfWork : UnitOfWorkBase<SqlConnection>
	{
		private readonly TransactionScope scope;

		/// <summary>
		/// 构造函数，使用工厂创建<see cref="TransactionScope"/>的目的是为了管理
		/// 创建的时机。
		/// </summary>
		/// <remarks>该UnitOfWork所管理的链接使用的连接字符串是由<see cref="ConnectionStrings.ConnStringOfSCM"/>取得的。</remarks>
		/// <param name="factory">用于创建<see cref="TransactionScope"/>的工厂</param>
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
		/// 完成所有的任务，提交所有的结果
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