using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.OraclePageCommon
{
	/// <summary>
	/// 分页器类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PagedList<T> : IPagedList<T>
	{
		///<summary>
		///</summary>
		public PagedList() { }

		///<summary>
		///</summary>
		///<param name="contents"></param>
		///<param name="recordCount"></param>
		///<param name="pageCount"></param>
		public PagedList(IEnumerable<T> contents, int recordCount, int pageCount)
		{
			ContentList = contents.ToList();
			RecordCount = recordCount;
			PageCount = pageCount;
		}

		#region IPagedList<T> Members

		public List<T> ContentList { get; set; }

		public int RecordCount { get; set; }

		public int PageCount { get; set; }

		#endregion
	}

	///<summary>
	/// 
	///</summary>
	public class PagedDataTable : IPagedDataTable
	{
		///<summary>
		/// 
		///</summary>
		public PagedDataTable() { }

		///<summary>
		///</summary>
		///<param name="content"></param>
		///<param name="recordCount"></param>
		///<param name="pageCount"></param>
		public PagedDataTable(DataTable content, int recordCount, int pageCount)
		{
			ContentData = content;
			RecordCount = recordCount;
			PageCount = pageCount;
		}

		#region IPagedDataTable Members

		public DataTable ContentData { get; set; }

		public int RecordCount { get; set; }

		public int PageCount { get; set; }

		#endregion
	}
}
