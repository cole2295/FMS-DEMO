using System;
using System.Transactions;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	public interface ITransactionScopeFactory
	{
		TransactionScope GetInstance();
	}

	public class TransactionScopeFactory : ITransactionScopeFactory
	{
		private readonly IUnitOfWorkDefinition unitOfWorkDefinition;

		public TransactionScopeFactory(IUnitOfWorkDefinition unitOfWorkDefinition)
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