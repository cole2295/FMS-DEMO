using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MerchantDeliverFeeHistory : BasePage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindMerchantDeliverFeeHistory();
			}
		}

		private void BindMerchantDeliverFeeHistory()
		{
			var condition = new SearchCondition()
			{
				MerchantID = CurrentMerchant
			};
            var data = deliverFeeService.BindDeliverFeeListHistory(condition);
			//绑定商家名称
            this.lblMerchantName.Text = "商家：" + deliverFeeService.GetMerchantNameFromDictionary(CurrentMerchant);
			if (data != null)
			{
				if (data.Rows.Count == 0)
				{
					//无数据时显示表头
					if (Request["status"] != ((int)MaintainStatus.Auditing).ToString())
					{
						GridViewHelper.ShowEmptyGridHeader(gvDeliverFeeHistory, data, "该商家无任何配送费历史数据！");
					}
					else
					{
						GridViewHelper.ShowEmptyGridHeader(gvDeliverFeeHistory, data, "该商家无任何可操作的配送费数据！");
					}
					return;
				}
				gvDeliverFeeHistory.DataSource = data;
				gvDeliverFeeHistory.DataBind();
				gvDeliverFeeHistory.RowStyle.HorizontalAlign = HorizontalAlign.Left;
			}
		}

		protected void btnReturn_Click(object sender, EventArgs e)
		{
			switch (Option)
			{
				case "m":
					Response.Redirect("UpdateDeliverFee.aspx?op=m&mid=" + CurrentMerchant);
					break;
				case "a":
					Response.Redirect("AuditDeliverFee.aspx");
					break;
				case "q":
					Response.Redirect("QueryDeliverFee.aspx");
					break;
			}
		}

		public int CurrentMerchant
		{
			get
			{
				return Request["mid"] != null ? int.Parse(Request["mid"]) : -1;
			}
		}

		public string Option
		{
			get
			{
				return Request["op"] ?? "m";
			}
		}

		protected void btnReject_Click(object sender, EventArgs e)
		{
			//驳回状态为"待维护"
			var result = UpdateDeliverFeeStatus(MaintainStatus.Maintain, AuditResult.Reject);
			if (!result)
			{
				Alert("驳回失败!");
				return;
			}
			Response.Redirect("AuditDeliverFee.aspx?op=a&mid=" + CurrentMerchant + "&status=" + (int)MaintainStatus.Maintain);
		}

		protected void btnAudited_Click(object sender, EventArgs e)
		{
			//审核状态为"已审核"
			var result = UpdateDeliverFeeStatus(MaintainStatus.Audited, AuditResult.Audited);
			if (!result)
			{
				Alert("审核失败!");
				return;
			}
			Response.Redirect("AuditDeliverFee.aspx?op=a&mid=" + CurrentMerchant + "&status=" + (int)MaintainStatus.Audited);
		}

		private bool UpdateDeliverFeeStatus(MaintainStatus currentStatus, AuditResult currentResult)
		{
			var merchant = new FMS_MerchantDeliverFee()
			{
				MerchantID = CurrentMerchant,
				AuditBy = Userid,
				AuditTime = DateTime.Now,
				AuditCode = UserCode,
				Status = currentStatus,
				AuditResult = currentResult
			};
			var station = new FMS_StationDeliverFee()
			{
				MerchantID = CurrentMerchant,
				AuditBy = Userid,
				AuditTime = DateTime.Now,
				AuditCode = UserCode,
				//Status = currentStatus,
				AuditResult = currentResult
			};
			//return service.AuditMerchantDeliverFee(merchant, station);
			return false;
		}
	}
}
