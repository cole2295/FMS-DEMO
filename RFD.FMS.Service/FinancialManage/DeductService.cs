using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;
using RFD.FMS.DAL.FinancialManage;
using Microsoft.JScript.Vsa;
using Vancl.TML.ServiceInterface;
namespace RFD.FMS.Service.FinancialManage
{
    public class DeductService : IService
    {
        private readonly FMS_DeductBaseInfoDao fmsDeductBaseInfoDao = new FMS_DeductBaseInfoDao();
        private readonly ExpressCompanyDeductDao expressCompanyDeductDao = new ExpressCompanyDeductDao();
        private readonly ExpressCompanyGoodsDeductDao expressCompanyGoodsDeductDao=new ExpressCompanyGoodsDeductDao();

        public decimal EvalJScript(string JScript)
        {
            object Result = null;
            try
            {
                VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return decimal.Parse(Result.ToString());

        }


        #region IService 成员

        public void DealDetail(TaskModel taskModel)
        {
            List<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> list = fmsDeductBaseInfoDao.GetModel();
            if (list.Count == 0)
            {
                return;
            }
            List<int> expressCompanyId = new List<int>();
            foreach (var model in list)
            {
                if (model.DeliverStationID != null && !expressCompanyId.Contains((int)model.DeliverStationID))
                {
                    expressCompanyId.Add((int)model.DeliverStationID);
                }
                if (model.ReceiveStationID != null && !expressCompanyId.Contains((int)model.ReceiveStationID))
                {
                    expressCompanyId.Add((int)model.ReceiveStationID);
                }
            }
            Dictionary<int, RFD.FMS.MODEL.BasicSetting.ExpressCompanyDeduct> expressCompanyDeducts =
                expressCompanyDeductDao.GetModel(expressCompanyId);
            if (expressCompanyDeducts.Count == 0)
            {
                return;
            }

            Dictionary<string , RFD.FMS.MODEL.BasicSetting.ExpressCompanyGoodsDeduct> expressCompanyGoodsDeducts =
                expressCompanyGoodsDeductDao.GetModel(expressCompanyId);


            foreach (var model in list)
            {
                taskModel.Logger.Info(model.WaybillNO+"开始处理");
                #region 发件;
                if (model.DeliverStationID != null)
                {
                    if (expressCompanyDeducts.ContainsKey((int)model.DeliverStationID))
                    {
                        RFD.FMS.MODEL.BasicSetting.ExpressCompanyDeduct expressCompanyDeduct = expressCompanyDeducts[(int)model.DeliverStationID];
                        if (!string.IsNullOrEmpty(expressCompanyDeduct.ExpressSendDeduct))
                        {
                            if (model.WayBillInfoWeight == null)
                            {
                                model.WayBillInfoWeight = 0;
                            }
                            if (expressCompanyDeduct.SendBASICWEIGHT > model.WayBillInfoWeight)
                            {
                                model.ExpressSendBasicDeduct = expressCompanyDeduct.SendWEIGHTADDCOMMISSION;
                            }
                            else
                            {
                                model.ExpressSendBasicDeduct =

                               EvalJScript(expressCompanyDeduct.ExpressSendDeduct.Replace("取整", "Math.floor").Replace("重量",
                                                                                          model.WayBillInfoWeight.
                                                                                              ToString()));
                            }
                           
                            // ExpressSendDeduct
                            if (expressCompanyDeduct.SendBASICWEIGHT == null)
                            {
                                expressCompanyDeduct.SendBASICWEIGHT = 0;
                            }
                            if (expressCompanyDeduct.SendWEIGHTADDCOMMISSION == null)
                            {
                                expressCompanyDeduct.SendWEIGHTADDCOMMISSION = 0;
                            }
                            decimal WeightDeduct =
                                (decimal)((model.WayBillInfoWeight - expressCompanyDeduct.SendBASICWEIGHT) *
                                           expressCompanyDeduct.SendWEIGHTADDCOMMISSION);
                            model.ExpressSendWeightDeduct = WeightDeduct > 0 ? WeightDeduct : 0;
                            //（重量-SendBASICWEIGHT）* SendWEIGHTADDCOMMISSION

                            if(expressCompanyGoodsDeducts.ContainsKey(model.DeliverStationID + "[" + model.WaybillCategory + "]"))
                            {
                                var temp =
                                    expressCompanyGoodsDeducts[
                                        model.DeliverStationID + "[" + model.WaybillCategory + "]"];
                                model.ProgramSendBasicDeduct = temp.BasicCommission;
                                model.ProgramSendWeightDeduct = temp.ExtraCommission > model.WayBillInfoWeight
                                                                    ? 0
                                                                    : temp.ExtraCommission;
                            }
                            else
                            {
                                model.IsDeductAcount = 5;//没有配置expressCompanyGoodsDeduct
                            }


                        }
                        else
                        {
                            model.IsDeductAcount = 7;//没有配置ExpressCompanyDeduct中的公式
                        }
                    }
                    else
                    {
                        model.IsDeductAcount = 8; //没有配置ExpressCompanyDeduct
                    }
                }
                #endregion

                #region 收件
                if (model.ReceiveStationID != null)
                {

                    if (expressCompanyDeducts.ContainsKey((int)model.ReceiveStationID))
                    {
                        RFD.FMS.MODEL.BasicSetting.ExpressCompanyDeduct expressCompanyDeduct = expressCompanyDeducts[(int)model.ReceiveStationID];
                        if (!string.IsNullOrEmpty(expressCompanyDeduct.ExpressReceiveDeduct))
                        {


                            if (model.WayBillInfoWeight == null)
                            {
                                model.WayBillInfoWeight = 0;
                            }
                            if (expressCompanyDeduct.ReceiveBASICWEIGHT > model.WayBillInfoWeight)
                            {
                                model.ExpressReceiveBasicDeduct = expressCompanyDeduct.ReceiveBasicCommission;
                            }
                            else
                            {
                                model.ExpressReceiveBasicDeduct =

                                EvalJScript(expressCompanyDeduct.ExpressReceiveDeduct.Replace("取整", "Math.floor").Replace("重量",
                                                                                           model.WayBillInfoWeight.
                                                                                               ToString()));
                            }
                            if (expressCompanyDeduct.ReceiveBASICWEIGHT == null)
                            {
                                expressCompanyDeduct.ReceiveBASICWEIGHT = 0;
                            }
                            if (expressCompanyDeduct.ReceiveWEIGHTADDCOMMISSION == null)
                            {
                                expressCompanyDeduct.ReceiveWEIGHTADDCOMMISSION = 0;
                            }
                            decimal WeightDeduct =
                                (decimal)((model.WayBillInfoWeight - expressCompanyDeduct.ReceiveBASICWEIGHT) *
                                           expressCompanyDeduct.ReceiveWEIGHTADDCOMMISSION);
                            model.ExpressReceiveWeightDeduct = WeightDeduct > 0 ? WeightDeduct : 0;
                            //（重量-ReceiveBASICWEIGHT）* ReceiveWEIGHTADDCOMMISSION
                        }
                        else
                        {
                            model.IsDeductAcount = 6;
                        }
                    }
                    else
                    {
                        model.IsDeductAcount = 9;
                    }
                }
                #endregion

                #region 更新状态和信息
                if (model.IsDeductAcount == 0)
                {
                    model.IsDeductAcount = 1;
                }
                fmsDeductBaseInfoDao.Update(model);
                taskModel.Logger.Info(model.WaybillNO + "处理结束");
                #endregion
            }
        }

        #endregion
    }
}
