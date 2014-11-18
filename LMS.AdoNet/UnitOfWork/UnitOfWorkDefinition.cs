using System;
using System.Data;
using RFD.FMS.Util;

namespace RFD.FMS.AdoNet.UnitOfWork
{
	[AttributeUsage(AttributeTargets.Method)]
	public class UnitOfWorkDefinition : Attribute, IUnitOfWorkDefinition
	{
		/// <summary>
		/// Default value:
		/// <table>
		/// <th><td>Property</td><td>Value</td></th>
		/// <tr><td>TransactionIsolationLevel</td><td><see cref="IsolationLevel.ReadCommitted"/></td></tr>
		/// <tr><td>TransactionTimeout</td><td>30</td></tr>
		/// <tr><td>ReadOnly</td><td><code>false</code></td></tr>
		/// <tr><td>Name</td><td><code>null</code></td></tr>
		/// <tr><td>System</td><td><see cref="DatabaseSource.Unspecified"/></td></tr>
		/// </table>
		/// </summary>
		public static IUnitOfWorkDefinition DefaultDefinition
		{
			get { return new DefaultAttribute(); }
		}

		public static IUnitOfWorkDefinition ExcludedDefinition
		{
			get { return new ExcludedAttribute(); }
		}

		public static IUnitOfWorkDefinition ReadOnlyDefinition
		{
			get { return new ReadOnlyAttribute(); }
		}

		public static IUnitOfWorkDefinition ReadOnlyDbDefinition
		{
			get { return new ReadOnlyDbAttribute(); }
		}

		#region IUnitOfWorkDefinition Members

		public System.Transactions.IsolationLevel TransactionIsolationLevel { get; set; }

		public int TransactionTimeout { get; set; }

		public bool ReadOnly { get; set; }

		public string Name { get; set; }

		public DatabaseSource? System { get; set; }

		public bool Exclude { get; set; }

		#endregion

		public override string ToString()
		{
			return string.Format("{0}:({1})",
			                     base.ToString(),
			                     new JsonObjectFormatter().Format(null, this, null));
		}

		#region Nested type: DefaultAttribute

		public class DefaultAttribute : UnitOfWorkDefinition
		{
			public DefaultAttribute()
			{
				TransactionIsolationLevel = global::System.Transactions.IsolationLevel.ReadCommitted;
				TransactionTimeout = 60;
				ReadOnly = false;
				Name = null;
				System = DatabaseSource.LMS_RFD;
				Exclude = false;
			}
		}

		#endregion

		#region Nested type: ExcludedAttribute

		public class ExcludedAttribute : DefaultAttribute
		{
			public ExcludedAttribute()
			{
				Exclude = true;
				System = null;
			}

			public ExcludedAttribute(DatabaseSource system)
				: this()
			{
				System = system;
			}
		}

		#endregion

		#region Nested type: ReadOnlyAttribute

		public class ReadOnlyAttribute : DefaultAttribute
		{
			public ReadOnlyAttribute()
			{
				ReadOnly = true;
				TransactionIsolationLevel = global::System.Transactions.IsolationLevel.ReadUncommitted;
			}
		}

		#endregion

		#region Nested type: ReadOnlyDbAttribute

		/// <summary>
		/// 
		/// </summary>
		public class ReadOnlyDbAttribute : ReadOnlyAttribute
		{
			public ReadOnlyDbAttribute()
			{
				System = DatabaseSource.UnspecifiedReadOnly;
			}
		}

		#endregion
	}
}