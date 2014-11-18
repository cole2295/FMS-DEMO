using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class StatusCodeEdit :BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(CodeNo))
                {
                    InitForm();
                }
            }
        }

        private void InitForm()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            InfoMode = service.GetModel(CodeType, CodeNo, base.DistributionCode,false);
            if (InfoMode == null)
                return;

            txtCodeDesc.Text = InfoMode.CodeDesc;
            rbFalse.Checked = InfoMode.Enable ? false : true;
            rbTrue.Checked = InfoMode.Enable ? true : false;
        }

        private StatusCodeInfo InfoMode
        {
            get { return ViewState["InfoMode"] == null ? null : ViewState["InfoMode"] as StatusCodeInfo; }
            set { ViewState.Add("InfoMode", value); }
        }

        private string CodeType
        {
            get
            {
                return string.IsNullOrEmpty(Request.QueryString["codeType"]) ? null : Request["codeType"].ToString();
            }
        }

        private string CodeNo
        {
            get
            {
                return string.IsNullOrEmpty(Request.QueryString["codeNo"]) ? null : Request["codeNo"].ToString();
            }
        }

        protected void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                StatusCodeInfo sci = GetModel();

                if (string.IsNullOrEmpty(CodeNo))
                {
                    AddStatusCode(sci);
                }
                else
                {
                    UpdateStatusCode(sci);
                }
            }
            catch (Exception ex)
            {
                Alert("操作错误<br>"+ex.Message);
            }
        }

        public void AddStatusCode(StatusCodeInfo sci)
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            if (service.AddByRelationType(sci))
            {
                RunJS("alert('添加成功');window.close();");
            }
            else
            {
                RunJS("alert('添加失败');window.close();");
            }
        }

        public void UpdateStatusCode(StatusCodeInfo sci)
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            if (service.Update(sci))
            {
                RunJS("alert('更新成功');window.close();");
            }
            else
            {
                RunJS("alert('更新失败');window.close();");
            }
        }

        public StatusCodeInfo GetModel()
        {
            if (string.IsNullOrEmpty(CodeType))
            {
                throw new Exception("获取分类源失败");
            }

            if(string.IsNullOrEmpty(txtCodeDesc.Text.Trim()))
                throw new Exception("名称不能为空");

            if (txtCodeDesc.Text.Length > 11)
            {
                throw new Exception("名称不能超过10个汉字");
            }

            StatusCodeInfo sci = new StatusCodeInfo();

            sci.CodeType = CodeType;
            sci.CodeNo = string.IsNullOrEmpty(CodeNo) ? "" : CodeNo;//系统生成
            sci.OrderBy = InfoMode == null ? 0 : InfoMode.OrderBy;
            sci.CodeDesc = txtCodeDesc.Text.Trim();
            sci.Enable = rbFalse.Checked ? false : true;
            sci.CreatBy = InfoMode == null ? base.Userid : InfoMode.CreatBy;
            sci.UpdateBy = base.Userid;
            sci.DistributionCode = InfoMode == null ? base.DistributionCode : InfoMode.DistributionCode;
            return sci;
        }
    }
}