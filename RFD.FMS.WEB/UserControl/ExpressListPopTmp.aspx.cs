using System;
using System.Data;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.WEB.UserControl
{
    public partial class ExpressListPopTmp : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState["order"] == null)
            {
                ViewState["order"] = " order by ExpressCompanyCode desc";
            }

            if (!IsPostBack)
            {
                txtCompanyName.Text = Request.QueryString["ID"].Replace("'", "''").Trim();
                BuidPage();
            }
        }

        /// <summary>
        /// 查询并绑定GridView
        /// </summary>
        private void BuidPage()
        {
            DataTable dataTable = DtSource();
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
                    GridView1.Rows[i].Attributes.Add("onclick", "SelectTR('" + dataTable.Rows[i]["序号"].ToString() + "','" + dataTable.Rows[i]["部门名称"].ToString() + "')");
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

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        private DataTable DtSource()
        {
            //实例化业务层

            var expressCompanyService=ServiceLocator.GetService<IExpressCompanyService>();
            ExpressCompany model = new ExpressCompany();
            model.DistributionCode = base.DistributionCode;
            model.CompanyFlag = 999999;
            model.ExpressCompanyCode = ExpressCompanyCode.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "");
            model.CompanyName = txtCompanyName.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "");
            model.SortStr = ViewState["order"].ToString();
            DataTable dataTable = expressCompanyService.GetExpressCompanyListLess(model).Tables[0];
            return dataTable;
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
