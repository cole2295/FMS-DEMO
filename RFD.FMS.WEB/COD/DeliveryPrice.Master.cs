using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util.ControlHelper;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.COD
{
	public partial class DeliveryPrice : System.Web.UI.MasterPage
	{
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                BindAreaType();
                BindAuditType();
                BindLinetType();
            }
			pager.PagerPageChanged += new EventHandler(pager_PageChanged);
		}

		protected void btSearch_Click(object sender, EventArgs e)
		{
			BindGridViewList(1);
		}

		public void BindGridViewList(int currentPageIndex)
		{
			if (currentPageIndex == -1)
			{
				currentPageIndex = pager.CurrentPageIndex;
			}

			gvList.DataSource = null;
			gvList.DataBind();
			PageInfo pi = new PageInfo(100);
			pager.PageSize = 100;
			pi.CurrentPageIndex = currentPageIndex;

			gvList.Columns[14].Visible = cbWaitEffect.Checked;

			JudgeConrrolExists("AddDeliveryPrice", !cbWaitEffect.Checked);
			JudgeConrrolExists("UpDateDeliveryPrice", !cbWaitEffect.Checked);
			JudgeConrrolExists("AddEffectDeliveryPrice", !cbWaitEffect.Checked);
			JudgeConrrolExists("UpdateEffectDeliveryPrice", cbWaitEffect.Checked);
            JudgeConrrolExists("BatchAddEffectDeliveryPrice", !cbWaitEffect.Checked);
            JudgeConrrolExists("BatchUpdateEffectDeliveryPrice", cbWaitEffect.Checked);
			JudgeConrrolExists("Delete", !cbWaitEffect.Checked);

			JudgeConrrolExists("Reset", !cbWaitEffect.Checked);
			JudgeConrrolExists("Audit", !cbWaitEffect.Checked);
			JudgeConrrolExists("ResetEffect", cbWaitEffect.Checked);
			JudgeConrrolExists("AuditEffect", cbWaitEffect.Checked);

            DataTable dt = GetSearchList(ref pi,true);

			pager.RecordCount = pi.ItemCount;
			gvList.DataSource = dt;
			gvList.DataBind();
			if (gvList.Rows.Count <= 0)
			{
				noData.Style.Add("display", "block");
				noData.Style.Add("text-align", "center");
				noData.Text = "查询无数据";
			}
			else
			{
				noData.Style.Add("display", "none");
			}
		}

        private DataTable GetSearchList(ref PageInfo pi,bool isPage)
        {
            string expressCompanyId = ucSelectStation.StationID == "ucSelectStation" ? "" : ucSelectStation.StationID;
            string lineStatus = LineStatus.SelectedValue == "-1" ? "" : LineStatus.SelectedValue;
            string auditStatus = AuditStatus.SelectedValue == "-1" ? "" : AuditStatus.SelectedValue;
            string areaType = AreaType.SelectedValue == "-1" ? "" : AreaType.SelectedValue;
            //string wareHouse1 = wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue.Replace("S_", "");
            //string wareHouseType = wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue.Contains("S_") ? "2" : "1";
            string wareHouse1 = "";
            string wareHouseType = "";
            bool waitEffect = cbWaitEffect.Checked;
            string merchantId = UCMerchantSourceTV.SelectMerchantID;
            int isCod = int.Parse(ddlIsCod.SelectedValue);
            return deliveryPriceService.GetDeliveryPriceList(expressCompanyId, lineStatus, auditStatus, areaType, wareHouse1, wareHouseType, waitEffect, merchantId,DistributionCode,isCod, ref pi, isPage);
        }

		protected void pager_PageChanged(object sender, EventArgs e)
		{
			BindGridViewList(pager.CurrentPageIndex);
		}

		private void JudgeConrrolExists(string btName, bool enableFlag)
		{
			Button b = OperateContent.FindControl(btName) as Button;
			if (b != null)
				b.Enabled = enableFlag;
		}

		protected void gvListData_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["expressCompanyId"] = ucSelectStation.StationID == "ucSelectStation" ? "" : ucSelectStation.StationID;
			e.InputParameters["lineStatus"] = LineStatus.SelectedValue == "-1" ? "" : LineStatus.SelectedValue;
			e.InputParameters["auditStatus"] = AuditStatus.SelectedValue == "-1" ? "" : AuditStatus.SelectedValue;
			e.InputParameters["areaType"] = AreaType.SelectedValue == "-1" ? "" : AreaType.SelectedValue;
			//e.InputParameters["wareHouse"] = wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue.Replace("S_", "");
			//e.InputParameters["wareHouseType"] = wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue.Contains("S_") ? "2" : "1";
            e.InputParameters["wareHouse"] = "";
            e.InputParameters["wareHouseType"] = "";
			e.InputParameters["waitEffect"] = cbWaitEffect.Checked;
		}

		/// <summary>
		/// GridView控件
		/// </summary>
		public GridView gvListControl
		{
			get { return gvList; }
		}

		public int gvListCount
		{
			get { return gvList.Rows.Count; }
		}

		/// <summary>
		/// gridview 列集合
		/// </summary>
		public DataControlFieldCollection gvListColumns
		{
			get { return gvListControl.Columns; }
		}

		/// <summary>
		/// griview 数组列
		/// </summary>
		public string[] gvListColumnArray
		{
            get
            {
                string[] DataColumns = { "价格配置编号", "配送商", "商家", "区域类型", "是否区分COD", "COD计算公式（元|票）", "非COD计算公式（元|票）", "线路状态", "审核状态", "最后修改", "生效日期" };
                return DataColumns;
            }
		}

		public DataTable gvListDataTable
		{
            get
            {
                PageInfo pi = new PageInfo(0);
                DataTable dt = GetSearchList(ref pi, false);

                DataTable dtResult = CreateDataTable(dt);

                return dtResult;
            }
		}

        private DataTable CreateDataTable(DataTable dtData)
        {
            DataTable dt = new DataTable();
            foreach (string s in gvListColumnArray)
            {
                dt.Columns.Add(s, typeof(String));
            }
            foreach (DataRow dr in dtData.Rows)
            {
                DataRow drNew=dt.NewRow();
                if (dtData.Columns.Contains("CODLineNO"))
                    drNew["价格配置编号"] = dr["CODLineNO"].ToString();

                if (dtData.Columns.Contains("CompanyName"))
                    drNew["配送商"] = dr["CompanyName"].ToString();

                //if (dtData.Columns.Contains("IsEchelonStr"))
                //    drNew["是否按发货地梯次收费"] = dr["IsEchelonStr"].ToString();

                //if (dtData.Columns.Contains("WarehouseName"))
                //    drNew["仓库/分拣中心"] = dr["WarehouseName"].ToString();

                if (dtData.Columns.Contains("MerchantName"))
                    drNew["商家"] = dr["MerchantName"].ToString();

                if (dtData.Columns.Contains("AreaType"))
                    drNew["区域类型"] = dr["AreaType"].ToString();

                if (dtData.Columns.Contains("IsCODStr"))
                    drNew["是否区分COD"] = dr["IsCODStr"].ToString();

                if (dtData.Columns.Contains("PriceFormula"))
                    drNew["COD计算公式（元|票）"] = dr["PriceFormula"].ToString();

                if (dtData.Columns.Contains("Formula"))
                    drNew["非COD计算公式（元|票）"] = dr["Formula"].ToString();

                if (dtData.Columns.Contains("LineStatusStr"))
                    drNew["线路状态"] = dr["LineStatusStr"].ToString();

                if (dtData.Columns.Contains("AuditStatusStr"))
                    drNew["审核状态"] = dr["AuditStatusStr"].ToString();

                if (dtData.Columns.Contains("UpdateBy"))
                    drNew["最后修改"] = dr["UpdateBy"].ToString();

                if (dtData.Columns.Contains("EffectDate"))
                    drNew["生效日期"] = dr["EffectDate"].ToString();

                dt.Rows.Add(drNew);
            }
            return dt;
        }

		public IList<KeyValuePair<string, string>> gvListChecked
		{
			get { return GridViewHelper.GetSelectedRows<string>(gvList, "cbCheck", 13); }
		}

        public IList<KeyValuePair<string, string>> gvListCheckedByAreaType
        {
            get { return GridViewHelper.GetSelectedRows<string>(gvList, "cbCheck", 6); }
        }

        //public void BindWarehouse(string disCode)
        //{
        //    DataTable data = new WareHouseService().GetWareHouseSortCenter(disCode);
        //    wareHouse.BindListData(data, "WarehouseName", "WarehouseId", "所有", "-1");
        //}

        public void BindAreaType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(AreaType, "所有", "-1", "AreaType", DistributionCode);
        }

        public void BindAuditType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(AuditStatus, "所有", "-1", "AreaTypeAudit", DistributionCode);
        }

        public void BindLinetType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(LineStatus, "所有", "-1", "AreaTypeLine", DistributionCode);
        }

        private string _distributionCode;
        /// <summary>
        /// 配送商编码
        /// </summary>
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
	}
}
