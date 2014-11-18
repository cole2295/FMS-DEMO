using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;
using RFD.FMS.DAL.FinancialManage;
using Microsoft.JScript.Vsa;
using Vancl.TML.ServiceInterface;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.FinancialManage;
using LMS.Util;
using System.Data;
using RFD.FMS.MODEL;
using LMS.AdoNet.UnitOfWork;

namespace RFD.FMS.Service.Reduct
{
    public class DeductService : IDeductService
    {
        private readonly FMS_DeductBaseInfoDao fmsDeductBaseInfoDao = new FMS_DeductBaseInfoDao();

        public void DealDetail()
        {
            //快递取件计算配送费
            DealExpressReceive();

            //快递派件计算配送费
            DealExpressSend();

            //项目派件计算配送费
            DealProjectSend();
        }

        /// <summary>
        /// 处理快递取件
        /// </summary>
        private bool DealExpressReceive()
        {
            IList<FMS_DeductBaseInfo> list = fmsDeductBaseInfoDao.GetExpressReceiveModel();

            if (list.Count == 0) return true;

            //查询所有取件站点
            string stationIds = GetReceiveStationIds(list);

            if (stationIds.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<int, string> dicStationFormula = ConvertToDictionary(fmsDeductBaseInfoDao.GetExpressReceiveFormula(stationIds));

            if (dicStationFormula.Count == 0) return true;

            //根据取件公式计算提成
            int stationId = -1;
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                foreach (var item in list)
                {
                    stationId = item.ReceiveStationID.Value;

                    if (dicStationFormula.ContainsKey(stationId) == false) return true;

                    formula = dicStationFormula[stationId];

                    basicCommission = 0;
                    basicWeight = 0;
                    weightAdd = 0;

                    TransferReductFormula(true, formula, ref basicWeight, ref weightAdd, ref basicCommission);

                    reductSubInfo = new FMS_DeductSubBaseInfo();

                    if (item.WayBillInfoWeight <= basicWeight)
                    {
                        reductSubInfo.WeightCommission = 0;
                    }
                    else
                    {
                        reductSubInfo.WeightCommission = GetExpressWeightCommisson(item, formula);
                    }

                    reductSubInfo.WaybillNO = item.WaybillNO;
                    reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressReceive;
                    reductSubInfo.Formula = formula;
                    reductSubInfo.StationId = stationId;
                    reductSubInfo.DeliverUser = item.ReceiveDeliverManID;
                    reductSubInfo.BasicCommission = basicCommission;
                    reductSubInfo.AreaCommission = DataConvert.ToDecimal(item.AreaReceiveDeduct);
                    reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                    reductSubInfo.CreateTime = DateTime.Now;
                    reductSubInfo.UpdateTime = DateTime.Now;
                    reductSubInfo.IsDeleted = 0;

                    item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                    if (fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;
                }

                work.Complete();
            }

            return true;
        }

        /// <summary>
        /// 处理快递派件
        /// </summary>
        private bool DealExpressSend()
        {
            IList<FMS_DeductBaseInfo> list = fmsDeductBaseInfoDao.GetExpressSendModel();

            if (list.Count == 0) return true;

            //查询所有派件站点
            string stationIds = GetSendStationIds(list);

            if (stationIds.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<int, string> dicStationFormula = ConvertToDictionary(fmsDeductBaseInfoDao.GetExpressSendFormula(stationIds));

            if (dicStationFormula.Count == 0) return true;

            //根据派件公式计算提成
            int stationId = -1;
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                foreach (var item in list)
                {
                    stationId = item.DeliverStationID;

                    if (dicStationFormula.ContainsKey(stationId) == false) continue;

                    formula = dicStationFormula[stationId];

                    basicCommission = 0;
                    basicWeight = 0;
                    weightAdd = 0;

                    TransferReductFormula(true, formula, ref basicWeight, ref weightAdd, ref basicCommission);

                    reductSubInfo = new FMS_DeductSubBaseInfo();

                    if (item.WayBillInfoWeight <= basicWeight)
                    {
                        reductSubInfo.WeightCommission = 0;
                    }
                    else
                    {
                        reductSubInfo.WeightCommission = GetExpressWeightCommisson(item, formula);
                    }

                    reductSubInfo.WaybillNO = item.WaybillNO;
                    reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressSend;
                    reductSubInfo.Formula = formula;
                    reductSubInfo.StationId = stationId;
                    reductSubInfo.DeliverUser = item.DeliverManID;
                    reductSubInfo.BasicCommission = basicCommission;
                    reductSubInfo.AreaCommission = DataConvert.ToDecimal(item.AreaSendDeduct);
                    reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                    reductSubInfo.CreateTime = DateTime.Now;
                    reductSubInfo.UpdateTime = DateTime.Now;
                    reductSubInfo.IsDeleted = 0;

                    item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                    if (fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;
                }

                work.Complete();
            }

            return true;
        }

        /// <summary>
        /// 处理项目派件
        /// </summary>
        private bool DealProjectSend()
        {
            IList<FMS_DeductBaseInfo> list = fmsDeductBaseInfoDao.GetProjectSendModel();

            if (list.Count == 0) return true;

            //查询所有派件站点
            string stationIds = "";
            string categoryCodes = "";

            GetProjectSendStationCategory(list, ref stationIds, ref categoryCodes);

            if (stationIds.Trim().Length == 0) return true;
            if (categoryCodes.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<string, string> dicStationFormula = ConvertToStationGoodsDictionary(fmsDeductBaseInfoDao.GetProjectSendFormula(stationIds, categoryCodes));

            if (dicStationFormula.Count == 0) return true;

            //根据派件公式计算提成
            int stationId = -1;
            string categoryCode = "";
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                foreach (var item in list)
                {
                    stationId = item.DeliverStationID;
                    categoryCode = item.WaybillCategory;

                    if (dicStationFormula.ContainsKey(stationId + "_" + categoryCode) == false) continue;

                    formula = dicStationFormula[stationId + "_" + categoryCode];

                    basicCommission = 0;
                    basicWeight = 0;
                    weightAdd = 0;

                    TransferReductFormula(false, formula, ref basicWeight, ref weightAdd, ref basicCommission);

                    reductSubInfo = new FMS_DeductSubBaseInfo();

                    if (item.WayBillInfoWeight <= basicWeight)
                    {
                        reductSubInfo.WeightCommission = 0;
                    }
                    else
                    {
                        reductSubInfo.WeightCommission = weightAdd;
                    }

                    reductSubInfo.WaybillNO = item.WaybillNO;
                    reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ProjectSend;
                    reductSubInfo.Formula = formula;
                    reductSubInfo.StationId = stationId;
                    reductSubInfo.DeliverUser = item.DeliverManID;
                    reductSubInfo.BasicCommission = basicCommission;
                    reductSubInfo.AreaCommission = DataConvert.ToDecimal(item.AreaSendDeduct);
                    reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                    reductSubInfo.CreateTime = DateTime.Now;
                    reductSubInfo.UpdateTime = DateTime.Now;
                    reductSubInfo.IsDeleted = 0;

                    item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                    if (fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;
                }

                work.Complete();
            }

            return true;
        }

        #region 帮助方法

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

        private string GetReceiveStationIds(IList<FMS_DeductBaseInfo> models)
        {
            FMS_DeductBaseInfo model = null;

            IList<int> ids = new List<int>();

            for (int i = 0; i < models.Count; i++)
            {
                model = models[i];

                if (model.ReceiveStationID == null) continue;
                if (model.ReceiveStationID.Value <= 0) continue;

                if (ids.Contains(model.ReceiveStationID.Value) == false)
                {
                    ids.Add(model.ReceiveStationID.Value);
                }
            }

            return DataConvert.ToDbIds(ids);
        }

        private void GetProjectSendStationCategory(IList<FMS_DeductBaseInfo> models, ref string stationIds, ref string categoryCodes)
        {
            IList<int> stationList = new List<int>();
            IList<string> categoryList = new List<string>();

            FMS_DeductBaseInfo model = null;

            for (int i = 0; i < models.Count; i++)
            {
                model = models[i];

                if (stationList.Contains(model.DeliverStationID) == false)
                {
                    stationList.Add(model.DeliverStationID);
                }

                if (categoryList.Contains(model.WaybillCategory) == false)
                {
                    categoryList.Add(model.WaybillCategory);
                }
            }

            stationIds = DataConvert.ToDbIds(stationList);
            categoryCodes = DataConvert.ToDbIds(categoryList);
        }

        private string GetSendStationIds(IList<FMS_DeductBaseInfo> models)
        {
            FMS_DeductBaseInfo model = null;

            IList<int> ids = new List<int>();

            for (int i = 0; i < models.Count; i++)
            {
                model = models[i];

                if (model.DeliverStationID <= 0) continue;

                if (ids.Contains(model.DeliverStationID) == false)
                {
                    ids.Add(model.DeliverStationID);
                }
            }

            return DataConvert.ToDbIds(ids);
        }

        private IDictionary<int, string> ConvertToDictionary(DataTable table)
        {
            IDictionary<int, string> dicStationFormula = new Dictionary<int, string>();

            DataRow row = null;

            int stationId = -1;
            string formula = "";

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                stationId = DataConvert.ToInt(row["StationId"]);
                formula = DataConvert.ToString(row["formula"]);

                if (dicStationFormula.ContainsKey(stationId) == false)
                {
                    dicStationFormula.Add(stationId, formula);
                }
            }

            return dicStationFormula;
        }

        private IDictionary<string, string> ConvertToStationGoodsDictionary(DataTable table)
        {
            IDictionary<string, string> dicStationFormula = new Dictionary<string, string>();

            DataRow row = null;

            int stationId = -1;
            string formula = "";
            string goodsCategoryCode = "";

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                stationId = DataConvert.ToInt(row["StationId"]);
                formula = DataConvert.ToString(row["formula"]);
                goodsCategoryCode = DataConvert.ToString(row["GoodsCategoryCode"]);

                if (dicStationFormula.ContainsKey(stationId + "_" + goodsCategoryCode) == false)
                {
                    dicStationFormula.Add(stationId + "_" + goodsCategoryCode, formula);
                }
            }

            return dicStationFormula;
        }

        private void TransferReductFormula(bool isExpress, string formula, ref decimal basicWeight, ref decimal weightAdd, ref decimal basicCommisson)
        {
            //快递
            if (isExpress == true)
            {
                var tempString = formula.Substring(formula.IndexOf('-') + 1);

                basicWeight = DataConvert.ToDecimal(tempString.Split(')')[0]);

                tempString = formula.Substring(formula.IndexOf('*') + 1);

                weightAdd = DataConvert.ToDecimal(tempString.Split('+')[0]);

                basicCommisson = DataConvert.ToDecimal(formula.Split('+')[1]);
            }
            //项目
            else
            {
                //if(重量>{0}) {{1}+{2}} else {2}

                var tempString = formula.Substring(formula.IndexOf('>') + 1);

                //{0}) {{1}+{2}} else {2}

                basicWeight = DataConvert.ToDecimal(tempString.Split(')')[0]);

                //{0}) {{1}

                tempString = tempString.Substring(0, tempString.IndexOf('+'));

                weightAdd = DataConvert.ToDecimal(tempString.Split('{')[1]);

                tempString = formula.Substring(formula.LastIndexOf(' ') + 1);

                basicCommisson = DataConvert.ToDecimal(tempString);
            }
        }

        private decimal GetExpressWeightCommisson(FMS_DeductBaseInfo model, string formula)
        {
            formula = formula.Substring(0,formula.IndexOf('+'));

            return EvalJScript(formula.Replace("取整", "Math.floor").Replace("重量", model.WayBillInfoWeight.ToString()));
        }

        #endregion
    }
}
