using System;
using System.Transactions;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet.Exceptions;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	public class AdoNetUnitOfWorkFactory : IUnitOfWorkFactory
	{
        public IUnitOfWork GetInstance(IUnitOfWorkDefinition definition)
        {
            return new AdoNetUnitOfWork(new DefaultTransactionScopeFactory());
        }
	}
}