using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.ApplicationBlocks.Data.Extension;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.FinancialManage
{
    class SortingTransferSearchingDao : SqlServerDao, ISortingTransferSearchingDao
    {

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
                             where 1=1 and std.IntoType =1 and std.IsDeleted = 0 and ec1.DistributionCode =@DistributionCode";
             List<SqlParameter> parameters = new List<SqlParameter>(); 
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});

            if(!string.IsNullOrEmpty(Model.StartTime) && !string.IsNullOrEmpty(Model.EndTime))
            {
                strSql += " and ToStationTime >= @StartTime and ToStationTime <=@EndTime ";
                parameters.Add(new SqlParameter("@StartTime",SqlDbType.DateTime){Value = Model.StartTime});
                parameters.Add(new SqlParameter("@EndTime",SqlDbType.DateTime){Value = Model.EndTime});

            }
            if(!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and std.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and std.MerchantID in ({0})", Model.MerchantIDs);
            }

            if(!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and std.SortCityID in ({0})", Model.CityIDs);
            }

            strSql += ") Select * from t where t.rownum between @StartRowNum  and @EndRowNum";
            parameters.Add(new SqlParameter("@StartRowNum", SqlDbType.Int) { Value = Model.startRowNum });
            parameters.Add(new SqlParameter("@EndRowNum", SqlDbType.Int) { Value = Model.endRowNum });
            var ds =SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityDetail(SortingDetail Model)
        {
            string strSql =
                @"
                          with t as (
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

        public DataTable ReturnToSortingCenterDetail(MODEL.FinancialManage.SortingDetail Model)
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
           
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
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

        public DataTable MerchantToSortingCenterDetail(MODEL.FinancialManage.SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "having count(std1.WaybillNo) <@cnt ";
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
                       and std.CreateCityID = t.CreateCityID 
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

            strSql += " ) g where g.rownum between @StartRowNum and @EndRowNum ";
            strSql += " drop table #SortingTransferDetail";
            parameters.Add(new SqlParameter("@StartRowNum", SqlDbType.Int) { Value = Model.startRowNum });
            parameters.Add(new SqlParameter("@EndRowNum", SqlDbType.Int) { Value = Model.endRowNum });
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable SortingTransferAndToStationDaily(SortingDetail Model)
        {
            string strSql = @" 
                 with  t as
                 (
                   select convert(nvarchar(12),std.ToStationTime,23) as SDate, std.SoringMerchantID,std.TSortingCenterID ,std.SortCityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std (nolock)  
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on std.DeliverStationID = ec.ExpressCompanyID 
                   where  std.IntoType =1 and std.IsDeleted = 0 and ec.DistributionCode =@DistributionCode and std.ToStationTime >= @StartTime and std.ToStationTime <= @EndTime 
                   group by convert(nvarchar(12),std.ToStationTime,23), std.SoringMerchantID,std.TSortingCenterID,std.SortCityID      
                 ) 
                 select t.SDate,t.SoringMerchantID
                 ,ec.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.TSortingCenterID = ec.ExpressCompanyID
                 Join RFD_PMS.dbo.City c(nolock) on t.SortCityID = c.CityID where 1=1 
                  
                  ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});
            
            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

            
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }
           

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.SortCityID in ({0})", Model.CityIDs);
            }

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable SortingToCityDaily(SortingDetail Model)
        {
            string strSql =string.Format(
                @"
               with  t as
                 (
                   select convert(nvarchar(12),std.OutBoundTime,23) as SDate, std.SoringMerchantID, w.CreatStation ,ec.CityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std (nolock) 
                   Join LMS_RFD.dbo.Waybill w(nolock) on std.WaybillNo = w.WaybillNo 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.CreatStation = ec.ExpressCompanyID 
                   where  std.OutType =1 and std.IsDeleted = 0 and std.MerchantID not in (8,9)  and std.SortingCenterID in ({0}) 
                          and std.OutBoundTime >= @StartTime and std.OutBoundTime <=@EndTime
                   group by convert(nvarchar(12),std.OutBoundTime,23), std.SoringMerchantID,w.CreatStation,ec.CityID     
                 ) 
                  select t.SDate, t.SoringMerchantID 
                 ,ec1.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on t.CreatStation = ec1.ExpressCompanyID
                 Join RFD_PMS.dbo.City c(nolock) on t.CityID = c.CityID where 1=1
               ", Model.SortingCenterIDs);
            List<SqlParameter> parameters = new List<SqlParameter>();
            
            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

           

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable ReturnToSortingCenterDaily(SortingDetail Model)
        {
            string strSql =
                @"
                  with t as
                (   
                   select  convert(nvarchar(12), std.ReturnTime ,23) as SDate, std.SoringMerchantID,wsr.SortationID as SortingCenter, ec.CityID, std.WaybillNo , std.AccountFare 
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.WarehouseSortRelation wsr (nolock) on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ec.ExpressCompanyID = wsr.SortationID 
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode = @DistributionCode
                   where  std.MerchantID  in (8,9)  and std.ReturnTime >= @StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0            
                  
                   
                   union all
                   
                   select  convert(nvarchar(12), std.ReturnTime ,23) as SDate, std.SoringMerchantID, ec.ExpressCompanyID as SortingCenter, ec.CityID , std.WaybillNo ,std.AccountFare
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on cast(ec.ExpressCompanyID as nvarchar(20)) = std.ReturnSortingCenterID 
                   where  std.MerchantID  not in (8,9)  and std.ReturnTime >= @StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   ) 
                   select t.SDate,t.SoringMerchantID,ec1.CompanyName as SortingCenterAll,c.CityName as City ,count(t.WaybillNo) as WaybillSum, null as price ,sum (t.AccountFare) as Fee
                   from t
                   Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on t.SortingCenter= ec1.ExpressCompanyID
                   Join RFD_PMS.dbo.City c(nolock) on t.CityID = c.CityID  where 1=1 ";

            List<SqlParameter> parameters = new List<SqlParameter>();
           
            parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});
          
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
            }
          

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            strSql += " group by t.SDate,t.SoringMerchantID, ec1.CompanyName, c.CityName";
           
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable MerchantToSortingCenterDaily(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "  having count(std.WaybillNo) <@cnt";
            string strSql =
                @"
                with t as 
                  ( 
                   select  convert(nvarchar(12), std.InSortingTime ,23) as SDate,std.SortingCenterID ,CreateCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock)
                   Join LMS_RFD.dbo.Waybill w on std.waybillno =w.waybillno 
                   where  std.InSortingTime is not null and w.Sources =2   
                          and std.InSortingTime >= @StartTime and std.InSortingTime <=@EndTime  and std.IsDeleted = 0       
                   group by convert(nvarchar(12), std.InSortingTime ,23),std.SoringMerchantID ,std.SortingCenterID ,CreateCityID,std.MerchantID " + appendStr+ @"
                   ) 
                   select t.SDate,t.SoringMerchantID,ec.CompanyName as SortingCenterAll,c.CityName as City, sum(t.WaybillSum) as WaybillSum , null as Price, sum(t.Fee) as Fee
                   from t 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.SortingCenterID = ec.ExpressCompanyID
                   Join RFD_PMS.dbo.City c(nolock) on t.CreateCityID = c.CityID where 1=1 

             

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
                strSql += string.Format(" and t.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }         

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            strSql += "Group by t.SDate,t.SoringMerchantID,ec.CompanyName ,c.CityName ";

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120,CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];

        }


        public DataTable SortingTransferAndToStationMerchant(SortingDetail Model)
        {
            string strSql = @" 
                 with  t as
                 (
                   select convert(nvarchar(12),std.ToStationTime,23) as SDate, std.SoringMerchantID,std.TSortingCenterID ,std.SortCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum 
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std (nolock) 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on std.DeliverStationID = ec.ExpressCompanyID  
                   where  std.IntoType = 1  and ec.DistributionCode = @DistributionCode  
                          and std.ToStationTime >= @StartTime and std.ToStationTime <=@EndTime and std.IsDeleted = 0
                   group by convert(nvarchar(12),std.ToStationTime,23), std.SoringMerchantID,std.TSortingCenterID,std.SortCityID ,std.MerchantID     
                 ) 
                 select t.SDate,t.SoringMerchantID 
                 ,ec.CompanyName as SortingCenterAll,t.TSortingCenterID,c.CityID,t.MerchantID
                  ,c.CityName as City,
                 mbi.MerchantName,
                 t.WaybillSum from t
                 Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.TSortingCenterID = ec.ExpressCompanyID
                 Join RFD_PMS.dbo.City c(nolock) on t.SortCityID = c.CityID
                 Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on t.MerchantID = mbi.ID 
                 where 1=1 
                  ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});
           
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

           
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.SortCityID in ({0})", Model.CityIDs);
            }

            if(!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityMerchant(SortingDetail Model)
        {
            string strSql =string.Format(
                @"
               with  t as
                 (
                   select convert(nvarchar(12),std.OutBoundTime,23) as SDate, std.SoringMerchantID,std.SortingCenterID ,std.CreateCityID,std.MerchantID, count(std.WaybillNo) as WaybillSum 
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std (nolock)
                   --Join LMS_RFD.dbo.Waybill w(nolock) on std.WaybillNo = w.WaybillNo
                   --Join RFD_PMS.dbo.ExpressCompany ec(nolock) on std.SortingCenterID = ec.ExpressCompanyID   
                   where  std.OutType = 1 and std.MerchantID not in (8,9) and std.SortingCenterID in ({0})
                   and std.OutBoundTime >= @StartTime and std.OutBoundTime <=@EndTime and std.IsDeleted = 0
                   group by convert(nvarchar(12),std.OutBoundTime,23), std.SoringMerchantID,std.SortingCenterID,std.CreateCityID ,std.MerchantID    
                 ) 
                  select t.SDate,t.SoringMerchantID
                 ,ec.CompanyName as SortingCenterAll,t.SortingCenterID,c.CityID,t.MerchantID
                  ,c.CityName as City,
                  mbi.MerchantName,
                  t.WaybillSum from t
                 Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.SortingCenterID = ec.ExpressCompanyID
                 Join RFD_PMS.dbo.City c(nolock) on t.CreateCityID = c.CityID 
                 Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on t.MerchantID = mbi.ID  
                  where 1=1
               ", Model.SortingCenterIDs);
            List<SqlParameter> parameters = new List<SqlParameter>();
            
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });

          
            
            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            if (!string.IsNullOrEmpty(Model.MerchantIDs))
            {
                strSql += string.Format(" and t.MerchantID in ({0})", Model.MerchantIDs);
            }

            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable ReturnToSortingCenterMerchant(SortingDetail Model)
        {
            string strSql =
               @"
                with t as
                (   
                   select  convert(nvarchar(12), std.ReturnTime ,23) as SDate,std.SoringMerchantID,wsr.SortationID as SortingCenter, ec.CityID,std.MerchantID, std.WaybillNo , std.AccountFare 
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.WarehouseSortRelation wsr (nolock) on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ec.ExpressCompanyID = wsr.SortationID 
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID and ec0.DistributionCode=@DistributionCode
                   where  std.MerchantID  in (8,9)  and std.ReturnTime >= @StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   
                   union all
                   
                   select  convert(nvarchar(12), std.ReturnTime ,23) as SDate,std.SoringMerchantID, ec.ExpressCompanyID as SortingCenter, ec.CityID ,std.MerchantID, std.WaybillNo ,std.AccountFare
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on cast(ec.ExpressCompanyID as nvarchar(20)) = std.ReturnSortingCenterID 
                   where   std.MerchantID  not in (8,9)  and std.ReturnTime >= @StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   ) 
                   select t.SDate,t.SoringMerchantID,ec1.CompanyName as SortingCenterAll,c.CityName as City , mbi.MerchantName ,count(t.WaybillNo) as WaybillSum
                   ,t.SortingCenter,c.CityID,t.MerchantID
                   from t
                   Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on t.SortingCenter= ec1.ExpressCompanyID
                   Join RFD_PMS.dbo.City c(nolock) on t.CityID = c.CityID 
                   Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on t.MerchantID = mbi.ID
                   where 1=1 ";

            List<SqlParameter> parameters = new List<SqlParameter>();
            ;
                parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
                parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
                parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});

           
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
           
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120,CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable MerchantToSortingCenterMerchant(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "  having count(std.WaybillNo) <@cnt";
            string strSql =
               @"
             with t as 
                  ( 
                   select  convert(nvarchar(12), std.InSortingTime ,23) as SDate, std.SoringMerchantID,std.SortingCenterID ,CreateCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum 
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock)
                   Join LMS_RFD.dbo.Waybill w on std.waybillno =w.waybillno 
                   where  std.InSortingTime is not null and w.Sources =2  
                         and std.InSortingTime >= @StartTime and std.InSortingTime <=@EndTime  and std.IsDeleted = 0        
                   group by convert(nvarchar(12), std.InSortingTime ,23),std.SoringMerchantID ,std.SortingCenterID ,CreateCityID,std.MerchantID" + appendStr + @"
                   ) 
                   select t.SDate,t.SoringMerchantID,ec.CompanyName as SortingCenterAll,c.CityName as City,mbi.MerchantName, t.WaybillSum
                    ,t.SortingCenterID,c.CityID,t.MerchantID
                   from t 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.SortingCenterID = ec.ExpressCompanyID
                   Join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on t.MerchantID = mbi.ID 
                   Join RFD_PMS.dbo.City c(nolock) on t.CreateCityID = c.CityID where 1=1
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
            var ds = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable SortingTransferAndToStationAll(SortingDetail Model)
        {
            string strSql = @" 
                 with  t as
                 (
                   select std.SoringMerchantID,std.TSortingCenterID ,std.SortCityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std (nolock)
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on std.DeliverStationID = ec.ExpressCompanyID   
                   where  std.IntoType =1 and ec.DistributionCode =@DistributionCode  
                          and std.ToStationTime >= @StartTime and std.ToStationTime <=@EndTime and std.IsDeleted = 0 
                   group by std.SoringMerchantID,std.TSortingCenterID,std.SortCityID      
                 ) 
                 select '合计' as StatisticsType, t.SoringMerchantID 
                 ,ec.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.TSortingCenterID = ec.ExpressCompanyID
                 Join RFD_PMS.dbo.City c(nolock) on t.SortCityID = c.CityID where 1=1 
                  
                  ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});
            parameters.Add(new SqlParameter("@StartTime",SqlDbType.DateTime){Value = Model.StartTime});
            parameters.Add(new SqlParameter("@EndTime",SqlDbType.DateTime){Value = Model.EndTime});
            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.TSortingCenterID in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.SortCityID in ({0})", Model.CityIDs);
            }

            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql,parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable SortingToCityAll(SortingDetail Model)
        {
            string strSql =string.Format(
               @"
               with  t as
                 (
                   select  std.SoringMerchantID,w.CreatStation ,ec.CityID, count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std (nolock)
                   Join LMS_RFD.dbo.Waybill w(nolock) on w.WaybillNo = std.WaybillNo   
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.CreatStation = ec.ExpressCompanyID   
                   where  std.OutType = 1 and std.MerchantID not in (8,9) and std.SortingCenterID in ({0})
                   and std.OutBoundTime >=@StartTime and std.OutBoundTime <=@EndTime and std.IsDeleted = 0
                   group by std.SoringMerchantID,w.CreatStation,ec.CityID     
                 ) 
                  select '合计'as StatisticsType, t.SoringMerchantID 
                 ,ec.CompanyName as SortingCenterAll
                  ,c.CityName as City,t.WaybillSum,t.price,t.Fee from t
                 Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.CreatStation = ec.ExpressCompanyID
                 Join RFD_PMS.dbo.City c(nolock) on t.CityID = c.CityID where 1=1
               ", Model.SortingCenterIDs);

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@StartTime",SqlDbType.DateTime){Value = Model.StartTime});
            parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql,parameters.ToArray());
            return ds.Tables[0];
        }

        public DataTable ReturnToSortingCenterAll(SortingDetail Model)
        {
            string strSql =
               @"
                  with t as
                (   
                   select  std.SoringMerchantID,wsr.SortationID as SortingCenter, ec.CityID, std.WaybillNo , std.AccountFare 
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.WarehouseSortRelation wsr (nolock) on std.ReturnSortingCenterID = wsr.WareHouseID 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ec.ExpressCompanyID = wsr.SortationID
                   Join RFD_PMS.dbo.ExpressCompany ec0(nolock) on ec0.ExpressCompanyID = std.TopCODCompanyID  and ec0.DistributionCode = @DistributionCode
                   where std.MerchantID  in (8,9) and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   
                   union all
                   
                   select  std.SoringMerchantID, ec.ExpressCompanyID as SortingCenter, ec.CityID , std.WaybillNo ,std.AccountFare
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock) 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on cast(ec.ExpressCompanyID as nvarchar(20)) = std.ReturnSortingCenterID 
                   where std.MerchantID  not in (8,9)  and std.ReturnTime >=@StartTime and std.ReturnTime <=@EndTime and std.IsDeleted = 0
                   ) 
                   select '合计'as StatisticsType, t.SoringMerchantID,ec1.CompanyName as SortingCenterAll,c.CityName as City ,count(t.WaybillNo) as WaybillSum, null as price ,sum (t.AccountFare) as Fee
                   from t
                   Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on t.SortingCenter= ec1.ExpressCompanyID
                   Join RFD_PMS.dbo.City c(nolock) on t.CityID = c.CityID  where 1=1 ";

            List<SqlParameter> parameters = new List<SqlParameter>();
             parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = Model.StartTime });
             parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = Model.EndTime });
             parameters.Add(new SqlParameter("@DistributionCode",SqlDbType.NVarChar){Value = Model.DistributionCode});

            if (!string.IsNullOrEmpty(Model.SortingCenterIDs))
            {
                strSql += string.Format(" and t.SortingCenter in ({0})", Model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CityID in ({0})", Model.CityIDs);
            }

            strSql += " group by t.SoringMerchantID, ec1.CompanyName, c.CityName";

            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql,parameters.ToArray());
            return ds.Tables[0];

        }

        public DataTable MerchantToSortingCenterAll(SortingDetail Model)
        {
            string appendStr = Model.InSortingCount == 0 ? "" : "  having count(std.WaybillNo) <@cnt";
            string strSql =
               @"
             with t as 
                  ( 
                   select  convert(nvarchar(12), std.InSortingTime ,23) as SDate, std.SoringMerchantID,std.SortingCenterID ,CreateCityID, std.MerchantID,count(std.WaybillNo) as WaybillSum , null as price, sum(std.AccountFare) as Fee
                   from RFD_FMS.dbo.FMS_SortingTransferDetail std(nolock)
                   Join LMS_RFD.dbo.Waybill w on std.waybillno =w.waybillno 
                   where  std.InSortingTime is not null and w.Sources =2
                   and std.InSortingTime >= @StartTime  and std.InSortingTime <=@EndTime  and std.IsDeleted = 0      
                   group by convert(nvarchar(12), std.InSortingTime ,23),std.SoringMerchantID ,std.SortingCenterID ,CreateCityID,std.MerchantID "+appendStr+@"
                   ) 
                   select '合计'as StatisticsType, t.SoringMerchantID,ec.CompanyName as SortingCenterAll,c.CityName as City, sum(t.WaybillSum) as WaybillSum , null as Price, sum(t.Fee) as Fee
                   from t 
                   Join RFD_PMS.dbo.ExpressCompany ec(nolock) on t.SortingCenterID = ec.ExpressCompanyID
                   Join RFD_PMS.dbo.City c(nolock) on t.CreateCityID = c.CityID where 1=1 
                  
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
                strSql += string.Format(" and t.SortingCenterID in ({0})", Model.SortingCenterIDs);
            }
            

            if (!string.IsNullOrEmpty(Model.CityIDs))
            {
                strSql += string.Format(" and t.CreateCityID in ({0})", Model.CityIDs);
            }

            strSql += "  Group by t.SoringMerchantID,ec.CompanyName ,c.CityName ";
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql,parameters.ToArray());
            return ds.Tables[0];
        }
    }
}
