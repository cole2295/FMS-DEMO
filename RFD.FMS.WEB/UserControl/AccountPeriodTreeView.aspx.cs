using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.BasicSetting;
using System.Text;
using RFD.FMS.MODEL;

namespace RFD.FMS.WEB.UserControl
{
    public partial class AccountPeriodTreeView : BasePage
    {
        private IExpressCompanyService expressService = ServiceLocator.GetService<IExpressCompanyService>();
        private IMerchantService merchantService = ServiceLocator.GetService<IMerchantService>();
        private ITypeRelationService relationService = ServiceLocator.GetService<ITypeRelationService>();
        private IAccountPeriodService accountPeriodService = ServiceLocator.GetService<IAccountPeriodService>();
        private static string parentId = "1.0.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PeriodDataSource = HttpContext.Current.Server.UrlDecode(Request.QueryString["periodDataSource"].Replace("'", "''").Trim());
                PeriodLoadType = HttpContext.Current.Server.UrlDecode(Request.QueryString["periodLoadType"].Replace("'", "''").Trim());
                GetAllDataSource();
                GetAccountPeriodList();
                BuildTreeRoot(false);
            }
        }

        private void GetAccountPeriodList()
        {
            AccountPeriodCondition apc = new AccountPeriodCondition()
            {
                PeriodRelationName = PeriodLoadType,
            };
            AccountPeriodList = accountPeriodService.SearchAccountPeriod(apc);
            foreach (AccountPeriod ap1 in AccountPeriodList)
            {
                AccountPeriod ap = ap1;
                ap.ImitateDate = DateTime.Now.ToString("yyyy-MM-dd");
                ap.ImitateNum = 10;
                accountPeriodService.GetImitatePeriod(ref ap);//获取到账期列表
            }

        }

        /// <summary>
        /// 原始数据
        /// </summary>
        private void GetAllDataSource()
        {
            switch (PeriodDataSource)
            {
                case "Express_All":
                    AllDataSource = expressService.GetThirdCompanyList(base.DistributionCode).Tables[0];break;
                case "Express_Third":
                    AllDataSource = expressService.GetThirdCompanyList(base.DistributionCode).Tables[0]; break;
                case "Express_RFDSite":
                    AllDataSource = expressService.GetRFDSiteList(base.DistributionCode).Tables[0]; break;
                case "Express_RFDSortCenter":
                    AllDataSource = expressService.GetRFDSortCenterList(base.DistributionCode).Tables[0]; break;
                case "Merchant_All":
                    AllDataSource = merchantService.GetAllMerchants(base.DistributionCode);break;
                case "Merchant_Third":
                    AllDataSource = merchantService.GetMerchants(base.DistributionCode); break;
                case "Merchant_ThirdNoExpress":
                    AllDataSource = merchantService.GetMerchantsNoExpress(base.DistributionCode); break;
                default:
                    AllDataSource = null;
                    break;
            }

        }

        public List<AccountPeriod> AccountPeriodList
        {
            get { return ViewState["AccountPeriodList"] == null ? null : ViewState["AccountPeriodList"] as List<AccountPeriod>; }
            set { ViewState.Add("AccountPeriodList", value); }
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

        public string SearchKeyWord
        {
            get { return ViewState["SearchKeyWord"] == null ? null : ViewState["SearchKeyWord"].ToString(); }
            set { ViewState.Add("SearchKeyWord", value); }
        }

        public string PeriodDataSource
        {
            get { return ViewState["PeriodDataSource"].ToString(); }
            set { ViewState.Add("PeriodDataSource",value); }
        }

        public string PeriodLoadType
        {
            get { return ViewState["PeriodLoadType"].ToString(); }
            set { ViewState.Add("PeriodLoadType", value); }
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
            PageInfo pi = new PageInfo(10000);
            List<TypeRelationModel> relation = relationService.SearchPeriodRelationList(PeriodLoadType, base.DistributionCode,null,ref pi);

            DataTable dtNew = dt.Clone();
            dtNew.Columns.Add("NodeID", typeof(String));
            dtNew.Columns.Add("NodeName", typeof(String));
            dtNew.Columns.Add("ParentID", typeof(String));
            dtNew.Columns.Add("Level", typeof(String));
            dtNew.Columns.Add("IsAccountPeriod", typeof(Boolean));

            DataRow drP = dtNew.NewRow();
            CreateNewRow(ref drP, parentId, "全部", "0", 0, false);
            dtNew.Rows.Add(drP.ItemArray);
            //添加分类
            var distinctTypeList = (from t in relation
                                    where t.TypeCodeNo != ""
                                    select new { t.TypeCodeNo, t.CodeDesc }).Distinct().ToList();
            for (int n = 0; n < distinctTypeList.Count(); n++)
            {
                DataRow drP1 = dtNew.NewRow();
                bool f = JudgeAccountPeriod(distinctTypeList[n].TypeCodeNo);
                CreateNewRow(ref drP1, parentId + distinctTypeList[n].TypeCodeNo, distinctTypeList[n].CodeDesc, parentId, 1,f);
                dtNew.Rows.Add(drP1.ItemArray);
            }
            DataRow drP2 = dtNew.NewRow();
            CreateNewRow(ref drP2, parentId + "0", "其他", parentId, 1, false);
            dtNew.Rows.Add(drP2.ItemArray);

            foreach (DataRow dr in dt.Rows)
            {
                DataRow drNew = dtNew.NewRow();
                CreateNewRow(ref drNew,
                    (PeriodLoadType.Contains("Merchant") ? dr["ID"] : PeriodLoadType.Contains("Express") ? dr["ExpressCompanyID"] : "").ToString(),
                    (PeriodLoadType.Contains("Merchant") ? dr["MerchantName"] : PeriodLoadType.Contains("Express") ? dr["CompanyName"] : "").ToString(),
                    SearchRowType(dr, relation),2,false);
                dtNew.Rows.Add(drNew.ItemArray);
            }

            return dtNew;
        }

        private void CreateNewRow(ref DataRow drNew,string nodeId,string nodeName,string parentId,int level,bool isAccount)
        {
            drNew["NodeID"] = nodeId;
            drNew["NodeName"] = nodeName;
            drNew["ParentID"] = parentId;
            drNew["Level"] = level;
            drNew["IsAccountPeriod"] = isAccount;
        }

        private bool JudgeAccountPeriod(string accountPeriodKid)
        {
            var ap1 = (from t in AccountPeriodList where t.AccountPeriodKid == accountPeriodKid select t).ToList();

            if (ap1 == null || ap1.Count<=0)
            {
                return false;
            }
            AccountPeriod ap = ap1[0] as AccountPeriod;
            List<AccountPeriodDate> aps = SelectCurrentAccountPeriod(ap);
            if (aps.Count <= 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取当前到达账期
        /// </summary>
        /// <param name="ap"></param>
        /// <returns></returns>
        private List<AccountPeriodDate> SelectCurrentAccountPeriod(AccountPeriod ap)
        {
            List<AccountPeriodDate> aps = (from t in ap.AccountPeriodList where t.AccountDate.ToShortDateString() == DateTime.Now.ToShortDateString() select t).ToList();
            return aps;
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
                                    where t.RelationId == int.Parse(dr["ID"].ToString())
                                    select t).ToList();
            if (distinctTypeList == null)
                return parentId + "0";
            else
                return parentId + (string.IsNullOrEmpty(distinctTypeList[0].TypeCodeNo) ? "0" : distinctTypeList[0].TypeCodeNo);
        }

        private void BuildTreeRoot(bool isSearchData)
        {
            if (AccountPeriodTree.Nodes.Count > 0)
            {
                AccountPeriodTree.Nodes.Clear();
            }

            BindTree(GetDateSource(isSearchData), null, "0");
            AccountPeriodTree.ExpandDepth = 1;
        }

        private void BindTree(DataTable dtSource, TreeNode parentNode, string parentID)
        {
            DataRow[] rows = dtSource.Select(string.Format("ParentID='{0}'", parentID));
            foreach (DataRow row in rows)
            {
                TreeNode node = new TreeNode();
                string nodeId = row["NodeID"].ToString();
                int level = int.Parse(row["Level"].ToString());
                bool isAccountPeriod = bool.Parse(row["IsAccountPeriod"].ToString());
                node.Text = row["NodeName"].ToString() +
                    (level == 1 ? "(" + dtSource.Select(string.Format("ParentID='{0}'", nodeId)).Length + ")" :
                    level == 0 ? "(" + dtSource.Select("Level='2'").Length + ")" : "");
                node.Value = nodeId;
                
                node.SelectAction = TreeNodeSelectAction.None;
                if (level == 0)
                    node.SelectAction = TreeNodeSelectAction.None;
                else if (level == 2)
                {
                    if (parentNode != null && parentNode.SelectAction == TreeNodeSelectAction.Select)
                    {
                        node.SelectAction = TreeNodeSelectAction.Select;
                        node.NavigateUrl = "";
                        node.Selected = false;
                    }
                }
                else
                {
                    AccountPeriod ap = GetAccountPeriod(nodeId);
                    if (ap != null)
                        node.ToolTip = ap.ImitateAccountPeriod.Replace("<br>", "\n").Substring(0, 300) + "\n...";
                    if (isAccountPeriod)
                    {
                        node.SelectAction = TreeNodeSelectAction.Select;
                        node.NavigateUrl = "";
                        node.Selected = false;
                    }
                }

                BindTree(dtSource, node, nodeId);
                if (parentNode == null)
                {
                    AccountPeriodTree.Nodes.Add(node);
                    node.Expand();
                }
                else
                {
                    parentNode.ChildNodes.Add(node);
                }
            }
        }

        private AccountPeriod GetAccountPeriod(string kid)
        {   
            if(kid.Contains("."))
                kid=kid.Substring(kid.LastIndexOf('.')+1, kid.Length - (kid.LastIndexOf('.')+1));
            var ap=(from t in AccountPeriodList
                            where t.AccountPeriodKid == kid
                            select t).ToList();
            if (ap != null && ap.Count > 0)
                return ap[0] as AccountPeriod;
            return null;
        }

        protected void AccountPeriodTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode parentNode = AccountPeriodTree.SelectedNode.Parent;
            TreeNode checkNode = AccountPeriodTree.SelectedNode;
            TreeNodeCollection collection = new TreeNodeCollection();
            collection.Add(checkNode);
            if (parentNode == null || string.IsNullOrEmpty(parentNode.ToolTip))
            {
                parentNode = checkNode;
            }
            CheckedSelectNodes(collection, parentNode);
        }

        private void CheckedSelectNodes(TreeNodeCollection collection, TreeNode parentNode)
        {
            TreeNode checkedNode = collection[0];
            StringBuilder sbObjectId = new StringBuilder();//查询的ID
            StringBuilder sbObjectName = new StringBuilder();//查询的Name
            string expressType = string.Empty;//是否配送公司
            string expressIds = string.Empty;//配送公司ID
            string expressNames = string.Empty;//配送公司名称
            string accountDate = string.Empty;//账期时间
            string accountDateStr = string.Empty;//账期开始时间
            string accountDateEnd = string.Empty;//账期结束时间
            string accountPeriodId = parentNode.Value;//账期ID
            string accountPeriodName = parentNode.Text;//账期名称 

            AccountPeriod ap = GetAccountPeriod(parentNode.Value);
            if (ap != null)
            {
                expressType = ap.IsExpress.ToString();
                expressIds = ap.ExpressIds;
                expressNames = ap.ExpressNames;
                List<AccountPeriodDate> aps = SelectCurrentAccountPeriod(ap);
                accountDate = aps[0].AccountDate.ToString("yyyy-MM-dd");
                accountDateStr = aps[0].AccountPeriodStartDate.ToString("yyyy-MM-dd");
                accountDateEnd = aps[0].AccountPeriodEndDate.ToString("yyyy-MM-dd");
            }
            //下面的子节点
            if (checkedNode.ChildNodes.Count>0)
            {
                foreach (TreeNode tn in checkedNode.ChildNodes)
                {
                    sbObjectId.Append(tn.Value + ",");
                    sbObjectName.Append(tn.Text + ",");
                }
            }
            else
            {
                sbObjectId.Append(checkedNode.Value + ",");
                sbObjectName.Append(checkedNode.Text + ",");
            }
            //mAccountId, mAccountName, mObjectName, mObjectId,mExpress,mExpressType
            RunJS(string.Format("fnSelectChecked('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                accountPeriodId,
                accountPeriodName,
                sbObjectName.ToString().TrimEnd(','),
                sbObjectId.ToString().TrimEnd(','),
                expressIds,
                expressType,
                accountDate,
                accountDateStr,
                accountDateEnd,
                expressNames));
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            SearchKeyWord = txtKeyWord.Text.Trim();
            if (string.IsNullOrEmpty(SearchKeyWord))
            {
                BuildTreeRoot(false);
            }
            string searchCondition = PeriodLoadType.Contains("Merchant") ? 
                "MerchantName like '%" + SearchKeyWord + "%'" :PeriodLoadType.Contains("Express")?
                "CompanyName like '%" + SearchKeyWord + "%'":"";
            if (string.IsNullOrEmpty(searchCondition))
            {
                Alert("读取错误");
                return;
            }
            DataRow[] search = AllDataSource.Select("MerchantName like '%" + SearchKeyWord + "%'");
            if (search == null || search.Length <= 0)
            {
                Alert("没有查询到商家");
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
    }
}