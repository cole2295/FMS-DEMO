using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Service.COD;
using RFD.FMS.Domain.COD;
using RFD.FMS.Util;
using System.Text.RegularExpressions;
using RFD.FMS.MODEL.FinancialManage;
using Microsoft.JScript.Vsa;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.MODEL;
using System.Data;

namespace RFD.FMS.WEBLOGIC.COD
{
    public class CODBaseInfoService : ICODBaseInfoService
    {
        private ICODBaseInfoDao _cODBaseInfoDao;
        private ICODBaseInfoService OracleService;

        public int Add(MODEL.COD.FMS_CODBaseInfo model)
        {
            if (OracleService!=null)
            {
                return OracleService.Add(model);
            }
            else
            {
                return _cODBaseInfoDao.Add(model);
            }
            
        }

        public MODEL.COD.FMS_CODBaseInfo GetModel(long id)
        {
            if (OracleService!=null)
            {
                return OracleService.GetModel(id);
            }
            else
            {
                return _cODBaseInfoDao.GetModel(id);
            }
           
        }

        public MODEL.COD.FMS_CODBaseInfo GetModelByWaybillNO(long waybillNo)
        {
            if (OracleService!=null)
            {
                return OracleService.GetModelByWaybillNO(waybillNo);
            }
            return _cODBaseInfoDao.GetModelByWaybillNO(waybillNo);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetModelList(Dictionary<string, object> searchParams)
        {
            if (OracleService!=null)
            {
                return OracleService.GetModelList(searchParams);
            }
            return _cODBaseInfoDao.GetModelList(searchParams);
        }

        public bool UpdateAmountByID(MODEL.COD.FMS_CODBaseInfo info)
        {
            if (OracleService!=null)
            {
                return OracleService.UpdateAmountByID(info);
            }
            return _cODBaseInfoDao.UpdateAmountByID(info);
        }

        public bool UpdateIsDeletedByID(MODEL.COD.FMS_CODBaseInfo info)
        {
            if (OracleService!=null)
            {
                return OracleService.UpdateIsDeletedByID(info);
            }
            return _cODBaseInfoDao.UpdateIsDeletedByID(info);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetDeliverDetails(int accountDays, int tops, string syncStartTime)
        {
            if (OracleService!=null)
            {
                return OracleService.GetDeliverDetails(accountDays, tops, syncStartTime);
            }
            return _cODBaseInfoDao.GetDeliverDetails(accountDays, tops, syncStartTime);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetReturnDetails(int accountDays, int tops, string syncStartTime)
        {
            if (OracleService!=null)
            {
                return OracleService.GetReturnDetails(accountDays, tops, syncStartTime);
            }
            return _cODBaseInfoDao.GetReturnDetails(accountDays, tops, syncStartTime);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetVisitReturnDetails(int accountDays, int tops, string syncStartTime)
        {
            if (OracleService!=null)
            {
                return OracleService.GetVisitReturnDetails(accountDays, tops, syncStartTime);
            }
            return _cODBaseInfoDao.GetVisitReturnDetails(accountDays, tops, syncStartTime);
        }

        public bool UpdateCodFare(MODEL.COD.FMS_CODBaseInfo detail)
        {
            if (OracleService != null)
            {
                return OracleService.UpdateCodFare(detail);
            }
            return _cODBaseInfoDao.UpdateCodFare(detail);
        }

        public bool UpdateBackError(MODEL.COD.FMS_CODBaseInfo detail)
        {
            if (OracleService!=null)
            {
                return OracleService.UpdateBackError(detail);
            }
            return _cODBaseInfoDao.UpdateBackError(detail);
        }


        public List<MODEL.COD.CodStatsLogModel> GetDeliverToDayStatsInfo(DateTime accountDate)
        {
            if (OracleService != null)
            {
                return OracleService.GetDeliverToDayStatsInfo(accountDate);
            }
            return _cODBaseInfoDao.GetDeliverToDayStatsInfo(accountDate);
        }

        public int GetDeliverAllCountByExpressWareHose(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService!=null)
            {
                return OracleService.GetDeliverAllCountByExpressWareHose(codStatsLog);
            }
            return _cODBaseInfoDao.GetDeliverAllCountByExpressWareHose(codStatsLog);
        }

        public int GetDeliverFareCountByExpressWareHouse(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetDeliverFareCountByExpressWareHouse(codStatsLog);
            }
            return _cODBaseInfoDao.GetDeliverFareCountByExpressWareHouse(codStatsLog);
        }

        public List<MODEL.COD.CodStatsModel> GetDeliverAccountByDay(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetDeliverAccountByDay(codStatsLog);
            }
            return _cODBaseInfoDao.GetDeliverAccountByDay(codStatsLog);
        }

        public List<MODEL.COD.CodStatsLogModel> GetReturnToDayStatsInfo(DateTime accountDate)
        {
            if (OracleService!=null)
            {
                return OracleService.GetReturnToDayStatsInfo(accountDate);
            }
            return _cODBaseInfoDao.GetReturnToDayStatsInfo(accountDate);
        }

        public int GetReturnAllCountByExpressWareHose(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetReturnAllCountByExpressWareHose(codStatsLog);
            }
            return _cODBaseInfoDao.GetReturnAllCountByExpressWareHose(codStatsLog);
        }

