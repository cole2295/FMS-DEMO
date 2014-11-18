using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class StationDeliverFeeHistory : BasePage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();

		/// <summary>
		/// 商家ID
		/// </summary>
		protected string MerchantID
		{
			get
			{
				if (!string.IsNullOrEmpty(Request.QueryString["MerchantID"]))
				{
					return Request.QueryString["MerchantID"].ToString();
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// 站点ID
		/// </summary>
		protected string StationID
		{
			get
			{
				if (!string.IsNullOrEmpty(Request.QueryString["StationID"]))
				{
					return Request.QueryString["StationID"].ToString();
				}
				return string.Empty;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			BindData();
		}

		/// <summary>
		/// 绑定数据
		/// </summary>
		private void BindData()
		{
			if (string.IsNullOrEmpty(MerchantID)
				|| string.IsNullOrEmpty(StationID))
			{
				Alert("没有数据");
				return;
			}
            lblMerchantName.Text = "商家: " + deliverFeeService.GetMerchantNameFromDictionary(int.Parse(MerchantID));
			FMS_StationDeliverFee model = new FMS_StationDeliverFee()
			{
				MerchantID = int.Parse(this.MerchantID),
				StationID = int.Parse(this.StationID)
			};
			//var data = bll.SearchStationDeliverFeeHistory(model);
			//if (data != null)
			//{
			//    if (data.Rows.Count == 0)
			//    {
			//        GridViewHelper.ShowEmptyGridHeader(gvDeliverFeeHistory, data);
			//        return;
			//    }               
			//    gvDeliverFeeHistory.DataSource = data;
			//    gvDeliverFeeHistory.DataBind();
			//}
		}

		protected void btnReturn_Click(object sender, EventArgs e)
		{
			Response.Redirect("BasicDeliverFee.aspx?mid=" + MerchantID);
		}

		protected void gvDeliverFeeHistory_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				DataRowView itemDataSource = e.Row.DataItem as DataRowView;
                if (itemDataSource != null)
                {
                    if (!itemDataSource["AuditResult"].IsNullData())
                    {
                        int nAuditResult = int.Parse(itemDataSource["AuditResult"].ToString());
                        //AuditResult
                        e.Row.Cells[8].Text = EnumUtil.GetEnumDescriptionByText(typeof(AuditResult), ((AuditResult)nAuditResult).ToString());
                    }
                    if (!itemDataSource["status"].IsNullData())
                    {
                        int nStatus = int.Parse(itemDataSource["status"].ToString());
                        //Status
                        e.Row.Cells[9].Text = EnumUtil.GetEnumDescriptionByText(typeof(MaintainStatus), ((MaintainStatus)nStatus).ToString());
                    }
                }
			}
		}

	}
}
