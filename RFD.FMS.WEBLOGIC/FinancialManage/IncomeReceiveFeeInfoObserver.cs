using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.JScript.Vsa;
using System.Text.RegularExpressions;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.WEBLOGIC.Mail;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class IncomeReceiveFeeInfoObserver : IIncomeReceiveFeeInfoObserver, IWaybillStatusObserver
    {
        private IFMS_ReceiveFeeInfoDao oracleDao = null;
        private IReceiveFeeInfoDao sqlServerDao = null;

        /// <summary>
        /// 实现主方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DoAction(WaybillStatusChangeLog model)
        {
            try
            {
                if (model.Status == "3")
                {
                    FMS_IncomeReceiveFeeInfo receiveModel = GetModel(model);

                    if (receiveModel == null)
                    {
                        //RFD.FMS.WEBLOGIC.Mail.Mail mail = new RFD.FMS.WEBLOGIC.Mail.Mail();

                        //mail.SendMailToUser("财务收款查询状态推送服务异常", model.WaybillNo + "没有取到妥投数据", "liyongf@rufengda.com");

                        return false;
                    }

                    receiveModel.SignStatus = model.Status;

                    //检查数据是否存在
                    if (oracleDao.ExistsProjectReceiveFeeInfo(model.WaybillNo))
                    {
                        //如果存在Update
                        return oracleDao.UpdateProjectReceiveFeeInfo(receiveModel);
                    }
                    else
                    {
                        //如果不存在插入
                        return oracleDao.InsertProjectReceiveFeeInfo(receiveModel);
                    }
                }
                else
                {
                    //删除已经记录的中间表数据
                    return oracleDao.DeleteProjectReceiveFeeInfo(model.WaybillNo);
                }
            }
            catch (Exception ex)
            {
                RFD.FMS.WEBLOGIC.Mail.Mail mail = new RFD.FMS.WEBLOGIC.Mail.Mail();

                mail.SendMailToUser("财务收款查询状态推送服务异常", ex.Message + ex.StackTrace, "zhangrongrong@vancl.cn");

                return false;
            }
        }

        public string GetSqlCondition()
        {
            return " Status in ('3','4','5','-9') ";
        }

        public FMS_IncomeReceiveFeeInfo GetModel(WaybillStatusChangeLog model)
        {
            IFMS_ReceiveFeeInfoDao dao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            FMS_IncomeReceiveFeeInfo receiveModel = sqlServerDao.GetProjectSendFeeInfoModel(model.WaybillNo);

            return receiveModel;
        }

        public bool IsFalseToRePush
        {
            get { return true; }
        }

        public string Key
        {
            get { return "IncomeReceiveFeeInfoObserver"; }
        }

        public void RepairData()
        {
            PrepareAcceptType();

            PrepareBackStationTime();
        }

        private void PrepareAcceptType()
        {
            DataTable table = oracleDao.GetRepairData();

            if (table.Rows.Count == 0) return;

            DataRow row = null;

            long waybillNo = -1;
            string acceptType = "";

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                waybillNo = DataConvert.ToLong(row["WaybillNO"]);

                acceptType = sqlServerDao.GetCurrentData(waybillNo);

                if (String.IsNullOrEmpty(acceptType))
                {
                    oracleDao.DeleteProjectReceiveFeeInfo(waybillNo);
                }
                else
                {
                    oracleDao.UpdateRepairData(waybillNo, acceptType);
                }
            }
        }

        private void PrepareBackStationTime()
        {
            DataTable table = oracleDao.GetErrorBackStationTimeData();

            DataRow row = null;

            long waybillNo = -1;
            DateTime? backStationTime;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                waybillNo = DataConvert.ToLong(row["WaybillNO"]);
                backStationTime = DataConvert.ToDateTime(row["Backstationtime"]);

                if (backStationTime == null) continue;

                oracleDao.UpdateBackStationTime(waybillNo,backStationTime.Value);
            }
        }
    }
}
