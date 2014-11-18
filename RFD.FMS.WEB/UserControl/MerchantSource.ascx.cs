using System;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Service.BasicSetting;


namespace RFD.FMS.WEB.UserControl
{
	public partial class  MerchantSource : System.Web.UI.UserControl
	{
        private IMerchantService bll = ServiceLocator.GetService<IMerchantService>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
				LoadData();
		     
		}

		private void LoadData()
		{
          if(MerchantSources.Items.Count<=0)
          {
              DataTable dt;
              switch (_loadDataType)
              {
                  case LoadMerchantDataType.All:
                      dt = bll.GetAllMerchants(DistributionCode); break;
                  case LoadMerchantDataType.ThirdMerchant:
                      dt = bll.GetMerchants(DistributionCode); break;
                  case LoadMerchantDataType.ThirdMerchantNoExpress:
                      dt = bll.GetMerchantsNoExpress(DistributionCode); break;
                  default:
                      dt = bll.GetAllMerchants(DistributionCode); break;
              }
              MerchantSources.DataSource = dt;
              MerchantSources.DataBind();
          }
           
		}

        protected LoadMerchantDataType _loadDataType;
        public LoadMerchantDataType LoadDataType
        {
            get { return _loadDataType; }
            set { _loadDataType = value; }
        }

		public string SelectMerchantSourcesID
		{
			
            set 
            { 
                string id =value;
                MerchantSources.Items.Clear();
                LoadData();
                foreach (ListItem item in MerchantSources.Items)
                {
                    if(item.Value == id)
                    {
                        item.Selected = true;
                    }
                }
            }
            get
			{
                return MerchantSources.SelectedValuesToString();
			}
		}

        public bool IsSelectAll
        {
            get
            {
                bool flag = false;
                string[] selectMerchants=SelectMerchantSourcesID.Split(',');
                if (selectMerchants.Length == MerchantSources.Items.Count)
                {
                    flag = true;
                }

                return flag;
            }
        }

		public string SelectMerchantSourcesName
		{

            set
            {
                string Name = value;
                MerchantSources.Items.Clear();
                LoadData();
                foreach (ListItem item in MerchantSources.Items)
                {
                    if (item.Text == Name)
                    {
                        item.Selected = true;
                    }
                }
            }
            
            get
			{
                return MerchantSources.SelectedLabelsToString();
			}
		}

		public bool SelectEnable
		{
			get
			{
                return MerchantSources.Enabled;
			}
			set
			{
                MerchantSources.Enabled = value;
			}
		}

        private string _distributionCode;
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
	}

    public enum LoadMerchantDataType
    {
        /// <summary>
        /// 所有商家
        /// </summary>
        All= 0,
        /// <summary>
        /// 所有商家 不包含vancl、vjia
        /// </summary>
        ThirdMerchant = 1,
        /// <summary>
        /// 商家去除快递散件
        /// </summary>
        ThirdMerchantNoExpress = 2,
    }
}