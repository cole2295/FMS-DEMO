using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util;
using RFD.FMS.Service.AudiMgmt;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEBLOGIC.AudiMgmt;

namespace RFD.FMS.WEB.AudiMgmt
{
    public partial class FinanceCompare : BasePage
    {
        IFinanceService service = ServiceLocator.GetService<IFinanceService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "buttons", "enableButtons();", true);
        }

        public string GetStatusName(StatusType status, object statusNo)
        {
            var statusInfoService = ServiceLocator.GetService<IStatusInfoService>();
            return statusInfoService.GetStatusNameByTypeCode((int)status, statusNo.ToString());
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            var condition = new SearchCondition()
            {
                DeliverStation = Convert.ToInt32(station.ID),
                SearchDate = Convert.ToDateTime(this.txtDeliverTime.Text)
            };
            var data = service.GetSystemWaybillInfo(condition);
            if (data != null)
            {
                ViewState["TotalData"] = data;
                this.hidTotalCount.Value = data.Rows.Count.ToString();
                this.gvSysOrders.DataSource = data;
                this.gvSysOrders.DataBind();
            }
        }
       
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gvSysOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "system",
                @"gridViewColor('" + gvSysOrders.ClientID + "',{count:" + int.Parse(this.hidTotalCount.Value) + ",clickable:false});", true);
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            //导入成功则设置成功标志以便打开导出对碰按钮
            var data = ImportExcel(this.txtFile);
            if (data != null)
            {
                Alert("导入成功！");
                this.hidImportSign.Value = "1";
                ViewState["ImportData"] = data;
            }
            else
            {
                this.hidImportSign.Value = "0";
            }
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), @"gridViewColor('" + gvSysOrders.ClientID + "',{count:" + int.Parse(this.hidTotalCount.Value) + ",clickable:false});", true);
        }

        protected void btnExportSysOrder_Click(object sender, EventArgs e)
        {
            var data = GetCompareOrder(CompareDirection.SystemToManual, "运单号");
            if (data != null)
            {
                var title = String.Format("{0}[{1}]", this.station.Name, this.txtDeliverTime.Text);
                ExportExcel(data, null, title + "系统对碰订单表");
            }
        }

        protected void btnExportManualOrder_Click(object sender, EventArgs e)
        {
            var data = GetCompareOrder(CompareDirection.ManualToSystem, "运单号");
            if (data != null)
            {
                var title = String.Format("{0}[{1}]", this.station.Name, this.txtDeliverTime.Text);
                ExportExcel(data, null, title + "手工对碰订单表");
            }
        }

        #region 订单对碰
        /// <summary>
        /// 根据报表对碰方向获取差异报表
        /// </summary>
        /// <param name="direction">报表对碰方向</param>
        /// <param name="keyField">比较的关键字段</param>
        /// <returns></returns>
        private DataTable GetCompareOrder(CompareDirection direction, string keyField)
        {
            var systemOrder = ViewState["TotalData"] as DataTable;
            var manualOrder = ViewState["ImportData"] as DataTable;
            var result = systemOrder.Clone();//复制表结构

            //添加差异原因列
            result.Columns.Add(new DataColumn("差异原因", typeof(String)));

            if (systemOrder != null && systemOrder.Rows.Count > 0 &&
                manualOrder != null && manualOrder.Rows.Count > 0)
            {
                var dvSysOrder = systemOrder.DefaultView;
                var dvManOrder = manualOrder.DefaultView;

                switch (direction)
                {
                    case CompareDirection.SystemToManual:
                        result = CompareOrder(dvSysOrder, dvManOrder, keyField);
                        break;
                    case CompareDirection.ManualToSystem:
                        result = CompareOrder(dvManOrder, dvSysOrder, keyField);
                        break;
                }
            }
            return result;
        }

        private DataTable CompareOrder(DataView dv1, DataView dv2, string keyField)
        {
            var dtCompare = new DataTable();
            var columns = this.hidCompareFields.Value.Trim().Split(',');
            foreach (DataRowView drv in dv1)
            {
                dv2.RowFilter = keyField + "='" + drv[keyField] + "'";
                if (dv2.Count > 0)
                {
                    var resaon = "";
                    if (!CompareUpdate(drv, dv2[0], columns, out resaon))
                    {
                        dtCompare.Rows.Add(drv.Row.ItemArray);
                        dtCompare.Select(dv2.RowFilter).First()["差异原因"] = resaon;
                    }
                }
                else
                {
                    dtCompare.Rows.Add(drv.Row.ItemArray);
                    dtCompare.Select(dv2.RowFilter).First()["差异原因"] = "多余订单";
                }
            }
            dv2.RowFilter = "";
            foreach (DataRowView drv in dv2)
            {
                dv1.RowFilter = keyField + "='" + drv[keyField] + "'";
                if (dv1.Count == 0)
                {
                    dtCompare.Rows.Add(drv.Row.ItemArray);
                    dtCompare.Select(dv1.RowFilter).First()["差异原因"] = "遗漏订单";
                }
            }
            return dtCompare;
        }

        /// <summary>
        /// 比较DataRow是否有不同
        /// </summary>
        /// <param name="dr1">第1个DataTable</param>
        /// <param name="dr2">第2个DataTable</param>
        /// <param name="columns">指定的差异列</param>
        /// <param name="reason">差异原因</param>
        /// <returns>是否存在差异</returns>
        private static bool CompareUpdate(DataRowView dr1, DataRowView dr2, string[] columns, out string reason)
        {
            //行里只要指定项不一样，整个行就不一样
            object val1;
            object val2;
            var isSameRow = true;
            reason = string.Empty;
            foreach (string key in columns)
            {
                val1 = dr1[key];
                val2 = dr2[key];
                if (!val1.Equals(val2))
                {
                    isSameRow = false;
                    reason += key + "不一致！\n";
                }
            }
            return isSameRow;
        }
        #endregion
    }
}
