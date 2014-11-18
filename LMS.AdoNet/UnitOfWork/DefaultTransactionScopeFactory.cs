using System;
using System.Transactions;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet.Exceptions;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	public class DefaultTransactionScopeFactory : ITransactionScopeFactory
	{
		private readonly IUnitOfWorkDefinition unitOfWorkDefinition;

		public DefaultTransactionScopeFactory()
		{
		}

		public DefaultTransactionScopeFactory(IUnitOfWorkDefinition unitOfWorkDefinition)
		{
			this.unitOfWorkDefinition = unitOfWorkDefinition;
		}

		#region ITransactionScopeFactory Members

		public TransactionScope GetInstance()
		{
			TransactionOptions transactionToUse;
			if (unitOfWorkDefinition == null)
			{
				transactionToUse = new TransactionOptions
				                   	{
				                   		IsolationLevel = IsolationLevel.ReadCommitted,
				                   		Timeout = new TimeSpan(0, 3, 0)
				                   	};
			}
			else
			{
				transactionToUse = new TransactionOptions
				                   	{
				                   		IsolationLevel = unitOfWorkDefinition.TransactionIsolationLevel,
				                   		Timeout = new TimeSpan(0, 0, unitOfWorkDefinition.TransactionTimeout)
				                   	};
			}

			return new TransactionScope(TransactionScopeOption.Required, transactionToUse);
		}

		#endregion
	}
}