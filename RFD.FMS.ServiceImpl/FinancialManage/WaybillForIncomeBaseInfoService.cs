using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.Service;
using System.Configuration;
using System.IO;
using Microsoft.JScript.Vsa;
using RFD.FMS.Service.Mail;
using RFD.FMS.Service.FinancialManage;
using System.Text.RegularExpressions;
using RFD.FMS.Service.BasicSetting;
using System.Threading;
using RFD.FMS.Domain.BasicSetting;using Oracle.DataAccess.Client;


namespace RFD.FMS.ServiceImpl.FinancialManage
{
    public class WaybillForIncomeBaseInfoService : IWaybillForIncomeBaseInfoService, IWaybillStatusObserver
    {
        private IWaybillForIncomeBaseInfoDao _waybillForIncomeBaseInfoDao;

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
                    return "zhangrongrong@vancl.cn";
                }
            }
        }

        #region 接货明细查询
        public DataTable SearchDetailsV2(ThirdPartyWaybillSearchConditons condition, ref PageInfo pi, out DataTable amount)
        {
            string searchCondition =string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);

            amount = _waybillForIncomeBaseInfoDao.SearchStatV2<OracleParameter>(searchCondition, parameterList);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0) return null;

            pi.ItemCount = int.Parse(amount.Rows[0]["订单量合计"].ToString());

            return _waybillForIncomeBaseInfoDao.SearchDetailsV2<OracleParameter>(searchCondition,parameterList, ref pi, true);
        }

        public DataTable SearchDetails(ThirdPartyWaybillSearchConditons condition,ref PageInfo pi,out DataTable amount)
        {
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);

            amount = _waybillForIncomeBaseInfoDao.SearchStat<OracleParameter>(searchCondition, parameterList);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0) return null;

            pi.ItemCount = int.Parse(amount.Rows[0]["订单量合计"].ToString());
            pi.PageCount = Convert.ToInt32(Math.Ceiling(pi.ItemCount * 1.0 / pi.PageSize));

            return _waybillForIncomeBaseInfoDao.SearchDetails<OracleParameter>(searchCondition,parameterList, ref pi, true);
        }

        private List<OracleParameter> BuildSearchCondition(ThirdPartyWaybillSearchConditons condition,out string sqlWhere)
        {
            List<OracleParameter> parameterList = new List<OracleParameter>();
            StringBuilder sbCondition = new StringBuilder();
            sbCondition.Append(" AND fibi.DistributionCode = :DistributionCode ");
            parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value=condition.DistributionCode});

            if (condition.DistributionCode == "rfd" && string.IsNullOrEmpty(condition.MerchantID))
            {
                sbCondition.Append(" AND fibi.MerchantID not in (8,9) ");
                //sbCondition.Append(Util.Common.GetOracleInParameterWhereSql("fibi.MerchantID", "MerchantIDNot", true, false));
                //parameterList.Add(new OracleParameter(":MerchantIDNot", OracleDbType.Varchar2, 100) { Value = "8,9" });
            }

            sbCondition.Append(" AND fifi.IsDeleted = :IsDeleted ");
            parameterList.Add(new OracleParameter(":IsDeleted", OracleDbType.Decimal) { Value = 0 });

            if (!string.IsNullOrEmpty(condition.WaybillNO.ToString()) && condition.WaybillNO > 0)
            {
                sbCondition.Append(" AND fibi.WaybillNo = :WaybillNo");
                parameterList.Add(new OracleParameter(":WaybillNo", OracleDbType.Decimal) { Value = condition.WaybillNO });
            }
            else if (!string.IsNullOrEmpty(condition.Customerorder))
            {
                sbCondition.Append(" AND fibi.CustomerOrder = :CustomerOrder");
                parameterList.Add(new OracleParameter(":CustomerOrder", OracleDbType.Varchar2,100) { Value = condition.Customerorder });
            }
            else
            {
                if (!string.IsNullOrEmpty(condition.MerchantID))
                {
                    sbCondition.Append(string.Format(" AND fibi.MerchantID IN ({0})", condition.MerchantID.Replace(" ", "")));
                 }

                if (!string.IsNullOrEmpty(condition.OutCreatTimeBegin.ToString()))
                {
                    sbCondition.Append(" AND fibi.RfdAcceptTime >= :RfdAcceptTimeStr ");
                    parameterList.Add(new OracleParameter(":RfdAcceptTimeStr", OracleDbType.Date) { Value = condition.OutCreatTimeBegin.Value});
                }

                if (!string.IsNullOrEmpty(condition.OutCreatTimeEnd.ToString()))
                {
                    sbCondition.Append(" AND fibi.RfdAcceptTime < :RfdAcceptTimeEnd ");
                    parameterList.Add(new OracleParameter(":RfdAcceptTimeEnd", OracleDbType.Date) { Value = condition.OutCreatTimeEnd.Value });
                }

                if (!string.IsNullOrEmpty(condition.InCreatTimeBegin.ToString()))
                {
                    sbCondition.Append(" AND fibi.ReturnTime >= :ReturnTimeStr ");
                    parameterList.Add(new OracleParameter(":ReturnTimeStr", OracleDbType.Date) { Value = condition.InCreatTimeBegin.Value });
                }

                if (!string.IsNullOrEmpty(condition.InCreatTimeEnd.ToString()))
                {
                    sbCondition.Append(" AND fibi.ReturnTime < :ReturnTimeEnd ");
                    parameterList.Add(new OracleParameter(":ReturnTimeEnd", OracleDbType.Date) { Value = condition.InCreatTimeEnd.Value });
                }

                if (!string.IsNullOrEmpty(condition.BackStationTimeBegin.ToString()))
                {
                    sbCondition.Append(" AND fibi.BackStationTime >= :BackStationTimeStr ");
                    parameterList.Add(new OracleParameter(":BackStationTimeStr", OracleDbType.Date) { Value = condition.BackStationTimeBegin.Value });
                }

                if (!string.IsNullOrEmpty(condition.BackStationTimeEnd.ToString()))
                {
                    sbCondition.Append(" AND fibi.BackStationTime < :BackStationTimeEnd ");
                    parameterList.Add(new OracleParameter(":BackStationTimeEnd", OracleDbType.Date) { Value = condition.BackStationTimeEnd.Value });
                }

                if (!string.IsNullOrEmpty(condition.SortingCenter))
                {
                    sbCondition.Append(string.Format(" AND fibi.ExpressCompanyID IN ({0})", condition.SortingCenter.Replace(" ", "")));
                }

                //使用结算主体查询
                if (!string.IsNullOrEmpty(condition.DeliverStationID))
                {
                    sbCondition.Append(string.Format(" AND fibi.TopCODCompanyID IN ({0})", condition.DeliverStationID.Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(condition.WaybillStatus))
                {
                    sbCondition.Append(" AND fibi.BackStationStatus = :BackStationStatus ");
                    parameterList.Add(new OracleParameter(":BackStationStatus", OracleDbType.Varchar2,20) { Value = condition.WaybillStatus });
                }

                if (!string.IsNullOrEmpty(condition.BackStatus))
                {
                    sbCondition.Append(" AND fibi.SubStatus = :SubStatus ");
                    parameterList.Add(new OracleParameter(":SubStatus", OracleDbType.Decimal) { Value = condition.BackStatus });
                }

                if (!string.IsNullOrEmpty(condition.WaybillType))
                {
                    sbCondition.Append(string.Format(" AND fibi.WaybillType = :WaybillType ", condition.WaybillType));
                    parameterList.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 20) { Value = condition.WaybillType });
                }

                if (!string.IsNullOrEmpty(condition.InefficacyStatus.ToString()))
                {
                    sbCondition.Append(" AND fibi.InefficacyStatus = :InefficacyStatus");
                    parameterList.Add(new OracleParameter(":InefficacyStatus", OracleDbType.Decimal) { Value = condition.InefficacyStatus });
                }

                if (!string.IsNullOrEmpty(condition.PaymentType))
                {
                    sbCondition.Append(" AND fibi.AcceptType = :AcceptType ");
                    parameterList.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2,40) { Value = condition.PaymentType });
                }

                if (!string.IsNullOrEmpty(condition.AreaExpressLevel.ToString()))
                {
                    if (condition.AreaExpressLevel == 99)
                    {
                        sbCondition.Append(" AND fifi.AreaType IS NULL");
                    }
                    else if (condition.AreaExpressLevel > 0)
                    {
                        sbCondition.Append(" AND fifi.AreaType=:AreaType ");
                        parameterList.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = condition.AreaExpressLevel });
                    }
                }
            }

            sqlWhere = sbCondition.ToString();
            return parameterList;
        }

        public DataTable SearchDetailsForExportV2(ThirdPartyWaybillSearchConditons condition)
        {
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);

            PageInfo pi = new PageInfo(0);

            return _waybillForIncomeBaseInfoDao.SearchDetailsV2<OracleParameter>(searchCondition, parameterList,ref pi, false);
        }

        public DataTable SearchDetailsForExport(ThirdPartyWaybillSearchConditons condition)
        {
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);
            PageInfo pi = new PageInfo(0);
            return _waybillForIncomeBaseInfoDao.SearchDetails<OracleParameter>(searchCondition,parameterList, ref pi, false);
        }

        public DataTable SearchSummary(ThirdPartyWaybillSearchConditons condition,out DataTable amount)
        {
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);

            amount = _waybillForIncomeBaseInfoDao.SearchStat<OracleParameter>(searchCondition, parameterList);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0)
                return null;

            return _waybillForIncomeBaseInfoDao.SearchSummary<OracleParameter>(searchCondition,parameterList, condition);
        }

        //汇总导出
        public DataTable SearchSummaryForExport(ThirdPartyWaybillSearchConditons condition)
        {
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);
            return _waybillForIncomeBaseInfoDao.SearchSummary<OracleParameter>(searchCondition,parameterList, condition );
        }
        #endregion

        #region 推数逻辑
        /// <summary>
        /// 实现主方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DoAction(WaybillStatusChangeLog model)
        {
            try
            {
                if (model.Status != "-5")
                {
                    if (InsertIntoInComeBaseInfo(model) == 1)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
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

        public string Key
        {
            get { return "WaybillForIncomeBaseInfoService"; }
        }

        public bool IsFalseToRePush
        {
            get { return true; }
        }

        /// <summary>
        /// 将对象插入数据表 add by wangyongc 2012-04-13 （Use财务静态表）
        /// </summary>
        /// <param name="Logmodel"></param>
        /// <returns></returns>
        public int InsertIntoInComeBaseInfo(WaybillStatusChangeLog Logmodel)
        {
            IWaybillForIncomeBaseInfoDao waybillDao = ServiceLocator.GetService<IWaybillForIncomeBaseInfoDao>();
            IFMSInterfaceService fmsDao = ServiceLocator.GetService<IFMSInterfaceService>();

            //取值
            string TipList = ""; //提示

            try
            {
                //如果判断已经存在数据
                if (!fmsDao.ExistsIncomeBaseInfo(Logmodel.WaybillNo))
                {
                    DataTable dt = waybillDao.GetWaybillInfoByNOForIncomeBaseInfo(Logmodel.WaybillNo);

                    if (dt == null || dt.Rows.Count != 1) return 0;

                    FMS_IncomeBaseInfo BaseModel = GetBaseInfoModel(dt.Rows[0]);
                    FMS_IncomeFeeInfo FeeModel = GetFeeInfoModel(dt.Rows[0]);

                    //获取区域类型 新增是寻找一次
                    int areaType = GetAreaType(DataConvert.ToInt(BaseModel.MerchantID), BaseModel.ExpressCompanyID.ToString(), BaseModel.DistributionCode, BaseModel.AreaID, BaseModel.IsCategory, BaseModel.WaybillCategory,BaseModel.DeliverStationID);
                    if (areaType > 0)
                    {
                        FeeModel.AreaType = areaType;
                    }

                    //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                    using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                    {
                        fmsDao.AddIncomeBaseInfo(BaseModel);
                        fmsDao.AddIncomeFeeInfo(FeeModel);

                        //提交事务
                        work.Complete();
                        //成功一笔记录一笔
                        TipList += "成功添加订单：" + Logmodel.WaybillNo + "   ";
                        //记录成功日志ID
                        //    }
                    }

                    return 1;
                }
                else
                {
                    #region 更新
                    //减少读取次数
                    DataTable dt = new DataTable();
                    if (Logmodel.Status == "-4" ||
                        Logmodel.Status == "-3" ||
                        Logmodel.Status == "-1" ||
                        Logmodel.Status == "3" ||
                        Logmodel.Status == "5" ||
                        Logmodel.Status == "10" ||
                        Logmodel.Status == "0" ||
                        Logmodel.Status == "-9")
                    {
                        dt = waybillDao.GetWaybillInfoByNOForIncomeBaseInfo(Logmodel.WaybillNo);
                    }
                    else
                    {
                        return 1;
                    }

                    if (dt == null || dt.Rows.Count != 1)
                        return 0;

                    FMS_IncomeBaseInfo BaseModel = GetBaseInfoModel(dt.Rows[0]);
                    FMS_IncomeFeeInfo FeeModel = GetFeeInfoModel(dt.Rows[0]);

                    //执行一次提交一个事务（防止一次提交太多造成死锁 ）
                    using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                    {
                        //存在重新称重，需要更新下信息
                        if (BaseModel.Status == "-3" || BaseModel.Status == "-4" || BaseModel.Status == "-1" || BaseModel.Status == "0" || BaseModel.Status == "10")
                        {
                            fmsDao.UpdateIncomeBaseInfo(BaseModel);
                        }

                        if (BaseModel.Status == "3" || BaseModel.Status == "5" || BaseModel.Status == "-9")
                        {
                            if (BaseModel.Status == "-9")
                            {
                                BaseModel.BackStationStatus = "-9";
                                //BaseModel.BackStationTime = DateTime.Now;
                            }

                            fmsDao.UpdateIncomeBaseInfoStatus(BaseModel);

                            //只要重新有状态，就将计算状态重置
                            fmsDao.UpdateInComeFeeInfoByBackStation(FeeModel);
                        }
                        if (BaseModel.SubStatus == 6 || BaseModel.SubStatus == 7 || BaseModel.SubStatus == 13)
                        {
                            fmsDao.UpdateBackStatus(BaseModel);
                            fmsDao.UpdateInComeFeeInfoByBackStation(FeeModel);
                        }
                        //提交事务
                        work.Complete();
                        //记录成功日志ID
                        //    }
                    }
                    return 1;
                    #endregion
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
        /// 查询AreaType
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="distributionCode"></param>
        /// <param name="areaId"></param>
        /// <param name="isCategory"></param>
        /// <param name="waybillCategory"></param>
        /// <returns></returns>
        private int GetAreaType(int merchantId, string wareHouseId, string distributionCode, string areaId, int isCategory, string waybillCategory )
        {
            AreaLevelIncomeSearchModel incomeSearchModel = new AreaLevelIncomeSearchModel();
            incomeSearchModel.MerchantID = merchantId;
            incomeSearchModel.WareHouse = wareHouseId;
            incomeSearchModel.DistributionCode = distributionCode;
            incomeSearchModel.AreaID = areaId;
            if (isCategory == 1)
            {
                incomeSearchModel.GoodsCategoryCode = waybillCategory;
            }
            IAreaExpressLevelIncomeDao areaExpressLevelIncomeDao = ServiceLocator.GetService<IAreaExpressLevelIncomeDao>();
            return areaExpressLevelIncomeDao.GetAreaTypeByCondition(incomeSearchModel);
        }
         /// <summary>
        /// 查询AreaType
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="distributionCode"></param>
        /// <param name="areaId"></param>
        /// <param name="isCategory"></param>
        /// <param name="waybillCategory"></param>
        /// <param name="expressId"></param>
        /// <returns></returns>
        private int GetAreaType(int merchantId, string wareHouseId, string distributionCode, string areaId, int isCategory, string waybillCategory,int? expressId)
        {
            AreaLevelIncomeSearchModel incomeSearchModel = new AreaLevelIncomeSearchModel();
            incomeSearchModel.MerchantID = merchantId;
            incomeSearchModel.WareHouse = wareHouseId;
            incomeSearchModel.DistributionCode = distributionCode;
            incomeSearchModel.AreaID = areaId;
            if (isCategory == 1)
            {
                incomeSearchModel.GoodsCategoryCode = waybillCategory;
            }
            incomeSearchModel.AuditStatus = 2;
            IAreaExpressLevelIncomeDao areaExpressLevelIncomeDao = ServiceLocator.GetService<IAreaExpressLevelIncomeDao>();
            DataTable dt = areaExpressLevelIncomeDao.GetAreaLevelIncomeList(incomeSearchModel);
            if (dt!=null&&dt.Rows.Count>0)
            {
                if (expressId!=null&&expressId>0)
                {
                    DataRow[] dataRows = dt.Select("EXPRESSCOMPANYID='" + expressId + "'");
                    if (dataRows.Length>0)
                    {
                        return int.Parse(dataRows[0]["AREATYPE"].ToString());
                    }
                    else
                    {
                        dataRows = dt.Select("EXPRESSCOMPANYID='" + 11 + "'");
                        if (dataRows.Length > 0)
                        {
                            return int.Parse(dataRows[0]["AREATYPE"].ToString());
                        } 
                    }
                }
                else
                {
                    DataRow[] dataRows = dt.Select("EXPRESSCOMPANYID='" + 11 + "'");
                    if (dataRows.Length>0)
                    {
                        return int.Parse(dataRows[0]["AREATYPE"].ToString());
                    } 
                }
                return int.Parse(dt.Rows[0]["AREATYPE"].ToString());
            }
            //incomeSearchModel.ExpressCompanyID = expressId;

            return 0;
        }

        #endregion

        #region 配送费、保价费、代收货款手续费计算

        public Model.FinancialManage.FMS_IncomeBaseInfo GetBaseInfoModel(DataRow row)
        {
            IFMSInterfaceService fmsDao = ServiceLocator.GetService<IFMSInterfaceService>();
            var model = new FMS_IncomeBaseInfo();

            if (row["WaybillNo"].ToString() != "")
            {
                model.WaybillNo = Int64.Parse(row["WaybillNo"].ToString());
            }
            model.WaybillType = row["WaybillType"].ToString();
            if (row["Status"].ToString() != "")
            {
                model.Status = row["Status"].ToString();
            }
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
            model.BackStationStatus = row["BackStationStatus"].ToString();
            if (row["BackStationTime"].ToString() != "")
            {
                model.BackStationTime = System.DateTime.Parse(row["BackStationTime"].ToString());
            }
            if (model.BackStationStatus != "3" && model.BackStationStatus != "5")
            {
                model.BackStationStatus = "";
                model.BackStationTime = null;
            }
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
            int WeightType = fmsDao.GetMerchantWeightType(Int32.Parse(row["MerchantID"].ToString()), row["DistributionCode"].ToString());
            switch (WeightType)
            {
                case -1:
                    model.AccountWeight = 0; break;
                case 0:
                    model.AccountWeight = DataConvert.ToDecimal(row["MerchantWeight"].ToString(), 0); break;
                case 1:
                    model.AccountWeight = DataConvert.ToDecimal(row["WayBillInfoWeight"].ToString(), 0); break;
                case 2:
                    model.AccountWeight = DataConvert.ToDecimal(row["MerchantWeight"].ToString(), 0) > DataConvert.ToDecimal(row["WayBillInfoWeight"].ToString(), 0) ?
                        DataConvert.ToDecimal(row["MerchantWeight"].ToString(), 0) : DataConvert.ToDecimal(row["WayBillInfoWeight"].ToString(), 0);
                    break;
                case 3:
                    model.AccountWeight = DataConvert.ToDecimal(row["MerchantWeight"].ToString(), 0); break;
                case 4:
                    model.AccountWeight = 0; break;
                default:
                    model.AccountWeight = 0; break;
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
            if (row["WaybillCategory"].ToString() != "")
            {
                model.WaybillCategory = row["WaybillCategory"].ToString();
            }
            if (row["DeliverCode"].ToString() != "")
            {
                model.DeliverCode = row["DeliverCode"].ToString();
            }
            model.IsCategory = fmsDao.GetMerchantIsCategory(Int32.Parse(row["MerchantID"].ToString()), row["DistributionCode"].ToString());
            model.createtime = DateTime.Now;

            model.updatetime = DateTime.Now;


            return model;
        }

        public Model.FinancialManage.FMS_IncomeFeeInfo GetFeeInfoModel(DataRow row)
        {
            var model = new FMS_IncomeFeeInfo();

            model.WaybillNO = Int64.Parse(row["WaybillNO"].ToString());
            if ((row["MerchantID"].ToString() == "8" ||
                row["MerchantID"].ToString() == "9" ||
                row["MerchantID"].ToString() == "30" ||
                !string.IsNullOrEmpty(row["PeriodAccountCode"].ToString()))
                && row["DistributionCode"].ToString() == "rfd")
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

        public void UpdateIncomeFeeInfoDao(int Num)
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();
            DataTable dt = FeeDao.GetInComeFeeInfo(Num);
            AccountFee(dt);
        }

        private void AccountFee(DataTable dt)
        {
            //取值
            string TipList = ""; //提示
            string WaybillNOSucList = "";//成功订单
            string WaybillNoFasList = "";//失败订单
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();

            if (dt == null && dt.Rows.Count == 0)
            {
                WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "没有取到需要计算费用的数据！");
            }

            WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "取到需要计算费用的数据" + dt.Rows.Count.ToString() + "！");

            StringBuilder sbErrorMsg = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                string errorMsg = string.Empty;
                FMS_IncomeFeeInfo Model = GetUpdateFeeInfoModel(row, out errorMsg);
                sbErrorMsg.Append(errorMsg);
                if (Model == null)
                    continue;
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

                var mailService = ServiceLocator.GetService<IMail>();

                mailService.SendMailToUser("商家结算异常明细", TipList, FailedMailAdress);
            }

            if (!string.IsNullOrEmpty(sbErrorMsg.ToString().Trim()))
            {
                var mailService = ServiceLocator.GetService<IMail>();

                string errorMsg = "<html><head></head><body>";
                errorMsg += sbErrorMsg.ToString();
                errorMsg += "</head></html>";

                mailService.SendMailToUser("商家结算异常明细", errorMsg, FailedMailAdress);
            }
        }

        public void AcountDeductFeeInfoDao(int Num)
        {
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
                if (Model == null)
                    continue;//直接跳出
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

        public void UpdateIncomeFeeInfoDao(List<string> WaybollNOList)
        {
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
                    WriteTest(DateTime.Now.ToString("yyyy年MM月dd  HH时mm分ss秒") + "取到需要计算费用的数据" + dt.Rows.Count.ToString() + "！");

                    StringBuilder sbErrorMsg = new StringBuilder();

                    foreach (DataRow row in dt.Rows)
                    {
                        string errorMsg = string.Empty;
                        FMS_IncomeFeeInfo Model = GetUpdateFeeInfoModel(row, out errorMsg);
                        sbErrorMsg.Append(errorMsg);
                        if (Model == null)
                            continue;//直接跳出
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
                        var mailService = ServiceLocator.GetService<IMail>();
                        mailService.SendMailToUser("商家结算异常明细", TipList, "zengwei@vancl.cn");
                    }

                    if (!string.IsNullOrEmpty(sbErrorMsg.ToString().Trim()))
                    {
                        var mailService = ServiceLocator.GetService<IMail>();
                        mailService.SendMailToUser("商家结算异常明细", sbErrorMsg.ToString(), "zengwei@vancl.cn");
                    }
                }
                catch (Exception e)
                {
                    WriteTest("异常：" + e.Message);
                    var mailService = ServiceLocator.GetService<IMail>();
                    mailService.SendMailToUser("商家结算异常明细", e.Message, "zengwei@vancl.cn");
                }
            }
        }

        ///// <summary>
        ///// 得到正向计算公式
        ///// </summary>
        ///// <param name="merchantId"></param>
        ///// <param name="warehouseId"></param>
        ///// <param name="areaType"></param>
        ///// <param name="isCategory"></param>
        ///// <param name="waybillCategory"></param>
        ///// <param name="distributionCode"></param>
        ///// <returns></returns>
        //private KeyValuePair<string, int> GetIncomeBasicDeliverFee(int merchantId, string warehouseId, int areaType, int isCategory, string waybillCategory, string distributionCode, decimal needPayAmount, string deliverStationId)
        //{
        //    //全部类型只要有应收金额，按区分COD来找公式
        //    var result = new KeyValuePair<string, int>();
        //    var merchantDeliverFeeDao = ServiceLocator.GetService<IMerchantDeliverFeeDao>();
        //    //先判断是否走配送商逻辑
        //    DataTable dtFee = merchantDeliverFeeDao.GetBasicDeliverFeeByCondition(merchantId, warehouseId, areaType, isCategory, waybillCategory, distributionCode);
        //    //走配送商逻辑,则返回配送商设置的公式，按照配送商的公式计算运费
        //    string weightormula;
        //    string weightdeliverfee;
        //    int isCod;
        //    if (dtFee == null || dtFee.Rows.Count == 0)
        //    {
        //        return new KeyValuePair<string, int>("", -1);
        //    }
        //    else
        //    {
        //        var datarows = dtFee.Select(string.Format("StationID='{0}'and ISEXPRESS='1'", deliverStationId));
        //        if (datarows.Length > 0)
        //        {
        //            weightormula = datarows[0]["BasicDeliverFee"].ToString();
        //            isCod = int.Parse(datarows[0]["IsCod"].ToString());
        //            weightdeliverfee = datarows[0]["DeliverFee"].ToString();
        //        }
        //        else
        //        {
        //            weightormula = dtFee.Rows[0]["BasicDeliverFee"].ToString();
        //            isCod = int.Parse(dtFee.Rows[0]["IsCod"].ToString());
        //            weightdeliverfee = dtFee.Rows[0]["DeliverFee"].ToString();
        //        }
        //    }
        //    result = needPayAmount > 0 ? new KeyValuePair<string, int>(weightormula, isCod) : new KeyValuePair<string, int>(weightdeliverfee,  isCod);
        //    return result;
        //}

        /// <summary>
        /// 验证区域类型、计算公式、重量
        /// </summary>
        /// <param name="row"></param>
        /// <param name="model"></param>
        private string JudgeGetIncomeFeeMsg(DataRow row, ref FMS_IncomeFeeInfo model)
        {
            int isCategory = DataConvert.ToInt(row["IsCategory"].ToString(), 0);
            var sbErrorMsg = new StringBuilder();
            //if (model.AreaType == null || model.AreaType <= 0)
            {
                //重新找区域类型
                model.AreaType = GetAreaType(DataConvert.ToInt(row["MerchantID"]), row["ExpressCompanyID"].ToString(), row["DistributionCode"].ToString(), row["AreaID"].ToString(), isCategory, row["WaybillCategory"].ToString(), int.Parse(row["DeliverStationID"].ToString()));
            }

            if (model.AreaType <= 0)
            {
                sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T3) + "、");
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T3;
                return sbErrorMsg.ToString();
            }
            else
            {
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T0;
            }
            //找计算公式
            decimal needPayAmount = 0M;
            decimal.TryParse(row["NeedPayAmount"].ToString(), out needPayAmount);//应收金额
            //**********************增加是否走配送商逻辑 zhangrongrong *********************************
            bool flag;
            string weightormula;
            string weightdeliverfee;
            int isCod;
            string s;
            //增加是否走配送商逻辑
            if (GetIncomeFeeMsg(row, model, sbErrorMsg, isCategory, out flag, out s, out weightormula, out weightdeliverfee, out isCod)) return s;
            model.IsExpress = flag ? 1 : 0;
            KeyValuePair<string, int> feeResult = needPayAmount > 0 ? new KeyValuePair<string, int>(weightormula, isCod) : new KeyValuePair<string, int>(weightdeliverfee, isCod);
            //*******************************************************************************************
            //计算费
            if (string.IsNullOrEmpty(feeResult.Key))
            {
                sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T4) + "、");
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T4;
                return sbErrorMsg.ToString();
            }
            else
            {
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T0;
            }
            model.AccountStandard = feeResult.Key;
          
            model.ISCod = feeResult.Value;
            if (!string.IsNullOrEmpty(model.AccountStandard) && model.AccountStandard.Contains("重量"))
            {
                model.AccountWeight = DataConvert.ToDecimal(row["AccountWeight"].ToString(), 0);
                if (model.AccountWeight == 0M)
                {
                    sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T6) + "、");
                    model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T6;
                    model.AccountStandard = "";
                    model.AccountFare = 0;
                    return sbErrorMsg.ToString();
                }
                else
                {
                    model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T0;
                }
                //得到最终重量
                model.AccountWeight = Util.Common.GetWeightByRule(DataConvert.ToInt(row["WeightValueRule"].ToString(), 9), DataConvert.ToInt(row["WeightType"].ToString(), 0), Decimal.Parse(model.AccountWeight.ToString()));
            }
            return sbErrorMsg.ToString();
        }

        private static bool GetIncomeFeeMsg(DataRow row, FMS_IncomeFeeInfo model, StringBuilder sbErrorMsg, int isCategory,
                                            out bool flag, out string s, out string weightormula, out string weightdeliverfee,
                                            out int isCod)
        {
            var merchantDeliverFeeDao = ServiceLocator.GetService<IMerchantDeliverFeeDao>();
            //先判断是否走配送商逻辑
            DataTable dtFee = merchantDeliverFeeDao.GetBasicDeliverFeeByCondition(DataConvert.ToInt(row["MerchantID"]),
                                                                                  row["ExpressCompanyID"].ToString(),
                                                                                  DataConvert.ToInt(model.AreaType), isCategory,
                                                                                  row["WaybillCategory"].ToString(),
                                                                                  row["DistributionCode"].ToString());

            if (dtFee == null || dtFee.Rows.Count == 0)
            {
                //没有配送商收入配送费设置信息
                sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T4) + "、");
                model.IsAccount = (int) BizEnums.IncomeFeeAccountType.T4;
                {
                    {
                        s = sbErrorMsg.ToString();
                        flag = false;
                        weightormula = null;
                        weightdeliverfee = null;
                        isCod = 0;
                        return true;
                    }
                }
            }
            else
            {
                var datarows =
                    dtFee.Select(string.Format("StationID='{0}'and ISEXPRESS='1'", row["DeliverStationID"].ToString()));
                if (datarows.Length > 0)
                {
                    weightormula = datarows[0]["BasicDeliverFee"].ToString();
                    isCod = int.Parse(datarows[0]["IsCod"].ToString());
                    weightdeliverfee = datarows[0]["DeliverFee"].ToString();
                    flag = true;
                }
                else
                {
                    var datarowsV =
                       dtFee.Select(string.Format("StationID='{0}'AND ExpressCompanyID='{1}'", "11", row["ExpressCompanyID"].ToString()));
                    if (datarowsV.Length > 0)
                    {
                        weightormula = datarowsV[0]["BasicDeliverFee"].ToString();
                        isCod = int.Parse(datarowsV[0]["IsCod"].ToString());
                        weightdeliverfee = datarowsV[0]["DeliverFee"].ToString();
                    }
                    else
                    {
                         flag = false;
                        //没有配送商收入配送费设置信息
                        s = "没有配送商收入配送费设置信息";
                         sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T4) + "、");
                         model.IsAccount = (int) BizEnums.IncomeFeeAccountType.T4;
                        weightormula = null;
                        weightdeliverfee = null;
                        isCod = 0;
                        return false;
                    }
                    //weightormula = dtFee.Rows[0]["BasicDeliverFee"].ToString();
                    //isCod = int.Parse(dtFee.Rows[0]["IsCod"].ToString());
                    //weightdeliverfee = dtFee.Rows[0]["DeliverFee"].ToString();
                }
            }
            flag = false;
            s = "没有配送商收入配送费设置信息";
            return false;
        }

        ///// <summary>
        ///// 判断是否走配送商逻辑
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="model"></param>
        ///// <param name="isCategory"></param>
        ///// <param name="sbErrorMsg"></param>
        ///// <param name="judgeGetIncomeFeeMsg"></param>
        ///// <param name="dtFee"></param>
        ///// <param name="flag"></param>
        ///// <returns></returns>
        //private static bool JudgeIsExpressMsg(DataRow row, FMS_IncomeFeeInfo model, int isCategory, StringBuilder sbErrorMsg,
        //                                      out string judgeGetIncomeFeeMsg, out DataTable dtFee, out bool flag)
        //{
        //    var merchantDeliverFeeDao = ServiceLocator.GetService<IMerchantDeliverFeeDao>();
        //    //先判断是否走配送商逻辑
        //    dtFee = merchantDeliverFeeDao.GetBasicDeliverFeeByCondition(DataConvert.ToInt(row["MerchantID"]),
        //                                                                row["ExpressCompanyID"].ToString(),
        //                                                                DataConvert.ToInt(model.AreaType), isCategory,
        //                                                                row["WaybillCategory"].ToString(),
        //                                                                row["DistributionCode"].ToString());
        //    flag = false;
        //    if (dtFee == null || dtFee.Rows.Count == 0)
        //    {
        //        //没有配送商收入配送费设置信息
        //        sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T4) + "、");
        //        model.IsAccount = (int) BizEnums.IncomeFeeAccountType.T4;
        //        {
        //            judgeGetIncomeFeeMsg = sbErrorMsg.ToString();
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        var datarows =
        //            dtFee.Select(string.Format("StationID='{0}'and ISEXPRESS='1'", row["DeliverStationID"].ToString()));
        //        if (datarows.Length > 0)
        //        {
        //            flag = true;
        //        }
        //    }
        //    judgeGetIncomeFeeMsg = string.Empty;
        //    return false;
        //}

        /// <summary>
        /// 计算配送费
        /// </summary>
        /// <param name="row"></param>
        /// <param name="model"></param>
        private string AccountFare(DataRow row, ref FMS_IncomeFeeInfo model)
        {
            StringBuilder sbErrorMsg = new StringBuilder();
            try
            {
                if (model.IsAccount != (int)BizEnums.IncomeFeeAccountType.T3 &&
                    model.IsAccount != (int)BizEnums.IncomeFeeAccountType.T4 &&
                    model.IsAccount != (int)BizEnums.IncomeFeeAccountType.T6)
                {
                    #region 公式
                    string waybillType = row["WaybillType"].ToString();
                    string backStatus = row["BackStationStatus"].ToString();
                    string subStatus = row["SubStatus"].ToString();
                    string formulaModel = "(({0})*{1})+{2}";
                    if ((waybillType == "0"||waybillType == "3") && backStatus == "3")
                    {
                        //妥投
                        model.AccountStandard = model.AccountStandard;
                    }
                    else if ((waybillType == "0" ||waybillType =="3" || waybillType == "1") && subStatus == "7")
                    {
                        //普通、换货拒收入库=基础*拒收费率+附加拒收费
                        formulaModel = string.Format(formulaModel, model.AccountStandard, row["RefuseFeeRate"], DataConvert.ToDecimal(row["ExtraRefuseFeeRate"], 0));
                        model.AccountStandard = formulaModel;
                    }
                    else if (waybillType == "2" && subStatus == "7")
                    {
                        //退货拒收
                        formulaModel = string.Format(formulaModel, model.AccountStandard, row["VisitReturnsVFee"], DataConvert.ToDecimal(row["ExtraVisitReturnsVFee"], 0));
                        model.AccountStandard = formulaModel;
                    }
                    else if (waybillType == "2" && subStatus == "6")
                    {
                        //退货入库
                        formulaModel = string.Format(formulaModel, model.AccountStandard, row["VisitReturnsFee"], DataConvert.ToDecimal(row["ExtraVisitReturnsFee"], 0));
                        model.AccountStandard = formulaModel;
                    }
                    else if (waybillType == "1" && subStatus == "6")
                    {
                        //换货入库=基础*换费率
                        formulaModel = string.Format(formulaModel, model.AccountStandard, row["VisitChangeFee"], DataConvert.ToDecimal(row["ExtraVisitChangeFee"], 0));
                        model.AccountStandard = formulaModel;
                    }
                    #endregion

                    #region 计算
                    if (!String.IsNullOrEmpty(model.AccountStandard))
                    {
                        string formula = model.AccountStandard;
                        Match m = Regex.Match(formula, @"(负数取零(\(重量-[\d\.]+\)))");
                        if (m.Success)
                        {
                            string str = m.Groups[1].Value;
                            string str1 = m.Groups[2].Value;
                            string str2 = str.Replace("负数取零", "Number");
                            str2 = "(" + str2 + ">0?" + str1 + ":0)";
                            formula = formula.Replace(str, str2);
                        }
                        formula = formula.Replace("向上取整", "Math.ceil").Replace("不取整", "").Replace("取整", "Math.floor").Replace("重量", model.AccountWeight.ToString());
                        model.AccountFare = Decimal.Parse(EvalJScript(formula).ToString());
                        model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T1;
                    }
                    else
                    {
                        model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T5;
                        return EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T5) + "、";
                    }
                    #endregion
                }
                return "";
            }
            catch (Exception ex)
            {
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T9;
                return sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T9) + "、").ToString();
            }
        }

        /// <summary>
        /// 计算保价费
        /// </summary>
        /// <param name="row"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string AccountProtectedFee(DataRow row, ref FMS_IncomeFeeInfo model)
        {
            try
            {
                if (!String.IsNullOrEmpty(row["ProtectedParmer"].ToString()))
                {
                    model.ProtectedStandard = DataConvert.ToDecimal(row["ProtectedParmer"].ToString(), 0);
                    if (DataConvert.ToDecimal(row["ProtectedAmount"], 0) > 0)
                    {
                        model.ProtectedFee = DataConvert.ToDecimal(row["ProtectedAmount"].ToString(), 0) * model.ProtectedStandard + DataConvert.ToDecimal(row["ExtraProtected"], 0);
                    }
                    else
                    {
                        model.ProtectedFee = 0;
                    }
                    model.IsProtected = (int)BizEnums.IncomeFeeAccountType.T1; //计算成功
                }
                else
                {
                    model.IsProtected = (int)BizEnums.IncomeFeeAccountType.T5;
                    return "没有维护保价费结算标准、";
                }
                return "";
            }
            catch (Exception ex)
            {
                model.IsProtected = (int)BizEnums.IncomeFeeAccountType.T9; //计算失败
                return "保价费计算失败、";
            }
        }

        /// <summary>
        /// 计算POS或现金代收货款手续费、服务费
        /// </summary>
        /// <param name="row"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string AccountCashOrPosFee(DataRow row, ref FMS_IncomeFeeInfo model)
        {
            try
            {
                string backStatus = row["BackStationStatus"].ToString();
                string subStatus = row["SubStatus"].ToString();
                if (backStatus == "5" || subStatus == "7")
                {
                    //拒收不计算
                    model.POSReceiveStandard = 0;
                    model.POSReceiveFee = 0;
                    model.POSReceiveServiceStandard = 0;
                    model.POSReceiveServiceFee = 0;
                    model.ReceiveStandard = 0;
                    model.ReceiveFee = 0;
                    model.CashReceiveServiceStandard = 0;
                    model.CashReceiveServiceFee = 0;
                    return "";
                }

                StringBuilder sbErrorMsg = new StringBuilder();
                if (row["AcceptType"].ToString() == "POS机")
                {
                    //服务费POS
                    if (row["ReceivePOSFeeRate"].ToString() != "")
                    {
                        model.POSReceiveStandard = System.Decimal.Parse(row["ReceivePOSFeeRate"].ToString());
                    }
                    else
                    {
                        sbErrorMsg.Append("没有维护代收货款POS手续费标准、");
                        model.POSReceiveStandard = 0;
                    }
                    if (row["NeedPayAmount"].ToString() != "")
                    {
                        model.POSReceiveFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) * model.POSReceiveStandard + DataConvert.ToDecimal(row["ExtraReceivePOSFeeRate"].ToString(), 0);
                    }

                    //服务费Cash
                    if (row["POSServiceFee"].ToString() != "")
                    {
                        model.POSReceiveServiceStandard =
                            System.Decimal.Parse(row["POSServiceFee"].ToString());
                    }
                    else
                    {
                        sbErrorMsg.Append("没有维护代收货款POS服务费标准、");
                        model.POSReceiveStandard = 0;
                    }

                    if (row["NeedPayAmount"].ToString() != "")
                    {
                        model.POSReceiveServiceFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) *
                                                        model.POSReceiveServiceStandard + DataConvert.ToDecimal(row["ExtraPOSServiceFee"].ToString(), 0);
                    }
                }
                else
                {
                    model.POSReceiveStandard = 0;
                    model.POSReceiveFee = 0;
                    model.POSReceiveStandard = 0;
                    model.POSReceiveServiceFee = 0;
                }
                if (row["AcceptType"].ToString() == "现金")
                {
                    if (!String.IsNullOrEmpty(row["ReceiveFeeRate"].ToString()))
                    {
                        model.ReceiveStandard = System.Decimal.Parse(row["ReceiveFeeRate"].ToString());
                    }
                    else
                    {
                        sbErrorMsg.Append("没有维护代收货款现金手续费标准、");
                        model.ReceiveStandard = 0;

                    }
                    if (row["NeedPayAmount"].ToString() != "")
                    {
                        model.ReceiveFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) * model.ReceiveStandard
                                            + DataConvert.ToDecimal(row["ExtraReceiveFeeRate"].ToString(), 0);
                    }

                    //Cash
                    if (row["CashServiceFee"].ToString() != "")
                    {
                        model.CashReceiveServiceStandard =
                            System.Decimal.Parse(row["CashServiceFee"].ToString());
                    }
                    else
                    {
                        model.CashReceiveServiceStandard = 0;
                        sbErrorMsg.Append("没有维护代收货款现金服务费标准、");

                    }
                    if (row["NeedPayAmount"].ToString() != "")
                    {
                        model.CashReceiveServiceFee = System.Decimal.Parse(row["NeedPayAmount"].ToString()) * model.CashReceiveServiceStandard + DataConvert.ToDecimal(row["ExtraCashServiceFee"].ToString(), 0);
                    }
                }
                else
                {
                    model.ReceiveStandard = 0;
                    model.ReceiveFee = 0;
                    model.CashReceiveServiceStandard = 0;
                    model.CashReceiveServiceFee = 0;
                }
                model.IsReceive = (int)BizEnums.IncomeFeeAccountType.T1;
                return sbErrorMsg.ToString().TrimEnd('、');
            }
            catch (Exception e)
            {
                model.IsReceive = (int)BizEnums.IncomeFeeAccountType.T9;
                return "代收货款手续费计算失败";
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        private FMS_IncomeFeeInfo GetUpdateFeeInfoModel(DataRow row, out string errorMsg)
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();
            var model = new FMS_IncomeFeeInfo();
            model = FeeDao.GetModel(Int64.Parse(row["WaybillNO"].ToString()));

            StringBuilder sbErrorMsg = new StringBuilder();
            string errorMsgStr = "<div>单号:" + row["WaybillNo"] + "商家:" + row["MerchantID"] + "区域类型:" + row["AreaType"] + "重量:" + row["AccountWeight"] + "{1} {0}</div>";

            string e1 = JudgeGetIncomeFeeMsg(row, ref model);
            sbErrorMsg.Append(string.IsNullOrEmpty(e1) ? "" : e1 + "、");

            string e2 = AccountFare(row, ref model);
            sbErrorMsg.Append(string.IsNullOrEmpty(e2) ? "" : e2 + "、");

            string e3 = AccountProtectedFee(row, ref model);
            sbErrorMsg.Append(string.IsNullOrEmpty(e3) ? "" : e3 + "、");

            string e4 = AccountCashOrPosFee(row, ref model);
            sbErrorMsg.Append(string.IsNullOrEmpty(e4) ? "" : e4 + "、");

            model.UpdateTime = DateTime.Now;
            if (!string.IsNullOrEmpty(sbErrorMsg.ToString()))
            {
                errorMsg = string.Format(errorMsgStr, sbErrorMsg.ToString().TrimEnd('、'), !string.IsNullOrEmpty(model.AccountStandard) ? "公式:" + model.AccountStandard : "");
            }
            else
            {
                errorMsg = "";
            }
            return model;
        }

        private FMS_IncomeFeeInfo GetUpdateDeductInfoModel(DataRow row)
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
        private void SendFailedMail(string mailSubject, string mailBody)
        {
            var mailService = ServiceLocator.GetService<IMail>();

            mailService.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
            //Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
        }

        private static void WriteTest(string tips)
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

        private static object EvalJScript(string JScript)
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
        #endregion

        #region 清除服务
        public string GetIncomeErrorLog()
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();
            StringBuilder sbLog = new StringBuilder();
            
            sbLog.Append("<html><head></head><body>");
            #region 计算失败
            sbLog.Append("<div style='font-weight:bold'>" + EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T9) + "：</div>");
            try
            {
                DataTable dtError9 = FeeDao.GetAccountError9();
                int n = 0;
                if (dtError9 == null || dtError9.Rows.Count <= 0)
                {
                    sbLog.Append("<div>无</div>");
                }
                else
                {
                    foreach (DataRow dr in dtError9.Rows)
                    {
                        sbLog.Append(dr["WaybillNO"].ToString() + ",");
                        n++;
                        if (n % 10 == 0)
                            sbLog.AppendLine("<br>");
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("<div>" + ex.Message.Substring(0, ex.Message.Length > 50 ? 50 : ex.Message.Length) + "</div>");
            }
            #endregion

            #region 没有结算标准
            sbLog.Append("<div style='font-weight:bold'>" + EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T5) + "：</div>");
            try
            {
                DataTable dtError5 = FeeDao.GetAccountError45((int)BizEnums.IncomeFeeAccountType.T5);
                if (dtError5 == null || dtError5.Rows.Count <= 0)
                    sbLog.Append("<div>无</div>");
                else
                {
                    var modelList = (from t in dtError5.AsEnumerable()
                                     group t by new
                                     {
                                         MerchantID = t["MerchantID"],
                                         MerchantName = t["MerchantName"],
                                         ExpressCompanyID = t["ExpressCompanyID"],
                                         CompanyName = t["CompanyName"],
                                         AreaID = t["AreaID"],
                                         ProvinceName = t["ProvinceName"],
                                         CityName = t["CityName"],
                                         AreaName = t["AreaName"],
                                     } into g
                                     select new { g.Key }).ToList();

                    foreach (var o in modelList)
                    {
                        sbLog.AppendLine("<div>");
                        sbLog.Append("商家:" + o.Key.MerchantName + "(" + o.Key.MerchantID + ") ");
                        sbLog.Append("分拣中心:" + o.Key.CompanyName + "(" + o.Key.ExpressCompanyID + ") ");
                        sbLog.Append("省市区:" + o.Key.ProvinceName + " " + o.Key.CityName + " " + o.Key.AreaName + "(" + o.Key.AreaID + ") ");
                        sbLog.AppendLine("</div>");
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("<div>" + ex.Message.Substring(0, ex.Message.Length > 50 ? 50 : ex.Message.Length) + "</div>");
            }
            #endregion

            #region 没有区域类型
            sbLog.AppendLine("<div style='font-weight:bold'>" + EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T3) + "：</div>");
            try
            {
                DataTable dtError3 = FeeDao.GetAccountError3();
                if (dtError3 == null || dtError3.Rows.Count <= 0)
                    sbLog.Append("<div>无</div>");
                else
                {
                    var modelList = (from t in dtError3.AsEnumerable()
                                     group t by new
                                     {
                                         MerchantID = t["MerchantID"],
                                         MerchantName = t["MerchantName"],
                                         ExpressCompanyID = t["ExpressCompanyID"],
                                         CompanyName = t["CompanyName"],
                                         AreaID = t["AreaID"],
                                         ProvinceName = t["ProvinceName"],
                                         CityName = t["CityName"],
                                         AreaName = t["AreaName"],
                                     } into g
                                     select new { g.Key }).ToList();

                    foreach (var o in modelList)
                    {
                        sbLog.AppendLine("<div>");
                        sbLog.Append("商家:" + o.Key.MerchantName + "(" + o.Key.MerchantID + ") ");
                        sbLog.Append("分拣中心:" + o.Key.CompanyName + "(" + o.Key.ExpressCompanyID + ") ");
                        sbLog.Append("省市区:" + o.Key.ProvinceName + " " + o.Key.CityName + " " + o.Key.AreaName + "(" + o.Key.AreaID + ") ");
                        sbLog.AppendLine("</div>");
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("<div>" + ex.Message.Substring(0, ex.Message.Length > 50 ? 50 : ex.Message.Length) + "</div>");
            }
            #endregion

            #region 没有区域类型
            sbLog.AppendLine("<div style='font-weight:bold'>" + EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T4) + "：</div>");
            try
            {
                DataTable dtError4 = FeeDao.GetAccountError45((int)BizEnums.IncomeFeeAccountType.T4);
                if (dtError4 == null || dtError4.Rows.Count <= 0)
                    sbLog.Append("<div>无</div>");
                else
                {
                    var modelList = (from t in dtError4.AsEnumerable()
                                     group t by new
                                     {
                                         MerchantID = t["MerchantID"],
                                         MerchantName = t["MerchantName"],
                                         ExpressCompanyID = t["ExpressCompanyID"],
                                         CompanyName = t["CompanyName"],
                                         AreaID = t["AreaID"],
                                         ProvinceName = t["ProvinceName"],
                                         CityName = t["CityName"],
                                         AreaName = t["AreaName"],
                                         AreaType = t["AreaType"],
                                     } into g
                                     select new { g.Key }).ToList();

                    foreach (var o in modelList)
                    {
                        sbLog.AppendLine("<div>");
                        sbLog.Append("商家:" + o.Key.MerchantName + "(" + o.Key.MerchantID + ") ");
                        sbLog.Append("分拣中心:" + o.Key.CompanyName + "(" + o.Key.ExpressCompanyID + ") ");
                        sbLog.Append("省市区:" + o.Key.ProvinceName + " " + o.Key.CityName + " " + o.Key.AreaName + "(" + o.Key.AreaID + ") ");
                        sbLog.Append("区域类型:" + o.Key.AreaType);
                        sbLog.AppendLine("</div>");
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("<div>" + ex.Message.Substring(0, ex.Message.Length > 50 ? 50 : ex.Message.Length) + "</div>");
            }
            #endregion

            sbLog.Append("</body></html>");
            return sbLog.ToString();
        }

        public void ClearIncomeIsAccount(int rowCount)
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();
            DataTable dt = FeeDao.GetClearDatalist();
            if (dt == null || dt.Rows.Count <= 0)
                return;

            int n = 0;
            foreach (DataRow dr in dt.Rows)
            {
                FeeDao.UpdateIncomeFeeIsAccount(Int64.Parse(dr["IncomeFeeID"].ToString()));
                if (n == rowCount)
                {
                    Thread.Sleep(2000);
                    n = 0;
                }
                n++;
            }
        }
        #endregion

        #region 计算历史服务
        public void AccountIncomeHistory(int rowCount)
        {
            IIncomeFeeInfoDao FeeDao = ServiceLocator.GetService<IIncomeFeeInfoDao>();
            DataTable dtDeliver = FeeDao.GetHistoryInComeFeeInfoDeliver(rowCount);
            AccountFee(dtDeliver);
            DataTable dtReturn = FeeDao.GetHistoryInComeFeeInfoReturn(rowCount);
            AccountFee(dtReturn);
            DataTable dtVisit = FeeDao.GetHistoryInComeFeeInfoVisit(rowCount);
            AccountFee(dtVisit);
        }
        #endregion

        #region 生效服务
        
        public void DisposeEffect(int rowCount)
        {
            try
            {
                IMerchantDeliverFee merchantDeliverFee = ServiceLocator.GetService<IMerchantDeliverFee>();
                IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
                DataTable dtBasic = deliverFeeService.GetWaitFeeList();//基础信息

                DataTable dtFee = merchantDeliverFee.GetWaitFeeList();//配送费

                UpdateToEffect(dtBasic, dtFee, rowCount);
            }
            catch (Exception ex)
            {
                IMail mailService = ServiceLocator.GetService<IMail>();
                mailService.SendMailToUser("收入价格生效服务", ex.Message + ex.StackTrace, FailedMailAdress);
            }
        }

        private void UpdateToEffect(DataTable dtBasic, DataTable dtFee, int rowCount)
        {
            try
            {
                IMerchantDeliverFee merchantDeliverFee = ServiceLocator.GetService<IMerchantDeliverFee>();
                IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
                //目前更新回SQL
                int n = 0;
                foreach (DataRow dr in dtBasic.Rows)
                {
                    if (n == rowCount)
                    {
                        Thread.Sleep(2000);
                        n = 0;
                    }

                    bool flag = deliverFeeService.UpdateToEffect(dr);
                    if (flag)
                    {
                        //更新回oracle的标识
                        bool flag1 = deliverFeeService.DeleteWaitMerchantDeliverFee(dr["effectid"].ToString());
                    }
                    n++;
                }
                n = 0;
                foreach (DataRow dr in dtFee.Rows)
                {
                    if (n == rowCount)
                    {
                        Thread.Sleep(2000);
                        n = 0;
                    }

                    bool flag = merchantDeliverFee.UpdateToEffect(dr);
                    if (flag)
                    {
                        //更新回Oracle
                        bool flag1 = merchantDeliverFee.DeleteWaitStationDeliverFee(dr["EffectKid"].ToString());
                    }
                    n++;
                }
            }
            catch (Exception ex)
            {
                IMail mailService = ServiceLocator.GetService<IMail>();
                mailService.SendMailToUser("收入价格生效服务", ex.Message + ex.StackTrace, FailedMailAdress);
            }
        }
        #endregion
    }
}
