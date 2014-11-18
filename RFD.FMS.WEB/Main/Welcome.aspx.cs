using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using RFD.FMS.Util;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.PMSOpenService;
using RFD.FMS.MODEL;


namespace RFD.FMS.WEB.Main
{
    public partial class Welcome : BasePage
    {
        protected string noticeHtml = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                noticeHtml=LoadNotice();
            }
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            Response.Redirect("http://lms.wuliusys.com/BasicSetting/ChangePassword.aspx");
            //string url = "http://lms.wuliusys.com/BasicSetting/ChangePassword.aspx";//http://lms.wuliusys.com/BasicSetting/ChangePassword.aspx
            //RunJS(MenuLibrary.GotoPasswordChanage(url).JsString);
        }

        private string LoadNotice()
        {
            try
            {
                PermissionOpenServiceClient client = new PermissionOpenServiceClient();
                var notice = client.GetSysNotice((int)RFD.FMS.MODEL.BizEnums.SystemType.LMS_RFD_FMS, base.DistributionCode);//
                if (string.IsNullOrEmpty(notice.NoitceContent))
                    return GetDefaultNotice();
                return notice.NoitceContent;
            }
            catch
            {
                return GetDefaultNotice();
            }

        }


        private string GetDefaultNotice()
        {
            return
                @"<div>
                <div style='color: Red; width:100%'>
                    <center>
                        <h2>
                            通 知</h2>
                    </center>
                    <p>
                        为了合理、高效的使用财务系统，不造成数据混乱，现特将以下经验和提示告知大家，请操作的时候注意：</p>
                    <p>
                        1、IE浏览器的设置：一定要设置为IE7或IE8，若不是请立即升级；</p>
                    <p>
                        2、打印机设置：一定要按照要求设置；</p>
                    <p>
                        3、您的电脑必须安装PDF阅读工具；</p>
                    <p>
                        4、新系统上线后一定不要在老系统中操作。</p>
                    <p  class='pdate'>
                        2012-07-16</p>
                </div>
            </div>";
        }
    }
}
