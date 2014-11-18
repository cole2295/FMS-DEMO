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
    public class SortingTransferSearchingDao : OracleDao, ISortingTransferSearchingDao
    {

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
                             else '普通订单' 
                             end as WaybillType ,mbi.MerchantName  
                             from  FMS_SortingTransferDetail std
                             Left Join PS_PMS.ExpressCompany ec on std.TSortingCenterID =ec.ExpressCompanyID
                             Left Join PS_PMS.City c on std.SortCityID = c.CityID
                             Left Join PS_PMS.ExpressCompany ec1 on std.DeliverStationID = ec1.ExpressCompanyID
                             Left Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID 
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =:DistributionCode";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            if (!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= :StartTime and ToStationTime <=:EndTime ";
                parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
                parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });

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
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityDetail(SortingDetail Model)
        {
            string strSql =
                @"
                          with t as (
                             select ROW_NUMBER() OVER(ORDER BY std.OutboundTime) AS num,std.DetailKid,std.SoringMerchantID,'' As SortingCenter,ec1.CompanyName as OutSortingCenter,c.CityName as CreateCity,d.DistributionName as Distribution,std.OutBoundTime,std.WaybillNo,case (std.WaybillType)
                             when '0' then '普通订单'
                             when '1' then '上门换货'
                             when '2' then '上门退货' 
                             else '普通订单' 
                             end as WaybillType ,mbi.MerchantName 
                             from FMS_SortingTransferDetail std 
                             Join PS_PMS.ExpressCompany ec1 on std.SortingCenterID = ec1.ExpressCompanyID
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

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120,CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];


        }

        public DataTable ReturnToSortingCenterDetail(MODEL.FinancialManage.SortingDetail Model)
        {
            string strSql =
                @" 
           WITH t AS  (
                   select std.DetailKid,std.SoringMerchantID,wsr.SortationID as SortingCenter, std.distributionCode, std.MerchantID, std.ReturnTime ,std.WaybillNo, std.WaybillType, std.TOPCODCompanyID,
                  
                   std.ReturnSortingCenterID
                   from FMS_SortingTransferDetail std
                   Join PS_PMS.WarehouseSortRelation wsr  on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = :DistributionCode 
                   where  std.MerchantID in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   
                   union all 

                   select std.DetailKid,std.SoringMerchantID,ec.ExpressCompanyID as SortingCenter, std.distributionCode, std.MerchantID , std.ReturnTime ,std.WaybillNo, std.WaybillType,std.TOPCODCompanyID,
                   std.ReturnSortingCenterID
                   from FMS_SortingTransferDetail std
                   left Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID) =std.ReturnSortingCenterID
                   where  std.MerchantID not in(8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                 )
               SELECT * FROM ( 
               select ROW_NUMBER()OVER (ORDER BY t.ReturnTime) AS num,t.DetailKid,
               t.SoringMerchantID,
               ec1.CompanyName as SortingCenter ,
               d.DistributionName as Distribution,
               mbi.MerchantName,
               t.ReturnTime,
               t.WaybillNo,
               case(t.WaybillType)
                  when '0' then '普通订单'
                  when '1' then '上门换货'
                  when '2' then '上门退货'
                  else '普通订单'
                  end as WaybillType
              from t 
               Left Join PS_PMS.ExpressCompany ec on t.TopCODCompanyID =ec.ExpressCompanyID
               Left Join PS_PMS.ExpressCompany ec1 on t.SortingCenter =ec1.ExpressCompanyID
               Left Join PS_PMS.Distribution d on ec.DistributionCode = d.DistributionCode
               Left Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.id
             where 1=1 ";

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
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
            string appendStr = Model.InSortingCount == 0 ? "" : "having count(std1.WaybillNo) <:cnt ";
            string strSql =
                @"
                with t as 
                  ( 
                   select to_char(std1.insortingtime,'yyyy-mm-dd') as SDate, std1.SoringMerchantID,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID,count(std1.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std1
                   where  std1.merchantid not in(8,9) and std1.insortingtime > :StartTime and std1.insortingtime <=:EndTime          
                   group by to_char(std1.insortingtime,'yyyy-mm-dd'), std1.SoringMerchantID ,std1.SortingCenterID ,std1.CreateCityID,std1.MerchantID " + appendStr + @"
                   ) 
                 
                 select * from (                 
  
                 select ROW_NUMBER()OVER (ORDER BY InSortingTime) AS num,std.DetailKid,t.SoringMerchantID,ec.CompanyName as SortingCenter,c.CityName as CreateCity,mbi.MerchantName,std.InSortingTime ,std.WaybillNo,case(std.WaybillType)
                  when '0' then '普通订单'
                  when '1' then '上门换货'
                  when '2' then '上门退货'
                  else '普通订单'
                  end as WaybillType

                       from FMS_SortingTransferDetail std 
                       Join t on to_char(std.insortingtime,'yyyy-mm-dd') = t.SDate 
                       --and std.SoringMerchantID = t.SoringMerchantID 
                       and std.SortingCenterID = t.SortingCenterID  
                       and std.CreateCityID = t.CreateCityID 
                       and std.MerchantID = t.MerchantID
                       Join PS_PMS.ExpressCompany ec  on std.SortingCenterID = ec.ExpressCompanyID
                       Join PS_PMS.City c on std.CreateCityID = c.CityID
                       Join PS_PMS.MerchantBaseInfo mbi on std.MerchantID = mbi.ID 
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

            strSql += " ) g where g.num between :StartRowNum and :EndRowNum ";

            parameters.Add(new OracleParameter(":StartRowNum", OracleDbType.Int32) { Value = Model.startRowNum });
            parameters.Add(new OracleParameter(":EndRowNum", OracleDbType.Int32) { Value = Model.endRowNum });
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection,120,CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable SortingTransferAndToStationDaily(SortingDetail Model)
        {
            string strSql = @" 
                 with  t as
                 (
                   select to_char(std.ToStationTime,'yyyy-mm-dd') as SDate, std.SoringMerchantID,std.TSortingCenterID ,std.SortCityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from FMS_SortingTransferDetail std  
                   Join PS_PMS.ExpressCompany ec on std.DeliverStationID = ec.ExpressCompanyID 
                   where  std.IntoType =1 and std.IsDeleted = 0 and ec.DistributionCode =:DistributionCode and std.ToStationTime >= :StartTime and std.ToStationTime <= :EndTime 
                   group by to_char(std.ToStationTime,'yyyy-mm-dd'), std.SoringMerchantID,std.TSortingCenterID,std.SortCityID      
                 ) 
                 select t.SDate,t.SoringMerchantID
                 ,ec.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join PS_PMS.ExpressCompany ec on t.TSortingCenterID = ec.ExpressCompanyID
                 Join PS_PMS.City c on t.SortCityID = c.CityID where 1=1 
                  
                  ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });


            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }


            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.SortCityID in ({0})", Model.CityIDs);
            }

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable SortingToCityDaily(SortingDetail Model)
        {
            string strSql = string.Format(
                @"
               with  t as
                 (
                   select to_char(std.OutBoundTime,'yyyy-mm-dd') as SDate, std.SoringMerchantID ,std,SortingCenterID,std.CreateCityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from FMS_SortingTransferDetail std 
                   where  std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)  and std.SortingCenterID in ({0}) 
                          and std.OutBoundTime >= :StartTime and std.OutBoundTime <=:EndTime
                   group by to_char(std.OutBoundTime,'yyyy-mm-dd'), std.SoringMerchantID,std.SortingCenterID,std.CreateCityID     
                 ) 
                  select t.SDate, t.SoringMerchantID 
                 ,ec1.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join PS_PMS.ExpressCompany ec1 on t.SortingCenterID = ec1.ExpressCompanyID
                 Join PS_PMS.City c on t.CreateCityID = c.CityID  where 1=1
               ", Model.SortingCenterIDs);
            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });



            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable ReturnToSortingCenterDaily(SortingDetail Model)
        {
            string strSql =
                @"
                  with t as
                (   
                   select to_char(std.ReturnTime,'yyyy-mm-dd') as SDate, std.SoringMerchantID,wsr.SortationID as SortingCenter, ec.CityID, std.WaybillNo , std.AccountFare 
                   from FMS_SortingTransferDetail std 
                   Join PS_PMS.WarehouseSortRelation wsr on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID 
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = :DistributionCode
                   where  std.MerchantID  in (8,9)  and std.ReturnTime >= :StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0            
                  
                   
                   union all
                   
                   select  to_char(std.ReturnTime,'yyyy-mm-dd') as SDate, std.SoringMerchantID, ec.ExpressCompanyID as SortingCenter, ec.CityID , std.WaybillNo ,std.AccountFare
                   from FMS_SortingTransferDetail std 
                   Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID) = std.ReturnSortingCenterID 
                   where  std.MerchantID  not in (8,9)  and std.ReturnTime >= :StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   ) 
                   select t.SDate,t.SoringMerchantID,ec1.CompanyName as SortingCenterAll,c.CityName as City ,count(t.WaybillNo) as WaybillSum, null as price ,sum (t.AccountFare) as Fee
                   from t
                   Join PS_PMS.ExpressCompany ec1 on t.SortingCenter= ec1.ExpressCompanyID
                   Join PS_PMS.City c on t.CityID = c.CityID  where 1=1 ";

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
            }


            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            strSql += " group by t.SDate,t.SoringMerchantID, ec1.CompanyName, c.CityName";

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable MerchantToSortingCenterDaily(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "  having count(std.WaybillNo) <:cnt";
            string strSql =
                @"
                with t as 
                  ( 
                   select  to_char(std.InSortingTime ,'yyyy-mm-dd') as SDate,std.SortingCenterID ,CreateCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from FMS_SortingTransferDetail std 
                   where  std.MerchantID not in (8,9)   
                          and std.InSortingTime >= :StartTime and std.InSortingTime <=:EndTime  and std.IsDeleted = 0       
                   group by to_char(std.InSortingTime ,'yyyy-mm-dd'),std.SoringMerchantID ,std.SortingCenterID ,CreateCityID,std.MerchantID " + appendStr + @"
                   ) 
                   select t.SDate,t.SoringMerchantID,ec.CompanyName as SortingCenterAll,c.CityName as City, sum(t.WaybillSum) as WaybillSum , null as Price, sum(t.Fee) as Fee
                   from t 
                   Join PS_PMS.ExpressCompany ec on t.SortingCenterID = ec.ExpressCompanyID
                   Join PS_PMS.City c on t.CreateCityID = c.CityID where 1=1 

             

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
                strSql += string.Format(" and t.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            strSql += "Group by t.SDate,t.SoringMerchantID,ec.CompanyName ,c.CityName ";

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }


        public DataTable SortingTransferAndToStationMerchant(SortingDetail Model)
        {
            string strSql = @" 
                 with  t as
                 (
                   select to_char(std.ToStationTime,'yyyy-mm-dd') as SDate, std.SoringMerchantID,std.TSortingCenterID ,std.SortCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std  
                   Join PS_PMS.ExpressCompany ec on std.DeliverStationID = ec.ExpressCompanyID  
                   where  std.IntoType = 1  and ec.DistributionCode = :DistributionCode  
                          and std.ToStationTime >= :StartTime and std.ToStationTime <=:EndTime and std.IsDeleted = 0
                   group by to_char(std.ToStationTime,'yyyy-mm-dd'), std.SoringMerchantID,std.TSortingCenterID,std.SortCityID ,std.MerchantID     
                 ) 
                 select t.SDate,t.SoringMerchantID 
                 ,ec.CompanyName as SortingCenterAll,t.TSortingCenterID,c.CityID,t.MerchantID
                  ,c.CityName as City,
                 mbi.MerchantName,
                 t.WaybillSum from t
                 Join PS_PMS.ExpressCompany ec on t.TSortingCenterID = ec.ExpressCompanyID
                 Join PS_PMS.City c on t.SortCityID = c.CityID
                 Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.ID 
                 where 1=1 
                  ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });


            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.SortCityID in ({0})", Model.CityIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityMerchant(SortingDetail Model)
        {
            string strSql = string.Format(
                @"
               with  t as
                 (
                   select to_char(std.OutBoundTime,'yyyy-mm-dd') as SDate, std.SoringMerchantID,std.SortingCenterID ,std.CreateCityID,std.MerchantID, count(std.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std 
                   where  std.OutType = 1 and std.MerchantID not in (8,9) and std.SortingCenterID in ({0})
                   and std.OutBoundTime >= :StartTime and std.OutBoundTime <=:EndTime and std.IsDeleted = 0
                   group by to_char(std.OutBoundTime,'yyyy-mm-dd'), std.SoringMerchantID,std.SortingCenterID,std.CreateCityID ,std.MerchantID    
                 ) 
                  select t.SDate,t.SoringMerchantID
                 ,ec.CompanyName as SortingCenterAll,t.SortingCenterID,c.CityID,t.MerchantID
                  ,c.CityName as City,
                  mbi.MerchantName,
                  t.WaybillSum from t
                 Join PS_PMS.ExpressCompany ec on t.SortingCenterID = ec.ExpressCompanyID
                 Join PS_PMS.City c on t.CreateCityID = c.CityID 
                 Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.ID  
                  where 1=1
               ", Model.SortingCenterIDs);
            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });



            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable ReturnToSortingCenterMerchant(SortingDetail Model)
        {
            string strSql =
               @"
                with t as
                (   
                   select  to_char(std.ReturnTime,'yyyy-mm-dd') as SDate,std.SoringMerchantID,wsr.SortationID as SortingCenter, ec.CityID,std.MerchantID, std.WaybillNo , std.AccountFare 
                   from FMS_SortingTransferDetail std 
                   Join PS_PMS.WarehouseSortRelation wsr  on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID 
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode=:DistributionCode
                   where  std.MerchantID  in (8,9)  and std.ReturnTime >= :StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   
                   union all
                   
                   select  to_char(std.ReturnTime,'yyyy-mm-dd') as SDate,std.SoringMerchantID, ec.ExpressCompanyID as SortingCenter, ec.CityID ,std.MerchantID, std.WaybillNo ,std.AccountFare
                   from FMS_SortingTransferDetail std
                   Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID) = std.ReturnSortingCenterID 
                   where   std.MerchantID  not in (8,9)  and std.ReturnTime >= :StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   ) 
                   select t.SDate,t.SoringMerchantID,ec1.CompanyName as SortingCenterAll,c.CityName as City , mbi.MerchantName ,count(t.WaybillNo) as WaybillSum
                   ,t.SortingCenter,c.CityID,t.MerchantID
                   from t
                   Join PS_PMS.ExpressCompany ec1 on t.SortingCenter= ec1.ExpressCompanyID
                   Join PS_PMS.City c on t.CityID = c.CityID 
                   Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.ID
                   where 1=1 ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            ;
            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });


            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            strSql += " group by t.SDate,t.SoringMerchantID, ec1.CompanyName, c.CityName , mbi.MerchantName,t.SortingCenter,c.CityID,t.MerchantID";

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable MerchantToSortingCenterMerchant(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "  having count(std.WaybillNo) <:cnt";
            string strSql =
               @"
             with t as 
                  ( 
                   select  to_char(std.InSortingTime ,'yyyy-mm-dd') as SDate, std.SoringMerchantID,std.SortingCenterID ,CreateCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum 
                   from FMS_SortingTransferDetail std
                   
                   where  std.MerchantID not in (8,9) 
                         and std.InSortingTime >= :StartTime and std.InSortingTime <=:EndTime  and std.IsDeleted = 0        
                   group by to_char(std.InSortingTime ,'yyyy-mm-dd'),std.SoringMerchantID ,std.SortingCenterID ,CreateCityID,std.MerchantID" + appendStr + @"
                   ) 
                   select t.SDate,t.SoringMerchantID,ec.CompanyName as SortingCenterAll,c.CityName as City,mbi.MerchantName, t.WaybillSum
                    ,t.SortingCenterID,c.CityID,t.MerchantID
                   from t 
                   Join PS_PMS.ExpressCompany ec on t.SortingCenterID = ec.ExpressCompanyID
                   Join PS_PMS.MerchantBaseInfo mbi on t.MerchantID = mbi.ID 
                   Join PS_PMS.City c on t.CreateCityID = c.CityID where 1=1
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
                strSql += string.Format(" and t.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable SortingTransferAndToStationAll(SortingDetail Model)
        {
            string strSql = @" 
                 with  t as
                 (
                   select std.SoringMerchantID,std.TSortingCenterID ,std.SortCityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from FMS_SortingTransferDetail std 
                   Join PS_PMS.ExpressCompany ec on std.DeliverStationID = ec.ExpressCompanyID   
                   where  std.IntoType =1 and ec.DistributionCode =:DistributionCode  
                          and std.ToStationTime >= :StartTime and std.ToStationTime <=:EndTime and std.IsDeleted = 0 
                   group by std.SoringMerchantID,std.TSortingCenterID,std.SortCityID      
                 ) 
                 select '合计' as StatisticsType, t.SoringMerchantID 
                 ,ec.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join PS_PMS.ExpressCompany ec on t.TSortingCenterID = ec.ExpressCompanyID
                 Join PS_PMS.City c on t.SortCityID = c.CityID where 1=1 
                  
                  ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });
            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.SortCityID in ({0})", Model.CityIDs);
            }

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityAll(SortingDetail Model)
        {
            string strSql = string.Format(
               @"
               with  t as
                 (
                   select  std.SoringMerchantID,std.SortingCenterID ,ec.CityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from FMS_SortingTransferDetail std 
                   where  std.OutType = 1 and std.MerchantID not in (8,9) and std.SortingCenterID in ({0})
                   and std.OutBoundTime >=:StartTime and std.OutBoundTime <=:EndTime and std.IsDeleted = 0
                   group by std.SoringMerchantID,std.SortingCenterID,ec.CityID     
                 ) 
                  select '合计'as StatisticsType, t.SoringMerchantID 
                 ,ec.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join PS_PMS.dbo.ExpressCompany ec on t.CreatStation = ec.ExpressCompanyID
                 Join PS_PMS.dbo.City c on t.CityID = c.CityID where 1=1
               ", Model.SortingCenterIDs);

            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value =DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable ReturnToSortingCenterAll(SortingDetail Model)
        {
            string strSql =
               @"
                  with t as
                (   
                   select  std.SoringMerchantID,wsr.SortationID as SortingCenter, ec.CityID, std.WaybillNo , std.AccountFare 
                   from FMS_SortingTransferDetail std 
                   Join PS_PMS.WarehouseSortRelation wsr  on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join PS_PMS.ExpressCompany ec on ec.ExpressCompanyID = wsr.SortationID
                   Join PS_PMS.ExpressCompany ec0 on ec0.ExpressCompanyID = std.TopCODCompanyID  and ec0.DistributionCode = :DistributionCode
                   where std.MerchantID  in (8,9) and std.ReturnTime >=:StartTime and std.ReturnTime <=:EndTime and std.IsDeleted = 0
                   
                   union all
                   
                   select  std.SoringMerchantID, ec.ExpressCompanyID as SortingCenter, ec.CityID , std.WaybillNo ,std.AccountFare
                   from FMS_SortingTransferDetail std
                   Join PS_PMS.ExpressCompany ec on to_char(ec.ExpressCompanyID) = std.ReturnSortingCenterID 
                   where std.MerchantID  not in (8,9)  and std.ReturnTime >=:StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   ) 
                   select '合计'as StatisticsType, t.SoringMerchantID,ec1.CompanyName as SortingCenterAll,c.CityName as City ,count(t.WaybillNo) as WaybillSum, null as price ,sum (t.AccountFare) as Fee
                   from t
                   Join PS_PMS.ExpressCompany ec1 on t.SortingCenter= ec1.ExpressCompanyID
                   Join PS_PMS.City c on t.CityID = c.CityID  where 1=1 ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.StartTime) });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = DataConvert.ToDateTime(Model.EndTime) });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = Model.DistributionCode });

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            strSql += " group by t.SoringMerchantID, ec1.CompanyName, c.CityName";

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable MerchantToSortingCenterAll(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "  having count(std.WaybillNo) <:cnt";
            string strSql =
               @"
             with t as 
                  ( 
                   select  to_char(std.InSortingTime ,'yyyy-mm-dd') as SDate, std.SoringMerchantID,std.SortingCenterID ,CreateCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from FMS_SortingTransferDetail std
                   
                   where std.MerchantID not in (8,9)
                   and std.InSortingTime >= :StartTime  and std.InSortingTime <=:EndTime  and std.IsDeleted = 0      
                   group by to_char(std.InSortingTime ,'yyyy-mm-dd'),std.SoringMerchantID ,std.SortingCenterID ,CreateCityID,std.MerchantID " + appendStr + @"
                   ) 
                   select '合计'as StatisticsType, t.SoringMerchantID,ec.CompanyName as SortingCenterAll,c.CityName as City, sum(t.WaybillSum) as WaybillSum , null as Price, sum(t.Fee) as Fee
                   from t 
                   Join PS_PMS.ExpressCompany ec on t.SortingCenterID = ec.ExpressCompanyID
                   Join PS_PMS.City c on t.CreateCityID = c.CityID where 1=1 
                  
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
                strSql += string.Format(" and t.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }


            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            strSql += "  Group by t.SoringMerchantID,ec.CompanyName ,c.CityName ";
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }
    }
}
