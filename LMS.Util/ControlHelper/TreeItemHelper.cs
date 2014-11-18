using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Data;

namespace LMS.Util.ControlHelper
{
	public static class TreeItemHelper
	{
		public static string GetHelp(this TreeItem ti)
		{
			return @"";
		}

		/// <summary>
		/// 由DataTable 生成树类
		/// </summary>
		/// <param name="ti">要装载的树类根节点</param>
		/// <param name="dt">数据表</param>
		/// <param name="sId">父类ID</param>
		/// <returns></returns>
		public static TreeItem GetTreeItemFromDataTable(this TreeItem ti, DataTable dt, string sId)
		{
			return ti.GetTreeItemFromDataTable(dt.AsEnumerable(), sId);
		}

		/// <summary>
		/// 生成树类
		/// </summary>
		/// <param name="ti">要装载的树类根节点</param>
		/// <param name="drs">数据表</param>
		/// <param name="sId">父类ID</param>
		/// <returns></returns>
		private static TreeItem GetTreeItemFromDataTable(this TreeItem ti, EnumerableRowCollection<DataRow> drs, string sId)
		{
			ti.ChildNodes = new List<TreeItem>();
			var drs0 = drs.Where(p => p.Field<int>("parent").ToString() == sId);
			foreach (DataRow dr in drs0)
			{
				string parent = dr["Id"].ToString();
				var ti0 = new TreeItem() { id = int.Parse(parent), value = parent, text = dr["text"].ToString() };
				ti0 = ti0.GetTreeItemFromDataTable(drs, parent);
				ti.Add(ti0);
			}
			return ti;
		}
	}

	public class TreeItem
	{
		public TreeItem()
		{
			showcheck = true;
			checkstate = 0;
			complete = true;
			isexpand = false;
		}

		#region 属性
		/// <summary>
		/// 项目ID
		/// </summary>
		public int id { get; set; }
		/// <summary>
		/// 项的文本
		/// </summary>
		public string text { get; set; }
		/// <summary>
		/// 项的值
		/// </summary>
		public string value { get; set; }
		/// <summary>
		/// 是否显示选择框
		/// </summary>
		public bool showcheck { set; get; }

		/// <summary>
		/// 子节点是否展开
		/// </summary>
		public bool isexpand { set; get; }

		/// <summary>
		/// 子节点选择框的状态
		/// </summary>
		public int checkstate { get; set; }
		/// <summary>
		/// 是否有子节点
		/// </summary>
		public bool hasChildren { get { return (ChildNodes != null && ChildNodes.Count > 0); } }
		/// <summary>
		/// 是否完成（用于区分是否要异步load数据）
		/// 此处之前有点误解，以为是标注是否是某层的最后一个节点，如果是有在最后一个节点才给 true,其它的地方都是给的 true.
		/// 但在生成的树中，点击含有子树的节点时，始终显示 "loading..."，通过分析 tree.js 才知道 这个地方的真正意义，对于非异步的该字段的取值都为1
		/// </summary>
		public bool complete { get; set; }

		/// <summary>
		/// 子节点列表
		/// </summary>
		public List<TreeItem> ChildNodes { get; set; }

		#endregion 属性

		#region 方法
		/// <summary>
		/// 生成 JSON 字符串，
		/// </summary>
		/// <returns></returns>
		public string ToJsonString()
		{
			JavaScriptSerializer jss = new JavaScriptSerializer();
			string sJson = jss.Serialize(this);
			return "var treedata=[" + sJson + "];";
		}

		/// <summary>
		/// 添加一个子节点
		/// </summary>
		/// <param name="ti"></param>
		/// <returns></returns>
		public List<TreeItem> Add(TreeItem ti)
		{
			if (ChildNodes == null) ChildNodes = new List<TreeItem>();
			ChildNodes.Add(ti);
			return ChildNodes;
		}

		/// <summary>
		/// 选择相应的子节点
		/// </summary>
		/// <param name="arrId"></param>
		/// <returns></returns>
		public TreeItem SelectItem(int[] arrId)
		{
			return SelectImte(this, arrId);
		}

		private TreeItem SelectImte(TreeItem ti, int value)
		{
			ti.checkstate = 1;
			if (ti.hasChildren)
			{
				for (int i = 0; i < ti.ChildNodes.Count; i++)
				{
					SelectImte(ti.ChildNodes[i], 1);
				}
			}
			return ti;
		}

		private TreeItem SelectImte(TreeItem ti, int[] arrId)
		{
			if (arrId.Any(p => p.ToString() == ti.value))
			{
				ti.checkstate = 1;

				if (ti.hasChildren)
				{
					for (int i = 0; i < ti.ChildNodes.Count; i++)
					{
						SelectImte(ti.ChildNodes[i], 1);
					}
				}
			}
			else
			{
				if (ti.hasChildren)
				{
					for (int i = 0; i < ti.ChildNodes.Count; i++)
					{
						SelectImte(ti.ChildNodes[i], arrId);
					}

					int nCount = ti.ChildNodes.Count(p => p.checkstate == 1);
					if (nCount == 0)
					{
						ti.checkstate = 0;
					}
					else if (ti.ChildNodes.Count == nCount)
					{
						ti.checkstate = 1;
					}
					else
					{
						ti.checkstate = 2;
					}
				}
			}

			return ti;
		}

		#endregion 方法
	}
}
