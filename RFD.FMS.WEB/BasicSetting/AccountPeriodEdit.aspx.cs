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
using System.Web.Services;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AccountPeriodEdit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                IsImitate = string.IsNullOrEmpty(Request.QueryString["isImitate"]) ? 0 : int.Parse(Request["isImitate"].ToString());
                PeriodSource = string.IsNullOrEmpty(Request.QueryString["periodSource"]) ? "" : Request["periodSource"].ToString();
                Kid = string.IsNullOrEmpty(Request.QueryString["kid"]) ? "" : Request["kid"].ToString();
                UserId = base.Userid.ToString();
                DisCode = base.DistributionCode;
                if (!string.IsNullOrEmpty(Kid))
                {
                    LoadEditData();
                }
            }
        }

        private void LoadEditData()
        {
            IAccountPeriodService accounrPeriodService = ServiceLocator.GetService<IAccountPeriodService>();
            AccountPeriodCondition apc = new AccountPeriodCondition()
            {
                PeriodRelationName = PeriodSource,
                AccountPeriodKid=Kid,
            };
            List<AccountPeriod> spList = accounrPeriodService.SearchAccountPeriod(apc);
            if (spList == null || spList.Count != 1)
            {
                Alert("读取数据错误");
                return;
            }
            hidExpressId.Value = spList[0].ExpressIds;
            hidIntervalNum.Value = spList[0].IntervalNum.ToString();
            hidIsExpress.Value = spList[0].IsExpress.ToString();
            hidIsMonthPeriod.Value = spList[0].IsMonthPeriod.ToString();
            hidPeriodStart.Value = spList[0].PeriodStart;
            hidPeriodType.Value = spList[0].PeriodType.ToString();
            hidPeriodTypeChild.Value = spList[0].PeriodTypeChild.ToString();
            hidIsDeleted.Value = spList[0].IsDeleted.ToString();
            txtPeriodName.Text = spList[0].PeriodName;
            PeriodSource = spList[0].PeriodRelationName;
            UCExpressCompanyTV.SelectExpressID = spList[0].ExpressIds;
            UCExpressCompanyTV.SelectExpressName = spList[0].ExpressNames;
        }

        [WebMethod]
        public static object LoadImitatePeriod(string userid, string distributionCode, string hidPeriodType, string hidPeriodTypeChild, string expressId,
            string hidPeriodStart, string hidIntervalNum, string txtPeriodName, string hidIsExpress, string hidIsMonthPeriod, string isEnable, string PeriodSource, string Kid)
        {
            AccountPeriodOutModel outModel = JudgeInput(hidPeriodType, hidPeriodTypeChild, hidPeriodStart, hidIntervalNum, txtPeriodName);
            if (outModel != null)
                return new { done = false, dataModel = outModel };

            AccountPeriod ap = BuildAccountPeriod(userid, distributionCode, hidPeriodType, hidPeriodTypeChild, expressId,
                                    hidPeriodStart, hidIntervalNum, txtPeriodName, hidIsExpress, hidIsMonthPeriod, isEnable, PeriodSource, Kid);

            IAccountPeriodService accountPeriodService = ServiceLocator.GetService<IAccountPeriodService>();
            string accountPeriod = accountPeriodService.ImitatePeriod(ap);

            AccountPeriodOutModel outModels = new AccountPeriodOutModel()
            {
                ErrorName = accountPeriod
            };
            return new { done = true, dataModel = outModels };
        }

        public int IsImitate
        {
            get { return string.IsNullOrEmpty(ViewState["IsImitate"].ToString()) ? 0 : int.Parse(ViewState["IsImitate"].ToString()); }
            set { ViewState.Add("IsImitate", value); }
        }

        public string PeriodSource
        {
            get { return string.IsNullOrEmpty(ViewState["PeriodSource"].ToString()) ? null : ViewState["PeriodSource"].ToString(); }
            set { ViewState.Add("PeriodSource", value); }
        }

        public string Kid
        {
            get { return string.IsNullOrEmpty(ViewState["Kid"].ToString()) ? null : ViewState["Kid"].ToString(); }
            set { ViewState.Add("Kid", value); }
        }

        public string UserId
        {
            get { return string.IsNullOrEmpty(ViewState["UserId"].ToString()) ? null : ViewState["UserId"].ToString(); }
            set { ViewState.Add("UserId", value); }
        }

        public string DisCode
        {
            get { return string.IsNullOrEmpty(ViewState["DistributionCode"].ToString()) ? null : ViewState["DistributionCode"].ToString(); }
            set { ViewState.Add("DistributionCode", value); }
        }

        [WebMethod]
        public static object Submit(string userid, string distributionCode, string hidPeriodType, string hidPeriodTypeChild, string expressId,
            string hidPeriodStart, string hidIntervalNum, string txtPeriodName, string hidIsExpress, string hidIsMonthPeriod, string isEnable, string PeriodSource, string Kid)
        {
            try
            {
                AccountPeriodOutModel outModel = JudgeInput(hidPeriodType, hidPeriodTypeChild, hidPeriodStart, hidIntervalNum, txtPeriodName);
                if(outModel!=null)
                    return new { done = false, dataModel = outModel };

                AccountPeriod ap = BuildAccountPeriod(userid, distributionCode, hidPeriodType, hidPeriodTypeChild, expressId,
                                        hidPeriodStart, hidIntervalNum, txtPeriodName, hidIsExpress, hidIsMonthPeriod, isEnable, PeriodSource, Kid);

                if (string.IsNullOrEmpty(Kid))
                {
                    return AddAccountPeriod(ap);
                }
                else
                {
                    return UpdateAccountPeriod(ap);
                }
            }
            catch (Exception ex)
            {
                return new { done = false, dataModel = new AccountPeriodOutModel() { ErrorName = ex.Message } };
            }
        }

        public static object AddAccountPeriod(AccountPeriod ap)
        {
            IAccountPeriodService accountPeriodService = ServiceLocator.GetService<IAccountPeriodService>();
            if (accountPeriodService.AddAccountPeriod(ap))
            {
                return new { done = false, dataModel = new AccountPeriodOutModel() { ErrorName = "添加成功" } };
            }
            else
            {
                return new { done = false, dataModel = new AccountPeriodOutModel() { ErrorName = "添加失败" } };
            }
        }

        public static object UpdateAccountPeriod(AccountPeriod ap)
        {
            IAccountPeriodService accountPeriodService = ServiceLocator.GetService<IAccountPeriodService>();
            if (accountPeriodService.UpdateAccountPeriod(ap))
            {
                return new { done = false, dataModel = new AccountPeriodOutModel() { ErrorName = "更新成功" } };
            }
            else
            {
                return new { done = false, dataModel = new AccountPeriodOutModel() { ErrorName = "更新失败" } };
            }
        }

        private static AccountPeriod BuildAccountPeriod(string userid,string distributionCode,string hidPeriodType, string hidPeriodTypeChild, string expressId,
            string hidPeriodStart, string hidIntervalNum, string txtPeriodName, string hidIsExpress, string hidIsMonthPeriod, string isEnable, string PeriodSource, string Kid)
        {
            AccountPeriod ap = new AccountPeriod();
            ap.CreateBy = int.Parse(userid);
            ap.UpdateBy = int.Parse(userid);
            ap.DistributionCode = distributionCode;
            ap.PeriodType = int.Parse(hidPeriodType);
            ap.PeriodTypeChild = int.Parse(hidPeriodTypeChild);
            ap.PeriodName = txtPeriodName.Trim();
            ap.PeriodRelationName = PeriodSource;
            ap.AccountPeriodKid = Kid;
            ap.IsExpress=int.Parse(hidIsExpress);
            ap.ExpressIds = ap.IsExpress > 0 ? expressId : "";
            ap.IsMonthPeriod = ap.PeriodType != 2 ? int.Parse(hidIsMonthPeriod) : 0;
            ap.PeriodStart = hidPeriodStart;
            ap.IntervalNum = (ap.PeriodType == 3 || ap.PeriodType == 1) ? int.Parse(hidIntervalNum) : 0;
            ap.IsDeleted = int.Parse(isEnable);

            return ap;
        }

        private static AccountPeriodOutModel JudgeInput(string hidPeriodType, string hidPeriodTypeChild, string hidPeriodStart, string hidIntervalNum, string txtPeriodName)
        {
            if (string.IsNullOrEmpty(hidPeriodType) || hidPeriodType == "0")
            {
                return new AccountPeriodOutModel() { ErrorName = "请选择账期类型" };
            }

            if (hidPeriodType=="1" &&(string.IsNullOrEmpty(hidPeriodTypeChild) || hidPeriodTypeChild == "0"))
            {
                return new AccountPeriodOutModel() { ErrorName = "请选择周子账期类型" };
            }

            if (string.IsNullOrEmpty(hidPeriodStart) || hidPeriodStart == "0")
            {
                return new AccountPeriodOutModel() { ErrorName = "请设定账期" };
            }

            if (hidPeriodType == "3" && (string.IsNullOrEmpty(hidIntervalNum) || hidIntervalNum == "0"))
            {
                return new AccountPeriodOutModel() { ErrorName = "周期起始日期不能为空" };
            }

            if (string.IsNullOrEmpty(txtPeriodName))
            {
                return new AccountPeriodOutModel() { ErrorName = "账期名称不能为空" };
            }
            return null;
        }
    }

    public class AccountPeriodOutModel
    {
        public String ErrorName { get; set; }
    }
}