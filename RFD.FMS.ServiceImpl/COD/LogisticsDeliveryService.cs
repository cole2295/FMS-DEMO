﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using RFD.FMS.Util;
using System.IO;
using System.Configuration;
using RFD.FMS.Service;
using RFD.FMS.Domain;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.COD;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.COD;
using RFD.FMS.Model;

namespace RFD.FMS.ServiceImpl.COD
{
    public class LogisticsDeliveryService : ILogisticsDeliveryService
    {
        private ILogisticsDeliveryDao _logisticsDeliveryDao;

        public LogisticsDeliveryService()
        {

        }

        private static readonly string FinancialCutDate = ConfigurationManager.AppSettings["FinancialCutDate"] == null ? null : ConfigurationManager.AppSettings["FinancialCutDate"];

        public DataTable SearchCodDetailsV2(CodSearchCondition condition, ref PageInfo pi, out DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);

            amount = _logisticsDeliveryDao.StatCodV2(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0)
                return null;

            pi.ItemCount = int.Parse(amount.Rows[0]["订单量合计"].ToString());

            return _logisticsDeliveryDao.SearchCodDetailsV2(searchCondition, ref pi, condition);
        }

        public DataTable SearchCodDetails(CodSearchCondition condition, ref PageInfo pi, out DataTable amount)
        {
            //string searchCondition = BuildSearchCondition(condition);
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);

