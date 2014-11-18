using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.WEB.UserControl
{
    public partial class ExpressListPopCommon : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState["order"] == null)
            {
                ViewState["order"] = " order by ExpressCompanyCode desc";
            }
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    CompanyName.Text =
                        HttpContext.Current.Server.UrlDecode(Request.QueryString["ID"].Replace("'", "''").Trim());
                    BuidPage();
                }
            }
        }

		private string LoadCompanyType
		{
			get {return HttpContext.Current.Server.UrlDecode(Request.QueryString["loadDataType"].Replace("'", "''").Trim()); }
		}

        /// <summary>
        /// 查询并绑定GridView
        /// </summary>
        private void BuidPage()
        {
			DataTable dataTable = new DataTable();
			switch (LoadCompanyType)
			{
				case "AllCompany":
					dataTable = DtAllSource();
					break;
				case "OnlyRFD":
					dataTable = DtRFDSource();
					break;
				case "OnlySite":
					dataTable = DtSiteSource();
					break;
				case "OnlyThirdCompany":
					dataTable = DtThirdCompanySource();
					break;
                case "OnlyThirdRFD":
                    dataTable = DtThirdRFDSource();
                    break;
                case "CompanySite":
                    dataTable = DtCompanySiteSource();
                    break;
				default:
					dataTable = DtAllSource();
					break;
			}

            int tbRowCount = dataTable.Rows.Count;
            if (tbRowCount == 0)
            {
                dataTable.Rows.Add(dataTable.NewRow());
            }

            //绑定
            GridView1.DataSource = dataTable;
            GridView1.DataBind();
            int columnCount = dataTable.Columns.Count;
            //如果数据为空
            if (tbRowCount == 0)
            {
                GridView1.Rows[0].Cells.Clear();
                GridView1.Rows[0].Cells.Add(new TableCell());
                GridView1.Rows[0].Cells[0].ColumnSpan = columnCount;
                GridView1.Rows[0].Cells[0].Text = "没有数据";
                GridView1.Rows[0].Cells[0].Style.Add("text-align", "center");
            }
            else
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    GridView1.Rows[i].Attributes.Add("onmouseover", "this.style.backgroundImage='url(../Images/QueryList.gif)';");
                    GridView1.Rows[i].Attributes.Add("onmouseout", "this.style.backgroundImage='';");
                    //GridView1.Rows[i].Attributes.Add("onclick", "SelectTR('" + dataTable.Rows[i]["序号"].ToString() + "','" + dataTable.Rows[i]["部门名称"].ToString() + "')");
                    int tempRow = (this.GridView1.PageSize * GridView1.PageIndex + i);
                    GridView1.Rows[i].Attributes.Add("onclick", "SelectTR('" + dataTable.Rows[tempRow]["序号"].ToString() + "','" + dataTable.Rows[tempRow]["部门名称"].ToString() + "')");
                    GridView1.Rows[i].Style.Add("cursor", "hand");
                }
            }
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            BuidPage();
        }

		private DataTable DtAllSource()
		{
			//实例化业务层
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dataTable = expressCompanyService.GetExpressCompanyListLess(SearchModel()).Tables[0];
			return dataTable;
		}

		private DataTable DtSiteSource()
		{
			//实例化业务层
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dataTable = expressCompanyService.GetSiteExpressCompanyList(SearchModel()).Tables[0];
			return dataTable;
		}

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        private DataTable DtRFDSource()
        {
            //实例化业务层
            IExpressCompanyService service = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dataTable = service.GetExpressCompanyListLess(SearchModel()).Tables[0];
            return dataTable;
        }

		private DataTable DtThirdCompanySource()
		{
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dataTable = expressCompanyService.GetThirdExpressCompanyList(SearchModel()).Tables[0];
			return dataTable;
		}

        private DataTable DtThirdRFDSource()
		{
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dataTable = expressCompanyService.GetThirdRFDList(SearchModel()).Tables[0];
			return dataTable;
		}

        private DataTable DtCompanySiteSource()
        {
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dataTable = expressCompanyService.GetCompanySiteList(SearchModel()).Tables[0];
            return dataTable;
        }

		private ExpressCompany SearchModel()
		{
			ExpressCompany model = new ExpressCompany();
            model.DistributionCode = base.DistributionCode;
			model.CompanyFlag = 999999;
			model.ExpressCompanyCode = ExpressCompanyCode.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "");
			model.CompanyName = CompanyName.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "");
			model.SortStr = ViewState["order"].ToString();
			return model;
		}

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnQuery_Click(object sender, EventArgs e)
        {
            BuidPage();
        }

        /// <summary>
        /// 翻页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BuidPage(); //重新绑定GridView数据的函数
        }


        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["order"] != null)
            {
                if (ViewState["order"].ToString() != " order by  " + e.SortExpression)
                    ViewState["order"] = " order by  " + e.SortExpression;
                else
                    ViewState["order"] = " order by  " + e.SortExpression + " desc";
            }
            else
            {
                ViewState["order"] = " order by  " + e.SortExpression;
            }
            BuidPage();
        }
    }
}
