using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.BasicSetting;
using System.Text;
using RFD.FMS.MODEL;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AccountPeriodSet : BasePage
    {
        IStatusCodeService statusService = ServiceLocator.GetService<IStatusCodeService>();
        ITypeRelationService relationService = ServiceLocator.GetService<ITypeRelationService>();
        IAccountPeriodService accountPeriodService = ServiceLocator.GetService<IAccountPeriodService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTypeSource();
                
            }
            UCPager.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            BindPeriodRelation(UCPager.CurrentPageIndex);
        }

        public void BindTypeSource()
        {
            statusService.BindDropDownListByCodeType(ddlPeriodSource, "全部", "", "PeriodSource", base.DistributionCode);
            statusService.BindDropDownListByCodeType(ddlPeriodSource1, "全部", "", "PeriodSource", base.DistributionCode);
        }

        #region 账期设置
        protected void btSearchPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                if (!JudgeSetPeriodInput()) return;

                IAccountPeriodService accountPeriodService = ServiceLocator.GetService<IAccountPeriodService>();
                AccountPeriodCondition apc = new AccountPeriodCondition()
                {
                    PeriodRelationName = ddlPeriodSource.SelectedValue
                };
                List<AccountPeriod> accountPeriodList= accountPeriodService.SearchAccountPeriod(apc);
                if (accountPeriodList == null || accountPeriodList.Count <= 0)
                {
                    Alert("没有查询到数据");
                    return;
                }

                gvPeriodSoureList.DataSource = accountPeriodList;
                gvPeriodSoureList.DataBind();
            }
            catch (Exception ex)
            {
                Alert("查询账期错误<br>" + ex.Message);
            }
        }

        private bool JudgeSetPeriodInput()
        {
            if (ddlPeriodSource.SelectedIndex == 0)
            {
                Alert("账期源必选");
                return false;
            }
            return true;
        }

        private IList<KeyValuePair<DataKey, GridViewRow>> PeriodSourceChecked
        {
            get { return Util.ControlHelper.GridViewHelper.GetSelectedRows(gvPeriodSoureList, "cbCheckBox"); }
        }

        private bool JudgeSelectRow()
        {
            if (PeriodSourceChecked.Count <= 0)
            {
                Alert("没有选择行");
                return false;
            }

            if (PeriodSourceChecked.Count >1)
            {
                Alert("只能操作一行");
                return false;
            }
            return true;
        }

        #endregion

        #region 关联账期
        public string TypeRelationName
        {
            get { return ViewState["TypeRelationName"] == null ? null : ViewState["TypeRelationName"].ToString(); }
            set { ViewState.Add("TypeRelationName", value); }
        }

        protected void btSearchRelation_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlPeriodSource1.SelectedIndex == 0)
                {
                    Alert("没有选择账期源");
                    return;
                }
                TypeRelationName = ddlPeriodSource1.SelectedValue;
                BindPeriodRelation(1);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>" + ex.Message);
            }
        }

        private void BindPeriodRelation(int currentPageIndex)
        {
            PageInfo pi = new PageInfo(20);
            UCPager.PageSize = 20;
            pi.CurrentPageIndex = currentPageIndex;
            List<TypeRelationModel> list = relationService.SearchPeriodRelationList(TypeRelationName, base.DistributionCode, merchantName.Text.Trim(), ref pi);
            if (list == null || list.Count <= 0)
            {
                throw new Exception("没有查询到数据");
            }
            UCPager.RecordCount = pi.ItemCount;
            gvPeriodRelationList.DataSource = list;
            gvPeriodRelationList.DataBind();
            AccountPeriodCondition apc=new AccountPeriodCondition()
            {
                PeriodRelationName=TypeRelationName,
                IsDeleted="0"
            };
            List<AccountPeriod> apList = accountPeriodService.SearchAccountPeriod(apc);
            if (apList!=null && apList.Count > 0)
            {
                ddlType.Items.Clear();
                ddlType.DataSource = apList;
                ddlType.DataTextField = "PeriodName";
                ddlType.DataValueField = "AccountPeriodKid";
                ddlType.DataBind();
            }
            ddlType.Items.Insert(0, new ListItem("全部", ""));
        }

        protected void btAddSetType_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlType.SelectedIndex == 0)
                {
                    Alert("没有选择需要设置的账期");
                    return;
                }
                string typeNo = ddlType.SelectedValue;
                IList<KeyValuePair<DataKey, GridViewRow>> list = gvPeriodRelationListChecked;
                if (list == null || list.Count <= 0)
                {
                    Alert("没有选择需要设置的行");
                    return;
                }

                StringBuilder sbError = new StringBuilder();
                foreach (KeyValuePair<DataKey, GridViewRow> KeyValuePair in list)
                {
                    TypeRelationModel model = BuildModel(KeyValuePair, typeNo);
                    if (!relationService.AddTypeRelation(model))
                    {
                        GridViewRow gvr = KeyValuePair.Value;
                        sbError.Append(gvr.Cells[2].Text + "," + ddlType.SelectedItem.ToString() + "<br>");
                    }
                }
                BindPeriodRelation(1);
                if (sbError.ToString().Length > 0)
                    Alert("设置重复<br>" + sbError.ToString());

            }
            catch (Exception ex)
            {
                Alert("设置错误<br>" + ex.Message);
            }
        }

        protected void btDeletePeriod_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sbError = new StringBuilder();
                IList<KeyValuePair<DataKey, GridViewRow>> list = gvPeriodRelationListChecked;
                if (list == null || list.Count <= 0)
                {
                    Alert("没有选择需要设置的行");
                    return;
                }
                foreach (KeyValuePair<DataKey, GridViewRow> KeyValuePair in list)
                {
                    TypeRelationModel model = BuildModel(KeyValuePair, "");
                    if (!relationService.DelTypeRelation(model))
                    {
                        GridViewRow gvr = KeyValuePair.Value;
                        sbError.Append(gvr.Cells[1].Text + "," + gvr.Cells[2].Text + "<br>");
                    }
                }
                BindPeriodRelation(1);
                if (sbError.ToString().Length > 0)
                    Alert("设置重复<br>" + sbError.ToString());
            }
            catch (Exception ex)
            {
                Alert("删除失败<br>"+ex.Message);
            }
        }

        private TypeRelationModel BuildModel(KeyValuePair<DataKey, GridViewRow> keyValuePair, string typeNo)
        {
            if (TypeRelationName == null)
                throw new Exception("获取分类源失败");
            DataKey dk = keyValuePair.Key as DataKey;
            GridViewRow gvr = keyValuePair.Value;
            TypeRelationModel model = new TypeRelationModel();
            model.TypeRelationKid = dk.Values[0].ToString();
            model.RelationNameID = int.Parse(dk.Values[1].ToString());
            model.TypeCodeNo = typeNo;
            model.TypeRelationName = TypeRelationName;
            model.IsDelete = false;
            model.DistributionCode = base.DistributionCode;
            model.CreateBy = base.Userid;
            model.UpdateBy = base.Userid;
            return model;
        }

        public IList<KeyValuePair<DataKey, GridViewRow>> gvPeriodRelationListChecked
        {
            get
            {
                return Util.ControlHelper.GridViewHelper.GetSelectedRows(gvPeriodRelationList, "cbCheckBox");
            }
        }
        #endregion
    }
}