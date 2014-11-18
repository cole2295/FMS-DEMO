using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.JScript.Vsa;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.DAL.Oracle.COD;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.COD;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.COD;
using RFD.FMS.Domain.COD;
using RFD.FMS.Util;

namespace RFD.FMS.ServiceImpl.COD
{
    public class CODBaseInfoService : ICODBaseInfoService
    {
         ICODBaseInfoDao _cODBaseInfoDao = new CODBaseInfoDao();

        public int Add(MODEL.COD.FMS_CODBaseInfo model)
        {
            return _cODBaseInfoDao.Add(model);
        }

        public MODEL.COD.FMS_CODBaseInfo GetModel(long id)
        {
            return _cODBaseInfoDao.GetModel(id);
        }

        public MODEL.COD.FMS_CODBaseInfo GetModelByWaybillNO(long waybillNo)
        {
            return _cODBaseInfoDao.GetModelByWaybillNO(waybillNo);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetModelList(Dictionary<string, object> searchParams)
        {
            return _cODBaseInfoDao.GetModelList(searchParams);
        }

        public bool UpdateAmountByID(MODEL.COD.FMS_CODBaseInfo info)
        {
            return _cODBaseInfoDao.UpdateAmountByID(info);
        }

        public bool UpdateIsDeletedByID(MODEL.COD.FMS_CODBaseInfo info)
        {
            return _cODBaseInfoDao.UpdateIsDeletedByID(info);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetDeliverDetails(int accountDays, int tops, string syncStartTime)
        {
            return _cODBaseInfoDao.GetDeliverDetails(accountDays, tops, syncStartTime);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetReturnDetails(int accountDays, int tops, string syncStartTime)
        {
            return _cODBaseInfoDao.GetReturnDetails(accountDays, tops, syncStartTime);
        }

        public List<MODEL.COD.FMS_CODBaseInfo> GetVisitReturnDetails(int accountDays, int tops, string syncStartTime)
        {
            return _cODBaseInfoDao.GetVisitReturnDetails(accountDays, tops, syncStartTime);
        }

        public bool UpdateCodFare(MODEL.COD.FMS_CODBaseInfo detail)
        {
            return _cODBaseInfoDao.UpdateCodFare(detail);
        }

        public bool UpdateBackError(MODEL.COD.FMS_CODBaseInfo detail)
        {
            return _cODBaseInfoDao.UpdateBackError(detail);
        }


        public List<MODEL.COD.CodStatsLogModel> GetDeliverToDayStatsInfo(DateTime accountDate)
        {
            return _cODBaseInfoDao.GetDeliverToDayStatsInfo(accountDate);
        }

        public int GetDeliverAllCountByExpressWareHose(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetDeliverAllCountByExpressWareHose(codStatsLog);
        }

        public int GetDeliverFareCountByExpressWareHouse(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetDeliverFareCountByExpressWareHouse(codStatsLog);
        }

        public List<MODEL.COD.CodStatsModel> GetDeliverAccountByDay(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetDeliverAccountByDay(codStatsLog);
        }

        public List<MODEL.COD.CodStatsLogModel> GetReturnToDayStatsInfo(DateTime accountDate)
        {
            return _cODBaseInfoDao.GetReturnToDayStatsInfo(accountDate);
        }

        public int GetReturnAllCountByExpressWareHose(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetReturnAllCountByExpressWareHose(codStatsLog);
        }

        public int GetReturnFareCountByExpressWareHouse(MODEL.COD.CodStatsLogModel codStatsLog)
        {
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
            return _cODBaseInfoDao.GetVisitAllCountByExpressWareHose(codStatsLog);
        }

        public int GetVisitFareCountByExpressWareHouse(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetVisitFareCountByExpressWareHouse(codStatsLog);
        }

        public List<MODEL.COD.CodStatsModel> GetVisitAccountByDay(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.GetVisitAccountByDay(codStatsLog);
        }

        public bool InsertDeliverAccount(List<MODEL.COD.CodStatsModel> codStatsList, string date)
        {
            return _cODBaseInfoDao.InsertDeliverAccount(codStatsList, date);
        }

        public bool InsertReturnsAccount(List<MODEL.COD.CodStatsModel> codStatsList, string date)
        {
            return _cODBaseInfoDao.InsertReturnsAccount(codStatsList, date);
        }

        public bool InsertVisitReturnsAccount(List<MODEL.COD.CodStatsModel> codStatsList, string date)
        {
            return _cODBaseInfoDao.InsertVisitReturnsAccount(codStatsList, date);
        }

        public bool JudgeLogExists(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.JudgeLogExists(codStatsLog);
        }

        public bool UpdateStatisticsLog(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.UpdateStatisticsLog(codStatsLog);
        }

        public bool WriteStatisticsLog(MODEL.COD.CodStatsLogModel codStatsLog)
        {
            return _cODBaseInfoDao.WriteStatisticsLog(codStatsLog);
        }

        public List<MODEL.COD.CodStatsLogModel> GetStatsLogError(int statisticsType, string dateRemove, int accountDays)
        {
            return _cODBaseInfoDao.GetStatsLogError(statisticsType, dateRemove, accountDays);
        }


        public System.Data.DataTable GetDeliver(int accountDays)
        {
            return _cODBaseInfoDao.GetDeliver(accountDays);
        }

        public bool ChangeDeliverBack(List<string> noList)
        {
            return _cODBaseInfoDao.ChangeDeliverBack(noList);
        }

        public System.Data.DataTable GetError8(int accountDays)
        {
            return _cODBaseInfoDao.GetError8(accountDays);
        }

        public System.Data.DataTable GetError9(int accountDays)
        {
            return _cODBaseInfoDao.GetError9(accountDays);
        }

        public System.Data.DataTable GetError7(int accountDays)
        {
            return _cODBaseInfoDao.GetError7(accountDays);
        }

        public System.Data.DataTable GetError6(int accountDays)
        {
            return _cODBaseInfoDao.GetError6(accountDays);
        }

        public System.Data.DataTable GetError5(int accountDays)
        {
            return _cODBaseInfoDao.GetError5(accountDays);
        }

        public System.Data.DataTable GetError34(int errorType, int accountDays)
        {
            return _cODBaseInfoDao.GetError34(errorType,accountDays);
        }


        public IDictionary<long,FMS_CODBaseInfoCheck> GetDeliveryList(FMS_CODBaseInfoCheck model)
        {
            
              var  dt = _cODBaseInfoDao.GetDeliveryList(model);
              IDictionary<long,FMS_CODBaseInfoCheck> deliveryDic = new Dictionary<long, FMS_CODBaseInfoCheck>();
              if( dt.Rows.Count>0)
              {
                  for (int i = 0; i < dt.Rows.Count;i++ )
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
                      deliveryDic.Add(mod.WaybillNo,mod);
                  }
                 
              }
              return deliveryDic;
        }


        public IDictionary<long, FMS_CODBaseInfoCheck> GetReturnList(FMS_CODBaseInfoCheck model)
        {
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
            DataTable table = _cODBaseInfoDao.GetDeliverFeeParameter(waybillNo, distributionCode);

            if (table.Rows.Count == 0) return null;

            DataRow row = table.Rows[0];

            DeliverFeeModel model = new DeliverFeeModel();

            model.WaybillNO = waybillNo;
            model.InfoID = DataConvert.ToLong(row["InfoID"]);
            model.Area = DataConvert.ToInt(row["Area"]);
            model.Weight = DataConvert.ToDecimal(row["Weight"]);
            model.Formula = DataConvert.ToString(row["Formula"]);
            model.DeliverFee = DataConvert.ToDecimal(row["DeliverFee"]);
            model.IsFare = DataConvert.ToInt(row["IsFare"], -1);

            return model;
        }

        public bool EvalDeliverFee(MODEL.FinancialManage.DeliverFeeModel model)
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

        public bool SaveDeliverFee(MODEL.FinancialManage.DeliverFeeModel model)
        {
            IAccountOperatorLogDao service = new RFD.FMS.DAL.Oracle.COD.AccountOperatorLogDao();

            string changeLog = "用户" + LoginUser.UserName + "修改运单" + model.WaybillNO + "运单编号" + model.InfoID + "的支出配送费为" + model.DeliverFee
                                   + ",配送区域改为" + model.Area + ",结算重量改为" + model.Weight + ",配送公式改为" + model.Formula;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                if (_cODBaseInfoDao.SaveDeliverFee(model) == false) return false;

                if (service.AddOperatorLogLog(model.InfoID.ToString(), LoginUser.Userid, changeLog, (int)BizEnums.OperatorlogType.ChangeCODFee) == false) return false;

                work.Complete();
            }

            return true;
        }


        public bool UpdateEvalStatus(long waybillNo)
        {
            return _cODBaseInfoDao.UpdateEvalStatus(waybillNo);
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            return _cODBaseInfoDao.UpdateEvalStatus(waybillNos);
        }


        public DataTable GetCODDeliveryFeeInfo(DataTable ImportTable,int Type)
        {

            string InfoIDs = "";
            DataTable InfoDt = new DataTable();
            foreach (DataRow drow in ImportTable.Rows)
            {
                InfoIDs += drow["InfoID"].ToString() + ",";
            }
            InfoIDs = InfoIDs.Trim(',');

            if(Type == 0)
            {
               
                _cODBaseInfoDao.UpdateEvalStatusByInfoID(InfoIDs);
                Thread.Sleep(1000*60);
                 InfoDt = _cODBaseInfoDao.GetCODDeliveryFeeInfo(InfoIDs);
                InfoDt.Columns.Add("IsSucc", typeof(string));

                foreach (DataRow dr in InfoDt.Rows)
                {
                        var model = new DeliverFeeModel()
                                {
                                    DeliverFee = DataConvert.ToDecimal(dr["Fare"]),
                                    Weight = DataConvert.ToDecimal(dr["accountWeight"]),
                                    Formula = dr["FareFormula"].ToString()
                                };
                      try
                      {
                          if (EvalDeliverFee(model))
                          {
                            dr["Fare"] = model.DeliverFee.ToString();
                            dr["IsSucc"] = "1";
                           }
                          else
                          {
                            dr["IsSucc"] = "0";
                          }

                    }
                    catch (Exception ex)
                    {

                        dr["IsSucc"] = "计算公式不正确";
                        dr["Fare"] = 0;
                    }
                      
               
                }
            }
            else if(Type == 1)
            {
                 InfoDt = _cODBaseInfoDao.GetCODDeliveryFeeInfo(InfoIDs);
                InfoDt.Columns.Add("IsSucc", typeof(string));

                foreach (DataRow infoDr in InfoDt.Rows)
                {
                    foreach (DataRow impDr in ImportTable.Rows)
                    {
                        if(impDr["InfoID"].ToString() == infoDr["InfoID"].ToString())
                        {
                            infoDr["AreaType"] = impDr["AreaType"];
                            infoDr["AccountWeight"] = impDr["AccountWeight"];
                            infoDr["FareFormula"] = impDr["FareFormula"];

                            var model = new DeliverFeeModel()
                            {
                                DeliverFee = DataConvert.ToDecimal(infoDr["Fare"]),
                                Weight = DataConvert.ToDecimal(infoDr["accountWeight"]),
                                Formula = infoDr["FareFormula"].ToString()
                            };
                            try
                            {
                                if (EvalDeliverFee(model))
                                {
                                    infoDr["Fare"] = model.DeliverFee.ToString();
                                    infoDr["IsSucc"] = "1";
                                }
                                else
                                {
                                    infoDr["IsSucc"] = "0";
                                    infoDr["Fare"] = "0";
                                }
                            }
                            catch (Exception ex)
                            {

                                infoDr["IsSucc"] = "计算公式不正确";
                                infoDr["Fare"] = "0";
                            }
                           

                        }
                    }
                }
            }
            
            

            return InfoDt;

        }


        public bool UpdateBatchDeliverFee(DataTable CurrTable)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                foreach (DataRow dr in CurrTable.Rows)
                {
                    DeliverFeeModel model = new DeliverFeeModel()
                                            {
                                               InfoID =DataConvert.ToLong(dr["InfoID"]),
                                               WaybillNO = DataConvert.ToLong(dr["WaybillNo"]),
                                               Area=DataConvert.ToInt(dr["AreaType"]),
                                               DeliverFee = DataConvert.ToDecimal(dr["Fare"]),
                                               Weight = DataConvert.ToDecimal(dr["AccountWeight"]),
                                               Formula = dr["FareFormula"].ToString()
                                            };
                   IAccountOperatorLogDao service = new RFD.FMS.DAL.Oracle.COD.AccountOperatorLogDao();

                     string changeLog = "用户" + LoginUser.UserName + "修改运单" + model.WaybillNO+"运单编号"+model.InfoID + "的支出配送费为" + model.DeliverFee
                                    +",配送区域改为"+model.Area+",结算重量改为"+model.Weight+",配送公式改为"+model.Formula;

               
                    if (_cODBaseInfoDao.SaveDeliverFeeByID(model) == false) return false;

                    if (service.AddOperatorLogLog(model.InfoID.ToString(), LoginUser.Userid, changeLog, (int)BizEnums.OperatorlogType.ChangeCODFee) == false) return false;

                  }
                work.Complete();
            }
            return true;
        }


        public bool ExsitCodBaseInfoByNo(long waybillNo,long infoID)
        {
            return _cODBaseInfoDao.ExsitCodBaseInfoByNo(waybillNo,infoID);
        }
    }
}
