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
			ddl.Items.Add(new ListItem("��ѡ��", ""));
		}
	}


	public static class DropDownListHelper
	{
		/// <summary>
		/// ��dropdownlist�ķ���
		/// </summary>
		/// <param name="drp">�ؼ�ID</param>
		/// <param name="dt">������Դ</param>
		/// <param name="text">DataTextField</param>
		/// <param name="value">DataValueField</param>
		public static void DropDownListBind(DropDownList drp, DataTable dt, string text, string value)
		{
			drp.DataSource = dt;
			drp.DataTextField = text;
			drp.DataValueField = value;
			drp.DataBind();
			drp.Items.Insert(0, new ListItem("---��ѡ��---", ""));
		}

		/// <summary>
		/// ��dropdownlist����ѡ��
		/// </summary>
		/// <param name="drp"></param>
		/// <param name="list"></param>
		public static void DropDownListBind(DropDownList drp, List<ListItem> list)
		{
			foreach (var listItem in list)
			{
				drp.Items.Add(listItem);
			}
			drp.Items.Insert(0, new ListItem("---��ѡ��---", ""));
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
		/// ��̬�����ݵ��б�ؼ���,Ĭ�ϼ���"--��ѡ��--"
		/// </summary>
		/// <param name="control">�б�ؼ�</param>
		/// <param name="data">�󶨵�����Դ</param>
		/// <param name="textField">�б�ؼ���ʾ�ļ�</param>
		/// <param name="valueField">�б�ؼ���ʾ��ֵ</param>
		/// <param name="selectedIndex">Ĭ��ѡ���������</param>
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
		/// ��̬�����ݵ��б�ؼ���
		/// </summary>
		/// <param name="control">�б�ؼ�</param>
		/// <param name="data">�󶨵�����Դ</param>
		/// <param name="textField">�б�ؼ���ʾ�ļ�</param>
		/// <param name="valueField">�б�ؼ���ʾ��ֵ</param>
		/// <param name="defaultItemValue">Ĭ�ϼ���ѡ��</param>
        public static void BindListData(this ListControl control, DataTable data, string textField, string valueField, string defaultItem, string defaultItemValue)
		{
            BindListData(control, data, textField, valueField, defaultItem, defaultItemValue, 0);
		}

        /// <summary>
        /// ��̬�����ݵ��б�ؼ���
        /// </summary>
        /// <param name="control">�б�ؼ�</param>
        /// <param name="data">�󶨵�����Դ</param>
        /// <param name="textField">�б�ؼ���ʾ�ļ�</param>
        /// <param name="valueField">�б�ؼ���ʾ��ֵ</param>
        /// <param name="selectedValue">Ĭ��ѡ����</param>
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
		/// ��̬�����ݵ��б�ؼ���
		/// </summary>
		/// <param name="control">�б�ؼ�</param>
		/// <param name="data">�󶨵�����Դ</param>
		/// <param name="textField">�б�ؼ���ʾ�ļ�</param>
		/// <param name="valueField">�б�ؼ���ʾ��ֵ</param>
        public static void BindListData(this ListControl control, DataTable data, string textField, string valueField, string defaultItemValue)
		{
            BindListData(control, data, textField, valueField, "--��ѡ��--", defaultItemValue, 0);
		}
		/// <summary>
		/// ����ǰ�������ֵ�󶨵��б�ؼ���
		/// </summary>
		/// <typeparam name="K">�����ֵ�ļ�</typeparam>
		/// <typeparam name="V">�����ֵ��ֵ</typeparam>
		/// <param name="control">�б�ؼ�</param>
		/// <param name="data">�����ֵ�</param>
		/// <param name="defaultItemValue">Ĭ�ϼ���ѡ��</param>
		/// <param name="selectedIndex">Ĭ��ѡ���������</param>
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
		/// ����ǰ�������ֵ�󶨵��б�ؼ���,Ĭ�ϼ���"--��ѡ��--"
		/// </summary>
		/// <typeparam name="K">�����ֵ�ļ�</typeparam>
		/// <typeparam name="V">�����ֵ��ֵ</typeparam>
		/// <param name="control">�б�ؼ�</param>
		/// <param name="data">�����ֵ�</param>
		public static void BindListData<K, V>(this ListControl control, Dictionary<K, V> data, string defaultItemValue)
		{
			BindListData(control, data, defaultItemValue, 0);
		}
		/// <summary>
		/// ����ǰ�������ֵ�󶨵��б�ؼ���,Ĭ�ϼ���"--��ѡ��--"
		/// </summary>
		/// <typeparam name="K">�����ֵ�ļ�</typeparam>
		/// <typeparam name="V">�����ֵ��ֵ</typeparam>
		/// <param name="control">�б�ؼ�</param>
		/// <param name="data">�����ֵ�</param>
		public static void BindListData<K, V>(this ListControl control, Dictionary<K, V> data)
		{
			BindListData(control, data, "--��ѡ��--", 0);
		}
	}
}