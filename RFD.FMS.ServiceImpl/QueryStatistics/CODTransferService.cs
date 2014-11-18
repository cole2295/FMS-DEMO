using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.QueryStatistics;
using System.Configuration;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.QueryStatistics;

namespace RFD.FMS.ServiceImpl.QueryStatistics
{
    public class CODTransferService : ICODTransferService
    {
        private static readonly string FinancialCutDate = ConfigurationManager.AppSettings["FinancialCutDate"] == null ? null : ConfigurationManager.AppSettings["FinancialCutDate"];

        private ICODTransferDao _cODTransferDao;

        public CODTransferService()
        {
            
        }

        #region ICODTransfer 成员

        public System.Data.DataTable SearchCodDetailsV2(MODEL.CodSearchCondition condition, ref MODEL.PageInfo pi, out System.Data.DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);

            if (string.IsNullOrEmpty(searchCondition))
            {
                amount = null;
                return null;
            }

            amount = _cODTransferDao.StatCodV2(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0)
                return null;

            pi.ItemCount = int.Parse(amount.Rows[0]["订单量合计"].ToString());
            pi.PageCount = Convert.ToInt32(Math.Ceiling(pi.ItemCount * 1.0 / pi.PageSize));

            return _cODTransferDao.SearchCodDetailsV2(searchCondition, ref pi, condition);
        }

        public System.Data.DataTable SearchCodDetails(MODEL.CodSearchCondition condition, ref MODEL.PageInfo pi, out System.Data.DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);

            if (string.IsNullOrEmpty(searchCondition))
            {
                amount = null;
                return null;
            }

            amount = _cODTransferDao.StatCod(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0]["订单量合计"].ToString()) <= 0)
                return null;

            pi.ItemCount = int.Parse(amount.Rows[0]["订单量合计"].ToString());
            return _cODTransferDao.SearchCodDetails(searchCondition, ref pi, condition);
        }

        public System.Data.DataTable SearchCodStatV2(MODEL.CodSearchCondition condition, out System.Data.DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);

            if (string.IsNullOrEmpty(searchCondition))
            {
                amount = null;

                return null;
            }

            amount = _cODTransferDao.StatCodV2(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0) return null;

            return _cODTransferDao.SearchCodStatV2(searchCondition, condition);
        }

        public System.Data.DataTable SearchCodStat(MODEL.CodSearchCondition condition, out System.Data.DataTable amount)
        {
            string searchCondition = BuildSearchCondition(condition);
            if (string.IsNullOrEmpty(searchCondition))
            {
                amount = null;
                return null;
            }
            amount = _cODTransferDao.StatCod(searchCondition, condition);

            if (amount == null || amount.Rows.Count <= 0)
                return null;

            return _cODTransferDao.SearchCodStat(searchCondition, condition);
        }

        public System.Data.DataTable SearchExprotDetailDataV2(MODEL.CodSearchCondition condition)
        {
            string searchCondition = BuildSearchCondition(condition);

            if (string.IsNullOrEmpty(searchCondition)) return null;
            
            return _cODTransferDao.SearchExprotDetailDataV2(searchCondition, condition);
        }

        public System.Data.DataTable SearchExprotDetailData(MODEL.CodSearchCondition condition)
        {
            string searchCondition = BuildSearchCondition(condition);
            if (string.IsNullOrEmpty(searchCondition))
            {
                return null;
            }
            return _cODTransferDao.SearchExprotDetailData(searchCondition, condition);
        }

        public System.Data.DataTable SearchExprotStatDataV2(MODEL.CodSearchCondition condition)
        {
            string searchCondition = BuildSearchCondition(condition);

            if (string.IsNullOrEmpty(searchCondition)) return null;
            
            return _cODTransferDao.SearchExprotStatDataV2(searchCondition, condition);
        }

        public System.Data.DataTable SearchExprotStatData(MODEL.CodSearchCondition condition)
        {
            string searchCondition = BuildSearchCondition(condition);
            if (string.IsNullOrEmpty(searchCondition))
            {
                return null;
            }
            return _cODTransferDao.SearchExprotStatData(searchCondition, condition);
        }

        #endregion

        private string BuildSearchCondition(CodSearchCondition condition)
        {
            StringBuilder sbCondition = new StringBuilder();

            sbCondition.Append(" AND fcbi.IsDeleted=0 ");

            if (!string.IsNullOrEmpty(condition.WaybillNO))
                sbCondition.AppendFormat(" AND fcbi.WaybillNO = {0} ", condition.WaybillNO);

            if (string.IsNullOrEmpty(condition.WaybillNO))
            {
                if (!string.IsNullOrEmpty(condition.ExpressCompanyID))
                    sbCondition.AppendFormat(" AND fcbi.TopCODCompanyID IN ({0}) ", condition.ExpressCompanyID);

                if (!string.IsNullOrEmpty(condition.Sources))
                    sbCondition.AppendFormat(" AND fcbi.MerchantID IN ({0}) ", condition.Sources);
            }
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
                    sbCondition.AppendFormat(" AND fcbi.WaybillType ={0}  ", condition.ShipmentType);

                dateColumn = condition.DateType == "0" ? "fcbi.RfdAcceptTime" : "fcbi.DeliverTime";
                houseColumn = condition.HouseType == "0" ? "fcbi.WarehouseId" : "fcbi.FinalExpressCompanyID";
                endHouseColumn = condition.HouseType == "0" ? "fcbi.ExpressCompanyID" : "fcbi.FinalExpressCompanyID";

                //初始末级切换时间
                if (condition.DateType == "1" && string.IsNullOrEmpty(condition.WaybillNO))
                {
                    if (string.IsNullOrEmpty(FinancialCutDate))
                        throw new Exception("初始最终切换时间点未找到");

                    sbCondition.AppendFormat(" AND fcbi.RfdAcceptTime >=to_date('{0}','yyyy-mm-dd hh24:mi:ss')  ", FinancialCutDate);
                }
            }

            //上门退
            if (condition.ReportType == "6")
            {
                sbCondition.Append(" AND fcbi.Flag=1 ");
                if (string.IsNullOrEmpty(condition.ShipmentType))
                    sbCondition.Append(" AND fcbi.WaybillType IN ('1','2') ");
                else
                    sbCondition.AppendFormat(" AND fcbi.WaybillType ={0}  ", condition.ShipmentType);

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
                    sbCondition.AppendFormat(" AND fcbi.WaybillType ={0}  ", condition.ShipmentType);

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
                if (s.Contains("s"))
                    sbS.Append(s.Replace("s", "") + ",");
                else
                    sbE.Append(s + ",");
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
    }
}
