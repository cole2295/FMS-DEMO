using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Model;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class IncomeAreaExpressLevelAuditV2 : BasePage
    {
        IAreaExpressLevelIncomeService areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
                Master.DistributionCode = base.DistributionCode;
            }
        }

        private void InitForm()
        {
            txtDoDate.Text = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtEffect;
                if (!DateTime.TryParse(txtDoDate.Text, out dtEffect))
                {
                    Alert("时间格式不正确");
                    return;
                }
                TimeSpan day = dtEffect - DateTime.Now;
                if (day.TotalDays <= 0)
                {
                    Alert("生效日期必须大于当天日期");
                    return;
                }

                IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs = Master.GridViewChecked;
                if (!JudgeInput(keyValuePairs, 0))
                    return;
                List<AreaExpressLevelIncome> areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
                foreach (var keyPair in keyValuePairs)
                {
                    DataKey dk = keyPair.Key as DataKey;
                    GridViewRow gvr = keyPair.Value;

                    AreaExpressLevelIncome areaExpressLevelIncome = new AreaExpressLevelIncome();
                    areaExpressLevelIncome.AutoId = int.Parse(dk.Values[0].ToString());
                    areaExpressLevelIncome.AuditBy = base.Userid;
                    areaExpressLevelIncome.DoDate = DateTime.Parse(txtDoDate.Text.Trim());
                    areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S2;
                    areaExpressLevelIncome.DistributionCode = base.DistributionCode;
                    areaExpressLevelIncomes.Add(areaExpressLevelIncome);
                }
                if (areaExpressLevelIncomes.Count <= 0)
                {
                    Alert("未能找到审批行");
                    return;
                }
                var areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                string msg = string.Empty;
                if (areaExpressLevelIncomeService.UpdateAreaLevelIncomeStatus(areaExpressLevelIncomes))
                {
                    Master.SearchData(Master.UCPager.CurrentPageIndex);
                    Alert("审批成功");
                }
                else
                    Alert("审批失败");
            }
            catch (Exception ex)
            {
                Alert("审批失败<br>" + ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs = Master.GridViewChecked;
                if (!JudgeInput(keyValuePairs, 1))
                    return;
                List<AreaExpressLevelIncome> areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
                foreach (var keyPair in keyValuePairs)
                {
                    DataKey dk = keyPair.Key as DataKey;
                    GridViewRow gvr = keyPair.Value;

                    AreaExpressLevelIncome areaExpressLevelIncome = new AreaExpressLevelIncome();
                    areaExpressLevelIncome.AutoId = int.Parse(dk.Values[0].ToString());
                    areaExpressLevelIncome.AuditBy = base.Userid;
                    areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S4;
                    areaExpressLevelIncome.DistributionCode = base.DistributionCode;
                    areaExpressLevelIncomes.Add(areaExpressLevelIncome);
                }
                if (areaExpressLevelIncomes.Count <= 0)
                {
                    Alert("未能找到置回行");
                    return;
                }
                var areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                string msg = string.Empty;
                if (areaExpressLevelIncomeService.UpdateAreaLevelIncomeStatus(areaExpressLevelIncomes))
                {
                    Master.SearchData(Master.UCPager.CurrentPageIndex);
                    Alert("置回成功");
                }
                else
                    Alert("置回失败");
            }
            catch (Exception ex)
            {
                Alert("置回失败<br>" + ex.Message);
            }
        }

        public bool JudgeInput(IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs, int n)
        {
            int auditColumnIndex = 11;
            if (keyValuePairs.Count <= 0)
            {
                Alert(string.Format("请选择需要操作的行"));
                return false;
            }

            foreach (var k in keyValuePairs)
            {
                if (k.Value.Cells[auditColumnIndex].Text == EnumHelper.GetDescription((AreaLevelStatus.S0)))
                {
                    Alert("只能操作非代维护状态的行");
                    return false;
                }
            }

            if (n == 0)
            {
                foreach (var k in keyValuePairs)
                {
                    if (k.Value.Cells[auditColumnIndex].Text == EnumHelper.GetDescription((AreaLevelStatus.S2)))
                    {
                        Alert("不只能包含已审核状态的行");
                        return false;
                    }
                }
            }

            if (n == 1)
            {
                foreach (var k in keyValuePairs)
                {
                    if (k.Value.Cells[auditColumnIndex].Text == EnumHelper.GetDescription((AreaLevelStatus.S4)))
                    {
                        Alert("不能包含已置回状态的行");
                        return false;
                    }
                }
            }

            return true;
        }

      
    }
}