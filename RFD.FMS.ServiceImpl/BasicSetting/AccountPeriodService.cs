using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class AccountPeriodService : IAccountPeriodService
    {
        private IAccountPeriodDao _accountPeriodDao;

        public bool AddAccountPeriod(AccountPeriod ap)
        {
            return _accountPeriodDao.AddAccountPeriod(ap);
        }

        public List<AccountPeriod> SearchAccountPeriod(AccountPeriodCondition apc)
        {
            DataTable dt = _accountPeriodDao.SearchAccountPeriod(apc);
            return TransformToAccountPeriod(dt);
        }

        private List<AccountPeriod> TransformToAccountPeriod(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            List<AccountPeriod> list = new List<AccountPeriod>();
            foreach (DataRow dr in dt.Rows)
            {
                AccountPeriod ap = new AccountPeriod();
                ap.AccountPeriodKid = dr["ACCOUNTPERIODKID"].ToString();
                ap.CreateBy = DataConvert.ToInt(dr["CREATEBY"].ToString(), 0);
                ap.CreateTime = DateTime.Parse(DataConvert.ToDateTime(dr["CREATETIME"].ToString()).ToString());
                ap.DistributionCode = dr["DISTRIBUTIONCODE"].ToString();
                ap.IntervalNum = DataConvert.ToInt(dr["INTERVALNUM"].ToString(), 0);
                ap.IsDeleted = DataConvert.ToInt(dr["ISDELETED"].ToString(), 0);
                ap.IsDeletedStr = ap.IsDeleted == 0 ? "启用" : "不启用";
                ap.IsExpress = DataConvert.ToInt(dr["ISEXPRESS"].ToString(), 0);
                ap.IsMonthPeriod = DataConvert.ToInt(dr["ISMONTHPERIOD"].ToString(), 0);
                ap.PeriodName = dr["PERIODNAME"].ToString();
                ap.PeriodRelationName = dr["PERIODRELATIONNAME"].ToString();
                ap.PeriodStart = dr["PERIODSTART"].ToString();
                ap.PeriodType = DataConvert.ToInt(dr["PERIODTYPE"].ToString(), 0);
                ap.PeriodTypeChild = DataConvert.ToInt(dr["PERIODTYPECHILD"].ToString(), 0);
                ap.UpdateBy = DataConvert.ToInt(dr["UPDATEBY"].ToString(), 0);
                ap.UpdateTime = DateTime.Parse(DataConvert.ToDateTime(dr["UPDATETIME"].ToString()).ToString());
                IExpressCompanyService expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
                ap.ExpressNames = expressCompanyService.SearchMergeCompanyName(dr["EXPRESSIDS"].ToString());
                ap.ExpressIds = expressCompanyService.SearchMergeCompanyIds(dr["EXPRESSIDS"].ToString());
                list.Add(ap);
            }

            return list;
        }

        public bool UpdateAccountPeriod(AccountPeriod ap)
        {
            return _accountPeriodDao.UpdateAccountPeriod(ap);
        }

        public string ImitatePeriod(AccountPeriod ap)
        {
            ap.ImitateDate=DateTime.Now.ToString("yyyy-MM-dd");
            ap.ImitateNum=10;
            GetImitatePeriod(ref ap);
            return ap.ImitateAccountPeriod;
        }

        #region 模拟账期
        /// <summary>
        /// 模拟账期
        /// </summary>
        /// <param name="DateNow">模拟时间</param>
        /// <param name="imitateNum">模拟账期延续次数</param>
        /// <param name="ap"></param>
        /// <returns></returns>
        public void GetImitatePeriod(ref AccountPeriod ap)
        {
            StringBuilder sbPeriod = new StringBuilder();
            sbPeriod.AppendFormat("账期名称：{0}， ", ap.PeriodName);
            sbPeriod.AppendFormat("模拟日期：{0}，", ap.ImitateDate);
            sbPeriod.AppendFormat("账期类型：{0}，", EnumHelper.GetDescription((RFD.FMS.MODEL.BizEnums.AccountPeriodType)ap.PeriodType));
            sbPeriod.Append(GetAccountPeriodChild(ap)+"<br>");
            sbPeriod.Append(GetAccountPeriod(ap));
            sbPeriod.AppendFormat("是否只结当月费用：{0}，", ap.IsMonthPeriod == 1 ? "是" : "否");
            sbPeriod.Append(GetAccountExpress(ap)+"<br>");
            GetAccountPeriodList(ref ap);
            sbPeriod.Append(BuildAccountPeriodList(ap));
            ap.ImitateAccountPeriod = sbPeriod.ToString();
        }

        private string BuildAccountPeriodList(AccountPeriod ap)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (AccountPeriodDate apd in ap.AccountPeriodList)
            {
                sb.AppendFormat("{0}、账期日：{1}，结算范围：{2}<br>", i, apd.AccountDate.ToString("yyyy-MM-dd"), apd.AccountPeriodStartDate.ToString("yyyy-MM-dd") + " ~ " + apd.AccountPeriodEndDate.ToString("yyyy-MM-dd"));
                i++;
            }
            sb.Append("......<br>");
            return sb.ToString();
        }

        private string GetAccountPeriodChild(AccountPeriod ap)
        {
            if (ap.PeriodType != (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T1)
            {
                return "";
            }

            return string.Format("子账期类型：{0}，", EnumHelper.GetDescription((RFD.FMS.MODEL.BizEnums.AccountPeriodChildType)ap.PeriodTypeChild));
        }

        private string GetAccountPeriod(AccountPeriod ap)
        {
            string model = "账期设定值：{0}{1}，";
            if (ap.PeriodType == (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T1)
            {
                return string.Format(model, "周" + ap.PeriodStart.Replace(",", ",周"), ";间隔" + ap.IntervalNum + "天");
            }
            if (ap.PeriodType == (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T2)
            {
                return string.Format(model, ap.PeriodStart + "日", "");
            }
            if (ap.PeriodType == (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T3)
            {
                return string.Format(model, "间隔" + ap.IntervalNum + "天", ";(起始日期:" + ap.PeriodStart + ")");
            }
            return "";
        }

        private string GetAccountExpress(AccountPeriod ap)
        {
            string model = "包含配送公司：{0}，";
            if (ap.IsExpress > 0)
            {
                return string.Format(model, EnumHelper.GetDescription((EnumPeriodExpressNexus)ap.IsExpress));
            }
            return "";
        }

        private void GetAccountPeriodList(ref AccountPeriod ap)
        {
            StringBuilder sb = new StringBuilder();
            switch (ap.PeriodType)
            {
                case (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T1:
                    switch (ap.PeriodTypeChild)
                    {
                        case (int)RFD.FMS.MODEL.BizEnums.AccountPeriodChildType.T1:
                            GetWeekAccountPeriodList1(ref ap);break;
                        case (int)RFD.FMS.MODEL.BizEnums.AccountPeriodChildType.T2:
                            GetWeekAccountPeriodList2(ref ap);break;
                        default:
                            break;
                    }
                    break;
                case (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T2:
                    GetMonthAccountPeriodList(ref ap);
                    break;
                case (int)RFD.FMS.MODEL.BizEnums.AccountPeriodType.T3:
                    GetCustomAccountPeriodList(ref ap);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 一周结N次
        /// </summary>
        /// <param name="ap"></param>
        private void GetWeekAccountPeriodList1(ref AccountPeriod ap)
        {
            List<AccountPeriodDate> apdList = new List<AccountPeriodDate>();
            StringBuilder sb = new StringBuilder();
            DateTime dt = DateTime.Parse(ap.ImitateDate);
            int weekOfYear = Util.DateHelper.WeekOfYear(dt);//账期周
            string[] accountDays = ap.PeriodStart.Replace(" ", "").Split(',');
            int dayBefore = 0;
            int dayBeforeWeek = weekOfYear;
            int dayNow = 0;
            DateTime dtEnd;
            DateTime dtStart;
            KeyValuePair<DateTime, DateTime> k;
            for (int i = 0; i < ap.ImitateNum; i++)
            {
                if (i > 0) weekOfYear++;
                dayBeforeWeek = weekOfYear;
                for (int j = 0; j < accountDays.Length; j++)
                {
                    dayNow = int.Parse(accountDays[j]);//账期日

                    if (j == 0) dayBefore = int.Parse(accountDays[accountDays.Length - 1]);
                    else dayBefore = int.Parse(accountDays[j - 1]);

                    if (dayNow <= dayBefore) dayBeforeWeek = weekOfYear - 1;//当当前结算周几<本身，表示跨周
                    DateTime dtNow = Util.DateHelper.DateTimeByYearAndWeekAndDay(dt.Year, weekOfYear, dayNow);
                    dtStart = Util.DateHelper.DateTimeByYearAndWeekAndDay(dt.Year, dayNow > dayBefore ? weekOfYear : dayBeforeWeek, dayBefore);
                    if (dayNow - 1 == 0) //周一时
                        dtEnd = Util.DateHelper.DateTimeByYearAndWeekAndDay(dt.Year, dayBeforeWeek, 7);
                    else
                        dtEnd = Util.DateHelper.DateTimeByYearAndWeekAndDay(dt.Year, weekOfYear, dayNow - 1);

                    //间隔
                    if (ap.IntervalNum > 0)
                    {
                        dtStart = dtStart.AddDays(-ap.IntervalNum);
                        dtEnd = dtEnd.AddDays(-ap.IntervalNum);
                    }

                    if (ap.IsMonthPeriod == 1) k = JudgeIsMonthPeriod(dtStart, dtEnd, apdList);
                    else k = new KeyValuePair<DateTime, DateTime>(dtStart, dtEnd);

                    apdList.Add(BuildAccountPeriodDate(dtNow, k.Key, k.Value));
                }
            }
            ap.AccountPeriodList = apdList;
        }

        /// <summary>
        /// 本周N结上一周
        /// </summary>
        /// <param name="ap"></param>
        private void GetWeekAccountPeriodList2(ref AccountPeriod ap)
        {
            List<AccountPeriodDate> apdList = new List<AccountPeriodDate>();
            DateTime dt = DateTime.Parse(ap.ImitateDate);
            int weekOfYear = Util.DateHelper.WeekOfYear(dt);
            DateTime dtNow ;
            KeyValuePair<DateTime, DateTime> kNow;

            for (int i = 0; i < ap.ImitateNum; i++)
            {
                if (i > 0) weekOfYear++;
                dtNow = Util.DateHelper.DateTimeByYearAndWeekAndDay(dt.Year, weekOfYear, int.Parse(ap.PeriodStart));
                kNow = Util.DateHelper.GetDateRangeByWeekAndYear(dt.Year, weekOfYear - 1);
                if (ap.IsMonthPeriod == 1) kNow = JudgeIsMonthPeriod(kNow.Key, kNow.Value, apdList);
                apdList.Add(BuildAccountPeriodDate(dtNow, kNow.Key, kNow.Value));
            }
 
            ap.AccountPeriodList = apdList;
        }

        /// <summary>
        /// 按月结
        /// </summary>
        /// <param name="ap"></param>
        private void GetMonthAccountPeriodList(ref AccountPeriod ap)
        {
            List<AccountPeriodDate> apdList = new List<AccountPeriodDate>();
            DateTime dt = DateTime.Parse(ap.ImitateDate);
            DateTime monthBefore;

            DateTime dtNow;
            DateTime dtStart;
            DateTime dtEnd;
            for (int i = 0; i < ap.ImitateNum; i++)
            {
                if (i > 0) dt=dt.AddMonths(1);
                monthBefore = dt.AddMonths(-1);
                dtNow = new DateTime(dt.Year, dt.Month, int.Parse(ap.PeriodStart));
                dtStart = new DateTime(monthBefore.Year, monthBefore.Month, 1);
                dtEnd = new DateTime(dt.Year, dt.Month, 1).AddDays(-1);
                apdList.Add(BuildAccountPeriodDate(dtNow, dtStart, dtEnd));
            }
            ap.AccountPeriodList = apdList;
        }

        /// <summary>
        /// 自定义
        /// </summary>
        /// <param name="ap"></param>
        private void GetCustomAccountPeriodList(ref AccountPeriod ap)
        {
            List<AccountPeriodDate> apdList = new List<AccountPeriodDate>();
            DateTime dt = DateTime.Parse(ap.ImitateDate);
            DateTime dtPeriodStart = DateTime.Parse(ap.PeriodStart);
            DateTime dtNow;
            DateTime dtStart;
            DateTime dtEnd;
            DateTime dtAfter;
            List<DateTime> dtList = new List<DateTime>();
            while ((dtPeriodStart -DateTime.Now).TotalDays>ap.IntervalNum)
            {
                dtPeriodStart = dtPeriodStart.AddDays(-ap.IntervalNum);
            }
            while ((dtPeriodStart - DateTime.Now).TotalDays < ap.IntervalNum)
            {
                dtPeriodStart=dtPeriodStart.AddDays(ap.IntervalNum);
            }

            KeyValuePair<DateTime, DateTime> k;
            dtPeriodStart = dtPeriodStart.AddDays(-ap.IntervalNum*2);
            for (int i = 0; i < ap.ImitateNum; i++)
            {
                if (i > 0) dtPeriodStart = dtPeriodStart.AddDays(ap.IntervalNum);
                dtNow = dtPeriodStart;
                dtAfter = dtPeriodStart.AddDays(-ap.IntervalNum);
                dtStart = dtAfter;
                dtEnd = dtPeriodStart.AddDays(-1);
                if (ap.IsMonthPeriod == 1) k = JudgeIsMonthPeriod(dtStart, dtEnd, apdList);
                else k = new KeyValuePair<DateTime, DateTime>(dtStart, dtEnd);
                apdList.Add(BuildAccountPeriodDate(dtNow, k.Key, k.Value));
            }
            ap.AccountPeriodList = apdList;
        }

        private KeyValuePair<DateTime, DateTime> JudgeIsMonthPeriod(DateTime dtStart, DateTime dtEnd, List<AccountPeriodDate> apdList)
        {
            KeyValuePair<DateTime, DateTime> k;
            DateTime dtStartTmp = dtStart;
            DateTime dtEndTmp = dtEnd;
            if (dtStart.Month != dtEnd.Month)
            {
                dtStartTmp = dtStart;
                dtEndTmp = new DateTime(dtEnd.Year, dtEnd.Month, 1).AddDays(-1);
            }
            else
            {
                if (apdList.Count > 0)
                {
                    AccountPeriodDate apd = apdList[apdList.Count - 1];
                    dtStartTmp = apd.AccountPeriodEndDate.AddDays(1);
                    dtEndTmp = dtEnd;
                }
            }
            k = new KeyValuePair<DateTime, DateTime>(dtStartTmp, dtEndTmp);
            return k;
        }

        private AccountPeriodDate BuildAccountPeriodDate(DateTime dtAccountDate, DateTime dtStartDate, DateTime dtStartEnd)
        {
            return new AccountPeriodDate()
            {
                AccountDate = dtAccountDate,
                AccountPeriodEndDate=dtStartEnd,
                AccountPeriodStartDate=dtStartDate
            };
        }
        #endregion
    }
}
