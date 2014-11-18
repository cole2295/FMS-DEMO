using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RFD.FMS.Util;
using System.Text;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.UserControl
{
	public partial class StationCheckBox : System.Web.UI.UserControl
	{
        IExpressCompanyService expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadData(true);
			}
		}

		public void LoadData(bool loadAgain)
		{
			if (loadAgain || ExpressCompanyCheckBox.Items.Count <= 0)
			{
                ExpressCompanyCheckBox.DataSource = GetSource();
				ExpressCompanyCheckBox.DataBind();
			}
		}

		private DataTable GetSource()
		{
			DataTable dt;
			switch (_loadDataType)
			{
				case LoadDataType.ThirdCompany:
                    dt = expressCompanyService.GetThirdCompanyList(DistributionCode).Tables[0];
					break;
				case LoadDataType.RFDSite:
                    dt = expressCompanyService.GetRFDSiteList(DistributionCode).Tables[0];
					break;
				case LoadDataType.RFDSortCenter:
                    dt = expressCompanyService.GetRFDSortCenterList(DistributionCode).Tables[0];
					break;
                case LoadDataType.All:
                    dt = expressCompanyService.GetThirdCompanyList(DistributionCode).Tables[0];
                    DataRow row =dt.NewRow();
                    row["ExpressCompanyID"]="11";
                    row["CompanyName"] = "北京如风达";
			        dt.Rows.Add(row);
                    break;
				default:
					dt = new DataTable();
					break;
			}
			return dt;
		}

		protected LoadDataType _loadDataType;
		public LoadDataType LoadDataType
		{
			get { return _loadDataType; }
			set { _loadDataType = value; }
		}

		public string SelectExpressCompany
		{
			get
			{
				return ExpressCompanyCheckBox.SelectedValuesToString().Replace(" ","");
			}
		}

        public bool IsSelectAll
        {
            get
            {
                bool flag = false;
                string[] selectCompany = SelectExpressCompany.Split(',');
                if (selectCompany.Length == ExpressCompanyCheckBox.Items.Count)
                {
                    flag = true;
                }

                return flag;
            }
        }

		public string SelectExpressCompanyChildrens
		{
			get
			{
				string sortCenterId = ExpressCompanyCheckBox.SelectedValuesToString().Replace(" ","");
				if (string.IsNullOrEmpty(sortCenterId))
					return "";
                DataTable dt = expressCompanyService.GetExpressBySortCenterID(sortCenterId);
				if (dt == null || dt.Rows.Count <= 0)
					return "";

				StringBuilder sb = new StringBuilder();
				foreach (DataRow dr in dt.Rows)
				{
					sb.Append(dr["ExpressCompanyID"].ToString() + ",");
				}
				return sb.ToString().TrimEnd(',');
			}
		}

		protected void cbCheckAllExpressCompany_CheckedChanged(object sender, EventArgs e)
		{
			LoadData(false);
			foreach (ListItem item in ExpressCompanyCheckBox.Items)
			{
				item.Selected = cbCheckAllExpressCompany.Checked;
			}
		}

		public CheckBox CheckAllExpressCompany
		{
			get { return cbCheckAllExpressCompany; }
		}

        private string _distributionCode;
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
	}

	public enum LoadDataType
	{
		ThirdCompany=0,
		RFDSite=1,
		RFDSortCenter=2,
        All =3
	}
}