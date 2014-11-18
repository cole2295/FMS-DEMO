using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.UserControl
{
    public partial class GoodsCategoryCheckBox : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            IGoodsCategoryService goodsCategoryService =ServiceLocator.GetService<IGoodsCategoryService>();
            DataTable dt = new DataTable();
            if (MerchantID == -1)
            {
                dt = goodsCategoryService.GetAllGoods();
            }
            else if (MerchantID > 0)
            {
                dt = goodsCategoryService.GetGoodsCategoryByMerchantID(MerchantID, DistributionCode);
            }

            GoodsCategoryCheckBoxs.DataSource = dt;
            GoodsCategoryCheckBoxs.DataBind();
        }

        public int MerchantID
        {
            get;
            set;
        }

        public string DistributionCode
        {
            get;
            set;
        }

        /// <summary>
        /// 选中的品类编号
        /// </summary>
        public string SelectCategoryID
        {
            get { return GoodsCategoryCheckBoxs.SelectedValuesToString().Replace(" ",""); }
            set
            {
                string[] ids = value.Split(',');
                foreach (ListItem item in GoodsCategoryCheckBoxs.Items)
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
        /// 选中的品类名称
        /// </summary>
        public string SelectCategoryNames
        {
            get
            {
                return GoodsCategoryCheckBoxs.SelectedLabelsToString().Replace(" ", "");
            }
        }

        /// <summary>
        /// 是否全选
        /// </summary>
        public bool IsSelectAll
        {
            get
            {
                return GoodsCategoryCheckBoxs.Items.Count == SelectCategoryID.Split(',').Length ? true : false;
            }
        }
    }
}