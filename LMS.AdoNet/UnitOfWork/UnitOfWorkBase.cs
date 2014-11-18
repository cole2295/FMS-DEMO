using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Common.Logging;
using RFD.FMS.Util.Context;
using RFD.FMS.Util.Debug;
using RFD.FMS.AdoNet.DbBase;
using System.Data.Common;
using System.Configuration;

namespace RFD.FMS.AdoNet.UnitOfWork
{
    public abstract class UnitOfWorkBase<T> : DaoBase<T>, IUnitOfWork
	{
		/// <summary>
		/// 记录事务日志
		/// </summary>
		protected static readonly ILog Logger = LogManager.GetLogger(typeof (UnitOfWorkBase<T>));

		/// <summary>
		/// 是否最外层事务，默认为true
		/// </summary>
		protected bool IsOuterMostUnitOfWork = true;

        protected DbConnection UnitConnection;

		/// <summary>
		/// 保存嵌套事务的栈
		/// </summary>
		private Stack<IUnitOfWork> threadBoundedStack;

		protected Stack<IUnitOfWork> ThreadBoundedStack
		{
			get
			{
				if (threadBoundedStack == null)
				{
					threadBoundedStack = NeutralContext.Get(Constants.NcUnitofworkStackThread) as Stack<IUnitOfWork>;
					if (threadBoundedStack == null)
					{
						threadBoundedStack = new Stack<IUnitOfWork>();
						NeutralContext.Put(Constants.NcUnitofworkStackThread, threadBoundedStack);
					}
				}
				return threadBoundedStack;
			}
		}

		/// <summary>
		/// 事务定义
		/// </summary>
		public IUnitOfWorkDefinition Definition { get; private set; }

		#region IUnitOfWork Members

		public virtual void Dispose()
		{
			Assert.State(this == ThreadBoundedStack.Pop(),
			             "Disposing unit of work must on top of the ThreadBoundedStack.");
			if (IsOuterMostUnitOfWork)
			{
				Logger.Debug(
					"Outer most unit of work disposing, disposing connection, set connection and stack in context null.");
                UnitConnection.Dispose();
				NeutralContext.Remove(Constants.NcUnitofworkTransactionConnectionThread);
				NeutralContext.Remove(Constants.NcUnitofworkStackThread);
			}
			else
			{
				Logger.Debug(m => m("Inner unit of work(deepth:{0}) disposing, keep connection in context.",
				                    ThreadBoundedStack.Count + 1));
			}
		}

		public virtual void Complete()
		{
			Logger.Debug("Unit Of Work Complete here.");
		}

		#endregion

		protected virtual void Init(IUnitOfWorkDefinition definition)
		{
            Definition = definition;
            UnitConnection = (DbConnection)NeutralContext.Get(Constants.NcUnitofworkTransactionConnectionThread);
            ThreadBoundedStack.Push(this);
            if (UnitConnection != null)
            {
                IsOuterMostUnitOfWork = false;
                Logger.Debug(m => m("Inner(deepth:{0}) unit of work initiates with definition({1})!",
                                    ThreadBoundedStack.Count, definition));
            }
            else
            {
                Logger.Debug(m => m("Outer most unit of work initiates with definition({0})!", definition));
                IsOuterMostUnitOfWork = true;

                UnitConnection = Connection as DbConnection;

                //Logger.Debug(m => m("Connection of unit of work created(connStr:{0})!", connectionString));
                NeutralContext.Put(Constants.NcUnitofworkTransactionConnectionThread, UnitConnection);
            }
            LastChanceOpenConnForInnerUnitOfWorkBeforeInnerScopeCreated();
		}

		private void LastChanceOpenConnForInnerUnitOfWorkBeforeInnerScopeCreated()
		{
            if (!IsOuterMostUnitOfWork && UnitConnection.State != ConnectionState.Open)
			{
				Logger.Debug(m => m("Last chance open conn for inner UnitOfWork before inner scope created!"));
                UnitConnection.Open();
			}
		}
	}
}