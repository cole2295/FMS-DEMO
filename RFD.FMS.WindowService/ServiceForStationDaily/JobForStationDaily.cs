using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using RFD.LMS.Service.BasicSetting;
using LMS.Util;
using RFD.LMS.Service.StationDaily;
using RFD.LMS.Service;
using System.IO;
using System.Data;
using RFD.LMS.Model;
using LMS.Model;
using System.Threading;

namespace ServiceForStationDaily
{
    public class JobForStationDaily : IStatefulJob
    {
        private readonly IExpressCompany _ecService = ServiceLocator.GetService<IExpressCompany>();
        private readonly IMerchantBaseInfo _mService = ServiceLocator.GetService<IMerchantBaseInfo>();
        private readonly IStationDailyService _sdService = ServiceLocator.GetService<IStationDailyService>();
        private readonly IServiceLog _sysService = ServiceLocator.GetService<IServiceLog>();

        public void Execute(JobExecutionContext context)
        {
            DailyServiceStart();
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        public void DailyServiceStart()
        {
            try
            {
                DataTable dtEc = _ecService.GetStationAndDistributionList();
                if (dtEc == null || dtEc.Rows.Count < 1)
                    throw new Exception("获取部门信息失败---" + DateTime.Now);
                foreach (DataRow dr in dtEc.Rows)
                {
                    LoadStationDailyData(dr["ExpressCompanyID"].ToString(), dr["CompanyName"].ToString(), Common.PreDayCount);
                    Common.WriteTest("生成" + dr["CompanyName"] + "配送报表数据---" + DateTime.Now);
                    Thread.Sleep(1000);
                }
                string emailMess = "共有" + dtEc.Rows.Count + "个站点配送报表生成成功---" + DateTime.Now;
                Common.SendFailedMail(Common.FailedSubject, emailMess);
                Common.WriteTest(emailMess);
            }
            catch (Exception ex)
            {
                Common.WriteTest(ex.Message);
            }
        }

        /// <summary>
        /// 加载配送报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        public void LoadStationDailyData(string expressCompanyId, string companyName, int preDay)
        {
            var searchInfo = new SearchModel();
            searchInfo.StationId = expressCompanyId;
            searchInfo.ExpressCompanyName = companyName;
            searchInfo.dtDailyDate = DateTime.Today.AddDays(-preDay);

            if (_sdService.Exists(searchInfo))
                _sdService.DeleteStationDailyData(searchInfo);

            LoadData(searchInfo);
        }

        /// <summary>
        /// 生成如风达报表数据
        /// </summary>
        /// <param name="searchInfo"></param>
        private void LoadData(SearchModel searchInfo)
        {
            try
            {
                DataTable dt = _sdService.GetStationDailyData(searchInfo);
                string[] statusArr =
                    {
                        Convert.ToString((int) EnumCommon.WaybillSourse.VANCL),
                        Convert.ToString((int) EnumCommon.WaybillSourse.VJIA),
                        Convert.ToString((int) EnumCommon.WaybillSourse.Other)
                    };
                foreach (string source in statusArr)
                {
                    searchInfo.Source = source;
                    if (searchInfo.Source == Convert.ToString((int)EnumCommon.WaybillSourse.Other))
                        LoadMerchandData(searchInfo, dt);
                    else
                        LoadVanclVjiaData(searchInfo, dt);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("{0}生成配送报表数据失败，详细信息:{1}", searchInfo.ExpressCompanyName, ex.Message);
                Common.WriteTest(string.Format("{0}---{1}", errorMessage, DateTime.Now));
                InsertFailInfo(searchInfo.StationId, searchInfo.dtDailyDate, errorMessage);
            }
        }

        /// <summary>
        /// 插入报表失败监控信息
        /// </summary>
        /// <param name="expressCompanyId"></param>
        /// <param name="dtTime"></param>
        /// <param name="errorMessage"></param>
        private void InsertFailInfo(string expressCompanyId, DateTime dtTime, string errorMessage)
        {
            try
            {
                var model = new ServiceMonitor();
                model.ServiceName = "ServiceForStationDaily";
                model.StationId = int.Parse(expressCompanyId);
                model.DailyTime = dtTime;
                model.ServiceType = (int)EnumCommon.ServiceNameEnum.ServiceForStationDaily;
                model.CreateTime = DateTime.Now;
                model.IsDeleted = 0;
                model.Remark = errorMessage;
                _sysService.InsertServiceFailInfo(model);
            }
            catch (Exception ex)
            {
                Common.WriteTest(string.Format("服务监控信息插入失败,详细信息:{0}---{1}", ex.Message, DateTime.Now));
            }
        }

        /// <summary>
        /// 生成Vancl,Vjia数据
        /// </summary>
        /// <param name="searchInfo"></param>
        /// <param name="dt"></param>
        private void LoadVanclVjiaData(SearchModel searchInfo, DataTable dt)
        {
            DataRow[] drDatas = dt.Select(string.Format("Sources={0}", searchInfo.Source));
            using (DataTable dtData = dt.Clone())
            {
                foreach (DataRow dr in drDatas)
                {
                    dtData.ImportRow(dr);
                    dtData.AcceptChanges();
                }
                if (dtData.Rows.Count > 0)
                {
                    _sdService.InsertStationDailyData(dtData, searchInfo);
                }
            }
        }

        /// <summary>
        /// 生成配送商配送报表
        /// </summary>
        /// <param name="searchInfo"></param>
        /// <param name="dt"></param>
        private void LoadMerchandData(SearchModel searchInfo, DataTable dt)
        {
            DataTable dtMerchant = _mService.GetAllMerchants();
            if (dtMerchant == null || dtMerchant.Rows.Count < 1) return;
            if (dt == null || dt.Rows.Count < 1) return;
            DataTable dtMerchantData = dt.Clone();
            foreach (DataRow drMerchant in dtMerchant.Rows)
            {
                dtMerchantData.Rows.Clear();
                DataRow[] drDatas =
                    dt.Select(string.Format("MerchantID ={0} and Sources={1}", drMerchant["ID"], searchInfo.Source));
                foreach (DataRow dr in drDatas)
                {
                    dtMerchantData.ImportRow(dr);
                    dtMerchantData.AcceptChanges();
                }
                if (dtMerchantData.Rows.Count <= 0) continue;
                searchInfo.MerchantId = drMerchant["ID"].ToString();
                _sdService.InsertStationDailyData(dtMerchantData, searchInfo);
            }
            dtMerchantData.Dispose();
        }
    }
}
