using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace RFD.FMS.Util.ControlHelper
{
	public static class DataTableHelper
	{
		public static void InitDataTable(DataTable table, int initRowCount, string idFieldName)
		{
			if (table.Rows.Count > initRowCount) return;

			DataRow dataRow = null;

			for (int i = 0; i < initRowCount - table.Rows.Count; i++)
			{
				dataRow = table.NewRow();

				dataRow[idFieldName] = -1000;

				table.Rows.Add(dataRow);
			}
		}

		public static IList<DataRow> QueryRealDataRow(DataTable dataTable, string idFieldName)
		{
			IList<DataRow> dataRowList = new List<DataRow>();

			DataRow dataRow = null;

			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataRow = dataTable.Rows[i];

				if (DataConvert.ToInt(dataRow[idFieldName]) == -1000)
				{
					dataRowList.Add(dataRow);
				}
			}

			return dataRowList;
		}

		public static void QueryRealDataTable(DataTable dataTable, string idFieldName)
		{
			DataRow dataRow = null;

			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataRow = dataTable.Rows[i];

				if (DataConvert.ToInt(dataRow[idFieldName]) == -1000)
				{
					dataTable.Rows.Remove(dataRow);

					i--;
				}
			}
		}

		/// <summary>
		/// 将当前的DataTable格式化(用于修改为中文列名)
		/// </summary>
		/// <param name="data">当前的DataTable</param>
		/// <param name="columns">需要格式化的中文列数组</param>
		/// <returns></returns>
		public static DataTable Format(this DataTable data, string[] columns)
		{
			//如果列数不相等，则不格式化
			if (data.Columns.Count != columns.Length) return data;
			for (var i = 0; i < data.Columns.Count; i++)
			{
				data.Columns[i].ColumnName = columns[i];
			}
			return data;
		}

		/// <summary>
		/// 将当前的DataTable格式化(用于移除不需要的列)
		/// </summary>
		/// <param name="data">当前的DataTable</param>
		/// <param name="columns">需要格式化的中文列数组</param>
		/// <param name="ignoreColumns">移除不需要的列</param>
		/// <returns></returns>
		public static DataTable Format(this DataTable data, string[] columns, string[] ignoreColumns)
		{
			//移除指定的列
			if (ignoreColumns != null && ignoreColumns.Length > 0)
			{
				foreach (var col in ignoreColumns)
				{
					data.Columns.Remove(col);
				}
			}
			if (columns != null)
			{
				data.Format(columns);//修改为中文表名
			}
			return data;
		}

		public static bool IsEmpty(this DataTable data)
		{
			return data == null || data.Rows.Count == 0;
		}

		/// <summary>
		/// 移除末尾的空行
		/// </summary>
		/// <param name="uploadData">上传数据</param>
		/// <returns></returns>
		public static void RemoveEmptyRow(this DataTable uploadData)
		{
			//比较上传数量与实际订单数量是否一致
			for (var i = uploadData.Rows.Count - 1; i >= 0; i--)
			{
				var dr = uploadData.Rows[i];
				if (StringUtil.IsEmptyDataRow(dr))
				{
					uploadData.Rows.RemoveAt(i);
				}
			}
		}
		/// <summary>
		/// 将当前的DataTable转化为Json的字符串表示
		/// </summary>
		/// <param name="data">当前的数据源</param>
		/// <param name="textField">当前显示的值</param>
		/// <param name="valueField">当前绑定的值</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="showDefaultItem">是否显示默认值</param>
		/// <returns></returns>
		public static string ToJsonListString(this DataTable data, string textField, string valueField, string defaultValue, bool showDefaultItem)
		{
            var defaultItem = showDefaultItem ? "{" + String.Format("\"{0}\":\"\",", defaultValue) : "{";
            if (data.IsEmpty()) return defaultItem.Remove(defaultItem.Length - 1, 1) + "}";
			
			var result = new StringBuilder(defaultItem);
			foreach (DataRow dr in data.Rows)
			{
				result.AppendFormat("\"{0}\":\"{1}\",", dr[textField], dr[valueField]);
			}
			result.Remove(result.Length - 1, 1).Append("}");
			return result.ToString();
		}
		/// <summary>
		/// 将当前的DataTable转化为Json的字符串表示(是否绑定默认值)
		/// </summary>
		/// <param name="data">当前的数据源</param>
		/// <param name="textField">当前显示的值</param>
		/// <param name="valueField">当前绑定的值</param>
		/// <param name="showDefaultItem">是否显示默认值</param>
		/// <returns></returns>
		public static string ToJsonListString(this DataTable data, string textField, string valueField, bool showDefaultItem)
		{
			return ToJsonListString(data, textField, valueField, showDefaultItem ? "--请选择--" : "", showDefaultItem);
		}
		/// <summary>
		/// 将当前的DataTable转化为Json的字符串表示(不绑定默认值)
		/// </summary>
		/// <param name="data">当前的数据源</param>
		/// <param name="textField">当前显示的值</param>
		/// <param name="valueField">当前绑定的值</param>
		/// <returns></returns>
		public static string ToJsonListString(this DataTable data, string textField, string valueField)
		{
			return ToJsonListString(data, textField, valueField, false);
		}
		/// <summary>
		/// 将当前的Dictionary转化为Json的字符串表示
		/// </summary>
		/// <param name="dict">当前的数据字典</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="showDefaultItem">是否显示默认值</param>
		/// <returns></returns>
		public static string ToJsonListString<K, V>(this Dictionary<K, V> dict, string defaultValue, bool showDefaultItem)
		{
			if (dict == null || dict.Count == 0) return string.Empty;
			var defaultItem = showDefaultItem ? "{" + String.Format("\"{0}\":\"\",", defaultValue) : "{";
			var result = new StringBuilder(defaultItem);
			foreach (KeyValuePair<K, V> kvp in dict)
			{
				result.AppendFormat("\"{0}\":\"{1}\",", kvp.Value, kvp.Key);
			}
			result.Remove(result.Length - 1, 1).Append("}");
			return result.ToString();
		}
		/// <summary>
		/// 将当前的Dictionary转化为Json的字符串表示(是否绑定默认值)
		/// </summary>
		/// <param name="dict">当前的数据字典</param>
		/// <param name="showDefaultItem">是否显示默认值</param>
		/// <returns></returns>
		public static string ToJsonListString<K, V>(this Dictionary<K, V> dict, bool showDefaultItem)
		{
			return ToJsonListString(dict, showDefaultItem ? "--请选择--" : "", showDefaultItem);
		}
		/// <summary>
		/// 将当前的Dictionary转化为Json的字符串表示(不绑定默认值)
		/// </summary>
		/// <param name="dict">当前的数据字典</param>
		/// <returns></returns>
		public static string ToJsonListString<K, V>(this Dictionary<K, V> dict)
		{
			return ToJsonListString(dict, false);
		}
	}
}
