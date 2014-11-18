using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.OraclePageCommon
{
	/// <summary>
	/// 分页数据类
	/// </summary>
	public class PaginatorDTO
	{

		///<summary>
		///分页尺寸
		///</summary>
		public int PageSize { get; set; }

		///<summary>
		///当前页码
		///</summary>
		public int PageNo { get; set; }

        ///<summary>
        ///总记录数
        ///</summary>
        public int ItemCount { get; set; }

        ///<summary>
        ///总页数
        ///</summary>
        public int Pages { get; set; }
	}
}
