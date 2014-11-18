using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.WEB.UserControl
{
    public partial class EmployeeListPop : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState["orderEmploy"] == null)
            {
                ViewState["orderEmploy"] = " order by EmployeeID Asc";
            }
            if (!IsPostBack)
            {
                EmployeeName.Text = HttpContext.Current.Server.UrlDecode(Request.QueryString["ID"].Replace("'", "''").Trim());

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
                    GridView1.Rows[i].Attributes.Add("onmouseover", "this.style.background-color='url(/Images/QueryList.gif)';");
                    GridView1.Rows[i].Attributes.Add("onmouseout", "this.style.backgroundImage='';");
                    GridView1.Rows[i].Attributes.Add("onclick", "SelectTRE('" + dataTable.Rows[i]["序号"].ToString() + "','" + dataTable.Rows[i]["员工名称"].ToString() + "','" + dataTable.Rows[i]["员工编码"].ToString() + "')");
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
            var userService = ServiceLocator.GetService<IUserService>();

            Employee model = new Employee();
            model.EmployeeCode = EmployeeCode.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "");
            model.EmployeeName = EmployeeName.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "");
            model.IsDeleted = false ;
            //model.StationID = LoginUser.ExpressId;
            model.SortStr = ViewState["orderEmploy"].ToString();
            DataTable dataTable = userService.GetSampList(model).Tables[0];
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
            if (ViewState["orderEmploy"] != null)
            {
                if (ViewState["orderEmploy"].ToString() != " order by  " + e.SortExpression)
                    ViewState["orderEmploy"] = " order by  " + e.SortExpression;
                else
                    ViewState["orderEmploy"] = " order by  " + e.SortExpression + " Asc";
            }
            else
            {
                ViewState["orderEmploy"] = " order by  " + e.SortExpression;
            }
            BuidPage();
        }
    }
}
