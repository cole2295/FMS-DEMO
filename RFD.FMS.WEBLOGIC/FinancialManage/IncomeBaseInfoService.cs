using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.FinancialManage;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.MODEL.FinancialManage;
using System.Text.RegularExpressions;
using Microsoft.JScript.Vsa;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Domain.COD;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class IncomeBaseInfoService : IIncomeBaseInfoService
    {
        private IIncomeBaseInfoDao Dao;

        private IIncomeBaseInfoService OracleService;

        public DataTable GetIncomeDailyReport(string beginTime, string endTime, string merchantIds, string distributionCode)
        {
            if(OracleService != null)
            {
                return OracleService.GetIncomeDailyReport(beginTime, endTime, merchantIds, distributionCode);
            }

            DataTable table = Dao.GetIncomeDailyReport(beginTime,endTime,merchantIds,distributionCode);

            return table;
        }

        public IDictionary<string, string> GetIncomeDailyReportSum(DataTable dtDetail)
        {
            Dictionary<string, string> dicValues = new Dictionary<string, string>();
            //dtDetail.Compute("sum(配送费)", "DataType=0").ToString().TryGetDecimal()

            dicValues.Add("WaybillCount", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["结算单量"].ToString()); }).ToString());
            dicValues.Add("AccountFare", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["配送费"].ToString()); }).ToString());
            dicValues.Add("ProtectedFee", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["保价费"].ToString()); }).ToString());
            dicValues.Add("ReceiveFee", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["手续费"].ToString()); }).ToString());
            dicValues.Add("POSReceiveServiceFee", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["服务费"].ToString()); }).ToString());
            dicValues.Add("SumFee", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["应收合计"].ToString()); }).ToString());
            dicValues.Add("AvgFee", dtDetail.AsEnumerable().Sum(row => { return Decimal.Parse(row["单均收入"].ToString()); }).ToString());

            return dicValues;
        }

        public IDictionary<string, string> GetIncomeDailyReportSum(string beginTime, string endTime, string merchantIds, string distributionCode)
        {
            if(OracleService != null)
            {
                return OracleService.GetIncomeDailyReportSum(beginTime, endTime, merchantIds, distributionCode);
            }

            DataTable table = Dao.GetIncomeDailyReportSum(beginTime,endTime,merchantIds,distributionCode);

            DataRow row = table.Rows[0];

            Dictionary<string, string> dicValues = new Dictionary<string, string>();

            dicValues.Add("WaybillCount",DataConvert.ToString(row["WaybillCount"]));
            dicValues.Add("AccountFare",DataConvert.ToString(row["AccountFare"]));
            dicValues.Add("ProtectedFee",DataConvert.ToString(row["ProtectedFee"]));
            dicValues.Add("ReceiveFee",DataConvert.ToString(row["ReceiveFee"]));
            dicValues.Add("POSReceiveServiceFee",DataConvert.ToString(row["POSReceiveServiceFee"]));
            dicValues.Add("SumFee",DataConvert.ToString(row["SumFee"]));
            dicValues.Add("AvgFee",DataConvert.ToString(row["AvgFee"]));

            return dicValues;
        }

        public DeliverFeeModel GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            if(OracleService != null)
            {
                return OracleService.GetDeliverFeeParameter(waybillNo, distributionCode);
            }
            DataTable table = Dao.GetDeliverFeeParameter(waybillNo,distributionCode);

            if (table.Rows.Count == 0) return null;

            DataRow row = table.Rows[0];

            DeliverFeeModel model = new DeliverFeeModel();

            model.WaybillNO = waybillNo;
            model.Area = DataConvert.ToInt(row["Area"]);
            model.Weight = DataConvert.ToDecimal(row["Weight"]);
            model.Formula = DataConvert.ToString(row["Formula"]);
            model.DeliverFee = DataConvert.ToDecimal(row["DeliverFee"]);

            return model;
        }

        public bool SaveDeliverFee(DeliverFeeModel model)
        {
            if(OracleService != null)
            {
                return OracleService.SaveDeliverFee(model);
            }

            var service = ServiceLocator.GetService<IAccountOperatorLogDao>();

            string changeLog = "用户" + LoginUser.UserName + "修改运单" + model.WaybillNO + "的收入配送费为" + model.DeliverFee;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                if (Dao.SaveDeliverFee(model) == false) return false;

                if (service.AddOperatorLogLog(model.WaybillNO.ToString(), LoginUser.Userid, changeLog, (int)BizEnums.OperatorlogType.ChangeIncomeFee) == false) return false;

                work.Complete();
            }

            return true;
        }

        public bool EvalDeliverFee(DeliverFeeModel model)
        {
            string formula = model.Formula;

            decimal fare = 0;

            if (decimal.TryParse(formula, out fare))
            {
                model.DeliverFee = fare;

                return true;
            }

            Match m = Regex.Match(formula, @"(负数取零(\(重量-[\d\.]+\)))");

            if (m.Success == false) return false;
            
            string str = m.Groups[1].Value;
            string str1 = m.Groups[2].Value;
            string str2 = str.Replace("负数取零", "Number");

            str2 = "(" + str2 + ">0?" + str1 + ":0)";

            formula = formula.Replace(str, str2);
            
            formula = formula.Replace("向上取整", "Math.ceil").Replace("不取整", "").Replace("取整", "Math.floor").Replace("重量",model.Weight.ToString());

            fare = System.Decimal.Parse(EvalJScript(formula).ToString());

            model.DeliverFee = fare;

            return true;
        }

        private decimal EvalJScript(string JScript)
        {
            object Result = null;

            try
            {
                VsaEngine Engine = VsaEngine.CreateEngine();

                Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return decimal.Parse(Result.ToString());
        }


        public bool UpdateEvalStatus(long waybillNo)
        {
            if(OracleService != null)
            {
                return OracleService.UpdateEvalStatus(waybillNo);
            }
            return Dao.UpdateEvalStatus(waybillNo);
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            if(OracleService != null)
            {
                return OracleService.UpdateEvalStatus(waybillNos);
            }
            return Dao.UpdateEvalStatus(waybillNos);
        }


        public DataTable GetIncomeDeliveryFeeInfo(DataTable ImportTable, int Type)
        {
            if (OracleService != null)
            {

                return OracleService.GetIncomeDeliveryFeeInfo(ImportTable, Type);

            }
            throw new Exception("SqlServer下未实现");
        }
        //public  bool UpdateBatchDeliverFee(DataTable CurrTable)
        //{
        //    if (OracleService != null)
        //    {

        //        return OracleService.UpdateBatchDeliverFee(CurrTable);

        //    }
        //    throw new Exception("SqlServer下未实现");
        //}
        public  bool ExsitIncomeFeeInfoByNo(long waybillNo, long incomeFeeID)
        {
            if (OracleService != null)
            {

                return OracleService.ExsitIncomeFeeInfoByNo(waybillNo, incomeFeeID);

            }
            throw new Exception("SqlServer下未实现");
        }
        public DataTable ExsitIncomeFeeInfoByNo(long waybillNo)
        {
            if (OracleService != null)
            {

                return OracleService.ExsitIncomeFeeInfoByNo(waybillNo);

            }
            throw new Exception("SqlServer下未实现");
        }
        public bool BatchSaveDeliverFee(DeliverFeeModel model)
        {
            if (OracleService != null)
            {

                return OracleService.BatchSaveDeliverFee(model);

            }
            throw new Exception("SqlServer下未实现");
        }
    }
}
