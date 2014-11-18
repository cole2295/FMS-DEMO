using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.RequisitionedService;
using System.IO;
using RFD.FMS.Util;
using System.Drawing;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEB.COD
{
	public partial class RequisitionedFormSearch : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				txtDateStr.Text = DateTime.Now.AddDays(-31).ToString("yyyy-MM-dd");
				//txtDateStr.Text = "2011-08-01";
				txtDateEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
			}
		}

        IRequisitionedForm requisitionedForm = ServiceLocator.GetService<IRequisitionedForm>();
		private LoanServiceClient RequisitionedService = new LoanServiceClient();

		protected void btnSearch_Click(object sender, EventArgs e)
		{
            try
            {
                if (string.IsNullOrEmpty(txtDateStr.Text.Trim()))
                {
                    Alert("开始日期不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(txtDateEnd.Text.Trim()))
                {
                    Alert("结束日期不能为空");
                    return;
                }
                TimeSpan day = DateTime.Parse(txtDateEnd.Text.Trim()) - DateTime.Parse(txtDateStr.Text.Trim());
                if (day.TotalDays <= 0)
                {
                    Alert("开始日期不能大于等于结束日期");
                }
                if (day.TotalDays > 31)
                {
                    Alert("日期范围不能大于31天");
                    //string js = "时间范围不能大于31天";
                    //this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), String.Format(" jAlert(\"{0}\");", js), true);
                    return;
                }
                string stationId = UCSelectStationCommon.StationID == "UCSelectStationCommon" ? "" : UCSelectStationCommon.StationID;
                List<RequisitionedFormModel> rList = requisitionedForm.GetRequisitionedOrderList(stationId, txtDateStr.Text.Trim(), txtDateEnd.Text.Trim());
                if (rList == null || rList.Count <= 0)
                    return;

                List<LoanInfoDto> LoanList = RequisitionedService.GetLoanInfoByOrderId(GetOrderId(rList));
                if (LoanList != null && LoanList.Count > 0)
                {
                    UniteModel(ref rList, LoanList);
                }

                gvList.DataSource = rList;
                gvList.DataBind();

                GetStat(rList);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
		}

        protected void btnSearch_ClickV2(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDateStr.Text.Trim()))
                {
                    Alert("开始日期不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(txtDateEnd.Text.Trim()))
                {
                    Alert("结束日期不能为空");
                    return;
                }
                TimeSpan day = DateTime.Parse(txtDateEnd.Text.Trim()) - DateTime.Parse(txtDateStr.Text.Trim());
                if (day.TotalDays <= 0)
                {
                    Alert("开始日期不能大于等于结束日期");
                }
                if (day.TotalDays > 31)
                {
                    Alert("日期范围不能大于31天");
                    //string js = "时间范围不能大于31天";
                    //this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), String.Format(" jAlert(\"{0}\");", js), true);
                    return;
                }
                string stationId = UCSelectStationCommon.StationID == "UCSelectStationCommon" ? "" : UCSelectStationCommon.StationID;
                List<RequisitionedFormModel> rList = requisitionedForm.GetRequisitionedOrderListV2(stationId, txtDateStr.Text.Trim(), txtDateEnd.Text.Trim());
                if (rList == null || rList.Count <= 0)
                    return;

                List<LoanInfoDto> LoanList = RequisitionedService.GetLoanInfoByOrderId(GetOrderId(rList));
                if (LoanList != null && LoanList.Count > 0)
                {
                    UniteModel(ref rList, LoanList);
                }

                gvList.DataSource = rList;
                gvList.DataBind();

                GetStat(rList);
            }
            catch (Exception ex)
            {
                Alert("查询出错<br>"+ex.Message);
            }
        }

		private List<string> GetOrderId(List<RequisitionedFormModel> rList)
		{
			List<string> l = new List<string>();
			foreach (RequisitionedFormModel m in rList)
			{
				l.Add(m.WaybillNO);
			}
			return l;
		}

		private void UniteModel(ref List<RequisitionedFormModel> rList, List<LoanInfoDto> LoanList)
		{
			foreach (RequisitionedFormModel m in rList)
			{
				var r = from t in LoanList
						where t.OrderId == m.WaybillNO
						select t ;

				List<LoanInfoDto> l = r.ToList();
				if (l.Count ==1)
				{
					m.RequisitionedNo = l[0].FormCode;
					m.RequisitionedBy = l[0].Borrower;
					m.Dept = l[0].DeptName;
					m.BuildBy = l[0].Maker;
				}
			}
		}

		private void GetStat(List<RequisitionedFormModel> rList)
		{
			if (rList == null || rList.Count <= 0)
				return;
			var stat = 
						   from t in rList
						   select new { t.CompanyName, t.WarehouseName, t.DeliveryFare,t.Weight,t.Dept } into tn
						   group tn by new { tn.CompanyName, tn.WarehouseName,tn.Dept } into g
						   select new RequisitionedStatModel
						   {
							   DeptName = g.Key.Dept,
							   CompanyName = g.Key.CompanyName,
							   WarehouseName = g.Key.WarehouseName,
							   FareSum = g.Sum(t => t.DeliveryFare),
							   CountNum = g.Count(),
							   WeightSum = g.Sum(t => t.Weight),
						   };

			var rStatList = stat.ToList();

			var s = new RequisitionedStatModel
			{
				CompanyName = "统计",
				WarehouseName = "",
				FareSum = rList.Select(t => t.DeliveryFare).Sum(),
				CountNum = rList.Count(),
				WeightSum = rList.Select(t => t.Weight).Sum(),
				DeptName = ""
			};

			rStatList.Add(s);

			gvListStat.DataSource = rStatList;
			gvListStat.DataBind();
		}

		protected void btnExprot_Click(object sender, EventArgs e)
		{
			try
			{
				if (gvList.Rows.Count <= 0)
				{
					Alert("请查询需要导出的数据");
					return;
				}
				List<string> l = new List<string>();
				foreach (DataControlField column in gvList.Columns)
				{
					l.Add(column.HeaderText);
				}
				ExportExcel(GridViewHelper.GridView2DataTable(gvList), l.ToArray(), "领用单明细");
			}
			catch (Exception ex)
			{
				Alert("导出失败");
			}
		}

		protected void gvListStat_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				if (e.Row.Cells[1].Text == "统计")
				{
					e.Row.BackColor = ColorTranslator.FromHtml("#A7DEF5");
				}
				else
				{
					e.Row.BackColor = Color.White;
				}
			}
		}
	}
}
