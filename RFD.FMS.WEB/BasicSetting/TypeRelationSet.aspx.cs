using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util.ControlHelper;
using System.Text;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class TypeRelationSet : BasePage
    {
        IStatusCodeService statusService = ServiceLocator.GetService<IStatusCodeService>();
        ITypeRelationService relationService = ServiceLocator.GetService<ITypeRelationService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTypeSource();
            }
        }

        public void BindTypeSource()
        {
            statusService.BindDropDownListByCodeType(ddlTypeSource, "全部", "", "TypeSource", base.DistributionCode);
            statusService.BindDropDownListByCodeType(ddlTypeSource1, "全部", "", "TypeSource", base.DistributionCode);
        }
        #region 分类添加
        public string TypeSouceName
        {
            get { return ViewState["TypeSouceName"] == null ? null : ViewState["TypeSouceName"].ToString(); }
            set { ViewState.Add("TypeSouceName", value); }
        }

        protected void btSearchType_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTypeSource.SelectedIndex == 0)
                {
                    Alert("没有选择分类源");
                    return;
                }
                TypeSouceName = ddlTypeSource.SelectedValue;
                BindgvTypeSoure();
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
        }

        private void BindgvTypeSoure()
        {
            IList<StatusCodeInfo> codeInfoList = statusService.GetListByType(TypeSouceName, base.DistributionCode, false);

            if (codeInfoList == null || codeInfoList.Count <= 0)
            {
                throw new Exception("没有查询到数据");
            }

            gvTypeSoureList.DataSource = codeInfoList;
            gvTypeSoureList.DataBind();
        }

        protected void btAddType_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTypeSource.SelectedIndex == 0)
                {
                    Alert("没有选择分类源");
                    return;
                }
                string url = string.Format("StatusCodeEdit.aspx?codeType={0}&codeNo={1}", ddlTypeSource.SelectedValue,"");
                RunJS(string.Format("fnOpenModalDialog('{0}', 200, 200);", url));
            }
            catch (Exception ex)
            {
                Alert("添加出错<br>"+ex.Message);
            }
        }

        protected void btUpdateType_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<string, string>> checkList = gvTypeSoureListChecked;
                if (checkList == null || checkList.Count <= 0)
                {
                    Alert("没有选择需要修改的分类");
                    return;
                }
                if (checkList.Count>1)
                {
                    Alert("只能同时修改一个分类");
                    return;
                }

                string url = string.Format("StatusCodeEdit.aspx?codeType={0}&codeNo={1}", checkList[0].Value,checkList[0].Key);
                RunJS(string.Format("fnOpenModalDialog('{0}', 200, 200);", url));
            }
            catch (Exception ex)
            {
                Alert("更新出错<br>" + ex.Message);
            }
        }

        public IList<KeyValuePair<string, string>> gvTypeSoureListChecked
        {
            get
            {
                IList<KeyValuePair<string, string>> keyPairValues = new List<KeyValuePair<string, string>>();
                foreach (GridViewRow row in gvTypeSoureList.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        if (((CheckBox)row.FindControl("cbCheckBox")).Checked)
                        {
                            DataKey dataKey = gvTypeSoureList.DataKeys[row.RowIndex];

                            string no = dataKey.Values[0].ToString();
                            string type = dataKey.Values[1].ToString();

                            keyPairValues.Add(new KeyValuePair<string, string>(no, type));
                        }
                    }
                }
                return keyPairValues;
            }
        }

        #endregion

        #region 分类设置
        protected void btSearchRelation_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTypeSource1.SelectedIndex == 0)
                {
                    Alert("没有选择分类源");
                    return;
                }
                TypeRelationName = ddlTypeSource1.SelectedValue;
                BindTypeRelation();
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
        }

        private void BindTypeRelation()
        {
            List<TypeRelationModel> list = relationService.SearchRelationList(TypeRelationName, base.DistributionCode);
            if (list == null || list.Count <= 0)
            {
                throw new Exception("没有查询到数据");
            }
            gvTypeRelationList.DataSource = list;
            gvTypeRelationList.DataBind();
            statusService.BindDropDownListByCodeType(ddlType, "全部", "", TypeRelationName, base.DistributionCode);
        }

        public string TypeRelationName
        {
            get { return ViewState["TypeRelationName"] == null ? null : ViewState["TypeRelationName"].ToString(); }
            set { ViewState.Add("TypeRelationName", value); }
        }

        protected void btAddSetType_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlType.SelectedIndex == 0)
                {
                    Alert("没有选择需要设置的分类");
                    return;
                }
                string typeNo=ddlType.SelectedValue;
                IList<KeyValuePair<string, string>> list = gvTypeRelationListChecked;
                if (gvTypeRelationListChecked == null || gvTypeRelationListChecked.Count <= 0)
                {
                    Alert("没有选择需要设置的行");
                    return;
                }

                StringBuilder sbError = new StringBuilder();
                foreach (KeyValuePair<string, string> KeyValuePair in list)
                {
                    TypeRelationModel model = BuildModel(KeyValuePair, typeNo);
                    if (!relationService.SetTypeRelation(model))
                    {
                        sbError.Append(model.RelationNameID + "," + model.TypeCodeNo + "<br>");
                    }
                }
                BindTypeRelation();
                if (sbError.ToString().Length > 0)
                    Alert("设置错误<br>" + sbError.ToString());
                    
            }
            catch (Exception ex)
            {
                Alert("设置错误<br>"+ex.Message);
            }
        }

        public IList<KeyValuePair<string, string>> gvTypeRelationListChecked
        {
            get
            {
                IList<KeyValuePair<string, string>> keyPairValues = new List<KeyValuePair<string, string>>();
                foreach (GridViewRow row in gvTypeRelationList.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        if (((CheckBox)row.FindControl("cbCheckBox")).Checked)
                        {
                            DataKey dataKey = gvTypeRelationList.DataKeys[row.RowIndex];

                            string kid = dataKey.Values[0].ToString();
                            string mid = dataKey.Values[1].ToString();

                            keyPairValues.Add(new KeyValuePair<string, string>(kid, mid));
                        }
                    }
                }
                return keyPairValues;
            }
        }

        private TypeRelationModel BuildModel(KeyValuePair<string, string> keyValuePair,string typeNo)
        {
            if (TypeRelationName == null)
                throw new Exception("获取分类源失败");

            TypeRelationModel model = new TypeRelationModel();
            model.TypeRelationKid = keyValuePair.Key;
            model.RelationNameID = int.Parse(keyValuePair.Value);
            model.TypeCodeNo = typeNo;
            model.TypeRelationName = TypeRelationName;
            model.IsDelete = false;
            model.DistributionCode = base.DistributionCode;
            model.CreateBy = base.Userid;
            model.UpdateBy = base.Userid;
            return model;
        }
        #endregion
    }
}