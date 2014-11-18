using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using Microsoft.JScript.Vsa;
using System.Text.RegularExpressions;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Util;
using RFD.FMS.Domain.COD;
using RFD.FMS.MODEL;
using System.Data;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
    public class IncomeBaseInfoService : IIncomeBaseInfoService
    {
        private IIncomeBaseInfoDao Dao;
        private IIncomeFeeInfoDao FeeDao;

        public DataTable GetIncomeDailyReport(string beginTime, string endTime, string merchantIds, string distributionCode)
        {
            DataTable table = Dao.GetIncomeDailyReport(beginTime, endTime, merchantIds, distributionCode);

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
            DataTable table = Dao.GetIncomeDailyReportSum(beginTime, endTime, merchantIds, distributionCode);

            DataRow row = table.Rows[0];

            Dictionary<string, string> dicValues = new Dictionary<string, string>();

            dicValues.Add("WaybillCount", DataConvert.ToString(row["WaybillCount"]));
            dicValues.Add("AccountFare", DataConvert.ToString(row["AccountFare"]));
            dicValues.Add("ProtectedFee", DataConvert.ToString(row["ProtectedFee"]));
            dicValues.Add("ReceiveFee", DataConvert.ToString(row["ReceiveFee"]));
            dicValues.Add("POSReceiveServiceFee", DataConvert.ToString(row["POSReceiveServiceFee"]));
            dicValues.Add("SumFee", DataConvert.ToString(row["SumFee"]));
            dicValues.Add("AvgFee", DataConvert.ToString(row["AvgFee"]));

            return dicValues;
        }

        public DeliverFeeModel GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            DataTable table = Dao.GetDeliverFeeParameter(waybillNo, distributionCode);

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
            IAccountOperatorLogDao service = new RFD.FMS.DAL.Oracle.COD.AccountOperatorLogDao();

            string changeLog = "用户" + LoginUser.UserName + "用户ID" + LoginUser.Userid + "修改运单" + model.WaybillNO + "运单编号" + model.InfoID + "的收入配送费为" + model.DeliverFee
                                      + ",配送区域改为" + model.Area + ",结算重量改为" + model.Weight + ",配送公式改为" + model.Formula;

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

            formula = formula.Replace("向上取整", "Math.ceil").Replace("不取整", "").Replace("取整", "Math.floor").Replace("重量", model.Weight.ToString());

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
            return Dao.UpdateEvalStatus(waybillNo);
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            return Dao.UpdateEvalStatus(waybillNos);
        }


        public DataTable GetIncomeDeliveryFeeInfo(DataTable ImportTable, int Type)
        {
            DataTable IncomeFeeDt = new DataTable();

            if (Type == 0)
            {
                long count = ImportTable.Rows.Count;

                long length = (long)Math.Ceiling(count * 1.00 / 500);


                for (int i = 0; i < length; i++)
                {
                    List<long> incomeFeeIDs = new List<long>();
                    DataTable dt = new DataTable();
                    for (int j = i * 500; j < (i + 1) * 500 && j < ImportTable.Rows.Count; j++)
                    {
                        incomeFeeIDs.Add(long.Parse(ImportTable.Rows[j]["IncomeFeeID"].ToString()));
                    }

                    FeeDao.UpdateEvalStatusByIncomeFeeID(incomeFeeIDs);
                    Thread.Sleep(1000 * 1);
                    dt = FeeDao.GetIncomeDeliveryFeeInfo(incomeFeeIDs);
                    dt.Columns.Add("IsSucc", typeof(string));
                    if (IncomeFeeDt.Rows.Count == 0)
                    {
                        IncomeFeeDt = dt.Clone();
                    }

                    foreach (DataRow dataRow in dt.Rows)
                    {
                        DataRow tempRow = IncomeFeeDt.NewRow();
                        tempRow["IncomeFeeID"] = dataRow["IncomeFeeID"];
                        tempRow["AreaType"] = dataRow["AreaType"];
                        tempRow["WaybillNo"] = dataRow["WaybillNo"];
                        tempRow["AccountWeight"] = dataRow["AccountWeight"];
                        tempRow["AccountStandard"] = dataRow["AccountStandard"];
                        tempRow["AccountFare"] = dataRow["AccountFare"];
                        IncomeFeeDt.Rows.Add(tempRow);
                    }
                }


                foreach (DataRow dr in IncomeFeeDt.Rows)
                {
                    var model = new DeliverFeeModel()
                    {
                        DeliverFee = DataConvert.ToDecimal(dr["AccountFare"]),
                        Weight = DataConvert.ToDecimal(dr["AccountWeight"]),
                        Formula = dr["AccountStandard"].ToString()
                    };
                    try
                    {
                        if (EvalDeliverFee(model))
                        {
                            dr["AccountFare"] = model.DeliverFee.ToString();
                            dr["IsSucc"] = "";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(dr["AreaType"].ToString()))
                            {
                                dr["IsSucc"] = dr["IsSucc"].ToString() + "区域类型未找到,";
                            }
                            if (!string.IsNullOrEmpty(dr["AreaType"].ToString()) && !Regex.Match(dr["AreaType"].ToString(), @"^\d+").Success)
                            {

                                dr["IsSucc"] = dr["IsSucc"].ToString() + "区域类型不正确,";
                            }
                            if (string.IsNullOrEmpty(dr["AccountWeight"].ToString()))
                            {
                                dr["IsSucc"] = dr["IsSucc"].ToString() + "结算重量未找到,";
                            }
                            if (!string.IsNullOrEmpty(dr["AccountWeight"].ToString()) && !Regex.Match(dr["AccountWeight"].ToString(), @"(^\d{1,3}$)|(^\d{1,3}\.\d{1,3}$)").Success)
                            {

                                dr["IsSucc"] = dr["IsSucc"].ToString() + "结算重量不正确,小数点不超过3位,";
                            }
                            if (string.IsNullOrEmpty(dr["AccountStandard"].ToString()))
                            {
                                dr["IsSucc"] = dr["IsSucc"].ToString() + "价格公式未找到";
                            }
                            else
                            {
                                dr["IsSucc"] = dr["IsSucc"].ToString() + "配送公式格式错误";
                            }


                            dr["AccountFare"] = 0;
                        }

                    }
                    catch (Exception ex)
                    {

                        dr["IsSucc"] = "计算公式不正确";
                        dr["AccountFare"] = 0;
                    }


                }
            }
            else if (Type == 1)
            {
                long count = ImportTable.Rows.Count;

                long length = (long)Math.Ceiling(count * 1.00 / 1000);


                for (int i = 0; i < length; i++)
                {
                    List<long> incomeFeeIDs = new List<long>();
                    DataTable dt = new DataTable();
                    for (int j = i * 1000; j < (i + 1) * 1000 && j < ImportTable.Rows.Count; j++)
                    {
                        incomeFeeIDs.Add(long.Parse(ImportTable.Rows[j]["IncomeFeeID"].ToString()));
                    }
                    dt = FeeDao.GetIncomeDeliveryFeeInfo(incomeFeeIDs);
                    dt.Columns.Add("IsSucc", typeof(string));
                    if (IncomeFeeDt.Rows.Count == 0)
                    {
                        IncomeFeeDt = dt.Clone();
                    }
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        DataRow tempRow = IncomeFeeDt.NewRow();
                        tempRow["IncomeFeeID"] = dataRow["IncomeFeeID"];
                        tempRow["AreaType"] = dataRow["AreaType"];
                        tempRow["WaybillNo"] = dataRow["WaybillNo"];
                        tempRow["AccountWeight"] = dataRow["AccountWeight"];
                        tempRow["AccountStandard"] = dataRow["AccountStandard"];
                        tempRow["AccountFare"] = dataRow["AccountFare"];
                        IncomeFeeDt.Rows.Add(tempRow);
                    }


                }

                foreach (DataRow infoDr in IncomeFeeDt.Rows)
                {
                     foreach (DataRow impDr in ImportTable.Rows)
                     {
                         if (impDr["IncomeFeeID"].ToString() == infoDr["IncomeFeeID"].ToString())
                         {
                             infoDr["AreaType"] = impDr["AreaType"];
                             infoDr["AccountWeight"] = impDr["AccountWeight"];
                             infoDr["AccountStandard"] = impDr["AccountStandard"];

                             var model = new DeliverFeeModel()
                             {
                                 DeliverFee = DataConvert.ToDecimal(infoDr["AccountFare"]),
                                 Weight = DataConvert.ToDecimal(infoDr["AccountWeight"]),
                                 Formula = infoDr["AccountStandard"].ToString()
                             };
                             try
                             {
                                 if (EvalDeliverFee(model))
                                 {
                                     infoDr["AccountFare"] = model.DeliverFee.ToString();
                                     infoDr["IsSucc"] = "";
                                 }
                                 else
                                 {
                                     if (string.IsNullOrEmpty(infoDr["AreaType"].ToString()))
                                     {
                                         infoDr["IsSucc"] = infoDr["IsSucc"].ToString() + "区域类型未找到,";
                                     }
                                     if (!string.IsNullOrEmpty(infoDr["AreaType"].ToString()) && !Regex.Match(infoDr["AreaType"].ToString(), @"^\d+").Success)
                                     {

                                         infoDr["IsSucc"] = infoDr["IsSucc"].ToString() + "区域类型不正确,";
                                     }
                                     if (string.IsNullOrEmpty(infoDr["AccountWeight"].ToString()))
                                     {
                                         infoDr["IsSucc"] = infoDr["IsSucc"].ToString() + "结算重量未找到,";
                                     }
                                     if (!string.IsNullOrEmpty(infoDr["AccountWeight"].ToString()) && !Regex.Match(infoDr["AccountWeight"].ToString(), @"(^\d{1,3}$)|(^\d{1,3}\.\d{1,3}$)").Success)
                                     {

                                         infoDr["IsSucc"] = infoDr["IsSucc"].ToString() + "结算重量不正确,小数点不超过3位,";
                                     }
                                     if (string.IsNullOrEmpty(infoDr["AccountStandard"].ToString()))
                                     {
                                         infoDr["IsSucc"] = infoDr["IsSucc"].ToString() + "价格公式未找到,";
                                     }
                                     else
                                     {
                                         infoDr["IsSucc"] = infoDr["IsSucc"].ToString() + "配送公式格式错误";
                                     }

                                     infoDr["AccountFare"] = "0";
                                 }
                             }
                             catch (Exception ex)
                             {

                                 infoDr["IsSucc"] = "计算公式不正确";
                                 infoDr["AccountFare"] = "0";
                             } 
                         }
                         
                     }

                }
            }
            return IncomeFeeDt;
        }

        public bool ExsitIncomeFeeInfoByNo(long waybillNo, long incomeFeeID)
        {
            return FeeDao.ExsitIncomeFeeInfoByNo(waybillNo, incomeFeeID);
        }
        public DataTable ExsitIncomeFeeInfoByNo(long waybillNO)
        {
            return FeeDao.ExsitIncomeFeeInfoByNo(waybillNO);
        }
        public bool BatchSaveDeliverFee(DeliverFeeModel model)
        {
            IAccountOperatorLogDao service = new RFD.FMS.DAL.Oracle.COD.AccountOperatorLogDao();

            string changeLog = "用户" + LoginUser.UserName +"用户ID"+LoginUser.Userid+ "修改运单" + model.WaybillNO + "运单编号" + model.InfoID + "的收入配送费为" + model.DeliverFee
                                     + ",配送区域改为" + model.Area + ",结算重量改为" + model.Weight + ",配送公式改为" + model.Formula;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                if (Dao.SaveDeliverFee(model) == false) return false;

                if (service.AddOperatorLogLog(model.WaybillNO.ToString(), LoginUser.Userid, changeLog, (int)BizEnums.OperatorlogType.ChangeIncomeFee) == false) return false;

                work.Complete();
            }

            return true;
        }

    }
}
