using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data;
using System;

namespace RFD.FMS.Util.ControlHelper
{
	public static class DropDownListExtension
	{
		public static void InitDropDownListWithDefault(this DropDownList ddl)
		{
			ddl.Items.Clear();
			ddl.AppendDataBoundItems = true;
			ddl.Items.Add(new ListItem("请选择", ""));
		}
	}


	public static class DropDownListHelper
	{
		/// <summary>
		/// 绑定dropdownlist的方法
		/// </summary>
		/// <param name="drp">控件ID</param>
		/// <param name="dt">绑定数据源</param>
		/// <param name="text">DataTextField</param>
		/// <param name="value">DataValueField</param>
		public static void DropDownListBind(DropDownList drp, DataTable dt, string text, string value)
		{
			drp.DataSource = dt;
			drp.DataTextField = text;
			drp.DataValueField = value;
			drp.DataBind();
			drp.Items.Insert(0, new ListItem("---请选择---", ""));
		}

		/// <summary>
		/// 给dropdownlist设置选项
		/// </summary>
		/// <param name="drp"></param>
		/// <param name="list"></param>
		public static void DropDownListBind(DropDownList drp, List<ListItem> list)
		{
			foreach (var listItem in list)
			{
				drp.Items.Add(listItem);
			}
			drp.Items.Insert(0, new ListItem("---请选择---", ""));
		}

		public static int GetItemIndexByValue(string value, DropDownList dropDownList)
		{
			for (int i = 0; i < dropDownList.Items.Count; i++)
			{
				if (dropDownList.Items[i].Value == value)
				{
					return i;
				}
			}

			return -1;
		}

		public static ListItem GetSelectItem(DropDownList dropDownList)
		{
			if (dropDownList.SelectedIndex == -1) return null;

			return dropDownList.SelectedItem;
		}
		/// <summary>
		/// 动态绑定数据到列表控件中,默认加载"--请选择--"
		/// </summary>
		/// <param name="control">列表控件</param>
		/// <param name="data">绑定的数据源</param>
		/// <param name="textField">列表控件显示的键</param>
		/// <param name="valueField">列表控件显示的值</param>
		/// <param name="selectedIndex">默认选中项的索引</param>
		public static void BindListData(this ListControl control, DataTable data, string textField, string valueField,string defaultItem, string defaultItemValue, int selectedIndex)
		{
            if (!data.IsEmpty())
            {
                control.DataSource = data;
                control.DataTextField = textField;
                control.DataValueField = valueField;
                control.DataBind();
            }
            if (!defaultItem.IsNullData())
            {
                control.Items.Insert(0, defaultItem);
                control.Items[0].Value = defaultItemValue;
            }
            if (selectedIndex < 0 || selectedIndex >= control.Items.Count) return;
            control.Items[selectedIndex].Selected = true;
		}
		/// <summary>
		/// 动态绑定数据到列表控件中
		/// </summary>
		/// <param name="control">列表控件</param>
		/// <param name="data">绑定的数据源</param>
		/// <param name="textField">列表控件显示的键</param>
		/// <param name="valueField">列表控件显示的值</param>
		/// <param name="defaultItemValue">默认加载选项</param>
        public static void BindListData(this ListControl control, DataTable data, string textField, string valueField, string defaultItem, string defaultItemValue)
		{
            BindListData(control, data, textField, valueField, defaultItem, defaultItemValue, 0);
		}

        /// <summary>
        /// 动态绑定数据到列表控件中
        /// </summary>
        /// <param name="control">列表控件</param>
        /// <param name="data">绑定的数据源</param>
        /// <param name="textField">列表控件显示的键</param>
        /// <param name="valueField">列表控件显示的值</param>
        /// <param name="selectedValue">默认选中项</param>
        public static void BindListData(this ListControl control, DataTable data, string textField, string valueField, int selectedValue)
        {
            if (!data.IsEmpty())
            {
                control.DataSource = data;
                control.DataTextField = textField;
                control.DataValueField = valueField;
                control.DataBind();
                var selectedIndex = GetItemIndexByValue(selectedValue.ToString(), control as DropDownList);
                if (selectedIndex < 0 || selectedIndex >= control.Items.Count) return;
                control.Items[selectedIndex].Selected = true;
            }
        }


		/// <summary>
		/// 动态绑定数据到列表控件中
		/// </summary>
		/// <param name="control">列表控件</param>
		/// <param name="data">绑定的数据源</param>
		/// <param name="textField">列表控件显示的键</param>
		/// <param name="valueField">列表控件显示的值</param>
        public static void BindListData(this ListControl control, DataTable data, string textField, string valueField, string defaultItemValue)
		{
            BindListData(control, data, textField, valueField, "--请选择--", defaultItemValue, 0);
		}
		/// <summary>
		/// 将当前的数据字典绑定到列表控件中
		/// </summary>
		/// <typeparam name="K">数据字典的键</typeparam>
		/// <typeparam name="V">数据字典的值</typeparam>
		/// <param name="control">列表控件</param>
		/// <param name="data">数据字典</param>
		/// <param name="defaultItemValue">默认加载选项</param>
		/// <param name="selectedIndex">默认选中项的索引</param>
		public static void BindListData<K, V>(this ListControl control, IDictionary<K, V> data, string defaultItemValue, int selectedIndex)
		{
			if (data != null && data.Count > 0)
			{
				foreach (KeyValuePair<K, V> kvp in data)
				{
					control.Items.Add(new ListItem(kvp.Value.ToString(), kvp.Key.ToString()));
				}
				if (control.Items.Count > 0)
				{
					if (!defaultItemValue.IsNullData())
					{
						control.Items.Insert(0, defaultItemValue);
						control.Items[0].Value = string.Empty;
					}
					if (selectedIndex < 0 || selectedIndex >= control.Items.Count) return;
					control.Items[selectedIndex].Selected = true;
				}
			}
		}
		/// <summary>
		/// 将当前的数据字典绑定到列表控件中,默认加载"--请选择--"
		/// </summary>
		/// <typeparam name="K">数据字典的键</typeparam>
		/// <typeparam name="V">数据字典的值</typeparam>
		/// <param name="control">列表控件</param>
		/// <param name="data">数据字典</param>
		public static void BindListData<K, V>(this ListControl control, Dictionary<K, V> data, string defaultItemValue)
		{
			BindListData(control, data, defaultItemValue, 0);
		}
		/// <summary>
		/// 将当前的数据字典绑定到列表控件中,默认加载"--请选择--"
		/// </summary>
		/// <typeparam name="K">数据字典的键</typeparam>
		/// <typeparam name="V">数据字典的值</typeparam>
		/// <param name="control">列表控件</param>
		/// <param name="data">数据字典</param>
		public static void BindListData<K, V>(this ListControl control, Dictionary<K, V> data)
		{
			BindListData(control, data, "--请选择--", 0);
		}
	}
}