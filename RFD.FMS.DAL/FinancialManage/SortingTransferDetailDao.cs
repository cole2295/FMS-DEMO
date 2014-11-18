using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.ApplicationBlocks.Data.Extension;
using RFD.FMS.AdoNet;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.DAL.BasicSetting;
using RFD.FMS.Domain.COD;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL;
using System.Collections.Generic;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.FinancialManage
{
    public class SortingTransferDetailDao : SqlServerDao, ISortingTransferDetailDao
    {
        public int Add(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into RFD_FMS.dbo.FMS_SortingTransferDetail(");
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
            strSql.Append("IntoType,");
            strSql.Append("IsChange");
            strSql.Append(")Values(");
            strSql.Append(" @DetailKid,");
            strSql.Append(" @MerchantID,");
            strSql.Append(" @WaybillNo,");
            strSql.Append(" @WaybillType,");
            strSql.Append(" @SortingCenterID,");
            strSql.Append(" @TSortingCenterID,");
            strSql.Append(" @ReturnSortingCenterID,");
            strSql.Append(" @SortingMerchantID,");
            strSql.Append(" @CreateCityID,");
            strSql.Append(" @SortingCityID,");
            strSql.Append(" @DeliverStationID,");
            strSql.Append(" @TopCODCompanyID,");
            strSql.Append(" @ToStationTime,");
            strSql.Append(" @OutBoundTime,");
            strSql.Append(" @ReturnTime,");
            strSql.Append(" @InSortingTime,");
            strSql.Append(" @DistributionCode,");
            strSql.Append(" @IsAccount,");
            strSql.Append(" @AccountFare,");
            strSql.Append(" @AccountFormula,");
            strSql.Append(" @IsDeleted,");
            strSql.Append(" getDate(),");
            strSql.Append(" getDate(),");
            strSql.Append(" @OutType,");
            strSql.Append(" @IntoType,");
            strSql.Append(" 1");
            strSql.Append(" )");
            SqlParameter[] parameters = {
                                            new SqlParameter("@DetailKid", SqlDbType.VarChar),
                                            new SqlParameter("@MerchantID", SqlDbType.Int),
                                            new SqlParameter("@WaybillNo", SqlDbType.BigInt),
                                            new SqlParameter("@WaybillType", SqlDbType.VarChar),
                                            new SqlParameter("@SortingCenterID", SqlDbType.Int),
                                            new SqlParameter("@TSortingCenterID", SqlDbType.Int),
                                            new SqlParameter("@ReturnSortingCenterID",SqlDbType.NVarChar), 
                                            new SqlParameter("@CreateCityID",SqlDbType.NVarChar),
                                            new SqlParameter("@SortingCityID",SqlDbType.NVarChar), 
                                            new SqlParameter("@SortingMerchantID", SqlDbType.Int),
                                            new SqlParameter("@DeliverStationID", SqlDbType.Int),
                                            new SqlParameter("@TopCODCompanyID", SqlDbType.Int),
                                            new SqlParameter("@ToStationTime", SqlDbType.DateTime),
                                            new SqlParameter("@OutBoundTime", SqlDbType.DateTime),
                                            new SqlParameter("@ReturnTime", SqlDbType.DateTime),
                                            new SqlParameter("@InSortingTime", SqlDbType.DateTime),
                                            new SqlParameter("@DistributionCode", SqlDbType.NVarChar),
                                            new SqlParameter("@IsAccount", SqlDbType.Int),
                                            new SqlParameter("@AccountFare", SqlDbType.Decimal),
                                            new SqlParameter("@AccountFormula", SqlDbType.NVarChar),
                                            new SqlParameter("@IsDeleted", SqlDbType.Bit),
                                            new SqlParameter("@OutType",SqlDbType.Int),
                                            new SqlParameter("@IntoType",SqlDbType.Int) 

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
            parameters[20].Value = model.IsDelete ? 1:0;
            parameters[21].Value = model.OutType;
            parameters[22].Value = model.IntoType;
            var Result = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result;
        }

        public bool ExistFMS_SortingTransferDetailByNo(long waybillno)
        {
            string strSql =
                @" select count(1) from RFD_FMS.dbo.FMS_SortingTransferDetail std(NoLock) 
                             where std.WaybillNo = @WaybillNo and std.IsDeleted = 0";
            SqlParameter[] parameters = {new SqlParameter("@WaybillNo", SqlDbType.BigInt)};
            parameters[0].Value = waybillno;
            var num = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            return Convert.ToInt32(num) > 0;

        }

        public string ExsitInSorting(FMS_SortingTransferDetail model)
        {
            string strSql =
                             @"select DetailKid from RFD_FMS.dbo.FMS_SortingTransferDetail std(NoLock)
                                                where std.WaybillNo = @WaybillNo and (std.SortingCenterID =@SortingCenterID or std.SortingCenterID is null) 
                                                and IsDeleted = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@WaybillNo", SqlDbType.BigInt),
                                            new SqlParameter("@SortingCenterID",SqlDbType.Int),
                                        };
            parameters[0].Value = model.WaybillNO;
            parameters[1].Value = model.SortingCenter;

            var DetailKid=SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            if (DetailKid == null)
                return string.Empty;
            else
                return DetailKid.ToString();
                           
        }

        public string ExistOutBound(FMS_SortingTransferDetail model)
        {
            
            string strSql =
                @" select DetailKid from RFD_FMS.dbo.FMS_SortingTransferDetail std(NoLock) 
                             where std.WaybillNo = @WaybillNo and std.IsDeleted = 0 and (std.OutType = 0 or (std.OutType =1 and std.OutBoundTime = @OutBoundTime))";
            SqlParameter[] parameters = {
                                            new SqlParameter("@WaybillNo", SqlDbType.BigInt),
                                            new SqlParameter("@OutBoundTime",SqlDbType.DateTime)
                                        };
            parameters[0].Value = model.WaybillNO;
            parameters[1].Value = model.OutBoundTime;
            var DetailKid = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            if (DetailKid == null)
                return string.Empty;
            else
                return DetailKid.ToString();
                           
        }


        public string ExistIntoStation(FMS_SortingTransferDetail model)
        {
            string strSql =
                @" select DetailKid from RFD_FMS.dbo.FMS_SortingTransferDetail std(NoLock) 
                             where std.WaybillNo = @WaybillNo and std.IsDeleted = 0 and (std.TSortingCenterID = @DeliverStationID or (std.DeliverStationID = @DeliverStationID and std.ToStationTime = @ToStationTime))";
            SqlParameter[] parameters = {
                                            new SqlParameter("@WaybillNo", SqlDbType.BigInt),
                                            new SqlParameter("@DeliverStationID",SqlDbType.Int) ,
                                            new SqlParameter("@ToStationTime",SqlDbType.DateTime) 
                                        };
            parameters[0].Value = model.WaybillNO;
            parameters[1].Value = model.DeliverStationID;
            parameters[2].Value = model.ToStationTime;
            var DetailKid = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql, parameters);
            if (DetailKid == null)
                return string.Empty;
            else
                return DetailKid.ToString();

        }

        public bool UpdateFMS_SortingToCity(FMS_SortingTransferDetail model)
        {



            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update RFD_FMS.dbo.FMS_SortingTransferDetail set ");

            strSql.Append("SortingCenterID = @SortingCenterID,");
            strSql.Append("SoringMerchantID =@SortingMerchantID,");
            strSql.Append("CreateCityID = @CreateCityID,");
            strSql.Append("TSortingCenterID = @TSortingCenterID,");
            strSql.Append("OutBoundTime =@OutBoundTime,");
            strSql.Append("DistributionCode=@DistributionCode,");
            strSql.Append("WaybillType=@WaybillType,");
            strSql.Append("UpdateTime = getDate(),");
            strSql.Append("OutType = 1, ");
            strSql.Append("IsChange= 1 ");
            strSql.Append("where DetailKid = @DetailKid");
            SqlParameter[] parameters = {
                                            
                                          
                                            new SqlParameter("@SortingCenterID", SqlDbType.Int),
                                            new SqlParameter("@SortingMerchantID", SqlDbType.Int),
                                            new SqlParameter("@CreateCityID",SqlDbType.NVarChar),
                                            new SqlParameter("@TSortingCenterID",SqlDbType.Int), 
                                            new SqlParameter("@OutBoundTime", SqlDbType.DateTime),
                                            new SqlParameter("@DistributionCode", SqlDbType.NVarChar),
                                            new SqlParameter("@WaybillType",SqlDbType.VarChar),
                                            new SqlParameter("@DetailKid",SqlDbType.VarChar) 
                                            
                                        };

            parameters[0].Value = model.SortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.CreateCityID;
            parameters[3].Value = model.TSortingCenter;
            parameters[4].Value = model.OutBoundTime;
            parameters[5].Value = model.DistributionCode;
            parameters[6].Value = model.WaybillType;
            parameters[7].Value = model.DetailKID;
            int Result = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),parameters);
            return Result>0;
        }

        public bool UpdateFMS_SortingToStation(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update RFD_FMS.dbo.FMS_SortingTransferDetail set ");
            strSql.Append("TSortingCenterID = @TSortingCenterID,");
            strSql.Append("SoringMerchantID = @SortingMerchantID,");
            strSql.Append("SortCityID =@SortCityID,");
            strSql.Append("DeliverStationID =@DeliverStationID,");
            strSql.Append("TopCODCompanyID =@TopCODCompanyID,");
            strSql.Append("ToStationTime =@ToStationTime,");
            strSql.Append("WaybillType=@WaybillType,");
            strSql.Append("UpdateTime =getDate(),");
            strSql.Append("IntoType= 1,  ");
            strSql.Append("IsChange= 1 ");
            strSql.Append("where DetailKid =@DetailKid");

            SqlParameter[] parameters = {
                                            new SqlParameter("@TSortingCenterID", SqlDbType.Int),
                                            new SqlParameter("@SortingMerchantID", SqlDbType.Int),
                                            new SqlParameter("@SortCityID", SqlDbType.NVarChar),
                                            new SqlParameter("@DeliverStationID", SqlDbType.Int),
                                            new SqlParameter("@TopCODCompanyID", SqlDbType.Int),
                                            new SqlParameter("@ToStationTime", SqlDbType.DateTime),
                                            new SqlParameter("@WaybillType",SqlDbType.VarChar),
                                            new SqlParameter("@DetailKid", SqlDbType.VarChar)

                                        };
            parameters[0].Value = model.TSortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.SortingCityID;
            parameters[3].Value = model.DeliverStationID;
            parameters[4].Value = model.TopCODCompanyID;
            parameters[5].Value = model.ToStationTime;
            parameters[6].Value = model.WaybillType;
            parameters[7].Value = model.DetailKID;

            var Result = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public bool UpdateFMS_ReturnToSortingCenter(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update RFD_FMS.dbo.FMS_SortingTransferDetail set ");
            strSql.Append("ReturnSortingCenterID =@ReturnSortingCenterID,");
            strSql.Append("SoringMerchantID =@SortingMerchantID,");
            strSql.Append("ReturnTime =@ReturnTime,");
            strSql.Append("DistributionCode =@DistributionCode,");
            strSql.Append("MerchantID =@MerchantID, ");
            strSql.Append("WaybillType=@WaybillType,");
            strSql.Append("UpdateTime =getDate(), ");
            strSql.Append("IsChange= 1");
            strSql.Append(@"where DetailKid = @DetailKid ");
            
            SqlParameter[] parameters = {
                                            new SqlParameter("@ReturnSortingCenterID", SqlDbType.NVarChar),
                                            new SqlParameter("@SortingMerchantID", SqlDbType.Int),
                                            new SqlParameter("@ReturnTime", SqlDbType.DateTime),
                                            new SqlParameter("@DistributionCode", SqlDbType.NVarChar),
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
                                            new SqlParameter("@WaybillType",SqlDbType.VarChar),
                                            new SqlParameter("@DetailKid", SqlDbType.VarChar)
                                           

                                        };
            parameters[0].Value = model.ReturnSortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.ReturnTime;
            parameters[3].Value = model.DistributionCode;
            parameters[4].Value = model.MerchantID;
            parameters[5].Value = model.WaybillType;
            parameters[6].Value = model.DetailKID;
           

            var Result = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public bool UpdateFMS_MerchantToSortingCenter(FMS_SortingTransferDetail model)
        {
            StringBuilder strSql = new StringBuilder();
          
            strSql.Append("Update RFD_FMS.dbo.FMS_SortingTransferDetail set ");
            strSql.Append("SortingCenterID = @SortingCenterID,");
            strSql.Append("SoringMerchantID = @SortingMerchantID,");
            strSql.Append("CreateCityID =@CreateCityID,");
            strSql.Append("InSortingTime =@InSortingTime,");
            strSql.Append("WaybillType=@WaybillType,");
            strSql.Append("UpdateTime =getDate(), ");
            strSql.Append("IsChange= 1 ");
            strSql.Append(@"where DetailKid =@DetailKid");

            SqlParameter[] parameters = {
                                            new SqlParameter("@SortingCenterID", SqlDbType.Int),
                                            new SqlParameter("@SortingMerchantID", SqlDbType.Int),
                                            new SqlParameter("@CreateCityID", SqlDbType.NVarChar),
                                            new SqlParameter("@InSortingTime", SqlDbType.DateTime),
                                            new SqlParameter("@WaybillType",SqlDbType.VarChar),
                                            new SqlParameter("@DetailKid", SqlDbType.VarChar)

                                        };
            parameters[0].Value = model.SortingCenter;
            parameters[1].Value = model.SortingMerchantID;
            parameters[2].Value = model.CreateCityID;
            parameters[3].Value = model.InSortingTime;
            parameters[4].Value = model.WaybillType;
            parameters[5].Value = model.DetailKID;

            var Result = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            return Result > 0;
        }

        public DataTable SortingTransferAndToStationDetail(SortingDetail Model)
        {
            string strSql =
               @"with t as (
                 
                 select ROW_NUMBER() OVER(ORDER BY std.ToStationTime) AS rownum,std.DetailKid, std.SoringMerchantID,
                  ec.CompanyName As TSortingCenter,
                  c.CityName as SortCity,
                  ec1.CompanyName as DeliverStation,
                  std.ToStationTime,std.WaybillNo,
                  case (std.WaybillType)
                             when 0 then '普通订单'
                             when 1 then '上门换货'
                             when 2 then '上门退货' 
                             else '普通订单' 
                             end as WaybillType ,mbi.MerchantName  
                             from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock)
                             Left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on std.TSortingCenterID =ec.ExpressCompanyID
                             Left Join RFD_PMS.dbo.City c(nolock) on std.SortCityID = c.CityID
                             Left Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on std.DeliverStationID = ec1.ExpressCompanyID
                             Left Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID 
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =@DistributionCode ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= @StartTime and ToStationTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            }

            if(Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = @WaybillNo " ;
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) {Value = Model.WaybillNo});
            }

            if(!string.IsNullOrEmpty(Model.StationIDs))
            {
                strSql += string.Format(" and std.DeliverStationID in ({0}) ", Model.StationIDs);
            }

            if(!string.IsNullOrEmpty(Model.waybillType))
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
            strSql += ") Select * from t where t.rownum between @StartRowNum  and @EndRowNum";
            parameters.Add(new SqlParameter("@StartRowNum",SqlDbType.Int){Value = Model.startRowNum});
            parameters.Add(new SqlParameter("@EndRowNum",SqlDbType.Int){Value = Model.endRowNum});

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120,CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
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
                             when 0 then '普通订单'
                             when 1 then '上门换货'
                             when 2 then '上门退货' 
                             else '普通订单' 
                             end as 订单类型 ,mbi.MerchantName as 商家  
                             from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock)
                             Left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on std.TSortingCenterID =ec.ExpressCompanyID
                             Left Join RFD_PMS.dbo.City c(nolock) on std.SortCityID = c.CityID
                             Left Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on std.DeliverStationID = ec1.ExpressCompanyID
                             Left Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID 
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =@DistributionCode ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar) { Value = Model.DistributionCode });
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= @StartTime and ToStationTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            }

            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
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
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable SortingToCityDetail(SortingDetail Model)
        {
            string strSql =
                @" with t as (
                             select ROW_NUMBER() OVER(ORDER BY std.OutboundTime) AS rownum,std.DetailKid,std.SoringMerchantID,ec.CompanyName As SortingCenter,ec1.CompanyName as OutSortingCenter,c.CityName as CreateCity,d.DistributionName as Distribution,std.OutBoundTime,std.WaybillNo,case (std.WaybillType)
                             when 0 then '普通订单'
                             when 1 then '上门换货'
                             when 2 then '上门退货' 
                             else '普通订单' 
                             end as WaybillType ,mbi.MerchantName 
                             from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                             Join LMS_RFD.dbo.Waybill w(nolock) on std.Waybillno =w.WaybillNo 
                             Join RFD_PMS.dbo.ExpressCompany ec(nolock) 
                             on w.CreatStation =ec.ExpressCompanyID 
                             Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on std.SortingCenterID = ec1.ExpressCompanyID
                             Join RFD_PMS.dbo.City c(nolock) on ec.CityID = c.CityID
                             Join RFD_PMS.dbo.ExpressCompany ec2(nolock) on std.TopCODCompanyID = ec2.ExpressCompanyID
                             Join RFD_PMS.dbo.Distribution d(nolock) on ec2.DistributionCode = d.DistributionCode
                             Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID where 1=1 and std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and OutBoundTime >= @StartTime and OutBoundTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            }
            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
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
            strSql += @" ) select * from t where rownum between @StartRowNum and @EndRowNum ";
            parameters.Add(new SqlParameter("@StartRowNum", SqlDbType.Int) { Value = Model.startRowNum });
            parameters.Add(new SqlParameter("@EndRowNum", SqlDbType.Int) { Value = Model.endRowNum });

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable ExportSortingToCityDetail(SortingDetail Model)
        {
            string strSql =
                @" 
                             select '北京柏松物流有限公司' as 拣运商,ec.CompanyName As 分拣中心,ec1.CompanyName as 出库分拣中心,c.CityName as 城市,d.DistributionName as 配送商,std.OutBoundTime as 出库时间,std.WaybillNo as 运单号,case (std.WaybillType)
                             when 0 then '普通订单'
                             when 1 then '上门换货'
                             when 2 then '上门退货' 
                             else '普通订单' 
                             end as 运单类型 ,mbi.MerchantName as 商家
                             from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                             Join LMS_RFD.dbo.Waybill w(nolock) on std.Waybillno =w.WaybillNo 
                             Join RFD_PMS.dbo.ExpressCompany ec(nolock) 
                             on w.CreatStation =ec.ExpressCompanyID 
                             Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on std.SortingCenterID = ec1.ExpressCompanyID
                             Join RFD_PMS.dbo.City c(nolock) on ec.CityID = c.CityID
                             Join RFD_PMS.dbo.ExpressCompany ec2(nolock) on std.TopCODCompanyID = ec2.ExpressCompanyID
                             Join RFD_PMS.dbo.Distribution d(nolock) on ec2.DistributionCode = d.DistributionCode
                             Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID where 1=1 and std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and OutBoundTime >= @StartTime and OutBoundTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            }
            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
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
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable ReturnToSortingCenterDetail(SortingDetail Model)
        {
            string strSql =
                @" 
            WITH t AS  (
                   select std.DetailKid,std.SoringMerchantID,wsr.SortationID as SortingCenter, std.distributionCode, std.MerchantID, std.ReturnTime ,std.WaybillNo, std.WaybillType, std.TOPCODCompanyID,
                  
                   std.ReturnSortingCenterID
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.WarehouseSortRelation wsr (nolock) on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ec.ExpressCompanyID = wsr.SortationID
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = @DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   
                   union all 

                   select std.DetailKid,std.SoringMerchantID,ec.ExpressCompanyID as SortingCenter, std.distributionCode, std.MerchantID , std.ReturnTime ,std.WaybillNo, std.WaybillType,std.TOPCODCompanyID,
                   std.ReturnSortingCenterID
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on cast(ec.ExpressCompanyID as nvarchar(20)) =std.ReturnSortingCenterID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                 )
               SELECT * FROM ( 
               select ROW_NUMBER()OVER (ORDER BY t.ReturnTime) AS rownum,t.DetailKid,
               t.SoringMerchantID,
               ec1.CompanyName as SortingCenter ,
               d.DistributionName as Distribution,
               mbi.MerchantName,
               t.ReturnTime,
               t.WaybillNo,
               case(t.WaybillType)
                  when 0 then '普通订单'
                  when 1 then '上门换货'
                  when 2 then '上门退货'
                  else '普通订单'
                  end as WaybillType
              from t 
               Left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.TopCODCompanyID =ec.ExpressCompanyID
               Left Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on t.SortingCenter =ec1.ExpressCompanyID
               Left Join RFD_PMS.dbo.Distribution d(nolock) on ec.DistributionCode = d.DistributionCode
               Left Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on t.MerchantID = mbi.id
             where 1=1 ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
                parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});

            if (Model.WaybillNo != -1)
            {
                strSql += " and t.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
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

            strSql += ") g Where g.rownum between @StartRowNum and @EndRowNum";
            parameters.Add(new SqlParameter("@StartRowNum", SqlDbType.Int) { Value = Model.startRowNum });
            parameters.Add(new SqlParameter("@EndRowNum", SqlDbType.Int) { Value = Model.endRowNum });
             
            

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable ExportReturnToSortingCenterDetail(SortingDetail Model)
        {
            string strSql =
                @" 
            WITH t AS  (
                   select std.DetailKid,std.SoringMerchantID,wsr.SortationID as SortingCenter, std.distributionCode, std.MerchantID, std.ReturnTime ,std.WaybillNo, std.WaybillType, std.TOPCODCompanyID,
                  
                   std.ReturnSortingCenterID
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.WarehouseSortRelation wsr (nolock) on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ec.ExpressCompanyID = wsr.SortationID
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = @DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   
                   union all 

                   select std.DetailKid,std.SoringMerchantID,ec.ExpressCompanyID as SortingCenter, std.distributionCode, std.MerchantID , std.ReturnTime ,std.WaybillNo, std.WaybillType,std.TOPCODCompanyID,
                   std.ReturnSortingCenterID
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on cast(ec.ExpressCompanyID as nvarchar(20)) =std.ReturnSortingCenterID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                 )
              
               select
                '北京柏松物流有限公司' as 拣运商,
               ec1.CompanyName as 分拣中心 ,
               d.DistributionName as 配送商,
               mbi.MerchantName as 商家,
               t.ReturnTime as 入库时间,
               t.WaybillNo as 运单号,
               case(t.WaybillType)
                  when 0 then '普通订单'
                  when 1 then '上门换货'
                  when 2 then '上门退货'
                  else '普通订单'
                  end as 运单类型
              from t 
               Left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.TopCODCompanyID =ec.ExpressCompanyID
               Left Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on t.SortingCenter =ec1.ExpressCompanyID
               Left Join RFD_PMS.dbo.Distribution d(nolock) on ec.DistributionCode = d.DistributionCode
               Left Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on t.MerchantID = mbi.id
             where 1=1 ";

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
            parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar) { Value = Model.DistributionCode });

            if (Model.WaybillNo != -1)
            {
                strSql += " and t.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
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


            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }


        public DataTable MerchantToSortingCenterDetail(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount ==0 ?"":" having count(std1.WaybillNo) <@cnt";
            string strSql =
                @"
                select convert(nvarchar(12), std.InSortingTime ,23) as SDate ,std.DetailKid, std.SoringMerchantID,std.SortingCenterID,std.CreateCityID,std.MerchantID,std.InSortingTime ,std.WaybillNo, std.AccountFare,std.WaybillType
               
                into #SortingTransferDetail from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) where std.InSortingTime >=@StartTime and std.InSortingTime<=@EndTime and std.IsDeleted = 0;
                 create index IX_#SortingTransferDetail_1
                 on #SortingTransferDetail(SortingCenterID,MerchantID,SDate,CreateCityID);
               
                with t as 
                  ( 
                   select std1.SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from #SortingTransferDetail std1
                   Join LMS_RFD.dbo.Waybill w on std1.waybillno =w.waybillno 
                   where  w.Sources =2           
                   group by std1.SDate, std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID" + appendStr + @"
                   ) 
                 
                 select * from (                 
  
                 select ROW_NUMBER()OVER (ORDER BY InSortingTime) AS rownum,std.DetailKid,t.SoringMerchantID,ec.CompanyName as SortingCenter,c.CityName as CreateCity,mbi.MerchantName,std.InSortingTime ,std.WaybillNo,case(std.WaybillType)
                  when 0 then '普通订单'
                  when 1 then '上门换货'
                  when 2 then '上门退货'
                  else '普通订单'
                  end as WaybillType

                       from #SortingTransferDetail std(nolock) 
                       Join t on std.SDate = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = std.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       Join RFD_PMS.dbo.ExpressCompany ec (nolock) on std.SortingCenterID = ec.ExpressCompanyID
                       Join RFD_PMS.dbo.City c(nolock) on std.CreateCityID = c.CityID
                       Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID 
                       where 1=1 
              ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            if(Model.InSortingCount >0)
            {
                parameters.Add(new SqlParameter("@cnt", SqlDbType.Int) { Value = Model.InSortingCount });
            }
            
          
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

           
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
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            strSql += " ) g where g.rownum between @StartRowNum and @EndRowNum ";
            strSql += " drop table #SortingTransferDetail";
            parameters.Add(new SqlParameter("@StartRowNum",SqlDbType.Int){Value = Model.startRowNum});
            parameters.Add(new SqlParameter("@EndRowNum",SqlDbType.Int){Value = Model.endRowNum});

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable ExportMerchantToSortingCenterDetail(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : " having count(std1.WaybillNo) <@cnt";
            string strSql =
                @"
                select convert(nvarchar(12), std.InSortingTime ,23) as SDate ,std.DetailKid, std.SoringMerchantID,std.SortingCenterID,std.CreateCityID,std.MerchantID,std.InSortingTime ,std.WaybillNo, std.AccountFare,std.WaybillType
               
                into #SortingTransferDetail from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) where std.InSortingTime >=@StartTime and std.InSortingTime<=@EndTime and std.IsDeleted = 0;
                 create index IX_#SortingTransferDetail_1
                 on #SortingTransferDetail(SortingCenterID,MerchantID,SDate,CreateCityID);
               
                with t as 
                  ( 
                   select std1.SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from #SortingTransferDetail std1
                   Join LMS_RFD.dbo.Waybill w on std1.waybillno =w.waybillno 
                   where  w.Sources =2           
                   group by std1.SDate, std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID" + appendStr + @"
                   ) 
                 
                               
  
                 select '北京柏松物流有限公司'as 拣运商,ec.CompanyName as 分拣中心,c.CityName as 城市,mbi.MerchantName as 商家,std.InSortingTime as 入库时间 ,std.WaybillNo as 运单号,case(std.WaybillType)
                  when 0 then '普通订单'
                  when 1 then '上门换货'
                  when 2 then '上门退货'
                  else '普通订单'
                  end as 运单类型

                       from #SortingTransferDetail std(nolock) 
                       Join t on std.SDate = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = std.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       Join RFD_PMS.dbo.ExpressCompany ec (nolock) on std.SortingCenterID = ec.ExpressCompanyID
                       Join RFD_PMS.dbo.City c(nolock) on std.CreateCityID = c.CityID
                       Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID 
                       where 1=1 
              ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            if (Model.InSortingCount > 0)
            {
                parameters.Add(new SqlParameter("@cnt", SqlDbType.Int) { Value = Model.InSortingCount });
            }


            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });


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
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            strSql += " Order by std.InSortingTime";
            strSql += " drop table #SortingTransferDetail";
           
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable ReverseMerchantToSortingCenterDetail(SortingDetail Model)
        {
             string strSql =
                @"
                select convert(nvarchar(12), std.InSortingTime ,23) as SDate ,std.DetailKid,std.SoringMerchantID,std.SortingCenterID,std.CreateCityID,std.MerchantID,std.InSortingTime ,std.WaybillNo, std.AccountFare,std.WaybillType
               
                into #SortingTransferDetail from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) where std.InSortingTime >=@StartTime and std.InSortingTime<=@EndTime and std.IsDeleted = 0;
                
                create index IX_#SortingTransferDetail_1
                on #SortingTransferDetail(SortingCenterID,MerchantID);
                with t as 
                  ( 
                   select std1.SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from #SortingTransferDetail std1
                   Join LMS_RFD.dbo.Waybill w on std1.waybillno =w.waybillno 
                   where  w.Sources =2           
                   group by std1.SDate, std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID having count(std1.WaybillNo) >= @cnt
                   ) 
                   
                 select std.DetailKid,t.SoringMerchantID,ec.CompanyName as SortingCenter,c.CityName as CreateCity,mbi.MerchantName,std.InSortingTime ,std.WaybillNo,case(std.WaybillType)
                  when 0 then '普通订单'
                  when 1 then '上门换货'
                  when 2 then '上门退货'
                  else '普通订单'
                  end as WaybillType

                       from #SortingTransferDetail std(nolock) 
                       Join t on std.SDate = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = std.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       Join RFD_PMS.dbo.ExpressCompany ec (nolock) on std.SortingCenterID = ec.ExpressCompanyID
                       Join RFD_PMS.dbo.City c(nolock) on std.CreateCityID = c.CityID
                       Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on std.MerchantID = mbi.ID 
                       where 1=1 
              ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@cnt",SqlDbType.Int){Value = Model.InSortingCount});
          
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

           
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
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            strSql += " drop table #SortingTransferDetail";
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }



        public DataTable GetTableByNo(long waybillno)
        {
            string Sqlstr =
                @"select DetailKid,CreateTime from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) where WaybillNo = @WaybillNo";
            SqlParameter[] parameters = {
                                           new SqlParameter("@WaybillNo", SqlDbType.BigInt)
                                       };
            parameters[0].Value = waybillno;
            var ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, Sqlstr, parameters);

            return ds.Tables[0];
        }


        public int CountSortingTransferAndStationDetail(SortingDetail Model)
        {
            string strSql =@"
            select count(1)
                             from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock)
                           
                             Left Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on std.DeliverStationID = ec1.ExpressCompanyID
                          
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =@DistributionCode ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= @StartTime and ToStationTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            }

            if(Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = @WaybillNo " ;
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) {Value = Model.WaybillNo});
            }

            if(!string.IsNullOrEmpty(Model.StationIDs))
            {
                strSql += string.Format(" and std.DeliverStationID in ({0}) ", Model.StationIDs);
            }

            if(!string.IsNullOrEmpty(Model.waybillType))
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

            var num = SqlHelperEx.ExecuteScalar(ReadOnlyConnection,CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public int CountSortingToCityDetail(SortingDetail Model)
        {
            string strSql =
               @"
                             select count(1)
                             from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                             Join LMS_RFD.dbo.Waybill w(nolock) on std.Waybillno =w.WaybillNo 
                             Join RFD_PMS.dbo.ExpressCompany ec2(nolock) on std.TopCODCompanyID = ec2.ExpressCompanyID
                             Join RFD_PMS.dbo.Distribution d(nolock) on ec2.DistributionCode = d.DistributionCode
                             where 1=1 and std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and OutBoundTime >= @StartTime and OutBoundTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            }
            if (Model.WaybillNo != -1)
            {
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
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
            var num = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }

        public int CountReturnToSortingCenterDetail(SortingDetail Model)
        {
            string strSql =
                @" 
            WITH t AS  (  select count(1) as rownum
                  from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.WarehouseSortRelation wsr (nolock) on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ec.ExpressCompanyID = wsr.SortationID
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = @DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   ";

            if (Model.WaybillNo != -1)
            {
                strSql += string.Format(" and std.WaybillNo = {0}",Model.WaybillNo);
               
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

                   select count(1) as rownum
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on cast(ec.ExpressCompanyID as nvarchar(20)) =std.ReturnSortingCenterID
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                 
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
                   string MerchantIDs =MerchantForOuter(Model.MerchantIDs);
                   if(!string.IsNullOrEmpty(MerchantIDs))
                   {
                       strSql += string.Format(" and std.MerchantID in ({0})",MerchantIDs);
                   }
                   
               }

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
            parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar) { Value = Model.DistributionCode });

            strSql += @")  Select sum(t.rownum) from t";

            var num = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }
        
        private string MerchantForOuter(string MerchantIDs)
        {
            string[] ss = MerchantIDs.Split(',');
            string ret = "";
            foreach (var s in ss)
            {
                if(s!="8" && s!="9")
                {
                    ret += s + ",";
                }
            }
            return ret.Trim(',');
        }

        private string ExpressCompanyForThirdParty(string ExpressCompanyIDs)
        {
            string[] ss = ExpressCompanyIDs.Split(',');
            string ret = "";
            foreach (var s in ss)
            {
                if (s != "11" )
                {
                    ret += s + ",";
                }
            }
            return ret.Trim(',');
        }
        public int CountMerchantToSortingCenterDetail(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : " having count(std1.WaybillNo) <@cnt";
            string strSql =
                @"
                select convert(nvarchar(12), std.InSortingTime ,23) as SDate ,std.DetailKid, std.SoringMerchantID,std.SortingCenterID,std.CreateCityID,std.MerchantID,std.InSortingTime ,std.WaybillNo, std.AccountFare,std.WaybillType
               
                into #SortingTransferDetail from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) where std.InSortingTime >=@StartTime and std.InSortingTime<=@EndTime and std.IsDeleted = 0;
                 create index IX_#SortingTransferDetail_1
                 on #SortingTransferDetail(SortingCenterID,MerchantID,SDate,CreateCityID);
               
                with t as 
                  ( 
                   select std1.SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from #SortingTransferDetail std1
                   Join LMS_RFD.dbo.Waybill w on std1.waybillno =w.waybillno 
                   where  w.Sources =2           
                   group by std1.SDate, std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID" + appendStr + @"
                   ) 
                   
                 select Count(1)

                       from #SortingTransferDetail std(nolock) 
                       Join t on std.SDate = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = t.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       where 1=1 
              ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            if (Model.InSortingCount > 0)
            {
                parameters.Add(new SqlParameter("@cnt", SqlDbType.Int) { Value = Model.InSortingCount });
            }


            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });


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
                strSql += " and std.WaybillNo = @WaybillNo ";
                parameters.Add(new SqlParameter("@WaybillNo", SqlDbType.BigInt) { Value = Model.WaybillNo });
            }


            if (!string.IsNullOrEmpty(Model.waybillType))
            {
                strSql += string.Format(" and std.WaybillType in ({0})", Model.waybillType);
            }

            strSql += " drop table #SortingTransferDetail";
            var num = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return Convert.ToInt32(num);
        }
    }
}
