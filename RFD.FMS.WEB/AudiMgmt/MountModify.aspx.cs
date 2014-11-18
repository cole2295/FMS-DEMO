using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util;
using RFD.FMS.Service.AudiMgmt;

namespace RFD.FMS.WEB.AudiMgmt
{
    public partial class MountModify : FMSBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            IFinanceService service = ServiceLocator.GetService<IFinanceService>();
            //
            decimal dMount = 0;
            if(!decimal.TryParse(Mount.Text.Trim(),out dMount))
            {
                Label1.Text="金额输入错误!";
                return;
            }
            Int64 iWaybillNO = 0;
            if (!Int64.TryParse(WaybillNO.Text.Trim(), out iWaybillNO))
            {
                Label1.Text="运单号输入错误!";
                return;
            }
            if(service.UpdateMount(this.WaybillNO.Text.Trim(), dMount, int.Parse(dplType.SelectedValue)))
            {
                
                Response.Write("<script>alert('修改成功!!!!');</script>");
            }
            else
            {
                Label1.Text = "修改失败!";
                return;
               
            }

        }

        protected void btnGetMount_Click(object sender, EventArgs e)
        {
            //LablePrintPriceDTO op = WMSPrintPriceService.GetObsOrderPrice(Convert.ToInt64(WaybillNO.Text));
            //if (op != null)
            //{
            //    if (op.Refund>0)
            //    {
            //        Mount.Text = op.Refund.ToString();
            //    }
            //    else
            //    {
            //        Mount.Text = op.Collection.ToString();
            //    }
            //}
            //else
            //{
            //    Response.Write("<script>alert('没有找到对应订单!!!!');</script>");
            //}
        }

        protected void btnCreateReport_Click(object sender, EventArgs e)
        { 
            
            if (!String.IsNullOrEmpty(txtWaybillNOs.Text))
            {
                List<string> waybillNOList = txtWaybillNOs.Text.Split(',').ToList();
                IStationDailyService sdService = ServiceLocator.GetService<IStationDailyService>();
               
                if(sdService.ReloadStationDailyByWaybillNos(txtWaybillNOs.Text)==waybillNOList.Count)
                {
                    Response.Write("<script>alert('报表重新生成成功!');</script>");
                }
                else
                {
                    Response.Write("<script>alert('报表重新生成失败!');</script>");
                }

            }
        }

        protected void btnCreateReport_ClickV2(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(txtWaybillNOs.Text))
            {
                List<string> waybillNOList = txtWaybillNOs.Text.Split(',').ToList();

                IStationDailyService sdService = ServiceLocator.GetService<IStationDailyService>();

                if (sdService.ReloadStationDailyByWaybillNosV2(txtWaybillNOs.Text) == waybillNOList.Count)
                {
                    Response.Write("<script>alert('报表重新生成成功!');</script>");
                }
                else
                {
                    Response.Write("<script>alert('报表重新生成失败!');</script>");
                }
            }
        }
    }
}