        public int GetReturnFareCountByExpressWareHouse(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetReturnFareCountByExpressWareHouse(codStatsLog);
            }
            return _cODBaseInfoDao.GetReturnFareCountByExpressWareHouse(codStatsLog);
        }

        public List<MODEL.COD.CodStatsModel> GetReturnAccountByDay(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetReturnAccountByDay(codStatsLog);
        }

        public List<MODEL.COD.CodStatsLogModel> GetVisitToDayStatsInfo(DateTime accountDate)
        {
            return _cODBaseInfoDao.GetVisitToDayStatsInfo(accountDate);
        }

        public int GetVisitAllCountByExpressWareHose(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetVisitAllCountByExpressWareHose(codStatsLog);
            }
            return _cODBaseInfoDao.GetVisitAllCountByExpressWareHose(codStatsLog);
        }

        public int GetVisitFareCountByExpressWareHouse(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetVisitFareCountByExpressWareHouse(codStatsLog);
            }
            return _cODBaseInfoDao.GetVisitFareCountByExpressWareHouse(codStatsLog);
        }

        public List<MODEL.COD.CodStatsModel> GetVisitAccountByDay(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.GetVisitAccountByDay(codStatsLog);
            }
            return _cODBaseInfoDao.GetVisitAccountByDay(codStatsLog);
        }

        public bool InsertDeliverAccount(List<MODEL.COD.CodStatsModel> codStatsList, string date)
        {
            if (OracleService != null)
            {
                return OracleService.InsertDeliverAccount(codStatsList, date);
            }
            return _cODBaseInfoDao.InsertDeliverAccount(codStatsList, date);
        }

        public bool InsertReturnsAccount(List<MODEL.COD.CodStatsModel> codStatsList, string date)
        {
            if (OracleService != null)
            {
                return OracleService.InsertReturnsAccount(codStatsList, date);
            }
            return _cODBaseInfoDao.InsertReturnsAccount(codStatsList, date);
        }

        public bool InsertVisitReturnsAccount(List<MODEL.COD.CodStatsModel> codStatsList, string date)
        {
            if (OracleService != null)
            {
                return OracleService.InsertVisitReturnsAccount(codStatsList, date);
            }
            return _cODBaseInfoDao.InsertVisitReturnsAccount(codStatsList, date);
        }

        public bool JudgeLogExists(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService!=null)
            {
                return OracleService.JudgeLogExists(codStatsLog);
            }
            return _cODBaseInfoDao.JudgeLogExists(codStatsLog);
        }

        public bool UpdateStatisticsLog(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService != null)
            {
                return OracleService.UpdateStatisticsLog(codStatsLog);
            }
            return _cODBaseInfoDao.UpdateStatisticsLog(codStatsLog);
        }

        public bool WriteStatisticsLog(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            if (OracleService!=null)
            {
                return OracleService.WriteStatisticsLog(codStatsLog);
            }
            return _cODBaseInfoDao.WriteStatisticsLog(codStatsLog);
        }

        public List<MODEL.COD.CodStatsLogModel> GetStatsLogError(int statisticsType, string dateRemove, int accountDays)
        {
            if (OracleService!=null)
            {
                return OracleService.GetStatsLogError(statisticsType, dateRemove, accountDays);
            }
            return _cODBaseInfoDao.GetStatsLogError(statisticsType, dateRemove, accountDays);
        }


        public System.Data.DataTable GetDeliver(int accountDays)
        {
            if (OracleService!=null)
            {
                return OracleService.GetDeliver(accountDays);
            }
            return _cODBaseInfoDao.GetDeliver(accountDays);
        }

        public bool ChangeDeliverBack(List<string> noList)
        {
            if (OracleService!=null)
            {
                return OracleService.ChangeDeliverBack(noList);
            }
            return _cODBaseInfoDao.ChangeDeliverBack(noList);
        }

        public System.Data.DataTable GetError8(int accountDays)
        {
            if (OracleService!=null)
            {
                return OracleService.GetError8(accountDays);
            }
            return _cODBaseInfoDao.GetError8(accountDays);
        }

        public System.Data.DataTable GetError9(int accountDays)
        {
            if (OracleService!=null)
            {
                return OracleService.GetError9(accountDays);
            }
            return _cODBaseInfoDao.GetError9(accountDays);
        }

        public System.Data.DataTable GetError7(int accountDays)
        {
            if (OracleService != null)
            {
                return OracleService.GetError7(accountDays);
            }
            return _cODBaseInfoDao.GetError7(accountDays);
        }

        public System.Data.DataTable GetError6(int accountDays)
        {
            if (OracleService != null)
            {
                return OracleService.GetError6(accountDays);
            }
            return _cODBaseInfoDao.GetError6(accountDays);
        }

        public System.Data.DataTable GetError5(int accountDays)
        {
            if (OracleService != null)
            {
                return OracleService.GetError5(accountDays);
            }
            return _cODBaseInfoDao.GetError5(accountDays);
        }

        public System.Data.DataTable GetError34(int errorType, int accountDays)
        {
            if (OracleService!=null)
            {
                return OracleService.GetError34(errorType, accountDays);
            }
            return _cODBaseInfoDao.GetError34(errorType,accountDays);
        }



        public IDictionary<long, FMS_CODBaseInfoCheck> GetDeliveryList(FMS_CODBaseInfoCheck model)
        {
            if(OracleService != null)
            {
                return OracleService.GetDeliveryList(model);
            }

            var dt = _cODBaseInfoDao.GetDeliveryList(model);
            IDictionary<long, FMS_CODBaseInfoCheck> deliveryDic = new Dictionary<long, FMS_CODBaseInfoCheck>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FMS_CODBaseInfoCheck mod = new FMS_CODBaseInfoCheck();
                    mod.WaybillNo = DataConvert.ToLong(dt.Rows[i]["订单号"]);
                    mod.AreaType = DataConvert.ToInt(dt.Rows[i]["区域类型"]);
                    mod.fee = DataConvert.ToDecimal(dt.Rows[i]["配送费"]);
                    mod.Status = DataConvert.ToInt(dt.Rows[i]["状态"]);
                    mod.WaybillType = DataConvert.ToInt(dt.Rows[i]["订单类型"]);
                    mod.MerchantName = DataConvert.ToString(dt.Rows[i]["商家"]);
                    mod.AccountCompany = DataConvert.ToString(dt.Rows[i]["结算单位"]);
                    mod.AccountWeight = DataConvert.ToDecimal(dt.Rows[i]["结算重量"]);
                    mod.Address = DataConvert.ToString(dt.Rows[i]["收货人地址"]);
                    mod.DeliveryTime = DataConvert.ToDateTime(dt.Rows[i]["最终发日期"]);
                    mod.FinalSorting = DataConvert.ToString(dt.Rows[i]["末级发货仓名称"]);
                    mod.IsChecked = false;

                   
                    deliveryDic.Add(mod.WaybillNo, mod);
                }
                
            }
            return deliveryDic;
        }


        public IDictionary<long, FMS_CODBaseInfoCheck> GetReturnList(FMS_CODBaseInfoCheck model)
        {
            if(OracleService != null)
            {
                return OracleService.GetReturnList(model);
            }

            var dt = _cODBaseInfoDao.GetReturnList(model);
            IDictionary<long, FMS_CODBaseInfoCheck> returnsDic = new Dictionary<long, FMS_CODBaseInfoCheck>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FMS_CODBaseInfoCheck mod = new FMS_CODBaseInfoCheck();
                    mod.WaybillNo = DataConvert.ToLong(dt.Rows[i]["订单号"]);
                    mod.AreaType = DataConvert.ToInt(dt.Rows[i]["区域类型"]);
                    mod.fee = DataConvert.ToDecimal(dt.Rows[i]["配送费"]);
                    mod.Status = DataConvert.ToInt(dt.Rows[i]["状态"]);
                    mod.WaybillType = DataConvert.ToInt(dt.Rows[i]["订单类型"]);
                    mod.MerchantName = DataConvert.ToString(dt.Rows[i]["商家"]);
                    mod.AccountCompany = DataConvert.ToString(dt.Rows[i]["结算单位"]);
                    mod.AccountWeight = DataConvert.ToDecimal(dt.Rows[i]["结算重量"]);
                    mod.Address = DataConvert.ToString(dt.Rows[i]["收货人地址"]);
                    mod.DeliveryTime = DataConvert.ToDateTime(dt.Rows[i]["最终发日期"]);
                    mod.FinalSorting = DataConvert.ToString(dt.Rows[i]["末级发货仓名称"]);
                    mod.IsChecked = false;
                    returnsDic.Add(mod.WaybillNo, mod);
                }
               
            }
            return returnsDic;
        }


        public IDictionary<long, FMS_CODBaseInfoCheck> GetVisitList(FMS_CODBaseInfoCheck model)
        {
            if(OracleService != null)
            {
               return  OracleService.GetVisitList(model);
            }
            var dt = _cODBaseInfoDao.GetVisitList(model);
            IDictionary<long, FMS_CODBaseInfoCheck> visitDic = new Dictionary<long, FMS_CODBaseInfoCheck>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FMS_CODBaseInfoCheck mod = new FMS_CODBaseInfoCheck();
                    mod.WaybillNo = DataConvert.ToLong(dt.Rows[i]["订单号"]);
                    mod.AreaType = DataConvert.ToInt(dt.Rows[i]["区域类型"]);
                    mod.fee = DataConvert.ToDecimal(dt.Rows[i]["配送费"]);
                    mod.Status = DataConvert.ToInt(dt.Rows[i]["状态"]);
                    mod.WaybillType = DataConvert.ToInt(dt.Rows[i]["订单类型"]);
                    mod.MerchantName = DataConvert.ToString(dt.Rows[i]["商家"]);
                    mod.AccountCompany = DataConvert.ToString(dt.Rows[i]["结算单位"]);
                    mod.AccountWeight = DataConvert.ToDecimal(dt.Rows[i]["结算重量"]);
                    mod.Address = DataConvert.ToString(dt.Rows[i]["收货人地址"]);
                    mod.DeliveryTime = DataConvert.ToDateTime(dt.Rows[i]["最终发日期"]);
                    mod.FinalSorting = DataConvert.ToString(dt.Rows[i]["末级发货仓名称"]);
                    mod.IsChecked = false;
                    visitDic.Add(mod.WaybillNo, mod);
                }
                
            }
            return visitDic;
        }


        public MODEL.FinancialManage.DeliverFeeModel GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            if(OracleService != null)
            {
                return OracleService.GetDeliverFeeParameter(waybillNo, distributionCode);
            }

            DataTable table = _cODBaseInfoDao.GetDeliverFeeParameter(waybillNo,distributionCode);

            if (table.Rows.Count == 0) return null;

            DataRow row = table.Rows[0];

            DeliverFeeModel model = new DeliverFeeModel();

            model.WaybillNO = waybillNo;
            model.Area = DataConvert.ToInt(row["Area"]);
            model.Weight = DataConvert.ToDecimal(row["Weight"]);
            model.Formula = DataConvert.ToString(row["Formula"]);
            model.DeliverFee = DataConvert.ToDecimal(row["DeliverFee"]);
            model.IsFare = DataConvert.ToInt(row["IsFare"],-1);

            return model;
        }

        public bool SaveDeliverFee(MODEL.FinancialManage.DeliverFeeModel model)
        {
            if(OracleService != null)
            {
                return OracleService.SaveDeliverFee(model);
            }
            var service = ServiceLocator.GetService<IAccountOperatorLogDao>();

            string changeLog = "用户" + LoginUser.UserName + "修改运单" + model.WaybillNO + "的支出配送费为" + model.DeliverFee;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                if (_cODBaseInfoDao.SaveDeliverFee(model) == false) return false;

                if (service.AddOperatorLogLog(model.WaybillNO.ToString(), LoginUser.Userid, changeLog, (int)BizEnums.OperatorlogType.ChangeCODFee) == false) return false;

                work.Complete();
            }

            return true;
        }

        public bool EvalDeliverFee(DeliverFeeModel model)
        {
            decimal fare = 0;
            string formula = model.Formula;
            decimal weight = model.Weight;

            //负数取零
            Match m = Regex.Match(formula, @"(负数取零(\(重量-[\d\.]+\)))");

            if (m.Success)
            {
                string str = m.Groups[1].Value;
                string str1 = m.Groups[2].Value;
                string str2 = str.Replace("负数取零", "Number");

                str2 = "(" + str2 + ">0?" + str1 + ":0)";

                formula = formula.Replace(str, str2);
            }

            //当-0.5表示宅急送公式，小于0.5的按0.5算，目的为取整得到0 公式纠正后可以删除这个逻辑，以上面的判断即可
            if (formula.Contains("-0.5") && weight < 0.5M)
            {
                weight = 0.5M;
            }

            if (decimal.TryParse(formula, out fare))
            {
                model.DeliverFee = fare;

                return true;
            }

            if (!string.IsNullOrEmpty(weight.ToString()) && weight.ToString() != "0.00")
            {
                formula = formula.Replace("向上取整", "Math.ceil").Replace("不取整", "").Replace("取整", "Math.floor").Replace("重量", "{0}");

                fare = Convert.ToDecimal(EvalJScript(string.Format(formula, weight)));

                model.DeliverFee = fare;

                return true;
            }

            return false;
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
            return _cODBaseInfoDao.UpdateEvalStatus(waybillNo);
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            if(OracleService != null)
            {
                return OracleService.UpdateEvalStatus(waybillNos);
            }
            return _cODBaseInfoDao.UpdateEvalStatus(waybillNos);
        }


        public DataTable GetCODDeliveryFeeInfo(DataTable ImportTable,int Type)
        {
            if(OracleService != null)
            {
                return OracleService.GetCODDeliveryFeeInfo(ImportTable,Type);
            }
            throw new Exception("SqlServer下未实现");
        }


        public bool UpdateBatchDeliverFee(DataTable CurrTable)
        {
            if (OracleService != null)
            {
                
               return  OracleService.UpdateBatchDeliverFee(CurrTable);
                
            }
            throw new Exception("SqlServer下未实现");
        }


        public bool ExsitCodBaseInfoByNo(long waybillNo,long infoID)
        {
            if (OracleService != null)
            {

                return OracleService.ExsitCodBaseInfoByNo(waybillNo,infoID);

            }
            throw new Exception("SqlServer下未实现");
        }
    }
}
