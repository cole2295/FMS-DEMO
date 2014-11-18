using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class DeliverFee : System.Web.UI.MasterPage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            UCPager.PagerPageChanged += new EventHandler(UCPager_PageChanged);
		}

        protected void UCPager_PageChanged(object sender, EventArgs e)
        {
            BindDeliverFeeList(UCPager.CurrentPageIndex);
        }

		protected void btnQuery_Click(object sender, EventArgs e)
		{
			if (SelectedStatusList == string.Empty)
			{
				lblReturn.Style.Add("display", "block");
				lblReturn.Text = "查询无数据";
			}
			else
			{ 
				lblReturn.Style.Add("display", "none");
				BindDeliverFeeList(1);
			}				
		}

        private void JudgeConrrolExists(string btName, bool enableFlag)
        {
            Button b = OperatorArea.FindControl(btName) as Button;
            if (b != null)
                b.Enabled = enableFlag;
        }

		public void BindDeliverFeeList(int n)
		{
            gvDeliverFeeList.DataSource = null;
            gvDeliverFeeList.DataBind();
            gvDeliverFeeList.Columns[1].Visible = cbEffect.Checked;
            Button button = OperatorArea.FindControl("btnViewPrice") as Button;
            if (button == null)
                gvDeliverFeeList.Columns[29].Visible = false;
            else
                gvDeliverFeeList.Columns[29].Visible = true;

            JudgeConrrolExists("btnEditPrice", !cbEffect.Checked);
            JudgeConrrolExists("btnEditBasic", !cbEffect.Checked);
            JudgeConrrolExists("btnAddEffectBasic", !cbEffect.Checked);
            JudgeConrrolExists("btnUpdateEffectBasic", cbEffect.Checked);

            JudgeConrrolExists("btnAuditBasic", !cbEffect.Checked);
            JudgeConrrolExists("btnResetBaic", !cbEffect.Checked);
            JudgeConrrolExists("btAuditEffectBasic", cbEffect.Checked);
            JudgeConrrolExists("btnResetEffectBasic", cbEffect.Checked);

			var condition = new SearchCondition()
			{
				MerchantID = CurrentMerchant,
				MerchantName = txtMerchantName.Text.Trim(),
				SimpleSpell = txtSimpleSpell.Text.Trim(),
				StatusList = SelectedStatusList,
				IsRawData = true,
                DistributionCode=DistributionCode,
                IsEffect=cbEffect.Checked,
			};
            PageInfo pi = new PageInfo(UCPager.PageSize);
            pi.CurrentPageIndex = n;
            var data = deliverFeeService.BindDeliverFeeList(condition, ref pi);
            UCPager.RecordCount = pi.ItemCount;

            
			if (data != null)
			{
				if (data.Rows.Count == 0)
				{
					//无数据时显示表头
					GridViewHelper.ShowEmptyGridHeader(gvDeliverFeeList, data);
					return;
				}
				gvDeliverFeeList.DataSource = data;
				gvDeliverFeeList.DataBind();
				gvDeliverFeeList.RowStyle.HorizontalAlign = HorizontalAlign.Left;
			}
		}

		public int CurrentMerchant
		{
			get
			{
				var merchantID = txtMerchantID.Text.Trim();
				if (String.IsNullOrEmpty(merchantID) || !StringUtil.IsNumeric(merchantID))
					return -1;
				return Convert.ToInt32(merchantID);
			}
		}
		/// <summary>
		/// 选中的维护状态列表
		/// </summary>
		public string SelectedStatusList
		{
			get
			{
				var statusList = string.Empty;
				foreach (ListItem li in cblMaintainStatus.Items)
				{
					if (li.Selected)
					{
						statusList += String.IsNullOrEmpty(statusList) ?
							"'" + li.Value + "'" : ",'" + li.Value + "'";
					}
				}
				return statusList;
			}
		}

		public TextBox MerchantIDTextBox
		{
			get { return txtMerchantID; }
			set { txtMerchantID = value; }
		}

		public CheckBoxList MaintainStatusCheckBoxList
		{
			get { return cblMaintainStatus; }
			set { cblMaintainStatus = value; }
		}

		/// <summary>
		/// 列表选中
		/// </summary>
		public IList<KeyValuePair<string,string>> MerchantChecked
		{
			get
			{
				return GridViewHelper.GetSelectedRows<string>(gvDeliverFeeList, "cbCheck", 5);
			}
		}

        private string _distributionCode;
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
	}
}
