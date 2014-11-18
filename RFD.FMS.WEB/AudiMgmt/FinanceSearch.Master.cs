using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.AudiMgmt
{
	public partial class FinanceSearch : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindOrderSource();
				BindExportFormat();
			}
		}

		#region 绑定数据
		/// <summary>
		/// 绑定订单来源
		/// </summary>
		public void BindOrderSource()
		{
			var data = EnumHelper.GetEnumValueAndDescriptions<WaybillSourse>();
			ddlOrderSource.BindListData(data);
		}
		/// <summary>
		/// 绑定商家来源
		/// </summary>
		public void BindMerchantSource(string disCode)
		{
            var service = ServiceLocator.GetService<IMerchantService>();
            var data = service.GetMerchants(disCode);
			ddlMerchantList.BindListData(data, "MerchantName", "ID",string.Empty);
		}
		public void BindExportFormat()
		{
			var data = EnumHelper.GetEnumValueAndDescriptions<ExportFileFormat>();
			rblExportFormat.BindListData(data, "");
		}
		#endregion

		#region 属性(供子页面使用)
		public DropDownList OrderSourceDropDownList
		{
			get { return ddlOrderSource; }
		}
		public DropDownList MerchantSourceDropDownList
		{
			get { return ddlMerchantList; }
		}
		public ExportFileFormat CurrentExportFormat
		{
			get { return (ExportFileFormat)rblExportFormat.SelectedIndex; }
		}
		public bool IsShowCustomerOrder
		{
			get
			{
				return this.ddlOrderSource.SelectedValue == ((int)WaybillSourse.Other).ToString();
			}
		}
		public bool IsShowMerchantName
		{
			get
			{
				return IsShowCustomerOrder || this.ddlOrderSource.SelectedIndex == 0;
			}
		}
		#endregion
	}
}
