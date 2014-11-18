using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.JScript.Vsa;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Service;
using RFD.Common.Interface;
using RFD.Common.Model;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class IncomeBaseInfoWaybillStatusObserver : IWaybillStatusObserver
    {
        /// <summary>
        /// 将得到的列表查询入数据表中 add by wangyongc 2012-04-13（Use财务静态表）
        /// </summary>
        /// <param name="ModelList">对象列表</param>
        /// <returns></returns>
        public int InertIntoBaseInfoList(List<WaybillStatusChangeLog> ModelList)
        {
            if (ModelList.Count > 0)
            {
                int iResult = 0;

                foreach (WaybillStatusChangeLog model in ModelList)
                {
                    iResult += InsertIntoInComeBaseInfo(model);
                }

                return iResult;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 实现主方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DoAction(object objModel)
        {
            try
            {
                WaybillStatusChangeLog model = objModel as WaybillStatusChangeLog;

                if (model.Status != "-5")
                {
                    if (InsertIntoInComeBaseInfo(model) == 1) return true;

                    return false;
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetSqlCondition()
        {
            return "";
        }

        /// <summary>
        /// 将对象插入数据表 add by wangyongc 2012-04-13 （Use财务静态表）
        /// </summary>
        /// <param name="Logmodel"></param>
        /// <returns></returns>
        public int InsertIntoInComeBaseInfo(WaybillStatusChangeLog Logmodel)
        {
            IIncomeBaseInfoDao BaseDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            string TipList = ""; //提示

            try
            {
                //如果判断已经存在数据
                if (!BaseDao.Exists(Logmodel.WaybillNo))
                {
                    DataTable dt = BaseDao.GetWaybillInfoByNO(Logmodel.WaybillNo);

                    if (dt == null || dt.Rows.Count != 1) return 0;

                    FMS_IncomeBaseInfo BaseModel = GetBaseInfoModel(dt.Rows[0]);
                    FMS_IncomeFeeInfo FeeModel = GetFeeInfoModel(dt.Rows[0]);

                    //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                    using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                    {
                        BaseDao.Add(BaseModel);
                        FeeDao.Add(FeeModel);

                        //提交事务
                        work.Complete();
                        //成功一笔记录一笔
                        TipList += "成功添加订单：" + Logmodel.WaybillNo + "   ";
                        //记录成功日志ID
                    }

                    return 1;
                }
                else
                {
                    //减少读取次数
                    DataTable dt = new DataTable();

                    if (Logmodel.Status == "-4" ||
                        Logmodel.Status == "-3" ||
                        Logmodel.Status == "-1" ||
                        Logmodel.Status == "3" || 
                        Logmodel.Status == "5" || 
                        Logmodel.Status == "4" || 
                        Logmodel.Status == "-9")
                    {
                        dt = BaseDao.GetWaybillInfoByNO(Logmodel.WaybillNo);
                    }
                    else
                    {
                        return 1;
                    }
                        
                    if (dt == null || dt.Rows.Count != 1) return 0;

                    FMS_IncomeBaseInfo BaseModel = GetBaseInfoModel(dt.Rows[0]);
                    FMS_IncomeFeeInfo FeeModel = GetFeeInfoModel(dt.Rows[0]);

                    //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                    using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                    {
                        if (Logmodel.Status == "4")
                        {
                            BaseDao.UpdateBackStation(BaseModel);
                        }

                        //存在重新称重，需要更新下信息
                        if (Logmodel.Status == "-3" || Logmodel.Status == "-4" || Logmodel.Status == "-1")
                        {
                            BaseDao.Update(BaseModel);
                        }

                        if (Logmodel.Status == "3" || Logmodel.Status == "5" || Logmodel.Status == "-9")
                        {
                            if (Logmodel.Status == "-9")
                            {
                                BaseModel.BackStationStatus = "-9";
                                //BaseModel.BackStationTime = DateTime.Now;
                            }

                            BaseDao.UpdateStatus(BaseModel);

                            //只有归班时才知道是否月结
                            if (!string.IsNullOrEmpty(BaseModel.PeriodAccountCode))
                            {
                                FeeDao.UpdateByBackStation(FeeModel);
                            }
                        }

                        if (Logmodel.SubStatus == 6 || Logmodel.SubStatus == 7)
                        {
                            BaseDao.UpdateBackStatus(BaseModel);
                        }
                        //提交事务
                        work.Complete();
                        //记录成功日志ID
                        //    }
                    }

                    return 1;
                }
            }
            catch (Exception e)
            {
                TipList += "失败订单：" + Logmodel.WaybillNo + "   " + e.Message;

                SendFailedMail("收入结算服务程序异常", "程序出现异常,异常原因：" + e.Message);

                return 0;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_IncomeBaseInfo GetBaseInfoModel(DataRow row)
        {
            var model = new FMS_IncomeBaseInfo();

            if (row["WaybillNo"].ToString() != "")
            {
                model.WaybillNo = Int64.Parse(row["WaybillNo"].ToString());
            }
            model.WaybillType = row["WaybillType"].ToString();
            if (row["MerchantID"].ToString() != "")
            {
                model.MerchantID = Int32.Parse(row["MerchantID"].ToString());
            }
            if (row["CreatStation"].ToString() != "")
            {
                model.ExpressCompanyID = Int32.Parse(row["CreatStation"].ToString());
            }
            if (row["FinalExpressCompanyID"].ToString() != "")
            {
                model.FinalExpressCompanyID = Int32.Parse(row["FinalExpressCompanyID"].ToString());
            }
            if (row["DeliverStationID"].ToString() != "")
            {
                model.DeliverStationID = Int32.Parse(row["DeliverStationID"].ToString());
            }
            if (row["TopCODCompanyID"].ToString() != "")
            {
                model.TopCODCompanyID = Int32.Parse(row["TopCODCompanyID"].ToString());
            }
            if (row["CreatTime"].ToString() != "")
            {
                model.RfdAcceptTime = System.DateTime.Parse(row["CreatTime"].ToString());
            }
            if (row["DeliverTime"].ToString() != "")
            {
                model.DeliverTime = System.DateTime.Parse(row["DeliverTime"].ToString());
            }
            if (row["ReturnTime"].ToString() != "")
            {
                model.ReturnTime = System.DateTime.Parse(row["ReturnTime"].ToString());
            }
            if (row["ReturnExpressCompanyID"].ToString() != "")
            {
                model.ReturnExpressCompanyID = Int32.Parse(row["ReturnExpressCompanyID"].ToString());
            }
            if (row["BackStationTime"].ToString() != "")
            {
                model.BackStationTime = System.DateTime.Parse(row["BackStationTime"].ToString());
            }
            model.BackStationStatus = row["BackStationStatus"].ToString();
            if (row["ProtectedAmount"].ToString() != "")
            {
                model.ProtectedAmount = System.Decimal.Parse(row["ProtectedAmount"].ToString());
            }
            if (row["TotalAmount"].ToString() != "")
            {
                model.TotalAmount = System.Decimal.Parse(row["TotalAmount"].ToString());
            }
            if (row["PaidAmount"].ToString() != "")
            {
                model.PaidAmount = System.Decimal.Parse(row["PaidAmount"].ToString());
            }
            if (row["NeedPayAmount"].ToString() != "")
            {
                model.NeedPayAmount = System.Decimal.Parse(row["NeedPayAmount"].ToString());
            }
            if (row["BackAmount"].ToString() != "")
            {
                model.BackAmount = System.Decimal.Parse(row["BackAmount"].ToString());
            }
            if (row["NeedBackAmount"].ToString() != "")
            {
                model.NeedBackAmount = System.Decimal.Parse(row["NeedBackAmount"].ToString());
            }
            if (row["AccountWeight"].ToString() != "")
            {
                model.AccountWeight = System.Decimal.Parse(row["AccountWeight"].ToString());
            }
            model.AreaID = row["AreaID"].ToString();
           
            model.ReceiveAddress = row["ReceiveAddress"].ToString();
           
            if (row["SignType"].ToString() != "")
            {
                model.SignType = Int32.Parse(row["SignType"].ToString());
            }
            if (row["InefficacyStatus"].ToString() != "")
            {
                model.InefficacyStatus = Int32.Parse(row["InefficacyStatus"].ToString());
            }


            if (row["ReceiveStationID"].ToString() != "")
            {
                model.ReceiveStationID = Int32.Parse(row["ReceiveStationID"].ToString());
            }
            if (row["ReceiveDeliverManID"].ToString() != "")
            {
                model.ReceiveDeliverManID = Int32.Parse(row["ReceiveDeliverManID"].ToString());
            }
            if (row["DistributionCode"].ToString() != "")
            {
                model.DistributionCode = row["DistributionCode"].ToString();
            }
            if (row["CurrentDistributionCode"].ToString() != "")
            {
                model.CurrentDistributionCode = row["CurrentDistributionCode"].ToString();
            }
            if (row["WayBillInfoWeight"].ToString() != "")
            {
                model.WayBillInfoWeight = System.Decimal.Parse(row["WayBillInfoWeight"].ToString());
            }
            if (row["BackStatus"].ToString() != "")
            {
                model.SubStatus = Int32.Parse(row["BackStatus"].ToString());
            }
            if (row["AcceptType"].ToString() != "")
            {
                model.AcceptType = row["AcceptType"].ToString();
            }
            if (row["CustomerOrder"].ToString() != "")
            {
                model.CustomerOrder = row["CustomerOrder"].ToString();
            }
            if (row["OriginDepotNo"].ToString() != "")
            {
                model.OriginDepotNo = row["OriginDepotNo"].ToString();
            }
            if (row["PeriodAccountCode"].ToString() != "")
            {
                model.PeriodAccountCode = row["PeriodAccountCode"].ToString();
            }

            model.createtime = DateTime.Now;

            model.updatetime = DateTime.Now;


            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_IncomeFeeInfo GetFeeInfoModel(DataRow row)
        {
            var model = new FMS_IncomeFeeInfo();

            model.WaybillNO = Int64.Parse(row["WaybillNO"].ToString());
            if ((row["MerchantID"].ToString() == "8" || 
                row["MerchantID"].ToString() == "9" || 
                row["MerchantID"].ToString() == "30" || 
                !string.IsNullOrEmpty(row["PeriodAccountCode"].ToString())) 
                && row["DistributionCode"].ToString() =="rfd")
            {
                model.AccountFare = System.Decimal.Parse(row["TransferFee"].ToString());
                model.IsAccount = 2;
            }
            else
            {
                model.AccountFare = 0;
                model.IsAccount = 0;
            }



            model.ProtectedFee = 0;
            model.IsDeductAcount = 0;
            model.ReceiveFee = 0;
            if (row["TransferPayType"].ToString() != "")
            {
                model.TransferPayType = Int32.Parse(row["TransferPayType"].ToString());
            }
            if (row["DeputizeAmount"].ToString() != "")
            {
                model.DeputizeAmount = System.Decimal.Parse(row["DeputizeAmount"].ToString());
            }
            model.CreateTime = DateTime.Now;
            return model;
        }


        /// <summary>
        /// 计算相关费用 add by wangyongc 2012-04-19 （Use财务静态表）
        /// </summary>
        /// <param name="Logmodel"></param>
        /// <returns></returns>
        public void UpdateIncomeFeeInfoDao(int Num)
        {
            IIncomeBaseInfoDao BaseDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            DataTable dt = FeeDao.GetInComeFeeInfo(Num);
            //取值
            string TipList = ""; //提示
            string WaybillNOSucList = "";//成功订单
            string WaybillNoFasList = "";//失败订单

            if (dt == null && dt.Rows.Count == 0)
            {
                WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "没有取到需要计算费用的数据！");
            }

            WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "取到需要计算费用的数据"+dt.Rows.Count.ToString()+"！");

            foreach (DataRow row in dt.Rows)
            {
                FMS_IncomeFeeInfo Model = GetUpdateFeeInfoModel(row);

                try
                {
                    //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                    using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                    {
                        FeeDao.Update(Model);
                        //提交事务
                        work.Complete();
                        //成功一笔记录一笔
                        WaybillNOSucList += "," + Model.WaybillNO + "   ";
                        //记录成功日志ID
                    }
                }
                catch (Exception e)
                {
                    WaybillNoFasList += "，" + Model.WaybillNO + "";

                    TipList += "程序出现异常,异常原因：" + e.Message + "   ";
                }

            }
            if (!String.IsNullOrEmpty(WaybillNOSucList))
            {
                WriteTest("计算成功订单：" + WaybillNOSucList);
            }
            if (!String.IsNullOrEmpty(WaybillNoFasList))
            {
                WriteTest("计算失败异常订单：" + WaybillNoFasList);
            }
            if (!String.IsNullOrEmpty(TipList))
            {
                WriteTest(TipList);
            }

            //return TipList;
        }

        /// <summary>
        /// 计算相关费用 add by wangyongc 2012-05-24 （Use财务静态表）
        /// </summary>
        /// <param name="Logmodel"></param>
        /// <returns></returns>
        public void UpdateIncomeFeeInfoDao(List<string> WaybollNOList)
        {
            IIncomeBaseInfoDao BaseDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            foreach (var WaybillNo in WaybollNOList)
            {
                try
                {
                    DataTable dt = FeeDao.GetInComeFeeInfo(WaybillNo);

                    //取值
                    string TipList = ""; //提示
                    string WaybillNOSucList = ""; //成功订单
                    string WaybillNoFasList = ""; //失败订单
                    if (dt == null && dt.Rows.Count == 0)
                    {
                        WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "没有取到需要计算费用的数据！");
                    }
                    WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "取到需要计算费用的数据" + dt.Rows.Count.ToString() +
                              "！");
                    foreach (DataRow row in dt.Rows)
                    {
                        FMS_IncomeFeeInfo Model = GetUpdateFeeInfoModel(row);

                        try
                        {
                            //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                            {

                                FeeDao.Update(Model);
                                //提交事务
                                work.Complete();
                                //成功一笔记录一笔
                                WaybillNOSucList += "," + Model.WaybillNO + "   ";
                                //记录成功日志ID
                                //    }
                            }
                        }
                        catch (Exception e)
                        {
                            WaybillNoFasList += "，" + Model.WaybillNO + "";
                            TipList += "程序出现异常,异常原因：" + e.Message + "   ";

                        }

                    }
                    if (!String.IsNullOrEmpty(WaybillNOSucList))
                    {
                        WriteTest("计算成功订单：" + WaybillNOSucList);
                    }
                    if (!String.IsNullOrEmpty(WaybillNoFasList))
                    {
                        WriteTest("计算失败异常订单：" + WaybillNoFasList);
                    }
                    if (!String.IsNullOrEmpty(TipList))
                    {
                        WriteTest(TipList);
                    }
                }
                catch (Exception e)
                {
                    WriteTest("异常：" + e.Message);

                }
            }
            //return TipList;
        }

        public static object EvalJScript(string JScript)
        {
            object Result = null;
            try
            {
                VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return Result;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_IncomeFeeInfo GetUpdateFeeInfoModel(DataRow row)
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            var model = new FMS_IncomeFeeInfo();
            model.WaybillNO = Int64.Parse(row["WaybillNO"].ToString());
            model = FeeDao.GetModel(model.WaybillNO);

            string strWriteText = "";
            //SELECT sdf.BasicDeliverFee FROM FMS_StationDeliverFee sdf(NOLOCK) WHERE MerchantID=4 AND ExpressCompanyID=2 AND AreaType=2)
            try
            {
                if (row["WaybillType"].ToString() == "0" && row["BackStationStatus"].ToString() == "3")
                {
                    model.AccountStandard = row["BasicDeliverFee"].ToString();
                }
                else if (row["WaybillType"].ToString() == "0" && row["SubStatus"].ToString() == "7")
                {
                    //SELECT mdf.RefuseFeeRate FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                    model.AccountStandard = row["BasicDeliverFee"] + "*" + row["RefuseFeeRate"];
                    //BasicDeliverFee*RefuseFeeRate
                }
                else if ((row["WaybillType"].ToString() == "1" || row["WaybillType"].ToString() == "2") && row["SubStatus"].ToString() == "7")
                {
                    //SELECT mdf.VisitReturnsFee FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                    model.AccountStandard = row["VisitReturnsFee"] + "*" + row["RefuseFeeRate"];
                }
                else if (row["WaybillType"].ToString() == "1" || row["WaybillType"].ToString() == "2" && row["SubStatus"].ToString() == "6")
                {
                    //SELECT mdf.VisitReturnsVFee FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                    model.AccountStandard = row["VisitReturnsFee"].ToString();
                }
                if (!String.IsNullOrEmpty(model.AccountStandard))
                {
                    string formula = model.AccountStandard.Replace("取整", "Math.floor").Replace("重量",
                                                                                               row["AccountWeight"].
                                                                                                   ToString());

                    model.AccountFare = System.Decimal.Parse(EvalJScript(formula).ToString());
                }
                else
                {
                    strWriteText = "商家" + row["MerchantID"] + "没有维护配送费结算标准";
                    //WriteTest("商家" + row["MerchantID"] + "没有维护配送费结算标准。");
                }

                //SELECT mdf.ProtectedParmer FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                try
                {
                    if (!String.IsNullOrEmpty(row["ProtectedParmer"].ToString()))
                    {
                        model.ProtectedStandard = System.Decimal.Parse(row["ProtectedParmer"].ToString());
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(strWriteText))
                        {
                            strWriteText += "、保价费结算标准";
                        }
                        else
                        {
                            strWriteText = "商家" + row["MerchantID"] + "没有维护保价费结算标准";
                        }
                        //WriteTest("商家" + row["MerchantID"] + "没有维护保价费结算标准。");
                    }
                    if (Convert.ToDecimal(row["ProtectedAmount"]) > 0)
                    {
                        model.ProtectedFee = System.Decimal.Parse(row["ProtectedAmount"].ToString()) *
                                             model.ProtectedStandard;
                    }
                    model.IsProtected = 1; //计算成功
                }
                catch (Exception)
                {
                    model.IsProtected = 9; //计算失败
                }


                try
                {

                    //SELECT mdf.ProtectedParmer FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                    //if (row["IsReceive"].ToString() != "")
                    //{
                    //    model.IsReceive = Int32.Parse(row["IsReceive"].ToString());
                    //}

                    if (row["AcceptType"].ToString() == "POS机")
                    {

                        //fuwufeiPOS
                        //
                        //SELECT mdf.ProtectedParmer FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                        if (row["ReceivePOSFeeRate"].ToString() != "")
                        {
                            model.POSReceiveStandard = System.Decimal.Parse(row["ReceivePOSFeeRate"].ToString());
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strWriteText))
                            {
                                strWriteText += "、代收货款POS手续费标准";
                            }
                            else
                            {
                                strWriteText = "商家" + row["MerchantID"] + "没有维护代收货款POS手续费标准";
                            }
                            //WriteTest("商家" + row["MerchantID"] + "没有维护代收货款POS手续费标准。");
                            model.POSReceiveStandard = 0;
                        }
                        if (row["NeedPayAmount"].ToString() != "")
                        {
                            model.POSReceiveFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) *
                                                  model.POSReceiveStandard;
                        }
                        //


                        //fuwufeiCash
                        //
                        //SELECT mdf.ProtectedParmer FROM FMS_MerchantDeliverFee mdf(NOLOCK) WHERE mdf.MerchantID=2 AND mdf.[Status]=已审核
                        if (row["POSServiceFee"].ToString() != "")
                        {
                            model.POSReceiveServiceStandard =
                                System.Decimal.Parse(row["POSServiceFee"].ToString());
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strWriteText))
                            {
                                strWriteText += "、代收货款POS服务费标准";
                            }
                            else
                            {
                                strWriteText = "商家" + row["MerchantID"] + "没有维护代收货款POS服务费标准";
                            }
                            //WriteTest("商家" + row["MerchantID"] + "没有维护代收货款POS服务费标准。");
                            model.POSReceiveStandard = 0;
                        }

                        if (row["NeedPayAmount"].ToString() != "")
                        {
                            model.POSReceiveServiceFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) *
                                                         model.POSReceiveServiceStandard;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(row["ReceiveFeeRate"].ToString()))
                        {
                            model.ReceiveStandard = System.Decimal.Parse(row["ReceiveFeeRate"].ToString());
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strWriteText))
                            {
                                strWriteText += "、代收货款现金手续费标准";
                            }
                            else
                            {
                                strWriteText = "商家" + row["MerchantID"] + "没有维护代收货款现金手续费标准";
                            }
                            //WriteTest("商家" + row["MerchantID"] + "没有维护代收货款现金手续费标准。");
                            model.ReceiveStandard = 0;

                        }
                        if (row["NeedPayAmount"].ToString() != "")
                        {
                            model.ReceiveFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) * model.ReceiveStandard;
                        }

                        //Cash
                        if (row["CashServiceFee"].ToString() != "")
                        {
                            model.CashReceiveServiceStandard =
                                System.Decimal.Parse(row["CashServiceFee"].ToString());
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strWriteText))
                            {
                                strWriteText += "、代收货款现金服务费标准";
                            }
                            else
                            {
                                strWriteText = "商家" + row["MerchantID"] + "没有维护代收货款现金服务费标准";
                            }
                            //WriteTest("商家" + row["MerchantID"] + "没有维护代收货款现金服务费标准。");
                            model.CashReceiveServiceStandard = 0;

                        }
                        if (row["NeedPayAmount"].ToString() != "")
                        {
                            model.CashReceiveServiceFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) *
                                                          model.CashReceiveServiceStandard;
                        }

                    }


                    //
                    model.IsReceive = 1;
                }
                catch (Exception e)
                {
                    model.IsReceive = 9;
                }
                if (model.IsReceive != 9 && model.IsProtected != 9)
                {
                    if (String.IsNullOrEmpty(strWriteText))
                    {
                        model.IsAccount = 1;
                    }
                    else
                    {
                        WriteTest(strWriteText);
                        model.IsAccount = 5;//基础数据没有维护
                    }
                }
                else
                {
                    WriteTest("计算失败订单:" + model.WaybillNO.ToString());
                    model.IsAccount = 9;
                }
            }
            catch (Exception e)
            {
                model.IsAccount = 9;
            }

            model.UpdateTime = DateTime.Now;

            return model;
        }



        /// <summary>
        /// 计算提成相关费用 add by wangyongc 2012-04-19 （Use财务静态表）
        /// </summary>
        /// <param name="Logmodel"></param>
        /// <returns></returns>
        public void AcountDeductFeeInfoDao(int Num)
        {
            IIncomeBaseInfoDao BaseDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            DataTable dt = FeeDao.GetInComeFeeInfo(Num);
            //取值
            string TipList = ""; //提示
            string WaybillNOSucList = ""; //成功订单
            string WaybillNoFasList = ""; //失败订单
            if (dt == null && dt.Rows.Count == 0)
            {
                WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "没有取到需要计算费用的数据！");
            }
            foreach (DataRow row in dt.Rows)
            {
                FMS_IncomeFeeInfo Model = GetUpdateDeductInfoModel(row);
                try
                {
                    //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                    using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                    {

                        FeeDao.Update(Model);
                        //提交事务
                        work.Complete();
                        //成功一笔记录一笔
                        WaybillNOSucList += "，" + Model.WaybillNO + "";
                        //记录成功日志ID
                        //    }
                    }
                }
                catch (Exception e)
                {
                    WaybillNoFasList += "，" + Model.WaybillNO + "";
                    TipList += "程序出现异常,异常原因：" + e.Message + "   ";
                    SendFailedMail("收入结算服务程序异常", "程序出现异常,异常原因：" + e.Message);

                }

            }
            WriteTest("计算成功订单：" + WaybillNOSucList);
            WriteTest("计算失败订单：" + WaybillNoFasList);
            WriteTest(TipList);
            //return TipList;
        }

        public FMS_IncomeFeeInfo GetUpdateDeductInfoModel(DataRow row)
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            var model = new FMS_IncomeFeeInfo();
            model.WaybillNO = Int64.Parse(row["WaybillNO"].ToString());
            model = FeeDao.GetModel(model.WaybillNO);

            return model;
        }
        /// <summary>
        /// 发送错误邮件
        /// </summary>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        public static void SendFailedMail(string mailSubject, string mailBody)
        {
            Mail mail = new Mail();
            mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
            //Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
        }

        /// <summary>
        /// 失败时发送邮件地址
        /// </summary>
        public static string FailedMailAdress
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["FailedMailAdress"];
                }
                catch (Exception ex)
                {
                    return "wangyongc@vancl.cn;zengwei@vancl.cn;";
                }
            }
        }

        ///// <summary>
        ///// 写日志
        ///// </summary>
        ///// <param name="tips"></param>
        public static void WriteTest(string tips)
        {
            try
            {
                tips = "(New)" + tips;
                //stringLog.Append(tips);
                string filepath = ConfigurationManager.AppSettings["BakFilePath"] + "/日志/";
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                var fs = new FileStream(filepath + DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt",
                                        FileMode.OpenOrCreate, FileAccess.Write);
                var mStreamWriter = new StreamWriter(fs);
                mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                mStreamWriter.WriteLine(tips);
                mStreamWriter.Flush();
                mStreamWriter.Close();
                fs.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
