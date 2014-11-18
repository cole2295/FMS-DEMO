using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.DAL.Oracle.BasicSetting;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using System.Data.SqlClient;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class SortingTransferDetailDao : OracleDao, ISortingTransferDetailDao
    {
        public int Add(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into FMS_SortingTransferDetail(");
            strSql.Append("DetailKid,");
            strSql.Append("MerchantID,");
            strSql.Append("WaybillNo,");
            strSql.Append("WaybillType,");
            strSql.Append("SortingCenterID,");
            strSql.Append("TSortingCenterID,");
            strSql.Append("ReturnSortingCenterID,");
            strSql.Append("SoringMerchantID,");
            strSql.Append("CreateCityID,");
            strSql.Append("SortCityID,");
            strSql.Append("DeliverStationID,");
            strSql.Append("TopCODCompanyID,");
            strSql.Append("ToStationTime,");
            strSql.Append("OutBoundTime,");
            strSql.Append("ReturnTime,");
            strSql.Append("InSortingTime,");
            strSql.Append("DistributionCode,");
            strSql.Append("IsAccount,");
            strSql.Append("AccountFare,");
            strSql.Append("AccountFormula,");
            strSql.Append("IsDeleted,");
            strSql.Append("CreateTime,");
            strSql.Append("UpdateTime,");
            strSql.Append("OutType,");
            strSql.Append("IntoType");
            strSql.Append(")Values(");
            strSql.Append(" :DetailKid,");
            strSql.Append(" :MerchantID,");
            strSql.Append(" :WaybillNo,");
            strSql.Append(" :WaybillType,");
            strSql.Append(" :SortingCenterID,");
            strSql.Append(" :TSortingCenterID,");
            strSql.Append(" :ReturnSortingCenterID,");
            strSql.Append(" :SortingMerchantID,");
            strSql.Append(" :CreateCityID,");
            strSql.Append(" :SortingCityID,");
            strSql.Append(" :DeliverStationID,");
            strSql.Append(" :TopCODCompanyID,");
            strSql.Append(" :ToStationTime,");
            strSql.Append(" :OutBoundTime,");
            strSql.Append(" :ReturnTime,");
            strSql.Append(" :InSortingTime,");
            strSql.Append(" :DistributionCode,");
            strSql.Append(" :IsAccount,");
            strSql.Append(" :AccountFare,");
            strSql.Append(" :AccountFormula,");
            strSql.Append(" :IsDeleted,");
            strSql.Append(" sysDate,");
            strSql.Append(" sysDate,");
            strSql.Append(" :OutType,");
            strSql.Append(" :IntoType");
            strSql.Append(" )");
            OracleParameter[] parameters = {
                                            new OracleParameter(":DetailKid", OracleDbType.Varchar2),
                                            new OracleParameter(":MerchantID", OracleDbType.Int32),
                                            new OracleParameter(":WaybillNo", OracleDbType.Int64),
                                            new OracleParameter(":WaybillType", OracleDbType.Varchar2),
                                            new OracleParameter(":SortingCenterID", OracleDbType.Int32),
                                            new OracleParameter(":TSortingCenterID", OracleDbType.Int32),
                                            new OracleParameter(":ReturnSortingCenterID",OracleDbType.Varchar2), 
                                            new OracleParameter(":CreateCityID",OracleDbType.Varchar2),
                                            new OracleParameter(":SortingCityID",OracleDbType.Varchar2), 
                                            new OracleParameter(":SortingMerchantID", OracleDbType.Int32),
                                            new OracleParameter(":DeliverStationID", OracleDbType.Int32),
                                            new OracleParameter(":TopCODCompanyID", OracleDbType.Int32),
                                            new OracleParameter(":ToStationTime", OracleDbType.Date),
                                            new OracleParameter(":OutBoundTime", OracleDbType.Date),
                                            new OracleParameter(":ReturnTime", OracleDbType.Date),
                                            new OracleParameter(":InSortingTime", OracleDbType.Date),
                                            new OracleParameter(":DistributionCode", OracleDbType.Varchar2),
                                            new OracleParameter(":IsAccount", OracleDbType.Int32),
                                            new OracleParameter(":AccountFare", OracleDbType.Decimal),
                                            new OracleParameter(":AccountFormula", OracleDbType.Varchar2),
                                            new OracleParameter(":IsDeleted", OracleDbType.Int32),
                                            new OracleParameter(":OutType",OracleDbType.Int32),
                                            new OracleParameter(":IntoType",OracleDbType.Int32) 

                                        };

            parameters[0].Value = model.DetailKID;
            parameters[1].Value = model.MerchantID;
            parameters[2].Value = model.WaybillNO;
            parameters[3].Value = model.WaybillType;
            parameters[4].Value = model.SortingCenter;
            parameters[5].Value = model.TSortingCenter;
            parameters[6].Value = model.ReturnSortingCenter;
            parameters[7].Value = model.CreateCityID;
            parameters[8].Value = model.SortingCityID;
            parameters[9].Value = model.SortingMerchantID;
            parameters[10].Value = model.DeliverStationID;
            parameters[11].Value = model.TopCODCompanyID;
            parameters[12].Value = model.ToStationTime;
            parameters[13].Value = model.OutBoundTime;
            parameters[14].Value = model.ReturnTime;
            parameters[15].Value = model.InSortingTime;
            parameters[16].Value = model.DistributionCode;
            parameters[17].Value = model.IsAccount;
            parameters[18].Value = model.AccountFare;
            parameters[19].Value = model.AccountFormula;
            parameters[20].Value = model.IsDelete ? 1 : 0;
            parameters[21].Value = model.OutType;
            parameters[22].Value = model.IntoType;
            var Result = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result;
        }

        public bool ExistFMS_SortingTransferDetailByNo(long waybillno)
        {
            string strSql =
                @" select count(1) from  FMS_SortingTransferDetail std
                             where std.WaybillNo = :WaybillNo and std.IsDeleted = 0";
            OracleParameter[] parameters = { new OracleParameter(":WaybillNo", OracleDbType.Int64) };
            parameters[0].Value = waybillno;
            var num = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            return Convert.ToInt32(num) > 0;

        }

        public string ExistOutBound(FMS_SortingTransferDetail model)
        {

            string strSql =
                @" select DetailKid from  FMS_SortingTransferDetail std 
                             where std.WaybillNo = :WaybillNo and std.IsDeleted = 0 and (std.OutType = 0 or (std.OutType =1 and std.OutBoundTime = :OutBoundTime))";
            OracleParameter[] parameters = {
                                            new OracleParameter(":WaybillNo", OracleDbType.Int64),
                                            new OracleParameter(":OutBoundTime",OracleDbType.Date)
                                        };
            parameters[0].Value = model.WaybillNO;
            parameters[1].Value = model.OutBoundTime;
            var DetailKid = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            if (DetailKid == null)
                return string.Empty;
            else
                return DetailKid.ToString();

        }

        public string ExistIntoStation(FMS_SortingTransferDetail model)
        {
            string strSql =
               @" select DetailKid from FMS_SortingTransferDetail std 
                             where std.WaybillNo = :WaybillNo and std.IsDeleted = 0 and (std.TSortingCenterID = :DeliverStationID or (std.DeliverStationID = :DeliverStationID and std.ToStationTime = :ToStationTime))";
            OracleParameter[] parameters = {
                                            new OracleParameter(":WaybillNo", OracleDbType.Int64),
                                            new OracleParameter(":DeliverStationID",OracleDbType.Int32) ,
                                            new OracleParameter(":ToStationTime",OracleDbType.Date) 
                                        };
            parameters[0].Value = model.WaybillNO;
            parameters[1].Value = model.DeliverStationID;
            parameters[2].Value = model.ToStationTime;
            var DetailKid = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            if (DetailKid == null)
                return string.Empty;
            else
                return DetailKid.ToString();

        }

        public string ExsitInSorting(FMS_SortingTransferDetail model)
        {
            string strSql =
                            @"select DetailKid from FMS_SortingTransferDetail std
                                                where std.WaybillNo = :WaybillNo and (std.SortingCenterID =:SortingCenterID or std.SortingCenterID is null) 
                                                and IsDeleted = 0 ";
            OracleParameter[] parameters = {
                                            new OracleParameter(":WaybillNo", OracleDbType.Int64),
                                            new OracleParameter(":SortingCenterID",OracleDbType.Int32),
                                        };
            parameters[0].Value = model.WaybillNO;
            parameters[1].Value = model.SortingCenter;

            var DetailKid = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            if (DetailKid == null)
                return string.Empty;
            else
                return DetailKid.ToString();
        }

        public bool UpdateFMS_SortingToCity(FMS_SortingTransferDetail model)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update FMS_SortingTransferDetail set ");

            strSql.Append("SortingCenterID = :SortingCenterID,");
            strSql.Append("SoringMerchantID =:SortingMerchantID,");
            strSql.Append("CreateCityID = :CreateCityID,");
            strSql.Append("TSortingCenterID = :TSortingCenterID,");
            strSql.Append("OutBoundTime =:OutBoundTime,");
            strSql.Append("DistributionCode=:DistributionCode,");
            strSql.Append("WaybillType=:WaybillType,");
            strSql.Append("UpdateTime = sysDate,");
            strSql.Append("MerchantID = :MerchantID, ");
            strSql.Append("OutType = 1,");
            strSql.Append("DeliverStationID =:DeliverStationID ");
            strSql.Append(" where DetailKid = :DetailKid");
            OracleParameter[] parameters = {
                                            
                                          
                                            new OracleParameter(":SortingCenterID", OracleDbType.Int32),
                                            new OracleParameter(":SortingMerchantID", OracleDbType.Int32),
                                            new OracleParameter(":CreateCityID",OracleDbType.Varchar2),
                                            new OracleParameter(":TSortingCenterID",OracleDbType.Int32),
                                            new OracleParameter(":OutBoundTime", OracleDbType.Date),
                                            new OracleParameter(":DistributionCode", OracleDbType.Varchar2),
                                            new OracleParameter(":WaybillType",OracleDbType.Varchar2),
                                            new OracleParameter(":MerchantID",OracleDbType.Int32), 
                                            new OracleParameter(":DetailKid",OracleDbType.Varchar2), 
                                            new OracleParameter(":DeliverStationID",OracleDbType.Int32)
                                            
                                        };

            parameters[0].Value = model.SortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.CreateCityID;
            parameters[3].Value = model.TSortingCenter;
            parameters[4].Value = model.OutBoundTime;
            parameters[5].Value = model.DistributionCode;
            parameters[6].Value = model.WaybillType;
            parameters[7].Value = model.MerchantID;
            parameters[8].Value = model.DetailKID;
            parameters[9].Value = model.DeliverStationID;
            int Result = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public bool UpdateFMS_SortingToStation(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update FMS_SortingTransferDetail set ");
            strSql.Append("TSortingCenterID = :TSortingCenterID,");
            strSql.Append("SoringMerchantID = :SortingMerchantID,");
            strSql.Append("SortCityID =:SortCityID,");
            strSql.Append("DeliverStationID =:DeliverStationID,");
            strSql.Append("TopCODCompanyID =:TopCODCompanyID,");
            strSql.Append("ToStationTime =:ToStationTime,");
            strSql.Append("WaybillType=:WaybillType,");
            strSql.Append("UpdateTime =sysDate,");
            strSql.Append("MerchantID =:MerchantID,");
            strSql.Append("IntoType= 1  ");
            strSql.Append("where DetailKid =:DetailKid");

            OracleParameter[] parameters = {
                                            new OracleParameter(":TSortingCenterID", OracleDbType.Int32),
                                            new OracleParameter(":SortingMerchantID", OracleDbType.Int32),
                                            new OracleParameter(":SortCityID", OracleDbType.Varchar2),
                                            new OracleParameter(":DeliverStationID", OracleDbType.Int32),
                                            new OracleParameter(":TopCODCompanyID", OracleDbType.Int32),
                                            new OracleParameter(":ToStationTime", OracleDbType.Date),
                                            new OracleParameter(":WaybillType",OracleDbType.Varchar2),
                                            new OracleParameter(":MerchantID",OracleDbType.Int32), 
                                            new OracleParameter(":DetailKid", OracleDbType.Varchar2)

                                        };
            parameters[0].Value = model.TSortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.SortingCityID;
            parameters[3].Value = model.DeliverStationID;
            parameters[4].Value = model.TopCODCompanyID;
            parameters[5].Value = model.ToStationTime;
            parameters[6].Value = model.WaybillType;
            parameters[7].Value = model.MerchantID;
            parameters[8].Value = model.DetailKID;

            var Result = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public bool UpdateFMS_ReturnToSortingCenter(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update FMS_SortingTransferDetail set ");
            strSql.Append("ReturnSortingCenterID =:ReturnSortingCenterID,");
            strSql.Append("SoringMerchantID =:SortingMerchantID,");
            strSql.Append("ReturnTime =:ReturnTime,");
            strSql.Append("DistributionCode =:DistributionCode,");
            strSql.Append("MerchantID =:MerchantID, ");
            strSql.Append("WaybillType=:WaybillType,");
            strSql.Append("UpdateTime =sysDate, ");
            strSql.Append("DeliverStationID =:DeliverStationID ");
            strSql.Append(@" where DetailKid = :DetailKid ");

            OracleParameter[] parameters = {
                                            new OracleParameter(":ReturnSortingCenterID", OracleDbType.Varchar2),
                                            new OracleParameter(":SortingMerchantID", OracleDbType.Int32),
                                            new OracleParameter(":ReturnTime", OracleDbType.Date),
                                            new OracleParameter(":DistributionCode", OracleDbType.Varchar2),
                                            new OracleParameter(":MerchantID",OracleDbType.Int32), 
                                            new OracleParameter(":WaybillType",OracleDbType.Varchar2), 
                                            new OracleParameter(":DetailKid", OracleDbType.Varchar2),
                                            new OracleParameter(":DeliverStationID",OracleDbType.Int32)

                                        };
            parameters[0].Value = model.ReturnSortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.ReturnTime;
            parameters[3].Value = model.DistributionCode;
            parameters[4].Value = model.MerchantID;
            parameters[5].Value = model.WaybillType;
            parameters[6].Value = model.DetailKID;
            parameters[7].Value = model.DeliverStationID;
            var Result = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public bool UpdateFMS_MerchantToSortingCenter(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("Update FMS_SortingTransferDetail set ");
            strSql.Append("SortingCenterID = :SortingCenterID,");
            strSql.Append("SoringMerchantID = :SortingMerchantID,");
            strSql.Append("CreateCityID =:CreateCityID,");
            strSql.Append("InSortingTime =:InSortingTime,");
            strSql.Append("WaybillType=:WaybillType,");
            strSql.Append("TopCODCompanyID =:TopCODCompanyID,");
            strSql.Append("MerchantID =:MerchantID, ");
            strSql.Append("UpdateTime =sysDate,");
            strSql.Append("DeliverStationID =:DeliverStationID ");
            strSql.Append(@" where DetailKid =:DetailKid");

            OracleParameter[] parameters = {
                                            new OracleParameter(":SortingCenterID", OracleDbType.Int32),
                                            new OracleParameter(":SortingMerchantID", OracleDbType.Int32),
                                            new OracleParameter(":CreateCityID", OracleDbType.Varchar2),
                                            new OracleParameter(":InSortingTime", OracleDbType.Date),
                                            new OracleParameter(":WaybillType",OracleDbType.Varchar2),
                                            new OracleParameter(":TopCODCompanyID",OracleDbType.Int32),
                                            new OracleParameter(":MerchantID",OracleDbType.Int32), 
                                            new OracleParameter(":DetailKid", OracleDbType.Varchar2),
                                            new OracleParameter(":DeliverStationID",OracleDbType.Int32) 

                                        };
            parameters[0].Value = model.SortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.CreateCityID;
            parameters[3].Value = model.InSortingTime;
            parameters[4].Value = model.WaybillType;
            parameters[5].Value = model.TopCODCompanyID;
            parameters[6].Value = model.MerchantID;
            parameters[7].Value = model.DetailKID;
            parameters[8].Value = model.DeliverStationID;

            var Result = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public DataTable SortingTransferAndToStationDetail(SortingDetail Model)
        {
            string strSql =
              @"with t as (
                 
                 select ROW_NUMBER() OVER(ORDER BY std.ToStationTime) AS num,std.DetailKid, std.SoringMerchantID,
                  ec.CompanyName As TSortingCenter,
                  c.CityName as SortCity,
                  ec1.CompanyName as DeliverStation,
                  std.ToStationTime,std.WaybillNo,
                  case (std.WaybillType)
                             when '0' then '普通订单'
                             when '1' then '上门换货'
                             when '2' then '上门退货'
                             when '3' then '签单返回' 
                             else '普通订单' 
                             end as WaybillType ,mbi.MerchantName  
                             from FMS_SortingTransferDetail std
                             Left Join PS_PMS.ExpressCompany ec on std.TSortingCenterID =ec.ExpressCompanyID
                             Left Join PS_PMS.City c on std.SortCityID = c.CityID
                             Left Join PS_PMS.ExpressCompany ec1 on std.DeliverStationID = ec1.ExpressCompanyID
                             Left Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID 
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =:DistributionCode ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= :StartTime and ToStationTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });

            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.StationIDs))
            {
                strSql += string.Format(" and std.DeliverStationID in ({0}) ", Model.StationIDs);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.SortCityID in ({0})", Model.CityIDs);
            }
            strSql += ") Select * from t where t.num between :StartRowNum  and :EndRowNum";
            parameters.Add(new OracleParameter(":StartRowNum", OracleDbType.Int32) { Value = Model.startRowNum });
            parameters.Add(new OracleParameter(":EndRowNum", OracleDbType.Int32) { Value = Model.endRowNum });

            var ds =OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityDetail(SortingDetail Model)
        {
            string strSql =
                 @" with t as (
                             select ROW_NUMBER() OVER(ORDER BY std.OutboundTime) AS num,std.DetailKid,std.SoringMerchantID,'' As SortingCenter,ec1.CompanyName as OutSortingCenter,c.CityName as CreateCity,d.DistributionName as Distribution,std.OutBoundTime,ec3.CompanyName as DeliverStation,std.WaybillNo,case (std.WaybillType)
                             when '0' then '普通订单'
                             when '1' then '上门换货'
                             when '2' then '上门退货'
                             when '3' then '签单返回'
                             else '普通订单' 
                             end as WaybillType ,mbi.MerchantName 
                             from FMS_SortingTransferDetail std
                             --Join LMS_RFD.dbo.Waybill w(nolock) on std.Waybillno =w.WaybillNo 
                             --Join PS_PMS.ExpressCompany ec
                             --on w.CreatStation =ec.ExpressCompanyID 
                             Join PS_PMS.ExpressCompany ec1 on std.SortingCenterID = ec1.ExpressCompanyID
                             Left Join PS_PMS.ExpressCompany ec3 on std.DeliverStationID =ec3.ExpressCompanyID
                             Join PS_PMS.City c on ec1.CityID = c.CityID
                             Join PS_PMS.ExpressCompany ec2 on std.TopCODCompanyID = ec2.ExpressCompanyID
                             Join PS_PMS.Distribution d on ec2.DistributionCode = d.DistributionCode
                             Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID where 1=1 and std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)";
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and OutBoundTime >= :StartTime and OutBoundTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });

            }
            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec2.distributionCode in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.CreateCityID in ({0})", Model.CityIDs);
            }
            strSql += @" ) select * from t where num between :StartRowNum and :EndRowNum ";
            parameters.Add(new OracleParameter(":StartRowNum", OracleDbType.Int32) { Value = Model.startRowNum });
            parameters.Add(new OracleParameter(":EndRowNum", OracleDbType.Int32) { Value = Model.endRowNum });

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable ReturnToSortingCenterDetail(MODEL.FinancialManage.SortingDetail Model)
        {
            string strSql= @"
            WITH t AS  (
                   select std.DetailKid,std.SoringMerchantID,wsr.SortationID as SortingCenter, std.distributionCode, std.MerchantID, std.ReturnTime ,std.WaybillNo, std.WaybillType, std.TOPCODCompanyID,
                   std.DeliverStationID,
                   std.ReturnSortingCenterID
                   from FMS_SortingTransferDetail std 
                   Join PS_PMS.WarehouseSortRelation wsr  on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = :DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   
                   union all 

                   select std.DetailKid,std.SoringMerchantID,ec.ExpressCompanyID as SortingCenter, std.distributionCode, std.MerchantID , std.ReturnTime ,std.WaybillNo, std.WaybillType,std.TOPCODCompanyID,
                   std.DeliverStationID,
                   std.ReturnSortingCenterID
                   from FMS_SortingTransferDetail std 
                   left Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID)=std.ReturnSortingCenterID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                 )
               SELECT * FROM ( 
               select ROW_NUMBER()OVER (ORDER BY t.ReturnTime) AS num,t.DetailKid,
               t.SoringMerchantID,
               ec1.CompanyName as SortingCenter ,
               d.DistributionName as Distribution,
               mbi.MerchantName,
               t.ReturnTime,
               ec3.CompanyName as DeliverStation,
               t.WaybillNo,
               case(t.WaybillType)
                  when '0' then '普通订单'
                  when '1' then '上门换货'
                  when '2' then '上门退货'
                  when '3' then '签单返回'
                  else '普通订单'
                  end as WaybillType
              from t 
               Left Join PS_PMS.ExpressCompany ec on t.TopCODCompanyID =ec.ExpressCompanyID
               Left Join PS_PMS.ExpressCompany ec1 on t.SortingCenter =ec1.ExpressCompanyID
               Left Join PS_PMS.Distribution d on ec.DistributionCode = d.DistributionCode
               Left Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.id
               Left Join PS_PMS.ExpressCompany ec3 on t.DeliverStationID =ec3.ExpressCompanyID
             where 1=1 ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
                parameters.Add(new OracleParameter(":DistributionCode",OracleDbType.Varchar2){Value = Model.DistributionCode});

            if (Model.WaybillNo != -1)
            {
                strSql += " and t.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec.distributionCode in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and t.WaybillType in ({0})", Model.waybillType);
            }
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter  in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            strSql += ") g Where g.num between :StartRowNum and :EndRowNum";
            parameters.Add(new OracleParameter(":StartRowNum", OracleDbType.Int32) { Value = Model.startRowNum });
            parameters.Add(new OracleParameter(":EndRowNum", OracleDbType.Int32) { Value = Model.endRowNum });
             
            

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable MerchantToSortingCenterDetail(MODEL.FinancialManage.SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : " having count(std1.WaybillNo) <:cnt";
            string strSql =
                @"
                
                with t as 
                  ( 
                   select to_char(std1.insortingtime,'yyyy-mm-dd') as SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std1
                   
                   where  std1.MerchantID not in(8,9) and std1.InSortingTime >=:StartTime and std1.InSortingTime <=:EndTime           
                   group by to_char(std1.insortingtime,'yyyy-mm-dd'), std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID" + appendStr + @"
                   ) 
                 
                 select * from (                 
  
                 select ROW_NUMBER()OVER (ORDER BY InSortingTime) AS num,std.DetailKid,std.SoringMerchantID,ec.CompanyName as SortingCenter,c.CityName as CreateCity,mbi.MerchantName,std.InSortingTime ,ec3.CompanyName as DeliverStation,std.WaybillNo,case(std.WaybillType)
                  when '0' then '普通订单'
                  when '1' then '上门换货'
                  when '2' then '上门退货'
                  when '3' then '签单返回'
                  else '普通订单'
                  end as WaybillType

                       from FMS_SortingTransferDetail std 
                       Join t on to_char(std.insortingtime,'yyyy-mm-dd') = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = t.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       Join PS_PMS.ExpressCompany ec on std.SortingCenterID = ec.ExpressCompanyID
                       Join PS_PMS.City c on std.CreateCityID = c.CityID
                       Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID 
                       Left Join PS_PMS.ExpressCompany ec3 on std.DeliverStationID = ec3.ExpressCompanyID
                       where 1=1 
              ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            if (Model.InSortingCount > 0)
            {
                parameters.Add(new OracleParameter(":cnt", OracleDbType.Int32) { Value = Model.InSortingCount });
            }


            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });


            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.CreateCityID in ({0})", Model.CityIDs);
            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            strSql += " ) g where g.num between :StartRowNum and :EndRowNum ";
            parameters.Add(new OracleParameter(":StartRowNum", OracleDbType.Int32) { Value = Model.startRowNum });
            parameters.Add(new OracleParameter(":EndRowNum", OracleDbType.Int32) { Value = Model.endRowNum });

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable ReverseMerchantToSortingCenterDetail(SortingDetail Model)
        {
            throw new NotImplementedException();
        }


        


        public DataTable GetTableByNo(long waybillno)
        {
            string Sqlstr =
                 @"select DetailKid,CreateTime from FMS_SortingTransferDetail std where WaybillNo = :WaybillNo";
            OracleParameter[] parameters = {
                                           new OracleParameter(":WaybillNo", OracleDbType.Int64)
                                       };
            parameters[0].Value = waybillno;
            var ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, Sqlstr, parameters);

            return ds.Tables[0];
        }


        public int CountSortingTransferAndStationDetail(SortingDetail Model)
        {
            string strSql = @"
            select count(1)
                             from FMS_SortingTransferDetail std
                           
                             Left Join PS_PMS.ExpressCompany ec1 on std.DeliverStationID = ec1.ExpressCompanyID
                          
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =:DistributionCode ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= :StartTime and ToStationTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });

            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.StationIDs))
            {
                strSql += string.Format(" and std.DeliverStationID in ({0}) ", Model.StationIDs);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.SortCityID in ({0})", Model.CityIDs);
            }

            var num = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public int CountSortingToCityDetail(SortingDetail Model)
        {
            string strSql =
               @"
                             select count(1)
                             from FMS_SortingTransferDetail std
                             --Join LMS_RFD.dbo.Waybill w(nolock) on std.Waybillno =w.WaybillNo 
                             Join PS_PMS.ExpressCompany ec2 on std.TopCODCompanyID = ec2.ExpressCompanyID
                             Join PS_PMS.Distribution d on ec2.DistributionCode = d.DistributionCode
                             where 1=1 and std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)";
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and OutBoundTime >= :StartTime and OutBoundTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.EndTime) });

            }
            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec2.distributionCode in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format("  and std.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.CreateCityID in ({0})", Model.CityIDs);
            }
            var num = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }

        public int CountReturnToSortingCenterDetail(SortingDetail Model)
        {
            string strSql =
                @" 
            WITH t AS  (  select count(1) as num
                  from FMS_SortingTransferDetail std 
                   Join PS_PMS.WarehouseSortRelation wsr  on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = :DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   ";

            if (Model.WaybillNo != -1)
            {
                strSql += string.Format(" and std.WaybillNo = {0}", Model.WaybillNo);

            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec0.ExpressCompanyID in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and ec.ExpressCompanyID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            strSql += @"
                   union all 

                   select count(1) as num
                   from FMS_SortingTransferDetail std
                   left Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID) =std.ReturnSortingCenterID
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                 
              ";

            if (Model.WaybillNo != -1)
            {
                strSql += string.Format(" and std.WaybillNo = {0}", Model.WaybillNo);

            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec0.ExpressCompanyID in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and ec.ExpressCompanyID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                string MerchantIDs = MerchantForOuter(Model.MerchantIDs);
                if (!string.IsNullOrEmpty(MerchantIDs))
                {
                    strSql += string.Format(" and std.MerchantID in ({0})", MerchantIDs);
                }

            }

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            strSql += @")  Select sum(t.num) from t";

            var num = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }

        private string MerchantForOuter(string MerchantIDs)
        {
            string[] ss = MerchantIDs.Split(',');
            string ret = "";
            foreach (var s in ss)
            {
                if (s != "8" && s != "9")
                {
                    ret += s + ",";
                }
            }
            return ret.Trim(',');
        }
        public int CountMerchantToSortingCenterDetail(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : " having count(std1.WaybillNo) <:cnt";
            string strSql =
                @"
               
                with t as 
                  ( 
                   select to_char(std1.insortingtime,'yyyy-mm-dd') as SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std1
                   where  std1.MerchantID not in (8,9) and std1.InSortingTime >=:StartTime and std1.InSortingTime <=:EndTime           
                   group by to_char(std1.insortingtime,'yyyy-mm-dd'), std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID" + appendStr + @"
                   ) 
                   
                 select Count(1)
                       from FMS_SortingTransferDetail std Join t on
                       to_char(std.insortingtime,'yyyy-mm-dd') = t.SDate
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = t.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       where 1=1 
              ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            if (Model.InSortingCount > 0)
            {
                parameters.Add(new OracleParameter(":cnt", OracleDbType.Int32) { Value = Model.InSortingCount });
            }


            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value =DataConvert.ToDateTime( Model.EndTime) });


            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.CreateCityID in ({0})", Model.CityIDs);
            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            
            var num = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public DataTable ExportSortingTransferAndToStationDetail(SortingDetail Model)
        {
            string strSql =
            @" 
            select '北京柏松物流有限公司' as 拣运商 ,
                  ec.CompanyName As 分拣中心,
                  c.CityName as 城市,
                  ec1.CompanyName as 配送站,
                  std.ToStationTime as 入站时间 ,std.WaybillNo as 运单号,
                  case (std.WaybillType)
                             when '0' then '普通订单'
                             when '1' then '上门换货'
                             when '2' then '上门退货' 
                             else '普通订单' 
                             end as 订单类型 ,mbi.MerchantName as 商家  
                             from FMS_SortingTransferDetail std
                             Left Join PS_PMS.ExpressCompany ec on std.TSortingCenterID =ec.ExpressCompanyID
                             Left Join PS_PMS.City c on std.SortCityID = c.CityID
                             Left Join PS_PMS.ExpressCompany ec1 on std.DeliverStationID = ec1.ExpressCompanyID
                             Left Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID 
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =:DistributionCode ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= :StartTime and ToStationTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });

            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.StationIDs))
            {
                strSql += string.Format(" and std.DeliverStationID in ({0}) ", Model.StationIDs);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.SortCityID in ({0})", Model.CityIDs);
            }
            strSql += @"Order by std.ToStationTime";
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable ExportSortingToCityDetail(SortingDetail Model)
        {
            string strSql =
               @" 
                             select '北京柏松物流有限公司' as 拣运商,'' As 分拣中心,ec1.CompanyName as 出库分拣中心,c.CityName as 城市,d.DistributionName as 配送商,std.OutBoundTime as 出库时间,ec3.CompanyName as 配送站，std.WaybillNo as 运单号,case (std.WaybillType)
                             when '0' then '普通订单'
                             when '1' then '上门换货'
                             when '2' then '上门退货' 
                             else '普通订单' 
                             end as 运单类型 ,mbi.MerchantName as 商家
                             from FMS_SortingTransferDetail std   
                             Join PS_PMS.ExpressCompany ec1 on std.SortingCenterID = ec1.ExpressCompanyID
                             Join PS_PMS.City c on ec1.CityID = c.CityID
                             Join PS_PMS.ExpressCompany ec2 on std.TopCODCompanyID = ec2.ExpressCompanyID
                             Join PS_PMS.Distribution d on ec2.DistributionCode = d.DistributionCode
                             Left Join PS_PMS.ExpressCompany ec3 on std.DeliverStationID = ec3.ExpressCompanyID
                             Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID where 1=1 and std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)";
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and OutBoundTime >= :StartTime and OutBoundTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.EndTime) });

            }
            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec2.distributionCode in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format("  and std.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.CreateCityID in ({0})", Model.CityIDs);
            }

            strSql += " Order by std.OutBoundTime ";
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable ExportReturnToSortingCenterDetail(SortingDetail Model)
        {
            string strSql =
              @" 
            WITH t AS  (
                   select std.DetailKid,std.SoringMerchantID,wsr.SortationID as SortingCenter, std.distributionCode, std.MerchantID, std.ReturnTime ,std.DeliverStationID,std.WaybillNo, std.WaybillType, std.TOPCODCompanyID,
                  
                   std.ReturnSortingCenterID
                   from FMS_SortingTransferDetail std
                   Join PS_PMS.WarehouseSortRelation wsr  on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = :DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   
                   union all 

                   select std.DetailKid,std.SoringMerchantID,ec.ExpressCompanyID as SortingCenter, std.distributionCode, std.MerchantID , std.ReturnTime ,std.DeliverStationID,std.WaybillNo, std.WaybillType,std.TOPCODCompanyID,
                   std.ReturnSortingCenterID
                   from FMS_SortingTransferDetail std
                   left Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID) =std.ReturnSortingCenterID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                 )
              
               select
                '北京柏松物流有限公司' as 拣运商,
               ec1.CompanyName as 分拣中心 ,
               d.DistributionName as 配送商,
               mbi.MerchantName as 商家,
               t.ReturnTime as 入库时间,
               ec3.CompanyName as 配送站，
               t.WaybillNo as 运单号,
               case(t.WaybillType)
                  when '0' then '普通订单'
                  when '1' then '上门换货'
                  when '2' then '上门退货'
                  else '普通订单'
                  end as 运单类型
              from t 
               Left Join PS_PMS.ExpressCompany ec on t.TopCODCompanyID =ec.ExpressCompanyID
               Left Join PS_PMS.ExpressCompany ec1 on t.SortingCenter =ec1.ExpressCompanyID
               Left Join PS_PMS.Distribution d on ec.DistributionCode = d.DistributionCode
               Left Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.id
               Left Join PS_PMS.ExpressCompany ec3 on t.DeliverStationID = ec3.ExpressCompanyID
             where 1=1 ";

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            if (Model.WaybillNo != -1)
            {
                strSql += " and t.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }

            if (!string.IsNullOrEmpty(Model.DistributionCodes))
            {
                strSql += string.Format(" and ec.distributionCode in ({0}) ", Model.DistributionCodes);
            }

            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and t.WaybillType in ({0})", Model.waybillType);
            }
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter  in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            strSql += " Order by t.ReturnTime ";


            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }


        public DataTable ExportMerchantToSortingCenterDetail(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : " having count(std1.WaybillNo) <:cnt";
            string strSql =
                @"
                with t as 
                  ( 
                   select to_char(std1.insortingtime,'yyyy-mm-dd') as SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std1
                   
                   where  std1.MerchantID not in(8,9) and std1.InSortingTime >=:StartTime and std1.InSortingTime <=:EndTime           
                   group by to_char(std1.insortingtime,'yyyy-mm-dd'), std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID" + appendStr + @"
                   ) 
                 
                               
  
                 select '北京柏松物流有限公司'as 拣运商,ec.CompanyName as 分拣中心,c.CityName as 城市,mbi.MerchantName as 商家,std.InSortingTime as 入库时间 ,ec3.CompanyName as 配送站，std.WaybillNo as 运单号,case(std.WaybillType)
                  when '0' then '普通订单'
                  when '1' then '上门换货'
                  when '2' then '上门退货'
                  else '普通订单'
                  end as 运单类型

                       from FMS_SortingTransferDetail std 
                       Join t on to_char(std.insortingtime,'yyyy-mm-dd') = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = t.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       Join PS_PMS.ExpressCompany ec  on std.SortingCenterID = ec.ExpressCompanyID
                       Join PS_PMS.City c on std.CreateCityID = c.CityID
                       Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID 
                       Left Join PS_PMS.ExpressCompany ec3 on std.DeliverStationID = ec3.ExpressCompanyID
                       where 1=1 
              ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            if (Model.InSortingCount > 0)
            {
                parameters.Add(new OracleParameter(":cnt", OracleDbType.Int32) { Value = Model.InSortingCount });
            }


            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });


            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.CreateCityID in ({0})", Model.CityIDs);
            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = :WaybillNo ";
                parameters.Add(new OracleParameter(":WaybillNo", OracleDbType.Int64) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            strSql += " Order by std.InSortingTime";
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable GetWaybillNosByMerchantID0(int rowcount)
        {
            string strSql =
                @"
                                select distinct WaybillNo from fms_sortingTransferDetail std where std.MerchantID = 0 
                                 and rownum <=:rowcount and CreateTime < to_date('2013-03-12 0:00:00','yyyy-mm-dd hh24:mi:ss')
                            ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":rowcount", OracleDbType.Int32){Value = rowcount}
                                          };
           var ds= OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            return ds.Tables[0];
        }

        public void UpdateMerchantID(long waybillno, int merchantID)
        {
            string strsql =
                @" Update fms_sortingTransferDetail set MerchantID = :MerchantID
                           where waybillno = :waybillno";
            OracleParameter[] parameters = {
                                               new OracleParameter(":MerchantID", OracleDbType.Int32)
                                                   {Value = merchantID},
                                               new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillno}
                                           };
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strsql, parameters);
        }
    }
}
