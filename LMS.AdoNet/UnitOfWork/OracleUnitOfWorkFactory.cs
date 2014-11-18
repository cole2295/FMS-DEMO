using System;
using System.Transactions;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet.Exceptions;

namespace RFD.FMS.AdoNet.UnitOfWork
{
    public class OracleUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork GetInstance(IUnitOfWorkDefinition definition)
        {
            return new OracleUnitOfWork(new DefaultTransactionScopeFactory());
        }
    }
}