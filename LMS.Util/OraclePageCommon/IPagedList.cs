using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.OraclePageCommon
{
	/// <summary>
	/// 分页器接口
	/// </summary>
	/// <typeparam name="T">IPagedList<T></typeparam>
	public interface IPagedList<T>
	{
		///<summary>
		/// 记录列表
		///</summary>
		List<T> ContentList { get; }

		/// <summary>
		/// 总记录数
		/// </summary>
		int RecordCount { get; }

		/// <summary>
		/// 总页数
		/// </summary>
		int PageCount { get; }
	}
	///<summary>
	/// 分页器接口(DataTable)
	///</summary>
	public interface IPagedDataTable
	{
		///<summary>
		/// 数据DataTable
		///</summary>
		DataTable ContentData { get; }

		/// <summary>
		/// 总记录数
		/// </summary>
		int RecordCount { get; }

		/// <summary>
		/// 总页数
		/// </summary>
		int PageCount { get; }
	}
}
