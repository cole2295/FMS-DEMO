using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.WEB.Main
{
	/// <summary>
	/// 所有页面的查询接口
	/// </summary>
	public interface ISearchable
	{
		/// <summary>
		/// 获取所有的查询条件
		/// </summary>
		/// <returns></returns>
		IDictionary<string,object> GetSearchModel();

		/// <summary>
		/// 根据查询条件获取记录总数
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		int GetSearchCount(IDictionary<string, object> model);

		/// <summary>
		/// 根据查询条件获取查询结果
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		object GetSearchData(IDictionary<string, object> model);

		/// <summary>
		/// 将查询结果绑定到数据控件上
		/// </summary>
		void BindData();
	}
}
