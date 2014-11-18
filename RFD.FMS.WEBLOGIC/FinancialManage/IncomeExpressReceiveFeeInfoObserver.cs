using System;
using System.Configuration;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.WEBLOGIC.Mail;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;
using System.Data;
using System.Collections.Generic;


namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class IncomeExpressReceiveFeeInfoObserver : IIncomeExpressReceiveFeeInfoObserver, IWaybillStatusObserver
    {
        private IFMS_ReceiveFeeInfoDao oracleDao =  null;
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
                SynAcceptType();
                bool isExists = oracleDao.ExistsExpressReceiveFeeInfo(model.WaybillNo);

                if (model.Status == "-3" ||
                    model.Status == "0")
                {
                    if (isExists == true) return true;

                    FMS_IncomeExpressReceiveFeeInfo receiveModel = GetReceiveModel(model);

                    if (receiveModel == null) return true;

                    receiveModel.Status = model.Status;

                    return oracleDao.AddExpressReceiveFeeInfo(receiveModel);
                }
                else if (model.Status == "3" ||
                    model.Status == "4" ||
                    model.Status == "5" ||
                    model.Status == "-9")
                {
                    if (model.Status == "3")
                    {
                        if(isExists == true)
                        {
                            FMS_IncomeExpressReceiveFeeInfo monthlyModel = GetMonthlyModel(model);
                            if (monthlyModel != null)
                            {
                                monthlyModel.Status = model.Status;
                                return oracleDao.UpdateExpressReceiveFeeInfo(monthlyModel);
                            } 
                        }
                        FMS_IncomeExpressReceiveFeeInfo receiveModel = GetSendModel(model);

                        if (receiveModel == null) return true;

                        receiveModel.Status = model.Status;

                        if (isExists == true)
                        {
                            return oracleDao.UpdateExpressReceiveFeeInfo(receiveModel);
                        }
                        else
                        {
                            return oracleDao.AddExpressReceiveFeeInfo(receiveModel);
                        }


                    }
                    else if (model.Status == "4" || model.Status == "5")
                    {
                        if (isExists == true)
                        {
                            FMS_IncomeExpressReceiveFeeInfo sendModel = GetSendModel(model);

                            if (sendModel != null)
                            {
                                return oracleDao.DeleteExpressFeeInfo(model.WaybillNo);
                            }
                        }
                    }
                    else
                    {
                        if (isExists == true)
                        {
                            return oracleDao.DeleteExpressFeeInfo(model.WaybillNo);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                RFD.FMS.WEBLOGIC.Mail.Mail mail = new RFD.FMS.WEBLOGIC.Mail.Mail();

                mail.SendMailToUser("快递收款查询状态推送服务异常", ex.Message + ex.StackTrace, "liyongf@rufengda.com");

                return false;
            }
        }

        private void SynAcceptType()
        {
            DataTable table = oracleDao.GetNullAcceptTypeValues();

            if (table.Rows.Count == 0) return;

            DataRow row = null;

            IList<long> waybillNos = new List<long>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                waybillNos.Add(DataConvert.ToLong(row["WaybillNO"]));
            }

            DataTable sqlTable = sqlServerDao.GetSynInfoByWaybillNO(waybillNos);

            long waybillNo = -1;
            string acceptType = "";

            for (int i = 0; i < sqlTable.Rows.Count; i++)
            {
                row = sqlTable.Rows[i];

                waybillNo = DataConvert.ToLong(row["WaybillNO"]);
                acceptType = DataConvert.ToString(row["AcceptType"]);

                if (String.IsNullOrEmpty(acceptType))
                {
                    acceptType = "现金";
                }

                oracleDao.UpdateSynInfo(waybillNo,acceptType);
            }
        }

        public string GetSqlCondition()
        {
            return " Status in ('-5','-3','0','3','4','5','-9')  ";
        }

        public FMS_IncomeExpressReceiveFeeInfo GetReceiveModel(WaybillStatusChangeLog model)
        {
            IFMS_ReceiveFeeInfoDao dao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            FMS_IncomeExpressReceiveFeeInfo receiveModel = sqlServerDao.GetExpressReceiveFeeInfoModel(model.WaybillNo);

            return receiveModel;
        }

        public FMS_IncomeExpressReceiveFeeInfo GetSendModel(WaybillStatusChangeLog model)
        {
            IFMS_ReceiveFeeInfoDao dao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            FMS_IncomeExpressReceiveFeeInfo receiveModel = sqlServerDao.GetExpressSendFeeInfoModel(model.WaybillNo);

            return receiveModel;
        }
        public FMS_IncomeExpressReceiveFeeInfo GetMonthlyModel(WaybillStatusChangeLog model)
        {

            FMS_IncomeExpressReceiveFeeInfo monthlyModel = sqlServerDao.GetExpressSendMonthlyModel(model.WaybillNo);

            return monthlyModel;
        }

        public bool IsFalseToRePush
        {
            get { return true; }
        }

        public string Key
        {
            get { return "IncomeExpressReceiveFeeInfoObserver"; }
        }
    }
}
