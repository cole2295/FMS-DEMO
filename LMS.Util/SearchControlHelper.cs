using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util
{
	/// <summary>
	/// 搜索组件帮助类-韩冰
	/// </summary>
	public class SearchControlHelper
	{
		public static SearchControlResult GetSearchResult(SearchData sd)
		{
			int pramindex = 0;

			SearchControlResult sr = new SearchControlResult();

			List<SqlParameter> pram = new List<SqlParameter>();

			foreach (var item in sd.Items)
			{
                if ((pram.Count != 0 && item.ralation == "-1") || (string.IsNullOrEmpty(item.v) || (item.v.Trim() == "-1") || (string.IsNullOrEmpty(item.col)) || (item.v.Trim() == "%"))) continue;

				string sql = string.Format(" {2} {0} {1} {3}", item.col, item.op, item.ralation == "-1" ? "and" : item.ralation, "@pram" + pramindex.ToString());

				sr.SqlStr += sql;

				pram.Add(new SqlParameter("@pram" + pramindex.ToString(), item.v));

				pramindex++;
			}

			sr.Prams = pram;

			return sr;
		}
	}

	public class SearchControlResult
	{
		public string SqlStr = "";
		public List<SqlParameter> Prams = null;
		public string OrderBy = "";

	}

	[Serializable]
	public class SearchData
	{
        private IList<SarchItem> items;

        public IList<SarchItem> Items
        {
            get 
            {
                if (items == null)
                {
                    items = new List<SarchItem>();
                }

                return items; 
            }
            set { items = value; }
        }
	}
	[Serializable]
	public class SarchItem
	{
		public SarchItem()
		{

		}
		public SarchItem(string col, string op, string v, string ralation)
		{
			this.col = col;
			this.op = op;
			this.v = v;
			this.ralation = ralation;
		}
		/// <summary>
		/// 列名
		/// </summary>
		public string col;
		/// <summary>
		/// 运算符
		/// </summary>
		public string op = "=";
		/// <summary>
		/// 值
		/// </summary>
		public string v;
		/// <summary>
		/// 条件(and or)
		/// </summary>
		public string ralation = "and";

	}




}
