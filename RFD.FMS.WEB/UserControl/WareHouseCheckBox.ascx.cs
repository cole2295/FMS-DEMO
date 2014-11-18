using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RFD.FMS.WEBLOGIC;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.UserControl
{
	public partial class WareHouseCheckBox : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
				LoadData();
		}

		private void LoadData()
		{
			if (WareHouseCheckBoxs.Items.Count <= 0)
			{
				WareHouseCheckBoxs.DataSource = GetSource();
				WareHouseCheckBoxs.DataBind();
			}
		}

		private DataTable GetSource()
		{
            IWareHouseService wareHouseService = ServiceLocator.GetService<IWareHouseService>();
			DataTable dt;
			switch (_loadDataType)
			{
				case EnumLoadType.All:
                    dt = wareHouseService.GetWareHouseSortCenter(DistributionCode);
					break;
				case EnumLoadType.SortCenter:
                    dt = wareHouseService.GetSortCenter(DistributionCode);
					break;
				case EnumLoadType.WareHouse:
                    dt = wareHouseService.GetWareHouse();
					break;
				default:
                    dt = wareHouseService.GetWareHouseSortCenter(DistributionCode);
					break;
			}
			return dt;
		}

        private string _distributionCode;
        public string DistributionCode
        {
            get
            {
                return _distributionCode;
            }
            set
            {
                _distributionCode = value;
            }
        }

		/// <summary>
		/// 选中的仓库ID
		/// </summary>
		public string SelectWareHouseIds
		{
			get
			{
				return WareHouseCheckBoxs.SelectedValuesToString().Replace(" ","");
			}
			set
			{
				string[] ids = value.Split(',');
				WareHouseCheckBoxs.Items.Clear();
				LoadData();
				foreach (ListItem item in WareHouseCheckBoxs.Items)
				{
					foreach (string id in ids)
					{
						if (item.Value == id)
							item.Selected = true;
					}
				}
			}
		}

		/// <summary>
		/// 选中的仓库名称
		/// </summary>
		public string SelectWareHouseNames
		{
			get
			{
				return WareHouseCheckBoxs.SelectedLabelsToString();
			}
		}

		public bool SelectEnable
		{
			get
			{
				return WareHouseCheckBoxs.Enabled;
			}
			set
			{
				WareHouseCheckBoxs.Enabled = value;
				cbCheckAllWareHouse.Enabled = value;
			}
		}

		protected void cbCheckAllWareHouse_CheckedChanged(object sender, EventArgs e)
		{
			LoadData();
            if (string.IsNullOrEmpty(_houseCheckType.ToString()) || _houseCheckType == EnumHouseCheckType.Distinguish)
            {
                foreach (ListItem item in WareHouseCheckBoxs.Items)
                {
                    if (item.Value != "0201" &&
                        item.Value != "0202" &&
                        item.Value != "0208" &&
                        item.Value != "0209")
                    {
                        item.Selected = cbCheckAllWareHouse.Checked;
                    }
                    else
                    {
                        item.Selected = !cbCheckAllWareHouse.Checked;
                    }
                }
            }
            else
            {
                foreach (ListItem item in WareHouseCheckBoxs.Items)
                {
                    item.Selected = cbCheckAllWareHouse.Checked;
                }
            }
		}

		protected EnumLoadType _loadDataType;
		public EnumLoadType LoadDataType
		{
			get { return _loadDataType; }
			set { _loadDataType = value; }
		}

        protected EnumHouseCheckType _houseCheckType;
        public EnumHouseCheckType HouseCheckType
        {
            get { return _houseCheckType; }
            set { _houseCheckType = value; }
        }
	}

	public enum EnumLoadType
	{
		All=0,
		WareHouse=1,
		SortCenter=2
	}

    /// <summary>
    /// 选择仓库类型
    /// </summary>
    public enum EnumHouseCheckType
    {
        
        Distinguish=1,
        all = 0
    }
}