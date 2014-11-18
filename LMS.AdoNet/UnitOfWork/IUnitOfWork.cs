using System;
using System.Transactions;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	/// <summary>
	/// IUnitOfWork�ڲ�����ʹ��SqlTransaction���������Rollback����û��Commit�����
	/// IUnitOfWork��<see cref="Complete"/>��<see cref="IDisposable.Dispose"/>�׳�
	/// <see cref="TransactionAbortedException"/>��
	/// </summary>
	public interface IUnitOfWork : IDisposable
	{
		void Complete();
	}
}