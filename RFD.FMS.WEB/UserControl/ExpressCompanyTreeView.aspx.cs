using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.UserControl
{
    public partial class ExpressCompanyTreeView : BasePage
    {
        private IExpressCompanyService expressService = ServiceLocator.GetService<IExpressCompanyService>();
        private ITypeRelationService relationService = ServiceLocator.GetService<ITypeRelationService>();
        private static string parentId = "1.0.";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
                GetAllDataSource();
                BuildTreeRoot(false);
            }
        }

        private void InitForm()
        {
            if (IsShowCheckBox)
            {
                pOk.Visible = true;
                tree.ShowCheckBoxes = TreeNodeTypes.All;
            }
            else
            {
                pOk.Visible = false;
                tree.ShowCheckBoxes = TreeNodeTypes.None;
                //tree.SelectedNodeChanged += tree_SelectedNodeChanged;
            }
        }
        public string TypeSource
        {
            get { return HttpContext.Current.Server.UrlDecode(Request.QueryString["typeSource"].Replace("'", "''").Trim()); }
        }

        public string LoadType
        {
            get { return HttpContext.Current.Server.UrlDecode(Request.QueryString["loadType"].Replace("'", "''").Trim()); }
        }

        public DataTable AllDataSource
        {
            get { return ViewState["AllDataSource"] == null ? null : ViewState["AllDataSource"] as DataTable; }
            set { ViewState.Add("AllDataSource", value); }
        }

        public DataTable SearchDataSource
        {
            get { return ViewState["SearchDataSource"] == null ? null : ViewState["SearchDataSource"] as DataTable; }
            set { ViewState.Add("SearchDataSource", value); }
        }

        public bool IsShowCheckBox
        {
            get { return bool.Parse(HttpContext.Current.Server.UrlDecode(Request.QueryString["showCheckBox"].Replace("'", "''").Trim()).ToLower()); }
        }
        /// <summary>
        /// 原始数据
        /// </summary>
        private void GetAllDataSource()
        {
            switch (LoadType)
            {
                case "All":
                    AllDataSource = expressService.GetThirdCompanyList(base.DistributionCode).Tables[0];
                    break;
                case "ThirdCompany":
                    AllDataSource = expressService.GetThirdCompanyList(DistributionCode).Tables[0]; break;
                case "RFDSite":
                    AllDataSource = expressService.GetRFDSiteList(DistributionCode).Tables[0]; break;
                case "RFDSortCenter":
                    AllDataSource = expressService.GetRFDSortCenterList(DistributionCode).Tables[0]; break;
                default:
                    AllDataSource = expressService.GetThirdCompanyList(base.DistributionCode).Tables[0];
                    break;
            }
        }

        private DataTable GetDateSource(bool isSearchData)
        {
            DataTable dt = new DataTable();
            if (isSearchData)
            {
                dt = SearchDataSource;
            }
            else
            {
                dt = AllDataSource;
            }

            List<TypeRelationModel> relation = relationService.SearchRelationList(TypeSource, base.DistributionCode);

            DataTable dtNew = dt.Clone();
            dtNew.Columns["ExpressCompanyID"].DataType = typeof(String);
            dtNew.Columns.Add("ParentID", typeof(String));
            dtNew.Columns.Add("Level", typeof(String));

            DataRow drP = dtNew.NewRow();
            drP["ExpressCompanyID"] = parentId;
            drP["CompanyName"] = "全部";
            drP["ParentID"] = 0;
            drP["Level"] = 0;
            dtNew.Rows.Add(drP.ItemArray);
            //添加分类
            var distinctTypeList = (from t in relation
                                    where t.TypeCodeNo != ""
                                    select new { t.TypeCodeNo, t.CodeDesc }).Distinct().ToList();
            for (int n = 0; n < distinctTypeList.Count(); n++)
            {
                drP["ExpressCompanyID"] = parentId + distinctTypeList[n].TypeCodeNo;
                drP["CompanyName"] = distinctTypeList[n].CodeDesc;
                drP["ParentID"] = parentId;
                drP["Level"] = 1;
                dtNew.Rows.Add(drP.ItemArray);
            }
            drP["ExpressCompanyID"] = parentId + "0";
            drP["CompanyName"] = "其他";
            drP["ParentID"] = parentId;
            drP["Level"] = 1;
            dtNew.Rows.Add(drP.ItemArray);

            foreach (DataRow dr in dt.Rows)
            {
                DataRow drNew = dtNew.NewRow();
                drNew["ExpressCompanyID"] = dr["ExpressCompanyID"];
                drNew["CompanyName"] = dr["CompanyName"];
                drNew["ParentID"] = SearchRowType(dr, relation);
                drNew["Level"] = 2;
                dtNew.Rows.Add(drNew.ItemArray);
            }

            return dtNew;
        }

        /// <summary>
        /// 找对应的分类节点
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        private string SearchRowType(DataRow dr, List<TypeRelationModel> relation)
        {
            var distinctTypeList = (from t in relation
                                    where t.RelationId == int.Parse(dr["ExpressCompanyID"].ToString())
                                    select t).ToList();
            if (distinctTypeList == null || distinctTypeList.Count<=0)
                return parentId + "0";
            else
                return parentId + (string.IsNullOrEmpty(distinctTypeList[0].TypeCodeNo) ? "0" : distinctTypeList[0].TypeCodeNo);
        }

        private void BuildTreeRoot(bool isSearchData)
        {
            if (tree.Nodes.Count > 0)
            {
                tree.Nodes.Clear();
            }

            BindTree(GetDateSource(isSearchData), null, "0");
            tree.ExpandDepth = 1;
        }

        private void BindTree(DataTable dtSource, TreeNode parentNode, string parentID)
        {
            DataRow[] rows = dtSource.Select(string.Format("ParentID='{0}'", parentID));
            foreach (DataRow row in rows)
            {
                TreeNode node = new TreeNode();
                string nodeId = row["ExpressCompanyID"].ToString();
                int level = int.Parse(row["Level"].ToString());
                node.Text = row["CompanyName"].ToString() +
                    (level == 1 ? "(" + dtSource.Select(string.Format("ParentID='{0}'", nodeId)).Length + ")" :
                    level == 0 ? "(" + dtSource.Select("Level='2'").Length + ")" : "");
                node.Value = nodeId;
                node.SelectAction = TreeNodeSelectAction.None;
                if (IsShowCheckBox)
                    node.SelectAction = TreeNodeSelectAction.None;
                else
                {
                    if (level == 1 || level == 0)
                        node.SelectAction = TreeNodeSelectAction.None;
                    else
                    {
                        node.SelectAction = TreeNodeSelectAction.Select;
                        node.NavigateUrl = "";
                        node.Selected = false;
                    }
                }

                BindTree(dtSource, node, nodeId);
                if (parentNode == null)
                {
                    tree.Nodes.Add(node);
                    node.Expand();
                }
                else
                {
                    parentNode.ChildNodes.Add(node);
                }
            }
        }

        protected void btOK_Click(object sender, EventArgs e)
        {
            TreeNodeCollection collection = tree.CheckedNodes;
            StringBuilder sbCheckedV = new StringBuilder();
            StringBuilder sbCheckedT = new StringBuilder();
            foreach (TreeNode tn in collection)
            {
                //只获取最后一级实际真实的商家
                if (tn.Depth == 2)
                {
                    sbCheckedV.Append(tn.Value + ",");
                    sbCheckedT.Append(tn.Text + ",");
                }
            }
            //Alert(sbCheckedV.ToString().TrimEnd(',') + "<br>" + sbCheckedT.ToString().TrimEnd(','));
            RunJS(string.Format("fnSelectChecked('{0}','{1}')", sbCheckedV.ToString().TrimEnd(','), sbCheckedT.ToString().TrimEnd(',')));
        }

        public string SearchKeyWord
        {
            get { return ViewState["SearchKeyWord"] == null ? null : ViewState["SearchKeyWord"].ToString(); }
            set { ViewState.Add("SearchKeyWord", value); }
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            SearchKeyWord = txtKeyWord.Text.Trim();
            if (string.IsNullOrEmpty(SearchKeyWord))
            {
                BuildTreeRoot(false);
            }

            DataRow[] search = AllDataSource.Select("CompanyName like '%" + SearchKeyWord + "%'");
            if (search == null || search.Length <= 0)
            {
                Alert("没有查询到配送商");
                return;
            }
            DataTable dtNew = AllDataSource.Clone();
            foreach (DataRow dr in search)
            {
                dtNew.Rows.Add(dr.ItemArray);
            }
            SearchDataSource = dtNew;
            BuildTreeRoot(true);
        }

        protected void btReset_Click(object sender, EventArgs e)
        {
            txtKeyWord.Text = "";
            SearchKeyWord = "";
            BuildTreeRoot(false);
        }

        protected void tree_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode tn = tree.SelectedNode;
            TreeNodeCollection collection = new TreeNodeCollection();
            collection.Add(tn);
            CheckedSelectNodes(collection);
        }

        private void CheckedSelectNodes(TreeNodeCollection collection)
        {
            StringBuilder sbCheckedV = new StringBuilder();
            StringBuilder sbCheckedT = new StringBuilder();
            if (IsShowCheckBox)
            {
                foreach (TreeNode tn in collection)
                {
                    //只获取最后一级实际真实的商家
                    if (tn.Depth == 2)
                    {
                        sbCheckedV.Append(tn.Value + ",");
                        sbCheckedT.Append(tn.Text + ",");
                    }
                }
            }
            else
            {
                sbCheckedV.Append(collection[0].Value + ",");
                sbCheckedT.Append(collection[0].Text + ",");
            }
            //Alert(sbCheckedV.ToString().TrimEnd(',') + "<br>" + sbCheckedT.ToString().TrimEnd(','));
            RunJS(string.Format("fnSelectChecked('{0}','{1}')", sbCheckedV.ToString().TrimEnd(','), sbCheckedT.ToString().TrimEnd(',')));
        }
    }
}