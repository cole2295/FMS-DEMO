using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.JScript.Vsa;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.WEBLOGIC.PMSOpenService;
using Vancl.TML.ServiceInterface;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet.UnitOfWork;
using System.IO;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Service;
using RFD.FMS.Service.BasicSetting;
using System.Windows.Forms;
using RFD.FMS.Service.Mail;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class DeductService : IDeductService
    {
        private IFMS_DeductBaseInfoDao _fmsDeductBaseInfoDao;
        IMail mail = ServiceLocator.GetService<IMail>();
        string DistributionCodeList = ConfigurationManager.AppSettings["DistributionCode"];
        public void DealDetail()
        {
            try
            {
                ////快递取件计算配送费
                DealExpressReceive();

                ////快递派件计算配送费
                DealExpressSend();

                ////项目派件计算配送费
                DealProjectSend();

                StatAreaDeductErrorInfo();
            }
            catch (Exception e )
            {
                mail.SendMailToUser("提成计算异常信息",e.Message+e.StackTrace, "wangyongc@vancl.cn;zengwei@vancl.cn");
            }

        }

        /// <summary>
        /// 处理快递取件
        /// </summary>
        private bool DealExpressReceive()
        {
            IList<FMS_DeductBaseInfo> list = _fmsDeductBaseInfoDao.GetExpressReceiveModel(DistributionCodeList);

            if (list.Count == 0) return true;

            //查询所有取件站点
            string stationIds = GetReceiveStationIds(list);

            if (stationIds.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<int, string> dicStationFormula = ConvertToDictionary(_fmsDeductBaseInfoDao.GetExpressReceiveFormula(stationIds));

            if (dicStationFormula.Count == 0) return true;

            //根据取件公式计算提成
            int stationId = -1;
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

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

                #region 计算区域提成逻辑


                bool bDeduct = false;
                string ResultMessage = "";
                decimal AreaCommission = 0;

                string StationID = ConfigurationManager.AppSettings["OpenStationV3"];

                string[] sList = StationID.Split(',');

                if (sList.Contains(stationId.ToString()))
                {

                    IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                    bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                            item.SendAddress);

                    if (bDeduct == false)
                    {
                        IAreaDeductErrorInfoDao areaDeductDao = new AreaDeductErrorInfoDao();
                        AreaDeductErrorInfo DeductErrorModel = new AreaDeductErrorInfo();
                        DeductErrorModel.WaybillNO = item.WaybillNO;
                        DeductErrorModel.StationId = stationId;
                        DeductErrorModel.ErrorInfo = ResultMessage;
                        DeductErrorModel.AddressInfo = item.SendAddress;
                        DeductErrorModel.Errortype = "2";
                        DeductErrorModel.DisposeStatus = "已计算";
                        DeductErrorModel.DeductType = "取件提成";
                        DeductErrorModel.CreateTime = DateTime.Now;
                        DeductErrorModel.UpdateTime = DateTime.Now;

                        try
                        {
                            int iRow = areaDeductDao.Add(DeductErrorModel);
                        }
                        catch (Exception)
                        {

                            StringBuilder builder = new StringBuilder();

                            builder.Append("<br/>");
                            builder.Append("单号:" + item.WaybillNO.ToString());
                            builder.Append("提成计算异常信息插入失败！");

                            mail.SendMailToUser("提成计算异常信息插入失败", builder.ToString(), "wangyongc@vancl.cn");
                        }

                    }

                }
                else
                {
                    AreaCommission = GetStationAreaDeduct(item.ReceiveStationID, item.AreaReceiveDeduct);
                }

                #endregion

               //_fmsDeductBaseInfoDao.DeleteOldDeduct(item.WaybillNO, (int)BizEnums.enumDeductType.ExpressReceive);

                reductSubInfo.WaybillNO = item.WaybillNO;
                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressReceive;
                reductSubInfo.Formula = formula;
                reductSubInfo.StationId = stationId;
                reductSubInfo.DeliverUser = item.ReceiveDeliverManID;
                reductSubInfo.BasicCommission = basicCommission;
                reductSubInfo.AreaCommission = AreaCommission;//GetStationAreaDeduct(item.ReceiveStationID, item.AreaReceiveDeduct);
                reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                reductSubInfo.CreateTime = DateTime.Now;
                reductSubInfo.UpdateTime = DateTime.Now;
                reductSubInfo.IsDeleted = 0;
                reductSubInfo.AdjustCommission = item.AreaSendDeduct ?? 0;
                item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                //if (.IsDeleted(item.DeductID)) continue;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    _fmsDeductBaseInfoDao.DeleteOldDeduct(item.WaybillNO,(int)BizEnums.enumDeductType.ExpressReceive);
                    if (_fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (_fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;

                    work.Complete();
                }
            }

            return true;
        }

        /// <summary>
        /// 处理快递派件
        /// </summary>
        private bool DealExpressSend()
        {
            IList<FMS_DeductBaseInfo> list = _fmsDeductBaseInfoDao.GetExpressSendModel(DistributionCodeList);

            if (list.Count == 0) return true;

            //查询所有派件站点
            string stationIds = GetSendStationIds(list);

            if (stationIds.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<int, string> dicStationFormula = ConvertToDictionary(_fmsDeductBaseInfoDao.GetExpressSendFormula(stationIds));

            if (dicStationFormula.Count == 0) return true;

            //根据派件公式计算提成
            int stationId = -1;
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

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

                #region 计算区域提成逻辑


                bool bDeduct = false;
                string ResultMessage = "";
                decimal AreaCommission = 0;

                string StationID = ConfigurationManager.AppSettings["OpenStationV3"];

                string[] sList = StationID.Split(',');

                if (sList.Contains(stationId.ToString()))
                {

                    IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                    bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                            item.ReceiveAddress);

                    if (bDeduct == false)
                    {
                        IAreaDeductErrorInfoDao areaDeductDao = new AreaDeductErrorInfoDao();
                        AreaDeductErrorInfo DeductErrorModel = new AreaDeductErrorInfo();
                        DeductErrorModel.WaybillNO = item.WaybillNO;
                        DeductErrorModel.StationId = stationId;
                        DeductErrorModel.ErrorInfo = ResultMessage;
                        DeductErrorModel.AddressInfo = item.ReceiveAddress;
                        DeductErrorModel.Errortype = "2";
                        DeductErrorModel.DisposeStatus = "已计算";
                        DeductErrorModel.DeductType = "快递派件提成";
                        DeductErrorModel.CreateTime = DateTime.Now;
                        DeductErrorModel.UpdateTime = DateTime.Now;

                        try
                        {
                            int iRow = areaDeductDao.Add(DeductErrorModel);
                        }
                        catch (Exception)
                        {

                            StringBuilder builder = new StringBuilder();

                            builder.Append("<br/>");
                            builder.Append("单号:" + item.WaybillNO.ToString());
                            builder.Append("提成计算异常信息插入失败！");

                            mail.SendMailToUser("提成计算异常信息插入失败", builder.ToString(), "wangyongc@vancl.cn");
                        }




                    }

                }
                else
                {
                    AreaCommission = GetStationAreaDeduct(item.DeliverStationID, item.AreaSendDeduct);
                }


                #endregion

                reductSubInfo.WaybillNO = item.WaybillNO;
                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressSend;
                reductSubInfo.Formula = formula;
                reductSubInfo.StationId = stationId;
                reductSubInfo.DeliverUser = item.DeliverManID;
                reductSubInfo.BasicCommission = basicCommission;
                reductSubInfo.AreaCommission = AreaCommission; //GetStationAreaDeduct(item.DeliverStationID, item.AreaSendDeduct);
                reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                reductSubInfo.CreateTime = (item.BackStationTime == null ? DateTime.Now : item.BackStationTime);
                reductSubInfo.UpdateTime = DateTime.Now;
                reductSubInfo.IsDeleted = 0;
                reductSubInfo.AdjustCommission = item.AreaSendDeduct??0;
                item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    _fmsDeductBaseInfoDao.DeleteOldDeduct(item.WaybillNO, (int)BizEnums.enumDeductType.ExpressSend);
                    if (_fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (_fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;

                    work.Complete();
                }
            }

            return true;
        }

        /// <summary>
        /// 处理项目派件
        /// </summary>
        private bool DealProjectSend()
        {
            IList<FMS_DeductBaseInfo> list = _fmsDeductBaseInfoDao.GetProjectSendModel(DistributionCodeList);

            if (list.Count == 0) return true;

            //查询所有派件站点
            string stationIds = "";
            string categoryCodes = "";

            GetProjectSendStationCategory(list, ref stationIds, ref categoryCodes);

            if (stationIds.Trim().Length == 0) return true;
            if (categoryCodes.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<string, string> dicStationFormula = ConvertToStationGoodsDictionary(_fmsDeductBaseInfoDao.GetProjectSendFormula(stationIds, categoryCodes));

            if (dicStationFormula.Count == 0) return true;

            //根据派件公式计算提成
            int stationId = -1;
            string categoryCode = "";
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

            IList<string> waybills = new List<string>();

            string key = "";

            foreach (var item in list)
            {
                stationId = item.DeliverStationID;
                categoryCode = item.WaybillCategory.Trim();

                key = stationId.ToString() + categoryCode.Trim();

                if (dicStationFormula.ContainsKey(key) == false)
                {
                    SendStationCategoryNotSetMail(stationId,categoryCode);

                    continue;
                }

                formula = dicStationFormula[key];

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

                #region 计算区域提成逻辑


                bool bDeduct = false;
                string ResultMessage = "";
                decimal AreaCommission = 0;

                string StationID = ConfigurationManager.AppSettings["OpenStationV3"];

                string[] sList = StationID.Split(',');

                if (sList.Contains(stationId.ToString()))
                {

                    IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                    bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                            item.ReceiveAddress);

                    if (bDeduct == false)
                    {
                        IAreaDeductErrorInfoDao areaDeductDao = new AreaDeductErrorInfoDao();
                        AreaDeductErrorInfo DeductErrorModel = new AreaDeductErrorInfo();
                        DeductErrorModel.WaybillNO = item.WaybillNO;
                        DeductErrorModel.StationId = stationId;
                        DeductErrorModel.ErrorInfo = ResultMessage;
                        DeductErrorModel.AddressInfo = item.ReceiveAddress;
                        DeductErrorModel.Errortype = "2";
                        DeductErrorModel.DisposeStatus = "已计算";
                        DeductErrorModel.DeductType = "项目派件提成";
                        DeductErrorModel.CreateTime = DateTime.Now;
                        DeductErrorModel.UpdateTime = DateTime.Now;

                        areaDeductDao.Add(DeductErrorModel);
                    }
                }
                else
                {
                    AreaCommission = GetStationAreaDeduct(item.DeliverStationID, item.AreaSendDeduct);
                }

                #endregion

                reductSubInfo.WaybillNO = item.WaybillNO;
                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ProjectSend;
                reductSubInfo.Formula = formula;
                reductSubInfo.StationId = stationId;
                reductSubInfo.DeliverUser = item.DeliverManID;
                reductSubInfo.BasicCommission = basicCommission;
                reductSubInfo.AreaCommission = AreaCommission; //GetStationAreaDeduct(item.DeliverStationID, item.AreaSendDeduct);
                reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                reductSubInfo.CreateTime = (item.BackStationTime == null ? DateTime.Now : item.BackStationTime);
                reductSubInfo.UpdateTime = DateTime.Now;
                reductSubInfo.IsDeleted = 0;
                reductSubInfo.AdjustCommission = item.AreaSendDeduct ?? 0;
                item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                //if () continue;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    _fmsDeductBaseInfoDao.DeleteOldDeduct(item.WaybillNO, (int) BizEnums.enumDeductType.ProjectSend);
                    if (_fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (_fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;

                    work.Complete();
                }
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

            //加载异常类型文件
            IDictionary<string,string> dicGoodsCategory = GetErrorGoodsCatelogs();

            IDictionary<int, string> dicMerchantVsCategory = new Dictionary<int, string>();

            IMerchantService merchantService = ServiceLocator.GetService<IMerchantService>();

            for (int i = 0; i < models.Count; i++)
            {
                model = models[i];

                if (stationList.Contains(model.DeliverStationID) == false)
                {
                    stationList.Add(model.DeliverStationID);
                }

                //如果货物品类为空，就读取商家设置的货物品类
                if (model.WaybillCategory.Trim().Length == 0)
                {
                    if (dicMerchantVsCategory.ContainsKey(model.MerchantID.Value))
                    {
                        model.WaybillCategory = dicMerchantVsCategory[model.MerchantID.Value];
                    }
                    else
                    {
                        var category = merchantService.GetMerchantCategory(model.MerchantID.Value);

                        model.WaybillCategory = category;

                        dicMerchantVsCategory.Add(model.MerchantID.Value,category);
                    }
                }

                //如果货物品类不存在，就发送邮件通知
                if (dicGoodsCategory.ContainsKey(model.WaybillCategory) == false)
                {
                    SendEmptyGoodsMail(model);

                    continue;
                }
                else
                {
                    model.WaybillCategory = dicGoodsCategory[model.WaybillCategory];
                }

                if (categoryList.Contains(model.WaybillCategory) == false)
                {
                    categoryList.Add(model.WaybillCategory);
                }
            }

            stationIds = DataConvert.ToDbIds(stationList);
            categoryCodes = DataConvert.ToDbIds(categoryList);
        }

        private IDictionary<string, string> GetErrorGoodsCatelogs()
        {
            IDictionary<string, string> dicKeys = new Dictionary<string, string>();

            string name = "";
            string code = "";

            using (StreamReader fileStream = new StreamReader(Application.StartupPath + @"\\异常类型.txt", System.Text.Encoding.Default))
            {
                string key = fileStream.ReadLine();

                while (key != null && key.Trim() != String.Empty)
                {
                    if (key.IndexOf(',') != -1)
                    {
                        name = key.Split(',')[0];
                        code = key.Split(',')[1];

                        dicKeys.Add(name, code);
                    }

                    key = fileStream.ReadLine();
                }
            }

            IGoodsCategoryService service = ServiceLocator.GetService<IGoodsCategoryService>();

            DataTable table = service.GetAllGoods();

            DataRow row = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                code = DataConvert.ToString(row["Code"]);

                dicKeys.Add(code,code);
            }

            return dicKeys;
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

                if (dicStationFormula.ContainsKey(stationId.ToString() + goodsCategoryCode.Trim()) == false)
                {
                    dicStationFormula.Add(stationId.ToString() + goodsCategoryCode.Trim(), formula);
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

        private void SendEmptyGoodsMail(FMS_DeductBaseInfo deductInfo)
        {
            IMail mail = ServiceLocator.GetService<IMail>();

            IMerchantService service = ServiceLocator.GetService<IMerchantService>();

            StringBuilder builder = new StringBuilder();

            builder.Append("<br/>");
            builder.Append("运单:");
            builder.Append(deductInfo.WaybillNO);
            builder.Append("<br/>");
            builder.Append("商家为:" + service.GetMerchantName(deductInfo.WaybillNO));
            builder.Append("<br/>");
            builder.Append("货物品类为:" + deductInfo.WaybillCategory);

            mail.SendMailToUser("提成货物品类异常", builder.ToString(), "wangyongc@vancl.cn");
        }

        private void SendStationCategoryNotSetMail(int stationId, string categoryCode)
        {
            IMail mail = ServiceLocator.GetService<IMail>();

            IExpressCompanyService expressService = ServiceLocator.GetService<IExpressCompanyService>();
            IGoodsCategoryService categoryService = ServiceLocator.GetService<IGoodsCategoryService>();

            StringBuilder builder = new StringBuilder();

            builder.Append("<br/>");
            builder.Append("站点:" + expressService.GetModel(stationId).CompanyName);
            builder.Append("未设置货物品类:" + categoryService.GetNameByCode(categoryCode));

            mail.SendMailToUser("站点货物品类", builder.ToString(), "wangyongc@vancl.cn");
        }

        public decimal GetStationAreaDeduct(int? stationId, decimal? areaDeduct)
        {
            if (stationId == null || areaDeduct == null) return 0;

            if (areaDeduct != -1000 && areaDeduct != 0) return areaDeduct.Value;

            return _fmsDeductBaseInfoDao.GetStationDefaultAreaDeduct(stationId.Value);
        }


        public void AreaDeductErrorStatAgain()
        {
            string StartTime = ConfigurationManager.AppSettings["StartTime"];
            string StationID = ConfigurationManager.AppSettings["OpenStationV3"];

            string[] TimeList = StartTime.Split(':');
            if (TimeList.Length <= 0)
            {
                return;
            }
            IAreaDeductErrorInfoDao areaDeductDao = new AreaDeductErrorInfoDao();
            if (DateTime.Now.Hour.ToString() == TimeList[0] && DateTime.Now.Minute.ToString() == TimeList[1])
            {
                string[] sList = StationID.Split(',');
                if (sList.Length <= 0)
                {
                    return;
                }
                foreach (string stationid in sList)
                {


                    DataTable dtList =
                        areaDeductDao.GetErrorTypeAreaDeductListForService(Convert.ToInt32(stationid));
                    if (dtList.Rows.Count <= 0)
                    {
                        return;
                    }
                    foreach (DataRow dr in dtList.Rows)
                    {
                        bool bDeduct = false;
                        string ResultMessage = "";
                        decimal AreaCommission = 0;
                        int stationId = Convert.ToInt32(dr["STATIONID"]);
                        IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                        bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                                Convert.ToString(dr["ADDRESSINFO"]));


                        if (bDeduct == true)
                        {
                            FMS_DeductSubBaseInfo reductSubInfo = new FMS_DeductSubBaseInfo();
                            reductSubInfo.WaybillNO = Convert.ToInt64(dr["WAYBILLNO"]);
                            reductSubInfo.AreaCommission = AreaCommission;

                            if (Convert.ToString(dr["DEDUCTTYPE"]) == "取件提成")
                            {
                                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressReceive;
                            }
                            if (Convert.ToString(dr["DEDUCTTYPE"]) == "快递派件提成")
                            {
                                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressSend;
                            }
                            if (Convert.ToString(dr["DEDUCTTYPE"]) == "项目派件提成")
                            {
                                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ProjectSend;
                            }

                            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                            {

                                _fmsDeductBaseInfoDao.UpdateSubAreaDeduct(reductSubInfo);

                                areaDeductDao.UpdateIsDeleted(Convert.ToInt64(dr["AREADEDUCTERRORINFOID"]));

                                work.Complete();
                            }

                        }
                    }
                }
            }
        }

        public void StatAreaDeductErrorInfo()
        {
            IAreaDeductErrorInfoDao _areaDeductErrorInfoDao = new AreaDeductErrorInfoDao();
            _fmsDeductBaseInfoDao=new FMS_DeductBaseInfoDao();
            DataTable dtList =
                _areaDeductErrorInfoDao.GetErrorAreaDeductListForService();
            if (dtList.Rows.Count <= 0)
            {
                return;
            }
            foreach (DataRow dr in dtList.Rows)
            {
                bool bDeduct = false;
                string ResultMessage = "";
                decimal AreaCommission = 0;
                int stationId = Convert.ToInt32(dr["STATIONID"]);
                IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                        Convert.ToString(dr["ADDRESSINFO"]));


                if (bDeduct == true)
                {
                    FMS_DeductSubBaseInfo reductSubInfo = new FMS_DeductSubBaseInfo();
                    reductSubInfo.WaybillNO = Convert.ToInt64(dr["WAYBILLNO"]);
                    reductSubInfo.AreaCommission = AreaCommission;

                    if (Convert.ToString(dr["DEDUCTTYPE"]) == "取件提成")
                    {
                        reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressReceive;
                    }
                    if (Convert.ToString(dr["DEDUCTTYPE"]) == "快递派件提成")
                    {
                        reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressSend;
                    }
                    if (Convert.ToString(dr["DEDUCTTYPE"]) == "项目派件提成")
                    {
                        reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ProjectSend;
                    }

                    using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                    {

                        _fmsDeductBaseInfoDao.UpdateSubAreaDeduct(reductSubInfo);

                        _areaDeductErrorInfoDao.UpdateIsDeleted(Convert.ToInt64(dr["AREADEDUCTERRORINFOID"]));

                        work.Complete();
                    }

                }
                else
                {
                    using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                    {

                        _areaDeductErrorInfoDao.UpdateErrorType(Convert.ToInt64(dr["AREADEDUCTERRORINFOID"]));

                        work.Complete();
                    }
                }
            }

        }




        /// <summary>
        /// 处理项目派件
        /// </summary>
        public bool DealProjectSendByWaybillNO(long WaybillNo)
        {
            IList<FMS_DeductBaseInfo> list = _fmsDeductBaseInfoDao.GetProjectSendModelByWaybillNO(WaybillNo);

            if (list.Count == 0) return true;

            //查询所有派件站点
            string stationIds = "";
            string categoryCodes = "";

            GetProjectSendStationCategory(list, ref stationIds, ref categoryCodes);

            if (stationIds.Trim().Length == 0) return true;
            if (categoryCodes.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<string, string> dicStationFormula = ConvertToStationGoodsDictionary(_fmsDeductBaseInfoDao.GetProjectSendFormula(stationIds, categoryCodes));

            if (dicStationFormula.Count == 0) return true;

            //根据派件公式计算提成
            int stationId = -1;
            string categoryCode = "";
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

            IList<string> waybills = new List<string>();

            string key = "";

            foreach (var item in list)
            {
                stationId = item.DeliverStationID;
                categoryCode = item.WaybillCategory.Trim();

                key = stationId.ToString() + categoryCode.Trim();

                if (dicStationFormula.ContainsKey(key) == false)
                {
                    SendStationCategoryNotSetMail(stationId, categoryCode);

                    continue;
                }

                formula = dicStationFormula[key];

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

                #region 计算区域提成逻辑


                bool bDeduct = false;
                string ResultMessage = "";
                decimal AreaCommission = 0;

                string StationID = ConfigurationManager.AppSettings["OpenStationV3"];

                string[] sList = StationID.Split(',');

                if (sList.Contains(stationId.ToString()))
                {

                    IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                    bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                            item.ReceiveAddress);

                    if (bDeduct == false)
                    {
                        IAreaDeductErrorInfoDao areaDeductDao = new AreaDeductErrorInfoDao();
                        AreaDeductErrorInfo DeductErrorModel = new AreaDeductErrorInfo();
                        DeductErrorModel.WaybillNO = item.WaybillNO;
                        DeductErrorModel.StationId = stationId;
                        DeductErrorModel.ErrorInfo = ResultMessage;
                        DeductErrorModel.AddressInfo = item.ReceiveAddress;
                        DeductErrorModel.Errortype = "1";
                        DeductErrorModel.DisposeStatus = "未处理";
                        DeductErrorModel.DeductType = "项目派件提成";
                        DeductErrorModel.CreateTime = DateTime.Now;
                        DeductErrorModel.UpdateTime = DateTime.Now;

                        areaDeductDao.Add(DeductErrorModel);
                    }
                }
                else
                {
                    AreaCommission = GetStationAreaDeduct(item.ReceiveStationID, item.AreaReceiveDeduct);
                }

                #endregion

                reductSubInfo.WaybillNO = item.WaybillNO;
                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ProjectSend;
                reductSubInfo.Formula = formula;
                reductSubInfo.StationId = stationId;
                reductSubInfo.DeliverUser = item.DeliverManID;
                reductSubInfo.BasicCommission = basicCommission;
                reductSubInfo.AreaCommission = AreaCommission; //GetStationAreaDeduct(item.DeliverStationID, item.AreaSendDeduct);
                reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                reductSubInfo.CreateTime = (item.BackStationTime == null ? DateTime.Now : item.BackStationTime);
                reductSubInfo.UpdateTime = DateTime.Now;
                reductSubInfo.IsDeleted = 0;
                reductSubInfo.AdjustCommission = item.AreaSendDeduct ?? 0;
                item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                //if (_fmsDeductBaseInfoDao.IsDeleted(item.DeductID)) continue;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    _fmsDeductBaseInfoDao.DeleteOldDeduct(item.WaybillNO, (int)BizEnums.enumDeductType.ProjectSend);
                    if (_fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (_fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;

                    work.Complete();
                }
            }

            return true;
        }



        /// <summary>
        /// 处理快递派件
        /// </summary>
        public bool DealExpressSendByWaybillNO(long WaybillNo)
        {
            IList<FMS_DeductBaseInfo> list = _fmsDeductBaseInfoDao.GetExpressSendModelByWaybillNO(WaybillNo);

            if (list.Count == 0) return true;

            //查询所有派件站点
            string stationIds = GetSendStationIds(list);

            if (stationIds.Trim().Length == 0) return true;

            //根据查询出站点查询出所有公式
            IDictionary<int, string> dicStationFormula = ConvertToDictionary(_fmsDeductBaseInfoDao.GetExpressSendFormula(stationIds));

            if (dicStationFormula.Count == 0) return true;

            //根据派件公式计算提成
            int stationId = -1;
            string formula = "";

            FMS_DeductSubBaseInfo reductSubInfo = null;

            decimal basicCommission = 0;
            decimal basicWeight = 0;
            decimal weightAdd = 0;

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

                #region 计算区域提成逻辑


                bool bDeduct = false;
                string ResultMessage = "";
                decimal AreaCommission = 0;

                string StationID = ConfigurationManager.AppSettings["OpenStationV3"];

                string[] sList = StationID.Split(',');

                if (sList.Contains(stationId.ToString()))
                {

                    IPermissionOpenService PMSOpenService = new PermissionOpenServiceClient();
                    bDeduct = PMSOpenService.GetDeductValue(out ResultMessage, out AreaCommission, stationId,
                                                            item.ReceiveAddress);

                    if (bDeduct == false)
                    {
                        IAreaDeductErrorInfoDao areaDeductDao = new AreaDeductErrorInfoDao();
                        AreaDeductErrorInfo DeductErrorModel = new AreaDeductErrorInfo();
                        DeductErrorModel.WaybillNO = item.WaybillNO;
                        DeductErrorModel.StationId = stationId;
                        DeductErrorModel.ErrorInfo = ResultMessage;
                        DeductErrorModel.AddressInfo = item.ReceiveAddress;
                        DeductErrorModel.Errortype = "1";
                        DeductErrorModel.DisposeStatus = "未处理";
                        DeductErrorModel.DeductType = "快递派件提成";
                        DeductErrorModel.CreateTime = DateTime.Now;
                        DeductErrorModel.UpdateTime = DateTime.Now;

                        int iRow = areaDeductDao.Add(DeductErrorModel);

                        if (iRow == 0)
                        {
                            StringBuilder builder = new StringBuilder();

                            builder.Append("<br/>");
                            builder.Append("单号:" + item.WaybillNO.ToString());
                            builder.Append("提成计算异常信息插入失败！");

                            mail.SendMailToUser("提成计算异常信息插入失败", builder.ToString(), "wangyongc@vancl.cn");
                        }
                    }

                }
                else
                {
                    AreaCommission = GetStationAreaDeduct(item.ReceiveStationID, item.AreaSendDeduct);
                }


                #endregion

                reductSubInfo.WaybillNO = item.WaybillNO;
                reductSubInfo.DeductType = (int)BizEnums.enumDeductType.ExpressSend;
                reductSubInfo.Formula = formula;
                reductSubInfo.StationId = stationId;
                reductSubInfo.DeliverUser = item.DeliverManID;
                reductSubInfo.BasicCommission = basicCommission;
                reductSubInfo.AreaCommission = AreaCommission; //GetStationAreaDeduct(item.DeliverStationID, item.AreaSendDeduct);
                reductSubInfo.SumCommission = reductSubInfo.BasicCommission + reductSubInfo.AreaCommission + reductSubInfo.WeightCommission;
                reductSubInfo.CreateTime = (item.BackStationTime == null ? DateTime.Now : item.BackStationTime);
                reductSubInfo.UpdateTime = DateTime.Now;
                reductSubInfo.IsDeleted = 0;
                reductSubInfo.AdjustCommission = item.AreaSendDeduct ?? 0;
                item.IsDeductAcount = (int)RFD.FMS.MODEL.BizEnums.DeductResult.Success;

                if (_fmsDeductBaseInfoDao.IsDeleted(item.DeductID)) continue;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    if (_fmsDeductBaseInfoDao.Add(reductSubInfo) <= 0) return false;

                    if (_fmsDeductBaseInfoDao.IsCalculateOK(item.DeductID) == false) return false;

                    work.Complete();
                }
            }

            return true;
        }
    }
}