            amount = _logisticsDeliveryDao.StatCod<OracleParameter>(searchCondition,condition,parameterList);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0)
                return null;

            pi.ItemCount = int.Parse(amount.Rows[0]["订单量合计"].ToString());
            pi.PageCount = Convert.ToInt32(Math.Ceiling(pi.ItemCount * 1.0 / pi.PageSize));

            return _logisticsDeliveryDao.SearchCodDetails<OracleParameter>(searchCondition, ref pi, condition,parameterList,true);
        }
        private List<OracleParameter> BuildSearchCondition(CodSearchCondition condition,out string sqlWhere)
        {
            List<OracleParameter> parameterList = new List<OracleParameter>();
            if (condition == null)
                throw new Exception("没有查询条件");
            StringBuilder sbCondition = new StringBuilder();

            sbCondition.Append(" AND fcbi.IsDeleted=0 ");

            if (!string.IsNullOrEmpty(condition.WaybillNO))
            {
                var Waybillnos = condition.WaybillNO.Trim().Replace("，", ",").Split(",".ToCharArray(),
                                                                                StringSplitOptions.RemoveEmptyEntries);

                int i = 0;
                string paraWaybillno = string.Empty;
                foreach (string waybillno in Waybillnos)
                {
                    paraWaybillno += ",:waybillno" + i;
                    parameterList.Add(new OracleParameter() { ParameterName = "waybillno" + i, Value = decimal.Parse(waybillno), OracleDbType = OracleDbType.Decimal });
                    i++;
                }
                sbCondition.Append(string.Format(" And fcbi.WaybillNO in ({0})", paraWaybillno.Substring(1)));
                //sbCondition.AppendFormat(" AND fcbi.WaybillNO = :WaybillNO ");
                //parameterList.Add(new OracleParameter(":WaybillNO", OracleDbType.Decimal) { Value = condition.WaybillNO }); 
            }
            if (string.IsNullOrEmpty(condition.WaybillNO))
            {
                sbCondition.AppendFormat("AND fcbi.CreateTime > sysdate-90");
                if (!string.IsNullOrEmpty(condition.ExpressCompanyID))
                {
                    //sbCondition.Append(Util.Common.GetOracleInParameterWhereSql("fcbi.TopCODCompanyID", "TopCODCompanyID", false, false));
                    //parameterList.Add(new OracleParameter(":TopCODCompanyID", OracleDbType.Varchar2, 2000) { Value = condition.ExpressCompanyID.Replace(" ", "") });
                    sbCondition.Append(string.Format(" and fcbi.TopCODCompanyID in ({0})", condition.ExpressCompanyID.Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(condition.AreaType))
                {
                    if (condition.AreaType == "99")
                    {
                        sbCondition.Append(" AND ael.AreaType IS NULL ");
                    }
                    else
                    {
                        sbCondition.AppendFormat(" AND ael.AreaType = :AreaType ");
                        parameterList.Add(new OracleParameter(":AreaType",OracleDbType.Varchar2,40){Value = condition.AreaType});
                    }
                  
                }

                if (!string.IsNullOrEmpty(condition.Sources))
                {
                    //sbCondition.Append(Util.Common.GetOracleInParameterWhereSql("fcbi.MerchantID", "MerchantID", false, false));
                    //parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Varchar2, 2000) { Value = condition.Sources.Replace(" ", "") });  
                    sbCondition.Append(string.Format(" AND fcbi.MerchantID IN ({0})", condition.Sources.Replace(" ", "")));
                }
                if (condition.IsCod==0)
                    sbCondition.Append(" AND (fcbi.IsCOD =0 OR fcbi.IsCOD IS NULL) ");
                if (condition.IsCod == 1)
                    sbCondition.Append(" AND fcbi.IsCOD=1 ");

                if (condition.AmountStr > 0)
                {
                    sbCondition.AppendFormat(" AND fcbi.TotalAmount>=:AmountStr");
                    parameterList.Add(new OracleParameter(":AmountStr", OracleDbType.Decimal) { Value = condition.AmountStr }); 
                }
                if (condition.AmountEnd > 0)
                {
                    sbCondition.AppendFormat(" AND fcbi.TotalAmount<:AmountEnd");
                    parameterList.Add(new OracleParameter(":AmountEnd", OracleDbType.Decimal) { Value = condition.AmountEnd });  
                }
            }

            //todo: zengwei 拆分方法
            string dateColumn = string.Empty;
            string houseColumn = string.Empty;
            string endHouseColumn = string.Empty;
            //发货
            if (condition.ReportType == "")
            {
                sbCondition.Append(" AND fcbi.Flag=1 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                {
                    sbCondition.Append(" AND fcbi.WaybillType IN ('0','1','3') ");
                }
                else
                {
                    sbCondition.AppendFormat(" AND fcbi.WaybillType =:WaybillType  ");
                    parameterList.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = condition.ShipmentType });
                }
                dateColumn = condition.DateType == "0" ? "fcbi.RfdAcceptDate" : "fcbi.DeliverTime";
                houseColumn = condition.HouseType == "0" ? "fcbi.WarehouseId" : "fcbi.WarehouseId";
                endHouseColumn = condition.HouseType == "0" ? "fcbi.ExpressCompanyID" : "fcbi.FinalExpressCompanyID";
            }

            //上门退
            if (condition.ReportType == "6")
            {
                sbCondition.Append(" AND fcbi.Flag=1 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                {
                    sbCondition.Append(" AND fcbi.WaybillType IN ('1','2') ");
                }
                else
                {
                    sbCondition.AppendFormat(" AND fcbi.WaybillType =:WaybillType  ");
                    parameterList.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = condition.ShipmentType });
 
                }
                dateColumn = "fcbi.ReturnTime";
                houseColumn = "fcbi.ReturnWareHouseID";
                endHouseColumn = "fcbi.ReturnExpressCompanyID";
            }

            //拒收
            if (condition.ReportType == "7")
            {
                sbCondition.Append(" AND fcbi.Flag=0 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                {
                    sbCondition.Append(" AND fcbi.WaybillType IN ('0','1','3') ");
                }
                else
                {
                    sbCondition.AppendFormat(" AND fcbi.WaybillType =:WaybillType  ");
                    parameterList.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = condition.ShipmentType });
                }
                dateColumn = "fcbi.ReturnTime";
                houseColumn = "fcbi.ReturnWareHouseID";
                endHouseColumn = "fcbi.ReturnExpressCompanyID";
            }

            if (string.IsNullOrEmpty(condition.WaybillNO))
            {
                sbCondition.AppendFormat(" AND {0}>=to_date(:DateStr,'yyyy-mm-dd ') AND {0}<to_date(:DateEnd,'yyyy-mm-dd ') ", dateColumn);
                parameterList.Add(new OracleParameter(":DateStr", OracleDbType.Varchar2, 40) { Value = condition.DateStr });
                parameterList.Add(new OracleParameter(":DateEnd", OracleDbType.Varchar2, 40) { Value = condition.DateEnd });
                sbCondition.Append(BuildHouseCondition(condition.HouseCode, houseColumn, endHouseColumn));
            }
            sqlWhere = sbCondition.ToString();
            return parameterList;
        }

       
        private string BuildSearchCondition(CodSearchCondition condition)
        {
            if (condition == null)
                throw new Exception("没有查询条件");
            StringBuilder sbCondition = new StringBuilder();

            sbCondition.Append(" AND fcbi.IsDeleted=0 ");

            if (!string.IsNullOrEmpty(condition.WaybillNO))
                sbCondition.AppendFormat(" AND fcbi.WaybillNO = {0} ", condition.WaybillNO);

            if (string.IsNullOrEmpty(condition.WaybillNO))
            {
                if (!string.IsNullOrEmpty(condition.ExpressCompanyID))
                    sbCondition.AppendFormat(" AND fcbi.TopCODCompanyID IN ({0}) ", condition.ExpressCompanyID);

                if (!string.IsNullOrEmpty(condition.AreaType))
                {
                    if (condition.AreaType == "99")
                        sbCondition.Append(" AND ael.AreaType IS NULL ");
                    else
                        sbCondition.AppendFormat(" AND ael.AreaType = {0} ", condition.AreaType);
                }

                if (!string.IsNullOrEmpty(condition.Sources))
                    sbCondition.AppendFormat(" AND fcbi.MerchantID IN ({0}) ", condition.Sources);

                if (condition.IsCod==0)
                    sbCondition.Append(" AND (fcbi.IsCOD =0 OR fcbi.IsCOD IS NULL) ");
                if (condition.IsCod == 1)
                    sbCondition.Append(" AND fcbi.IsCOD=1 ");

                if (condition.AmountStr > 0)
                    sbCondition.AppendFormat(" AND fcbi.TotalAmount>={0}", condition.AmountStr);
                if (condition.AmountEnd > 0)
                    sbCondition.AppendFormat(" AND fcbi.TotalAmount<{0}", condition.AmountEnd);
            }

            //todo: zengwei 拆分方法
            string dateColumn = string.Empty;
            string houseColumn = string.Empty;
            string endHouseColumn = string.Empty;
            //发货
            if (condition.ReportType == "")
            {
                sbCondition.Append(" AND fcbi.Flag=1 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                    sbCondition.Append(" AND fcbi.WaybillType IN ('0','1','3') ");
                else
                    sbCondition.AppendFormat(" AND fcbi.WaybillType ='{0}'  ", condition.ShipmentType);

                dateColumn = condition.DateType == "0" ? "fcbi.RfdAcceptDate" : "fcbi.DeliverTime";
                houseColumn = condition.HouseType == "0" ? "fcbi.WarehouseId" : "fcbi.WarehouseId";
                endHouseColumn = condition.HouseType == "0" ? "fcbi.ExpressCompanyID" : "fcbi.FinalExpressCompanyID";

                //初始末级切换时间
                //if (condition.DateType == "1")
                //{
                //    if (string.IsNullOrEmpty(FinancialCutDate))
                //        throw new Exception("初始最终切换时间点未找到");

                //    sbCondition.AppendFormat(" AND fcbi.RfdAcceptTime >='{0}'  ", FinancialCutDate);
                //}
            }

            //上门退
            if (condition.ReportType == "6")
            {
                sbCondition.Append(" AND fcbi.Flag=1 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                    sbCondition.Append(" AND fcbi.WaybillType IN ('1','2') ");
                else
                    sbCondition.AppendFormat(" AND fcbi.WaybillType ='{0}'  ", condition.ShipmentType);

                dateColumn = "fcbi.ReturnTime";
                houseColumn = "fcbi.ReturnWareHouseID";
                endHouseColumn = "fcbi.ReturnExpressCompanyID";
            }

            //拒收
            if (condition.ReportType == "7")
            {
                sbCondition.Append(" AND fcbi.Flag=0 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                    sbCondition.Append(" AND fcbi.WaybillType IN ('0','1','3') ");
                else
                    sbCondition.AppendFormat(" AND fcbi.WaybillType ='{0}'  ", condition.ShipmentType);

                dateColumn = "fcbi.ReturnTime";
                houseColumn = "fcbi.ReturnWareHouseID";
                endHouseColumn = "fcbi.ReturnExpressCompanyID";
            }

            if (string.IsNullOrEmpty(condition.WaybillNO))
            {
                sbCondition.AppendFormat(" AND {0}>=to_date('{1}','yyyy-mm-dd hh24:mi:ss') AND {0}<to_date('{2}','yyyy-mm-dd hh24:mi:ss') ", dateColumn, condition.DateStr, condition.DateEnd);
                sbCondition.Append(BuildHouseCondition(condition.HouseCode, houseColumn, endHouseColumn));
            }

            return sbCondition.ToString();
        }

        private string BuildHouseCondition(string houseStr, string houseColumn, string endHouseColumn)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbE = new StringBuilder();
            StringBuilder sbS = new StringBuilder();

            if (string.IsNullOrEmpty(houseStr))
                return "";

            string[] houses = houseStr.Split(',');
            foreach (string s in houses)
            {
                if (s.ToLower().Contains("s_"))
                    sbS.Append("'" + s.Replace("s_", "").Replace("S_", "").Replace(" ", "") + "',");
                else
                    sbE.Append("'" + s.Replace(" ", "") + "',");
            }
            sb.Append(" AND (");
            if (!string.IsNullOrEmpty(sbE.ToString()))
                sb.AppendFormat(" {0} IN ({1}) ", houseColumn, sbE.ToString().TrimEnd(','));
            if (!string.IsNullOrEmpty(sbS.ToString()) && !string.IsNullOrEmpty(sbE.ToString()) && sb.Length > 0)
                sb.Append(" OR ");
            if (!string.IsNullOrEmpty(sbS.ToString()))
                sb.AppendFormat(" {0} IN ({1}) ", endHouseColumn, sbS.ToString().TrimEnd(','));
            sb.Append(" )");

            if (sb.ToString().Length > 0)
                return sb.ToString();
            else
                return "";
        }

        public DataTable SearchCodStatV2(CodSearchCondition condition, out DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);

            amount = _logisticsDeliveryDao.StatCodV2(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0)
                return null;

            return _logisticsDeliveryDao.SearchCodStatV2(searchCondition, condition);
        }

        public DataTable SearchCodStat(CodSearchCondition condition, out DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);

            amount = _logisticsDeliveryDao.StatCod(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0)
                return null;

            return _logisticsDeliveryDao.SearchCodStat(searchCondition, condition);
        }

        public DataTable SearchExprotDetailDataV2(CodSearchCondition condition, string exprotPath)
        {
            string searchCondition = BuildSearchCondition(condition);

            return _logisticsDeliveryDao.SearchExprotDetailDataV2(searchCondition, condition);
        }

        public DataTable SearchExprotDetailData(CodSearchCondition condition,string exprotPath)
        {
            //string searchCondition = BuildSearchCondition(condition);
            string searchCondition = string.Empty;
            List<OracleParameter> parameterList = BuildSearchCondition(condition, out searchCondition);

            return _logisticsDeliveryDao.SearchExprotDetailData<OracleParameter>(searchCondition, condition,parameterList);
        }

        public DataTable SearchExprotStatDataV2(CodSearchCondition condition)
        {
            string searchCondition = BuildSearchCondition(condition);

            return _logisticsDeliveryDao.SearchExprotStatDataV2(searchCondition, condition);
        }

        public DataTable SearchExprotStatData(CodSearchCondition condition)
        {
            string searchCondition = BuildSearchCondition(condition);

            return _logisticsDeliveryDao.SearchExprotStatData(searchCondition, condition);
        }

        private void ExportDistributionReports(Dictionary<string, Dictionary<string, DataTable>> reports, string exprotPath,string exprotType)
        {
            foreach (KeyValuePair<string, Dictionary<string, DataTable>> source in reports)
            {
                CreateExportReportPath(Path.Combine(exprotPath, source.Key));
                foreach (KeyValuePair<string, DataTable> station in source.Value)
                {
                    var fileName = String.Format(@"{0}\{1}\{2}{3}-{1}.xls",
                        exprotPath,
                        source.Key,
                        station.Key,
                        DateTime.Now.ToString("yyyy-MM-dd"));
                    ExcelHelper.Export(station.Value, fileName);
                }
            }
            CompressExportReports(exprotPath, exprotType);
        }

        private void CompressExportReports(string exprotPath,string exprotType)
        {
            var zip = new ZipUtil();
            var error = String.Empty;
            var fileName = exprotPath + ".7z";
            var result = zip.CompressDirectory(exprotPath, fileName, out error);
            if (result)
            {
                ExcelHelper.ExportByWeb(exprotPath, fileName);
            }
            else
            {
                throw new Exception(error);
            }
        }

        private void CreateExportReportPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public DataTable SearchLogisticsDailyV2(CODSearchCondition condition)
        {
            string type = string.Empty;
            string conditionStr = string.Empty;
            string accountDateColumn = string.Empty;
            Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

            //普通发货
            type = "D";
            conditionStr = BuildSearchDailyCondition(condition, type, out accountDateColumn);
            DataTable dt_D = _logisticsDeliveryDao.SearchLogisticsDeliverDailyV2(conditionStr, type, condition, accountDateColumn);
            tables.Add(type, dt_D);

            //普通发货拒收
            type = "DR";
            conditionStr = BuildSearchDailyCondition(condition, type, out accountDateColumn);
            DataTable dt_DR = _logisticsDeliveryDao.SearchLogisticsReturnsDailyV2(conditionStr, type, condition, accountDateColumn);
            tables.Add(type, dt_DR);

            //上门换发货
            type = "DV";
            conditionStr = BuildSearchDailyCondition(condition, type, out accountDateColumn);
            DataTable dt_DV = _logisticsDeliveryDao.SearchLogisticsDeliverDailyV2(conditionStr, type, condition, accountDateColumn);
            tables.Add(type, dt_DV);

            //上门换发货拒收
            type = "DVR";
            conditionStr = BuildSearchDailyCondition(condition, type, out accountDateColumn);
            DataTable dt_DVR = _logisticsDeliveryDao.SearchLogisticsReturnsDailyV2(conditionStr, type, condition, accountDateColumn);
            tables.Add(type, dt_DVR);

            //上门退货
            type = "V";
            conditionStr = BuildSearchDailyCondition(condition, type, out accountDateColumn);
            DataTable dt_V = _logisticsDeliveryDao.SearchLogisticsDeliverDailyV2(conditionStr, type, condition, accountDateColumn);
            tables.Add(type, dt_V);

            return UniteDailyTable(tables, condition);
        }

        public DataTable SearchLogisticsDaily(CODSearchCondition condition,ref string TimeMessage)
        {
            Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();
            var asyncTaskHelper = new AsyncTaskHelper<DataTable>();
            //string TimeMessage = "";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //普通发货
            string type_d = "D";
            string accountDateColumn_d = string.Empty;
            string conditionStr_d = string.Empty;
            List<OracleParameter> parameters_d = BuildSearchDailyCondition(condition, type_d,out accountDateColumn_d,out conditionStr_d);

            //普通发货拒收
            string type_dr = "DR";
            string accountDateColumn_dr = string.Empty;
            string conditionStr_dr = string.Empty;
            List<OracleParameter> parameters_dr = BuildSearchDailyCondition(condition, type_dr, out accountDateColumn_dr, out conditionStr_dr);

            //上门换发货
            string type_dv = "DV";
            string accountDateColumn_dv = string.Empty;
            string conditionStr_dv = string.Empty;
            List<OracleParameter> parameters_dv = BuildSearchDailyCondition(condition, type_dv, out accountDateColumn_dv, out conditionStr_dv);


            //上门换发货拒收
            string type_dvr = "DVR";
            string accountDateColumn_dvr = string.Empty;
            string conditionStr_dvr = string.Empty;
            List<OracleParameter> parameters_dvr = BuildSearchDailyCondition(condition, type_dvr, out accountDateColumn_dvr, out conditionStr_dvr);

            //上门退货
            string type_v = "V";
            string accountDateColumn_v = string.Empty;
            string conditionStr_v = string.Empty;
            List<OracleParameter> parameters_v = BuildSearchDailyCondition(condition, type_v, out accountDateColumn_v, out conditionStr_v);
            
            //签单返回发
            string type_sr_d = "SRD";
            string accountDateColumn_sr_d = string.Empty;
            string conditionStr_sr_d = string.Empty;
            List<OracleParameter> parameters_sr_d = BuildSearchDailyCondition(condition, type_sr_d, out accountDateColumn_sr_d, out conditionStr_sr_d);

            //签单返回据
            string type_sr_r = "SRR";
            string accountDateColumn_sr_r = string.Empty;
            string conditionStr_sr_r = string.Empty;
            List<OracleParameter> parameters_sr_r = BuildSearchDailyCondition(condition, type_sr_r, out accountDateColumn_sr_r, out conditionStr_sr_r);

            DataTable dt_D = _logisticsDeliveryDao.SearchLogisticsDeliverDaily<OracleParameter>(conditionStr_d, type_d, condition, accountDateColumn_d, parameters_d);
            tables.Add(type_d, dt_D);
            DataTable dt_DR = _logisticsDeliveryDao.SearchLogisticsReturnsDaily<OracleParameter>(conditionStr_dr, type_dr, condition, accountDateColumn_dr, parameters_dr);
            tables.Add(type_dr, dt_DR);
            DataTable dt_DV = _logisticsDeliveryDao.SearchLogisticsDeliverDaily<OracleParameter>(conditionStr_dv, type_dv, condition, accountDateColumn_dv, parameters_dv);
            tables.Add(type_dv, dt_DV);
            DataTable dt_DVR = _logisticsDeliveryDao.SearchLogisticsReturnsDaily<OracleParameter>(conditionStr_dvr, type_dvr, condition, accountDateColumn_dvr, parameters_dvr);
            tables.Add(type_dvr, dt_DVR);
            DataTable dt_V = _logisticsDeliveryDao.SearchLogisticsDeliverDaily<OracleParameter>(conditionStr_v, type_v, condition, accountDateColumn_v, parameters_v);
            tables.Add(type_v, dt_V);
            DataTable dt_SRD = _logisticsDeliveryDao.SearchLogisticsDeliverDaily<OracleParameter>(conditionStr_sr_d, type_sr_d, condition, accountDateColumn_sr_d, parameters_sr_d);
            tables.Add(type_sr_d,dt_SRD);

            DataTable dt_SRR = _logisticsDeliveryDao.SearchLogisticsReturnsDaily<OracleParameter>(conditionStr_sr_r, type_sr_r, condition, accountDateColumn_sr_r, parameters_sr_r);
            tables.Add(type_sr_r, dt_SRR);
            TimeMessage +="获取数据耗时:"+sw.ElapsedMilliseconds.ToString()+"毫秒，";
            sw.Stop();
            DataTable dtn = UnitDailyTableNew(tables, condition,ref TimeMessage);
            
            return dtn;

        }
        private List<OracleParameter> BuildSearchDailyCondition(CODSearchCondition condition, string type, out string accountDateColumn,out string sqlWhere)
        {
            List<OracleParameter> parameterList = new List<OracleParameter>();
            if (condition == null)
            {
                accountDateColumn = "";
                sqlWhere = null;
                return parameterList;

            }

            string dateColumn = string.Empty;
            string houseColumn = string.Empty;
            string endHouseColumn = string.Empty;
            string accountDate = string.Empty;
            StringBuilder sbStr = new StringBuilder();
            sbStr.Append(" AND fci.IsDeleted=0 ");
            //查询权限
            sbStr.AppendFormat(" AND (fci.DistributionCode=:DistributionCode OR ec.DistributionCode=:DistributionCode) ");
            parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = condition.DistributionCode });
            //配送公司
            if (!string.IsNullOrEmpty(condition.ExpressCompanyID))
            {
                sbStr.Append(string.Format(" AND fci.TopCODCompanyID IN ({0}) ", condition.ExpressCompanyID.Replace(" ", "")));
                //sbStr.Append(Util.Common.GetOracleInParameterWhereSql("fci.TopCODCompanyID", "TopCODCompanyID", false, false));
                //parameterList.Add(new OracleParameter(":TopCODCompanyID", OracleDbType.Varchar2, 2000) { Value = condition.ExpressCompanyID.Replace(" ", "") });
            }
                
            //商家
            if (!string.IsNullOrEmpty(condition.MerchantID))
            {
                sbStr.Append(string.Format(" AND fci.MerchantID IN ({0}) ", condition.MerchantID));
                //sbStr.Append(Util.Common.GetOracleInParameterWhereSql("fci.MerchantID", "MerchantID", false, false));
                //parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Varchar2, 2000) { Value = condition.MerchantID.Replace(" ", "") });   
            }
            

            switch (type)
            {
                case "D"://发货
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('0') ");
                    dateColumn = "fci.DeliverTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.FinalExpressCompanyID";
                    accountDate = "DeliverDate";
                    break;
                case "DR"://发拒
                    sbStr.Append(" AND fci.Flag=0 ");
                    sbStr.Append(" AND fci.WaybillType IN ('0') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.ReturnWareHouseID";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                case "DV"://换发
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('1') ");
                    dateColumn = "fci.DeliverTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.FinalExpressCompanyID";
                    accountDate = "DeliverDate";
                    break;
                case "DVR"://换拒
                    sbStr.Append(" AND fci.Flag=0 ");
                    sbStr.Append(" AND fci.WaybillType IN ('1') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.ReturnWareHouseID";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                case "V"://上门退
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('2') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.ReturnWareHouseID";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                case "SRD": //签单返回发
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('3') ");
                    dateColumn = "fci.DeliverTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "DeliverDate";
                    break;
                case "SRR": //签单返回拒
                    sbStr.Append(" AND fci.Flag=0 ");
                    sbStr.Append(" AND fci.WaybillType IN ('3') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                default:
                    break;
            }
            sbStr.AppendFormat(" AND {0}>=to_date(:Date_D_S,'yyyy-mm-dd hh24:mi:ss') AND {0} <to_date( :Date_D_E,'yyyy-mm-dd hh24:mi:ss') ", dateColumn);
            parameterList.Add(new OracleParameter(":Date_D_S", OracleDbType.Varchar2, 20) { Value = condition.Date_D_S });
            parameterList.Add(new OracleParameter(":Date_D_E", OracleDbType.Varchar2, 20) { Value = condition.Date_D_E });
            sbStr.Append(BuildHouseCondition(condition.HouseD, houseColumn, endHouseColumn));

            accountDateColumn = accountDate;
            sqlWhere = sbStr.ToString();

            return parameterList;
        }
        private string BuildSearchDailyCondition(CODSearchCondition condition,string type, out string accountDateColumn)
        {
            if (condition == null)
            {
                accountDateColumn = "";
                return null;
            }

            string dateColumn = string.Empty;
            string houseColumn = string.Empty;
            string endHouseColumn = string.Empty;
            string accountDate = string.Empty;
            StringBuilder sbStr = new StringBuilder();
            sbStr.Append(" AND fci.IsDeleted=0 ");
            //查询权限
            sbStr.Append(string.Format(" AND (fci.DistributionCode='{0}' OR ec.DistributionCode='{0}') ", condition.DistributionCode));
            //配送公司
            if(!string.IsNullOrEmpty(condition.ExpressCompanyID))
                sbStr.Append(string.Format(" AND fci.TopCODCompanyID IN ({0}) ", condition.ExpressCompanyID));
            //商家
            if(!string.IsNullOrEmpty(condition.MerchantID))
                sbStr.Append(string.Format(" AND fci.MerchantID IN ({0}) ", condition.MerchantID));

            switch (type)
            {
                case "D"://发货
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('0') ");
                    dateColumn = "fci.DeliverTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.FinalExpressCompanyID";
                    accountDate = "DeliverDate";
                    break;
                case "DR"://发拒
                    sbStr.Append(" AND fci.Flag=0 ");
                    sbStr.Append(" AND fci.WaybillType IN ('0') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.ReturnWareHouseID";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                case "DV"://换发
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('1') ");
                    dateColumn = "fci.DeliverTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.FinalExpressCompanyID";
                    accountDate = "DeliverDate";
                    break;
                case "DVR"://换拒
                    sbStr.Append(" AND fci.Flag=0 ");
                    sbStr.Append(" AND fci.WaybillType IN ('1') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.ReturnWareHouseID";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                case "V"://上门退
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('2') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.ReturnWareHouseID";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                case "SRD": //签单返回发
                    sbStr.Append(" AND fci.Flag=1 ");
                    sbStr.Append(" AND fci.WaybillType IN ('3') ");
                    dateColumn = "fci.DeliverTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "DeliverDate";
                    break;
                case "SRR": //签单返回拒
                    sbStr.Append(" AND fci.Flag=0 ");
                    sbStr.Append(" AND fci.WaybillType IN ('3') ");
                    dateColumn = "fci.ReturnTime";
                    houseColumn = "fci.WarehouseId";
                    endHouseColumn = "fci.ReturnExpressCompanyID";
                    accountDate = "trunc(ReturnTime)";
                    break;
                default:
                    break;
            }
            sbStr.AppendFormat(" AND {0}>=to_date('{1}','yyyy-mm-dd hh24:mi:ss') AND {0}<to_date('{2}','yyyy-mm-dd hh24:mi:ss') ", dateColumn, condition.Date_D_S, condition.Date_D_E);
            sbStr.Append(BuildHouseCondition(condition.HouseD, houseColumn, endHouseColumn));

            accountDateColumn = accountDate;

            return sbStr.ToString();
        }


        private DataTable UnitDailyTableNew(Dictionary<string, DataTable> tables, CODSearchCondition condition,ref string TimeMessage)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (tables == null)
                return null;
            
            //配送商分类集合
            ITypeRelationService relationService = ServiceLocator.GetService<ITypeRelationService>();
            List<TypeRelationModel> relation = relationService.SearchRelationList("nk_Express", condition.DistributionCode);
            IExpressCompanyService expressService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dtExpress = expressService.GetFirstLevelSortCenterDt(condition.DistributionCode);

            //行转列
            DataTable dt = CreateTable(condition);
            foreach (KeyValuePair<string, DataTable> k in tables)
            {
                foreach (DataRow dr in k.Value.Rows)
                {
                    if (condition.IsAreaType)
                    {
                        if (dr["FareFormula"] == DBNull.Value || string.IsNullOrEmpty(dr["FareFormula"].ToString()))
                        {
                            dr["FareFormula"] = "无";
                        }
                        if ((dr["AreaType"] == DBNull.Value || string.IsNullOrEmpty(dr["AreaType"].ToString())))
                        {
                            dr["AreaType"] = "-1";
                        }
                    }

                    if (condition.IsCOD)
                    {
                        if ((dr["IsCOD"] == DBNull.Value || string.IsNullOrEmpty(dr["IsCOD"].ToString())))
                        {
                            dr["IsCOD"] = "0";
                        }
                    }

                    DataRow drNew = dt.NewRow();
                    drNew["配送商分类"] = SearchRowType(dr, relation, dtExpress);
                    BuildDataRowNew(ref drNew, dr, condition);
                    dt.Rows.Add(drNew);
                    
                }
            }
            TimeMessage += "合并表格用时:" + sw.ElapsedMilliseconds+"毫秒，";
            sw.Reset();
            DataRow drAll;
            var newdt = GroupByTable( dt, condition,out drAll);

            if (newdt == null || newdt.Rows.Count <= 0)
            {
                throw new Exception("没有查询到数据");
            }
          

            //排序
            DataView dv = newdt.DefaultView;
            if (condition.IsAreaType)
                dv.Sort = "商家,结算单位,分配区域";
            else
                dv.Sort = "商家,结算单位";
            DataTable dtResult = dv.ToTable();

            dtResult.Rows.Add(drAll.ItemArray);//增加合计列

            //去除不需要传输的列
            List<string> columns = new List<string>();
            foreach (DataColumn dc in dtResult.Columns)
            {
                columns.Add(dc.ColumnName);
            }
            string[] ignoreColumns = { "普通发货费", "普通拒收费", "上门换发费", "上门换拒费", "上门退单费", "签单返回发费", "签单返回拒费" };


            dtResult.Format(columns.ToArray(), ignoreColumns);
            TimeMessage += "分组排序计算用时:" + sw.ElapsedMilliseconds + "毫秒";

            return dtResult;

        }


        private DataTable GroupByTable(DataTable dt, CODSearchCondition condition, out DataRow drAll)
        {

            DataTable newdt = new DataTable();
            newdt = dt.Clone();
            newdt.Clear();

            int allCount_d = 0;
            int allCount_dr = 0;
            int allCount_dv = 0;
            int allCount_dvr = 0;
            int allCount_v = 0;
            int allCount_srd = 0;
            int allCount_srr = 0;
            int allCount = 0;
            decimal allFee = 0;
          

            if (!condition.IsAreaType && !condition.IsCOD)
            {
                 var query = dt.AsEnumerable().GroupBy(p => new
                  {
                      配送商分类 = p["配送商分类"],
                      日期 = p["日期"],
                      商家 = p["商家"],
                      结算单位 = p["结算单位"]
                  }).Select(m => new
                  {
                   m.Key.配送商分类,
                   m.Key.日期,
                   m.Key.商家,
                   m.Key.结算单位,
                普通发货数 = m.Sum(k => Convert.ToInt32(k["普通发货数"].ToString())),
                普通发货费 = m.Sum(k => Convert.ToDecimal(k["普通发货费"].ToString())),
                普通拒收数 = m.Sum(k => Convert.ToInt32(k["普通拒收数"].ToString())),
                普通拒收费 = m.Sum(k => Convert.ToDecimal(k["普通拒收费"].ToString())),
                上门换发数 = m.Sum(k => Convert.ToInt32(k["上门换发数"].ToString())),
                上门换发费 = m.Sum(k => Convert.ToDecimal(k["上门换发费"].ToString())),
                上门换拒数 = m.Sum(k => Convert.ToInt32(k["上门换拒数"].ToString())),
                上门换拒费 = m.Sum(k => Convert.ToDecimal(k["上门换拒费"].ToString())),
                上门退单数 = m.Sum(k => Convert.ToInt32(k["上门退单数"].ToString())),
                上门退单费 = m.Sum(k => Convert.ToDecimal(k["上门退单费"].ToString())),
                签单返回发数 = m.Sum(k => Convert.ToInt32(k["签单返回发数"].ToString())),
                签单返回发费 = m.Sum(k => Convert.ToDecimal(k["签单返回发费"].ToString())),
                签单返回拒数 = m.Sum(k => Convert.ToInt32(k["签单返回拒数"].ToString())),
                签单返回拒费 = m.Sum(k => Convert.ToDecimal(k["签单返回拒费"].ToString())),
                实际支付订单量 = m.Sum(k => Convert.ToInt32(k["实际支付订单量"].ToString())),
                实际支付配送费 = m.Sum(k => Convert.ToDecimal(k["实际支付配送费"].ToString()))

               });
                 foreach (var q in query)
                 {
                     DataRow dr = newdt.NewRow();
                     dr["配送商分类"] = q.配送商分类.ToString();
                     dr["日期"] = q.日期.ToString();
                     dr["商家"] = q.商家.ToString();
                     dr["结算单位"] = q.结算单位.ToString();
                     dr["普通发货数"] = q.普通发货数.ToString();
                     dr["普通发货费"] = q.普通发货费.ToString();
                     dr["普通拒收数"] = q.普通拒收数.ToString();
                     dr["普通拒收费"] = q.普通拒收费.ToString();
                     dr["上门换发数"] = q.上门换发数.ToString();
                     dr["上门换发费"] = q.上门换发费.ToString();
                     dr["上门换拒数"] = q.上门换拒数.ToString();
                     dr["上门换拒费"] = q.上门换拒费.ToString();
                     dr["上门退单数"] = q.上门退单数.ToString();
                     dr["上门退单费"] = q.上门退单费.ToString();
                     dr["签单返回发数"] = q.签单返回发数.ToString();
                     dr["签单返回发费"] = q.签单返回发费.ToString();
                     dr["签单返回拒数"] = q.签单返回拒数.ToString();
                     dr["签单返回拒费"] = q.签单返回拒费.ToString();
                     if (dr["结算单位"].ToString().Contains("宅急送"))//拒收的需要*0.8，且全部相加
                     {

                         if (dr["结算单位"].ToString().Contains("广州代收") || dr["结算单位"].ToString().Contains("北京代收"))
                         {
                             dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                             Convert.ToInt32(dr["上门换发数"])
                                             - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);

                             dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                             Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                         }
                         else
                         {
                             dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["普通拒收数"]) +
                                             Convert.ToInt32(dr["上门换发数"])
                                             + Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);
                             dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + (Convert.ToDecimal(dr["普通拒收费"]) * 0.8M) +
                                             Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                         }

                     }

                     else if (dr["结算单位"].ToString().Contains("邮政速递"))//计算所有发货的
                     {
                         dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) +Convert.ToInt32(dr["上门换发数"])+ Convert.ToInt32(dr["上门退单数"]);
                         dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"])+Convert.ToDecimal(dr["上门换发费"])+ Convert.ToDecimal(dr["上门退单费"]));
                     }
                     else
                     {
                         dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                             Convert.ToInt32(dr["上门换发数"])
                                             - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"])+Convert.ToInt32(dr["签单返回发数"])-Convert.ToInt32(dr["签单返回拒数"]);

                         dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                         Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"])+Convert.ToDecimal(dr["签单返回发费"])-Convert.ToDecimal(dr["签单返回拒费"]));
                     }

                      allCount_d += Convert.ToInt32(dr["普通发货数"]);
                      allCount_dr += Convert.ToInt32(dr["普通拒收数"]);
                      allCount_dv += Convert.ToInt32(dr["上门换发数"]);
                      allCount_dvr += Convert.ToInt32(dr["上门换拒数"]);
                      allCount_v += Convert.ToInt32(dr["上门退单数"]);
                      allCount_srd += Convert.ToInt32(dr["签单返回发数"]);
                      allCount_srr += Convert.ToInt32(dr["签单返回拒数"]);
                      allCount += Convert.ToInt32(dr["实际支付订单量"]);
                      allFee += Convert.ToDecimal(dr["实际支付配送费"]);

                     newdt.Rows.Add(dr);
                  }

              
            }
            else if(condition.IsAreaType && !condition.IsCOD)
            {
               
                var query = dt.AsEnumerable().GroupBy(p => new
                {
                    配送商分类 = p["配送商分类"],
                    日期 = p["日期"],
                    商家 = p["商家"],
                    结算单位 = p["结算单位"],
                    分配区域= p["分配区域"],
                    配送单价 =p["配送单价"]
                }).Select(m => new
                {
                    m.Key.配送商分类,
                    m.Key.日期,
                    m.Key.商家,
                    m.Key.结算单位,
                    m.Key.分配区域,
                    m.Key.配送单价,
                    普通发货数 = m.Sum(k => Convert.ToInt32(k["普通发货数"].ToString())),
                    普通发货费 = m.Sum(k => Convert.ToDecimal(k["普通发货费"].ToString())),
                    普通拒收数 = m.Sum(k => Convert.ToInt32(k["普通拒收数"].ToString())),
                    普通拒收费 = m.Sum(k => Convert.ToDecimal(k["普通拒收费"].ToString())),
                    上门换发数 = m.Sum(k => Convert.ToInt32(k["上门换发数"].ToString())),
                    上门换发费 = m.Sum(k => Convert.ToDecimal(k["上门换发费"].ToString())),
                    上门换拒数 = m.Sum(k => Convert.ToInt32(k["上门换拒数"].ToString())),
                    上门换拒费 = m.Sum(k => Convert.ToDecimal(k["上门换拒费"].ToString())),
                    上门退单数 = m.Sum(k => Convert.ToInt32(k["上门退单数"].ToString())),
                    上门退单费 = m.Sum(k => Convert.ToDecimal(k["上门退单费"].ToString())),
                    签单返回发数 = m.Sum(k => Convert.ToInt32(k["签单返回发数"].ToString())),
                    签单返回发费 = m.Sum(k => Convert.ToDecimal(k["签单返回发费"].ToString())),
                    签单返回拒数 = m.Sum(k => Convert.ToInt32(k["签单返回拒数"].ToString())),
                    签单返回拒费 = m.Sum(k => Convert.ToDecimal(k["签单返回拒费"].ToString())),
                    实际支付订单量 = m.Sum(k => Convert.ToInt32(k["实际支付订单量"].ToString())),
                    实际支付配送费 = m.Sum(k => Convert.ToDecimal(k["实际支付配送费"].ToString()))

                });
                newdt = dt.Clone();
                foreach (var q in query)
                {
                    DataRow dr = newdt.NewRow();
                    dr["配送商分类"] = q.配送商分类.ToString();
                    dr["日期"] = q.日期.ToString();
                    dr["商家"] = q.商家.ToString();
                    dr["结算单位"] = q.结算单位.ToString();
                    dr["普通发货数"] = q.普通发货数.ToString();
                    dr["普通发货费"] = q.普通发货费.ToString();
                    dr["普通拒收数"] = q.普通拒收数.ToString();
                    dr["普通拒收费"] = q.普通拒收费.ToString();
                    dr["上门换发数"] = q.上门换发数.ToString();
                    dr["上门换发费"] = q.上门换发费.ToString();
                    dr["上门换拒数"] = q.上门换拒数.ToString();
                    dr["上门换拒费"] = q.上门换拒费.ToString();
                    dr["上门退单数"] = q.上门退单数.ToString();
                    dr["上门退单费"] = q.上门退单费.ToString();
                    dr["签单返回发数"] = q.签单返回发数.ToString();
                    dr["签单返回发费"] = q.签单返回发费.ToString();
                    dr["签单返回拒数"] = q.签单返回拒数.ToString();
                    dr["签单返回拒费"] = q.签单返回拒费.ToString();
                    dr["分配区域"] = q.分配区域.ToString();
                    dr["配送单价"] = q.配送单价.ToString();
                    if (dr["结算单位"].ToString().Contains("宅急送"))//拒收的需要*0.8，且全部相加
                    {

                        if (dr["结算单位"].ToString().Contains("广州代收") || dr["结算单位"].ToString().Contains("北京代收"))
                        {
                            dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);

                            dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                            Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                        }
                        else
                        {
                            dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            + Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);
                            dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + (Convert.ToDecimal(dr["普通拒收费"]) * 0.8M) +
                                            Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                        }

                    }

                    else if (dr["结算单位"].ToString().Contains("邮政速递"))//计算所有发货的
                    {
                        dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["上门换发数"]) + Convert.ToInt32(dr["上门退单数"]);
                        dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门退单费"]));
                    }
                    else
                    {
                        dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]) + Convert.ToInt32(dr["签单返回发数"]) - Convert.ToInt32(dr["签单返回拒数"]);

                        dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                        Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]) + Convert.ToDecimal(dr["签单返回发费"]) - Convert.ToDecimal(dr["签单返回拒费"]));
                    }


                    allCount_d += Convert.ToInt32(dr["普通发货数"]);
                    allCount_dr += Convert.ToInt32(dr["普通拒收数"]);
                    allCount_dv += Convert.ToInt32(dr["上门换发数"]);
                    allCount_dvr += Convert.ToInt32(dr["上门换拒数"]);
                    allCount_v += Convert.ToInt32(dr["上门退单数"]);
                    allCount_srd += Convert.ToInt32(dr["普通发货数"]);
                    allCount_srr += Convert.ToInt32(dr["普通发货数"]);
                    allCount += Convert.ToInt32(dr["实际支付订单量"]);
                    allFee += Convert.ToDecimal(dr["实际支付配送费"]);
                    newdt.Rows.Add(dr);
                }

              

            }
            if (condition.IsCOD && !condition.IsAreaType)
            {
               
                              
                var query = dt.AsEnumerable().GroupBy(p => new
                {
                    配送商分类 = p["配送商分类"],
                    日期 = p["日期"],
                    商家 = p["商家"],
                    结算单位 = p["结算单位"],
                    业务类型 = p["业务类型"]
                }).Select(m => new
                {
                    m.Key.配送商分类,
                    m.Key.日期,
                    m.Key.商家,
                    m.Key.结算单位,
                    m.Key.业务类型,
                    普通发货数 = m.Sum(k => Convert.ToInt32(k["普通发货数"].ToString())),
                    普通发货费 = m.Sum(k => Convert.ToDecimal(k["普通发货费"].ToString())),
                    普通拒收数 = m.Sum(k => Convert.ToInt32(k["普通拒收数"].ToString())),
                    普通拒收费 = m.Sum(k => Convert.ToDecimal(k["普通拒收费"].ToString())),
                    上门换发数 = m.Sum(k => Convert.ToInt32(k["上门换发数"].ToString())),
                    上门换发费 = m.Sum(k => Convert.ToDecimal(k["上门换发费"].ToString())),
                    上门换拒数 = m.Sum(k => Convert.ToInt32(k["上门换拒数"].ToString())),
                    上门换拒费 = m.Sum(k => Convert.ToDecimal(k["上门换拒费"].ToString())),
                    上门退单数 = m.Sum(k => Convert.ToInt32(k["上门退单数"].ToString())),
                    上门退单费 = m.Sum(k => Convert.ToDecimal(k["上门退单费"].ToString())),
                    签单返回发数 = m.Sum(k => Convert.ToInt32(k["签单返回发数"].ToString())),
                    签单返回发费 = m.Sum(k => Convert.ToDecimal(k["签单返回发费"].ToString())),
                    签单返回拒数 = m.Sum(k => Convert.ToInt32(k["签单返回拒数"].ToString())),
                    签单返回拒费 = m.Sum(k => Convert.ToDecimal(k["签单返回拒费"].ToString())),
                    实际支付订单量 = m.Sum(k => Convert.ToInt32(k["实际支付订单量"].ToString())),
                    实际支付配送费 = m.Sum(k => Convert.ToDecimal(k["实际支付配送费"].ToString()))

                });
              
                foreach (var q in query)
                {
                    DataRow dr = newdt.NewRow();
                    dr["配送商分类"] = q.配送商分类.ToString();
                    dr["日期"] = q.日期.ToString();
                    dr["商家"] = q.商家.ToString();
                    dr["结算单位"] = q.结算单位.ToString();
                    dr["普通发货数"] = q.普通发货数.ToString();
                    dr["普通发货费"] = q.普通发货费.ToString();
                    dr["普通拒收数"] = q.普通拒收数.ToString();
                    dr["普通拒收费"] = q.普通拒收费.ToString();
                    dr["上门换发数"] = q.上门换发数.ToString();
                    dr["上门换发费"] = q.上门换发费.ToString();
                    dr["上门换拒数"] = q.上门换拒数.ToString();
                    dr["上门换拒费"] = q.上门换拒费.ToString();
                    dr["上门退单数"] = q.上门退单数.ToString();
                    dr["上门退单费"] = q.上门退单费.ToString();
                    dr["签单返回发数"] = q.签单返回发数.ToString();
                    dr["签单返回发费"] = q.签单返回发费.ToString();
                    dr["签单返回拒数"] = q.签单返回拒数.ToString();
                    dr["签单返回拒费"] = q.签单返回拒费.ToString();
                    if (dr["结算单位"].ToString().Contains("宅急送"))//拒收的需要*0.8，且全部相加
                    {

                        if (dr["结算单位"].ToString().Contains("广州代收") || dr["结算单位"].ToString().Contains("北京代收"))
                        {
                            dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);

                            dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                            Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                        }
                        else
                        {
                            dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            + Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);
                            dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + (Convert.ToDecimal(dr["普通拒收费"]) * 0.8M) +
                                            Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                        }

                    }

                    else if (dr["结算单位"].ToString().Contains("邮政速递"))//计算所有发货的
                    {
                        dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["上门换发数"]) + Convert.ToInt32(dr["上门退单数"]);
                        dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门退单费"]));
                    }
                    else
                    {
                        dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]) + Convert.ToInt32(dr["签单返回发数"]) - Convert.ToInt32(dr["签单返回拒数"]);

                        dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                        Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]) + Convert.ToDecimal(dr["签单返回发费"]) - Convert.ToDecimal(dr["签单返回拒费"]));
                    }
                    dr["业务类型"] = q.业务类型.ToString();

                    allCount_d += Convert.ToInt32(dr["普通发货数"]);
                    allCount_dr += Convert.ToInt32(dr["普通拒收数"]);
                    allCount_dv += Convert.ToInt32(dr["上门换发数"]);
                    allCount_dvr += Convert.ToInt32(dr["上门换拒数"]);
                    allCount_v += Convert.ToInt32(dr["上门退单数"]);
                    allCount_srd += Convert.ToInt32(dr["普通发货数"]);
                    allCount_srr += Convert.ToInt32(dr["普通发货数"]);
                    allCount += Convert.ToInt32(dr["实际支付订单量"]);
                    allFee += Convert.ToDecimal(dr["实际支付配送费"]);
                    newdt.Rows.Add(dr);
                }

                
            }
            else if(condition.IsAreaType && condition.IsCOD)
            {
                var query = dt.AsEnumerable().GroupBy(p => new
                {
                    配送商分类 = p["配送商分类"],
                    日期 = p["日期"],
                    商家 = p["商家"],
                    结算单位 = p["结算单位"],
                    分配区域 = p["分配区域"],
                    配送单价 = p["配送单价"],
                    业务类型 = p["业务类型"]
                }).Select(m => new
                {
                    m.Key.配送商分类,
                    m.Key.日期,
                    m.Key.商家,
                    m.Key.结算单位,
                    m.Key.分配区域,
                    m.Key.配送单价,
                    m.Key.业务类型,
                    普通发货数 = m.Sum(k => Convert.ToInt32(k["普通发货数"].ToString())),
                    普通发货费 = m.Sum(k => Convert.ToDecimal(k["普通发货费"].ToString())),
                    普通拒收数 = m.Sum(k => Convert.ToInt32(k["普通拒收数"].ToString())),
                    普通拒收费 = m.Sum(k => Convert.ToDecimal(k["普通拒收费"].ToString())),
                    上门换发数 = m.Sum(k => Convert.ToInt32(k["上门换发数"].ToString())),
                    上门换发费 = m.Sum(k => Convert.ToDecimal(k["上门换发费"].ToString())),
                    上门换拒数 = m.Sum(k => Convert.ToInt32(k["上门换拒数"].ToString())),
                    上门换拒费 = m.Sum(k => Convert.ToDecimal(k["上门换拒费"].ToString())),
                    上门退单数 = m.Sum(k => Convert.ToInt32(k["上门退单数"].ToString())),
                    上门退单费 = m.Sum(k => Convert.ToDecimal(k["上门退单费"].ToString())),
                    签单返回发数 = m.Sum(k => Convert.ToInt32(k["签单返回发数"].ToString())),
                    签单返回发费 = m.Sum(k => Convert.ToDecimal(k["签单返回发费"].ToString())),
                    签单返回拒数 = m.Sum(k => Convert.ToInt32(k["签单返回拒数"].ToString())),
                    签单返回拒费 = m.Sum(k => Convert.ToDecimal(k["签单返回拒费"].ToString())),
                    实际支付订单量 = m.Sum(k => Convert.ToInt32(k["实际支付订单量"].ToString())),
                    实际支付配送费 = m.Sum(k => Convert.ToDecimal(k["实际支付配送费"].ToString()))

                });
                
                foreach (var q in query)
                {
                    DataRow dr = newdt.NewRow();
                    dr["配送商分类"] = q.配送商分类.ToString();
                    dr["日期"] = q.日期.ToString();
                    dr["商家"] = q.商家.ToString();
                    dr["结算单位"] = q.结算单位.ToString();
                    dr["普通发货数"] = q.普通发货数.ToString();
                    dr["普通发货费"] = q.普通发货费.ToString();
                    dr["普通拒收数"] = q.普通拒收数.ToString();
                    dr["普通拒收费"] = q.普通拒收费.ToString();
                    dr["上门换发数"] = q.上门换发数.ToString();
                    dr["上门换发费"] = q.上门换发费.ToString();
                    dr["上门换拒数"] = q.上门换拒数.ToString();
                    dr["上门换拒费"] = q.上门换拒费.ToString();
                    dr["上门退单数"] = q.上门退单数.ToString();
                    dr["上门退单费"] = q.上门退单费.ToString();
                    dr["签单返回发数"] = q.签单返回发数.ToString();
                    dr["签单返回发费"] = q.签单返回发费.ToString();
                    dr["签单返回拒数"] = q.签单返回拒数.ToString();
                    dr["签单返回拒费"] = q.签单返回拒费.ToString();
                    dr["实际支付订单量"] = q.实际支付订单量.ToString();
                    dr["实际支付配送费"] = q.实际支付配送费.ToString();
                    dr["分配区域"] = q.分配区域.ToString();
                    dr["配送单价"] = q.配送单价.ToString();
                    dr["业务类型"] = q.业务类型.ToString();
                    if (dr["结算单位"].ToString().Contains("宅急送"))//拒收的需要*0.8，且全部相加
                    {

                        if (dr["结算单位"].ToString().Contains("广州代收") || dr["结算单位"].ToString().Contains("北京代收"))
                        {
                            dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);

                            dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - (Convert.ToDecimal(dr["普通拒收费"]) * 0.8M) +
                                            Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                        }
                        else
                        {
                            dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            + Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]);
                            dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + (Convert.ToDecimal(dr["普通拒收费"]) * 0.8M) +
                                            Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]));
                        }

                    }

                    else if (dr["结算单位"].ToString().Contains("邮政速递"))//计算所有发货的
                    {
                        dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) + Convert.ToInt32(dr["上门换发数"]) + Convert.ToInt32(dr["上门退单数"]);
                        dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) + Convert.ToDecimal(dr["上门换发费"]) + Convert.ToDecimal(dr["上门退单费"]));
                    }
                    else
                    {
                        dr["实际支付订单量"] = Convert.ToInt32(dr["普通发货数"]) - Convert.ToInt32(dr["普通拒收数"]) +
                                            Convert.ToInt32(dr["上门换发数"])
                                            - Convert.ToInt32(dr["上门换拒数"]) + Convert.ToInt32(dr["上门退单数"]) + Convert.ToInt32(dr["签单返回发数"]) - Convert.ToInt32(dr["签单返回拒数"]);

                        dr["实际支付配送费"] = Convert.ToDecimal(Convert.ToDecimal(dr["普通发货费"]) - Convert.ToDecimal(dr["普通拒收费"]) +
                                        Convert.ToDecimal(dr["上门换发费"]) - Convert.ToDecimal(dr["上门换拒费"]) + Convert.ToDecimal(dr["上门退单费"]) + Convert.ToDecimal(dr["签单返回发费"]) - Convert.ToDecimal(dr["签单返回拒费"]));
                    }
                    allCount_d += Convert.ToInt32(dr["普通发货数"]);
                    allCount_dr += Convert.ToInt32(dr["普通拒收数"]);
                    allCount_dv += Convert.ToInt32(dr["上门换发数"]);
                    allCount_dvr += Convert.ToInt32(dr["上门换拒数"]);
                    allCount_v += Convert.ToInt32(dr["上门退单数"]);
                    allCount_srd += Convert.ToInt32(dr["普通发货数"]);
                    allCount_srr += Convert.ToInt32(dr["普通发货数"]);
                    allCount += Convert.ToInt32(dr["实际支付订单量"]);
                    allFee += Convert.ToDecimal(dr["实际支付配送费"]);
                    newdt.Rows.Add(dr);
                }

              
            }

            drAll = newdt.NewRow();
            drAll["结算单位"] = "合计";
            drAll["普通发货数"] = allCount_d;
            drAll["普通拒收数"] = allCount_dr;
            drAll["上门换发数"] = allCount_dv;
            drAll["上门换拒数"] = allCount_dvr;
            drAll["上门退单数"] = allCount_v;
            drAll["签单返回发数"] = allCount_srd;
            drAll["签单返回拒数"] = allCount_srr;
            drAll["实际支付订单量"] = allCount;
            drAll["实际支付配送费"] = allFee;

            return newdt;


        }

        private DataTable UniteDailyTable(Dictionary<string, DataTable> tables, CODSearchCondition condition)
        {
            if (tables == null)
                return null;

            //配送商分类集合
            ITypeRelationService relationService = ServiceLocator.GetService<ITypeRelationService>();
            List<TypeRelationModel> relation = relationService.SearchRelationList("nk_Express", condition.DistributionCode);
            IExpressCompanyService expressService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable dtExpress = expressService.GetFirstLevelSortCenterDt(condition.DistributionCode);

            //行转列
            DataTable dt = CreateTable(condition);
            //int n = 1;//序号
            int m = 0;
            foreach (KeyValuePair<string, DataTable> k in tables)
            {
                int j = 0;
                foreach (DataRow dr in k.Value.Rows)
                {
                    j++;
                    //dr["RowNums"] = n;
                    if (condition.IsAreaType)
                    {
                        if (dr["FareFormula"] == DBNull.Value || string.IsNullOrEmpty(dr["FareFormula"].ToString()))
                        {
                            dr["FareFormula"] = "无";
                        }
                        if ((dr["AreaType"] == DBNull.Value || string.IsNullOrEmpty(dr["AreaType"].ToString())))
                        {
                            dr["AreaType"] = "-1";
                        }
                    }

                    if (condition.IsCOD)
                    {
                        if ((dr["IsCOD"] == DBNull.Value || string.IsNullOrEmpty(dr["IsCOD"].ToString())))
                        {
                            dr["IsCOD"] = "0";
                        }
                    }
                       
                    if (m == 0)
                    {
                        //第一次不用判断
                        DataRow drNew = dt.NewRow();
                        drNew["配送商分类"] = SearchRowType(dr, relation, dtExpress);
                        BuildDataRow(ref drNew, dr, condition);
                        dt.Rows.Add(drNew);
                    }
                    else
                    {
                        #region 累加计算
                        List<DataRow> drs =
                            dt.AsEnumerable().AsParallel().Where(row =>
                            {
                                var flag = Convert.ToDateTime(row["日期"]).ToString("yyyy-MM-dd") == Convert.ToDateTime(dr["AccountDate"]).ToString("yyyy-MM-dd") &&
                                    row["商家"].ToString() == dr["MerchantName"].ToString() &&
                                    row["结算单位"].ToString() == dr["AccountCompanyName"].ToString();

                                if (condition.IsAreaType)
                                {
                                    flag = flag && row["分配区域"].ToString() == dr["AreaType"].ToString() && row["配送单价"].ToString() == dr["FareFormula"].ToString();
                                }

                                if (condition.IsCOD)
                                {
                                    flag = flag && row["业务类型"].ToString() == (dr["IsCOD"] == DBNull.Value ? "否" : int.Parse(dr["IsCOD"].ToString()) == 0 ? "否" : "是").ToString();
                                }
                                return flag;
                            }).ToList();
                        if (drs.Count == 1)
                        {
                            DataRow dr1 = drs[0];
                            //更新列
                            UpdateDataRow(ref dr1, dr);
                        }
                        else
                        {
                            //新增列
                            DataRow drNew = dt.NewRow();
                            drNew["配送商分类"] = SearchRowType(dr, relation, dtExpress);
                            BuildDataRow(ref drNew, dr, condition);
                            dt.Rows.Add(drNew);
                            //n++;
                        }
                        #endregion
                    }
                }
                m++;
            }
            if (dt == null || dt.Rows.Count <= 0)
            {
                throw new Exception("没有查询到数据");
            }
            //合并金额
            DataRow drAll;
            AccountDaily(ref dt,out drAll);

            //排序
            DataView dv = dt.DefaultView;
            if(condition.IsAreaType)
                dv.Sort = "商家,结算单位,分配区域";
            else
                dv.Sort = "商家,结算单位";
            DataTable dtResult = dv.ToTable();

            dtResult.Rows.Add(drAll.ItemArray);//增加合计列

            //去除不需要传输的列
            List<string> columns = new List<string>();
            foreach (DataColumn dc in dtResult.Columns)
            {
                columns.Add(dc.ColumnName);
            }
            string[] ignoreColumns = { "普通发货费", "普通拒收费", "上门换发费", "上门换拒费", "上门退单费","签单返回发费","签单返回拒费" };


            dtResult.Format(columns.ToArray(), ignoreColumns);

            return dtResult;
        }

        private DataTable CreateTable(CODSearchCondition condition)
        {
            DataTable dt = new DataTable();

            //dt.Columns.Add("序号", typeof(int));
            dt.Columns.Add("配送商分类", typeof(string));
            dt.Columns.Add("日期", typeof(string));
            dt.Columns.Add("商家", typeof(string));
            dt.Columns.Add("结算单位", typeof(string));
            if (condition.IsAreaType)
            {
                dt.Columns.Add("分配区域", typeof(int));
                dt.Columns.Add("配送单价", typeof(string));
            }
            if (condition.IsCOD)
            {
                dt.Columns.Add("业务类型", typeof(string));
            }
            dt.Columns.Add("普通发货数", typeof(int));
            dt.Columns.Add("普通拒收数", typeof(int));
            dt.Columns.Add("上门换发数", typeof(int));
            dt.Columns.Add("上门换拒数", typeof(int));
            dt.Columns.Add("上门退单数", typeof(int));
            dt.Columns.Add("签单返回发数", typeof(int));
            dt.Columns.Add("签单返回拒数", typeof(int));
            dt.Columns.Add("实际支付订单量", typeof(int));
            
            dt.Columns.Add("实际支付配送费", typeof(decimal));

            dt.Columns.Add("普通发货费", typeof(decimal));
            dt.Columns.Add("普通拒收费", typeof(decimal));
            dt.Columns.Add("上门换发费", typeof(decimal));
            dt.Columns.Add("上门换拒费", typeof(decimal));
            dt.Columns.Add("上门退单费", typeof(decimal));
            dt.Columns.Add("签单返回发费", typeof(decimal));
            dt.Columns.Add("签单返回拒费", typeof(decimal));

            return dt;
        }

        private void BuildDataRow(ref DataRow dr, DataRow drData, CODSearchCondition condition)
        {
            //dr["序号"]=drData["RowNums"];
            //if (drData["AccountDate"]!=DBNull.Value)
                dr["日期"] = Convert.ToDateTime(drData["AccountDate"]).ToString("yyyy-MM-dd");
            dr["商家"] = drData["MerchantName"];
            dr["结算单位"] = drData["AccountCompanyName"];
            if (condition.IsAreaType)
            {
                dr["分配区域"] = drData["AreaType"];
                dr["配送单价"] = drData["FareFormula"];
            }
            if (condition.IsCOD)
            {
                dr["业务类型"] = drData["IsCOD"]==DBNull.Value?"否":int.Parse(drData["IsCOD"].ToString())==0?"否":"是";
            }
            UpdateDataRow(ref dr, drData);

            dr["实际支付订单量"] = 0;
            dr["实际支付配送费"] = 0;
        }

        private void BuildDataRowNew(ref DataRow dr, DataRow drData, CODSearchCondition condition)
        {
            //dr["序号"]=drData["RowNums"];
            //if (drData["AccountDate"]!=DBNull.Value)
            dr["日期"] = Convert.ToDateTime(drData["AccountDate"]).ToString("yyyy-MM-dd");
            dr["商家"] = drData["MerchantName"];
            dr["结算单位"] = drData["AccountCompanyName"];
            if (condition.IsAreaType)
            {
                dr["分配区域"] = drData["AreaType"];
                dr["配送单价"] = drData["FareFormula"];
            }
            if (condition.IsCOD)
            {
                dr["业务类型"] = drData["IsCOD"] == DBNull.Value ? "否" : int.Parse(drData["IsCOD"].ToString()) == 0 ? "否" : "是";
            }
            UpdateDataRowNew(ref dr, drData);

            dr["实际支付订单量"] = 0;
            dr["实际支付配送费"] = 0;
        }

        private string SearchRowType(DataRow dr, List<TypeRelationModel> relation, DataTable dtExpress)
        {
            int statid = int.Parse(dr["ExpressCompanyID"].ToString());
            var distinctTypeList = (from t in relation
                                    where t.RelationId == statid
                                    select t).ToList();
            if (distinctTypeList == null || distinctTypeList.Count <= 0)
            {
                ////如风达站点显示站点上级分拣中心
                List<DataRow> drs = dtExpress.AsEnumerable().Where(row =>
                {
                    return row["ExpressCompanyID"].ToString() == statid.ToString();
                }).ToList();
                if (drs.Count > 0)
                {
                    return drs[0]["SortName"].ToString();
                }
                else
                    return "其他";
            }
            else
                return (string.IsNullOrEmpty(distinctTypeList[0].CodeDesc) ? "其他" : distinctTypeList[0].CodeDesc);
        }

        private void UpdateDataRow(ref DataRow dr, DataRow drData)
        {
            switch (drData["CountType"].ToString())
            {
                case "D":
                    dr["普通发货数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["普通发货费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];//string.Format("{0:n2} ", drData["CountFare"]);
                    break;
                case "DR":
                    dr["普通拒收数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["普通拒收费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "DV":
                    dr["上门换发数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["上门换发费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "DVR":
                    dr["上门换拒数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["上门换拒费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "V":
                    dr["上门退单数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["上门退单费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "SRD":
                    dr["签单返回发数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["签单返回发费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "SRR":
                    dr["签单返回拒数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["签单返回拒费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                default:
                    break;
            }
        }

        private void UpdateDataRowNew(ref DataRow dr, DataRow drData)
        {
            dr["普通发货数"] = "0";
            dr["普通发货费"] = "0";
            dr["普通拒收数"] = "0";
            dr["普通拒收费"] = "0";
            dr["上门换发数"] = "0";
            dr["上门换发费"] = "0";
            dr["上门换拒数"] = "0";
            dr["上门换拒费"] = "0";
            dr["上门退单数"] = "0";
            dr["上门退单费"] = "0";
            dr["签单返回发数"] = "0";
            dr["签单返回发费"] = "0";
            dr["签单返回拒数"] = "0";
            dr["签单返回拒费"] = "0";
            switch (drData["CountType"].ToString())
            {
                case "D":
                    dr["普通发货数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["普通发货费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];//string.Format("{0:n2} ", drData["CountFare"]);
                    break;
                case "DR":
                    dr["普通拒收数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["普通拒收费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "DV":
                    dr["上门换发数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["上门换发费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "DVR":
                    dr["上门换拒数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["上门换拒费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "V":
                    dr["上门退单数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["上门退单费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "SRD":
                    dr["签单返回发数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["签单返回发费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                case "SRR":
                    dr["签单返回拒数"] = drData["CountNum"] == DBNull.Value ? "0" : drData["CountNum"];
                    dr["签单返回拒费"] = drData["CountFare"] == DBNull.Value ? "0" : drData["CountFare"];
                    break;
                default:
                    break;
            }
        }

        private void AccountDaily(ref DataTable dt,out DataRow drAll)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                drAll = null;
                return;
            }

            int allCount_d = 0;
            int allCount_dr = 0;
            int allCount_dv = 0;
            int allCount_dvr = 0;
            int allCount_v = 0;
            int allCount_srd = 0;
            int allCount_srr = 0;
            int allCount = 0;
            decimal allFee = 0;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["普通发货数"] == DBNull.Value)
                    dr["普通发货数"] = 0;

                if (dr["普通拒收数"] == DBNull.Value)
                    dr["普通拒收数"] = 0;

                if (dr["上门换发数"] == DBNull.Value)
                    dr["上门换发数"] = 0;

                if (dr["上门换拒数"] == DBNull.Value)
                    dr["上门换拒数"] = 0;

                if (dr["上门退单数"] == DBNull.Value)
                    dr["上门退单数"] = 0;

                if (dr["签单返回发数"] == DBNull.Value)
                    dr["签单返回发数"] = 0;

                if (dr["签单返回拒数"] == DBNull.Value)
                    dr["签单返回拒数"] = 0;

                if (dr["普通发货费"] == DBNull.Value)
                    dr["普通发货费"] = 0;

                if (dr["普通拒收费"] == DBNull.Value)
                    dr["普通拒收费"] = 0;

                if (dr["上门换发费"] == DBNull.Value)
                    dr["上门换发费"] = 0;

                if (dr["上门换拒费"] == DBNull.Value)
                    dr["上门换拒费"] = 0;

                if (dr["上门退单费"] == DBNull.Value)
                    dr["上门退单费"] = 0;

                if (dr["签单返回发费"] == DBNull.Value)
                    dr["签单返回发费"] = 0;

                if (dr["签单返回拒费"] == DBNull.Value)
                    dr["签单返回拒费"] = 0;

                int count_d=int.Parse(dr["普通发货数"].ToString());
                int count_dr = int.Parse(dr["普通拒收数"].ToString());
                int count_dv = int.Parse(dr["上门换发数"].ToString());
                int count_dvr = int.Parse(dr["上门换拒数"].ToString());
                int count_v = int.Parse(dr["上门退单数"].ToString());
                int count_srd = int.Parse(dr["签单返回发数"].ToString());
                int count_srr = int.Parse(dr["签单返回拒数"].ToString());

                decimal fare_d = dr["普通发货费"] == DBNull.Value ? 0 : decimal.Parse(dr["普通发货费"].ToString());
                decimal fare_dr = dr["普通拒收费"] == DBNull.Value ? 0 : decimal.Parse(dr["普通拒收费"].ToString());
                decimal fare_dv = dr["上门换发费"] == DBNull.Value ? 0 : decimal.Parse(dr["上门换发费"].ToString());
                decimal fare_dvr = dr["上门换拒费"] == DBNull.Value ? 0 : decimal.Parse(dr["上门换拒费"].ToString());
                decimal fare_v = dr["上门退单费"] == DBNull.Value ? 0 : decimal.Parse(dr["上门退单费"].ToString());
                decimal fare_srd = dr["签单返回发费"] == DBNull.Value ? 0 : decimal.Parse(dr["签单返回发费"].ToString());
                decimal fare_srr = dr["签单返回拒费"] == DBNull.Value ? 0 : decimal.Parse(dr["签单返回拒费"].ToString());

                if (dr["结算单位"].ToString().Contains("宅急送"))//拒收的需要*0.8，且全部相加
                {

                    if (dr["结算单位"].ToString().Contains("广州代收") || dr["结算单位"].ToString().Contains("北京代收"))
                    {
                        dr["实际支付订单量"] = count_d - count_dr + count_dv - count_dvr + count_v;
                        dr["实际支付配送费"] = Convert.ToDecimal(fare_d - (fare_dr) + fare_dv - (fare_dvr) + fare_v );
                    }
                    else
                    {
                        dr["实际支付订单量"] = count_d + count_dr + count_dv + count_dvr + count_v;
                        dr["实际支付配送费"] = Convert.ToDecimal(fare_d + (fare_dr * 0.8M) + fare_dv + (fare_dvr * 0.8M) + fare_v);
                    }
                    
                }
             
                else if (dr["结算单位"].ToString().Contains("邮政速递"))//计算所有发货的
                {
                    dr["实际支付订单量"] = count_d  + count_dv  + count_v;
                    dr["实际支付配送费"] = Convert.ToDecimal(fare_d  + fare_dv  + fare_v);
                }
                else
                {
                    dr["实际支付订单量"] = count_d + count_srd - count_dr + count_dv - count_dvr + count_v - count_srr;
                    dr["实际支付配送费"] = Convert.ToDecimal(fare_d + fare_srd - fare_dr + fare_dv - fare_dvr + fare_v - fare_srr);
                }

                allCount_d += count_d;
                allCount_dr += count_dr;
                allCount_dv += count_dv;
                allCount_dvr += count_dvr;
                allCount_v += count_v;
                allCount_srr += count_srr;
                allCount_srd += count_srd;
                allCount += Convert.ToInt32(dr["实际支付订单量"]);
                allFee += Convert.ToDecimal(dr["实际支付配送费"]);
            }

            drAll = dt.NewRow();
            drAll["结算单位"] = "合计";
            drAll["普通发货数"] = allCount_d;
            drAll["普通拒收数"] = allCount_dr;
            drAll["上门换发数"] = allCount_dv;
            drAll["上门换拒数"] = allCount_dvr;
            drAll["上门退单数"] = allCount_v;
            drAll["签单返回发数"] = allCount_srd;
            drAll["签单返回拒数"] = allCount_srr;
            drAll["实际支付订单量"] = allCount;
            drAll["实际支付配送费"] = allFee;
        }

        
    }
}
