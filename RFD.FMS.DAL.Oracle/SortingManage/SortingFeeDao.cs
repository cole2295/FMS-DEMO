using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.SortingManage;
using Oracle.DataAccess.Client;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;


namespace RFD.FMS.DAL.Oracle.SortingManage
{
    public class SortingFeeDao:OracleDao ,ISortingFeeDao
    {
        public int AddSortingFee(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string strSql = @"select count(1) from FMS_SortingFee where SortingMerchantID = :SortingMerchantID
                              and SortingCenterID =:SortingCenterID 
                              and CityID =:CityID and MerchantID =:MerchantID
                              and FareType =:FareType and IsDeleted = 0 
                              ";
            OracleParameter[] para = { 
                                       new OracleParameter(":SortingMerchantID",OracleDbType.Int32),
                                       new OracleParameter(":SortingCenterID",OracleDbType.Int32),
                                       new OracleParameter(":CityID",OracleDbType.Varchar2),
                                       new OracleParameter(":MerchantID",OracleDbType.Int32),
                                       new OracleParameter(":FareType",OracleDbType.Int32) 
                                     };
            para[0].Value = model.SortingMerchantID;
            para[1].Value = model.SortingCenterID;
            para[2].Value = model.CityID;
            para[3].Value = model.MerchantID;
            para[4].Value = model.FareType;

            var obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, para);
           if (obj != null)
           {
               if (Convert.ToInt32(obj) > 0)
               {
                   return 0;
               }
           }

            strSql =
                @"
                              Insert into FMS_SortingFee 
                              ( 
                                SortingFeeID,
                                SortingMerchantID,
                                SortingCenterID,
                                CityID,
                                FareType,
                                MerchantID,
                                AccountFare,
                                Status,
                                WaybillCount,
                                IsAccountBill,
                                CreateBy,
                                AuditBy,
                                UpdateBy,
                                DistributionCode,
                                IsDeleted,
                                IsChange,
                                CreateTime,
                                UpdateTime,
                                EffectDate
                                
                               )Values(
                                :SortingFeeID,
                                :SortingMerchantID,
                                :SortingCenterID,
                                :CityID,
                                :FareType,
                                :MerchantID,
                                :AccountFare,
                                :Status,
                                :WaybillCount,
                                :IsAccountBill,
                                :CreateBy,
                                :AuditBy,
                                :UpdateBy,
                                :DistributionCode,
                                :IsDeleted,
                                :IsChange,
                                 sysDate,
                                 sysDate,
                                :EffectDate
                               )";
            OracleParameter[] parameters = {
                                               new OracleParameter(":SortingFeeID", OracleDbType.Varchar2){Value =model.SortingFeeID},
                                               new OracleParameter(":SortingMerchantID", OracleDbType.Int32){Value = model.SortingMerchantID},
                                               new OracleParameter(":SortingCenterID", OracleDbType.Int32){Value = model.SortingCenterID},
                                               new OracleParameter(":CityID", OracleDbType.Varchar2){Value = model.CityID},
                                               new OracleParameter(":FareType", OracleDbType.Int32){Value = model.FareType},
                                               new OracleParameter(":MerchantID", OracleDbType.Int32){Value = model.MerchantID},
                                               new OracleParameter(":AccountFare", OracleDbType.Varchar2){Value = model.AccountFare},
                                               new OracleParameter(":Status", OracleDbType.Int32){Value = model.Status},
                                               new OracleParameter(":WaybillCount", OracleDbType.Int32){Value = model.WaybillCount},
                                               new OracleParameter(":IsAccountBill", OracleDbType.Int32){Value = model.IsAccountBill},
                                               new OracleParameter(":CreateBy", OracleDbType.Int32){Value = model.CreateBy},
                                               new OracleParameter(":AuditBy", OracleDbType.Int32){Value = model.AuditBy},
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32){Value = model.UpdateBy},
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2){Value = string.IsNullOrEmpty(model.DistributionCode)? "":model.DistributionCode},
                                               new OracleParameter(":IsDeleted", OracleDbType.Int32){Value = model.IsDeleted?1:0},
                                               new OracleParameter(":IsChange", OracleDbType.Int32){Value = model.IsChange? 1:0},
                                               new OracleParameter(":EffectDate",OracleDbType.Date){Value = DateTime.Parse(model.EffectDate.ToString("yyyy-MM-dd"))},         
                                           };
           var num =OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);
           return Convert.ToInt32(num);
        }


        public DataTable GetSoringFee(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string sqlStr =
                @"Select fsf.SortingFeeID as ID,d.DistributionName as SortingMerchant,ec.CompanyName as SortingCenter, mbi.MerchantName ,
                               c.CityName, (case  when fsf.FareType =1 then '运输费用项目'
                                                  when fsf.FareType =2 then '分拣到城市项目'
                                                  when fsf.FareType =3 then '分拣到站点项目'  
                                                  when fsf.FareType =4 then '逆向入库项目'
                                                  when fsf.FareType =5 then '提货补偿项目' end) as FareTypeStr,
                             AccountFare, (case when fsf.Status = 0 then '待审核'
                                                when fsf.Status = 1 then '已审核'
                                                when fsf.Status = 3 then '已生效'
                                                when fsf.Status = 2 then '置回'
                                                when fsf.Status = 4 then '无效' end) as Status,
                             fsf.Status as StatusCode,
                             to_char(fsf.EffectDate,'yyyy-MM-dd') EffectDate,
                             to_char(fsf.CreateTime,'yyyy-MM-dd') CreateTime,to_char(fsf.CreateTime,'yyyy-MM-dd') UpdateTime,e.EmployeeName as CreateBy, e1.EmployeeName as AuditBy
                 from FMS_SortingFee fsf Join PS_PMS.Distribution d on fsf.SortingMerchantID = d.DistributionID
                      Join pS_PMS.Expresscompany ec on fsf.sortingcenterid = ec.expresscompanyid
                 Left Join PS_PMS.City c on fsf.CityID = c.CityID 
                      Join PS_PMS.Employee e on fsf.CreateBy = e.EmployeeID
                 Left Join PS_PMS.Employee e1 on  fsf.AuditBy = e1.EmployeeID
                      Join PS_PMS.MerchantBaseInfo mbi on fsf.MerchantID = mbi.ID
                 where 1=1 and fsf.CreateTime >= :StartTime and fsf.CreateTime <= :EndTime
";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":StartTime",OracleDbType.Date ) {Value = model.StartTime});
            parameters.Add(new OracleParameter(":EndTime",OracleDbType.Date){Value = model.EndTime});

            if(!string.IsNullOrEmpty(model.CityIDs))
            {
                sqlStr += string.Format(@"and fsf.CityID in ({0}) ", model.CityIDs);
            }

            if(!string.IsNullOrEmpty(model.SortingCenterIDs))
            {
                sqlStr += string.Format(@"and fsf.SortingCenterID in ({0}) ", model.SortingCenterIDs);
            }

            if(! string.IsNullOrEmpty(model.SortingMerchantIDs))
            {
                sqlStr += string.Format(@"and fsf.SortingMerchantID in ({0}) ", model.SortingMerchantIDs);
            }

            if( model.FareType != -1)
            {
                sqlStr += "and fsf.FareType = :FareType ";
                parameters.Add(new OracleParameter(":FareType",OracleDbType.Int32){Value = model.FareType});
            }

            if(model.Status != -1 && model.Status!=-2)
            {
                sqlStr += "and fsf.Status = :Status "; 
                parameters.Add(new OracleParameter(":Status",OracleDbType.Int32){Value = model.Status});
            }

            if(model.Status == -2)
            {
                sqlStr += "and fsf.Status in (0,1) ";
            }
            sqlStr += model.Status == 4 ? "and fsf.IsDeleted =1 " : "and fsf.IsDeleted =0 ";

           var ds= OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray());
            return ds.Tables[0];
        }


        public DataTable GetSortingFeeModel(string SortingFeeID)
        {
            string sqlStr =
                @"Select d.DistributionName as SortingMerchant,fsf.SortingMerchantID,ec.CompanyName as SortingCenter,fsf.SortingCenterID,
                  mbi.MerchantName ,fsf.MerchantID,
                               c.CityName,fsf.CityID, (case  when fsf.FareType =1 then '运输费用项目'
                                                  when fsf.FareType =2 then '分拣到城市项目'
                                                  when fsf.FareType =3 then '分拣到站点项目'  
                                                  when fsf.FareType =4 then '逆向入库项目'
                                                  when fsf.FareType =5 then '提货补偿项目' end) as FareTypeStr,fsf.FareType,
                             AccountFare 
                  from FMS_SortingFee fsf Join PS_PMS.Distribution d on fsf.SortingMerchantID = d.DistributionID
                  Join PS_PMS.Expresscompany ec on fsf.sortingcenterid = ec.expresscompanyid
                  Join PS_PMS.City c on fsf.CityID = c.CityID 
                  Join PS_PMS.MerchantBaseInfo mbi on fsf.MerchantID = mbi.ID
                  where SortingFeeID = :SortingFeeID
                            ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":SortingFeeID", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = SortingFeeID;

            var ds =OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            return ds.Tables[0];
        }

        public DataTable GetSortingFeeWaitModel(string SortingFeeWaitID)
        {
            string sqlStr =
                @"Select d.DistributionName as SortingMerchant,fsfw.SortingMerchantID,ec.CompanyName as SortingCenter,fsfw.SortingCenterID,
                  mbi.MerchantName ,fsfw.MerchantID,
                               c.CityName,fsfw.CityID, (case  when fsfw.FareType =1 then '运输费用项目'
                                                  when fsfw.FareType =2 then '分拣到城市项目'
                                                  when fsfw.FareType =3 then '分拣到站点项目'  
                                                  when fsfw.FareType =4 then '逆向入库项目'
                                                  when fsfw.FareType =5 then '提货补偿项目' end) as FareTypeStr,fsfw.FareType,
                             fsfw.AccountFare 
                  from FMS_SortingFeeWait fsfw Join PS_PMS.Distribution d on fsfw.SortingMerchantID = d.DistributionID
                  Join PS_PMS.Expresscompany ec on fsfw.sortingcenterid = ec.expresscompanyid
                  Join PS_PMS.City c on fsfw.CityID = c.CityID 
                  Join PS_PMS.MerchantBaseInfo mbi on fsfw.MerchantID = mbi.ID
                  where SortingFeeWaitID = :SortingFeeWaitID
                            ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":SortingFeeWaitID", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = SortingFeeWaitID;

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            return ds.Tables[0];
        }


        public int UpdateSortingFee(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string sqlStr =
                @"Update FMS_SortingFee Set SortingMerchantID =:SortingMerchantID ,
                                                        SortingCenterID =:SortingCenterID,
                                                        CityID =:CityID,
                                                        MerchantID =:MerchantID,
                                                        FareType =:FareType,
                                                        AccountFare =:AccountFare,
                                                        WaybillCount =:WaybillCount,
                                                        IsAccountBill =:IsAccountBill,
                                                        UpdateBy =:UpdateBy,
                                                        UpdateTime =sysDate,
                                                        DistributionCode = :DistributionCode,
                                                        Status =0,
                                                        IsChange = 1,
                                                        EffectDate =:EffectDate
                               where SortingFeeID = :SortingFeeID
                    ";
            List<OracleParameter> parameters= new List<OracleParameter>();
            parameters.Add(new OracleParameter(":SortingMerchantID",OracleDbType.Int32){Value = model.SortingMerchantID});
            parameters.Add(new OracleParameter(":SortingCenterID", OracleDbType.Int32) { Value = model.SortingCenterID });
            parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2) { Value = model.CityID });
            parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Int32) { Value = model.MerchantID });
            parameters.Add(new OracleParameter(":FareType", OracleDbType.Int32) { Value = model.FareType });
            parameters.Add(new OracleParameter(":AccountFare", OracleDbType.Varchar2) { Value = model.AccountFare });
            parameters.Add(new OracleParameter(":WaybillCount", OracleDbType.Int32) { Value = model.WaybillCount });
            parameters.Add(new OracleParameter(":IsAccountBill", OracleDbType.Int32) { Value = model.IsAccountBill });
            parameters.Add(new OracleParameter(":UpdateBy", OracleDbType.Int32) { Value = model.UpdateBy });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = model.DistributionCode });
            parameters.Add(new OracleParameter(":SortingFeeID", OracleDbType.Varchar2) { Value = model.SortingFeeID });
            parameters.Add(new OracleParameter(":EffectDate", OracleDbType.Date){ Value = DateTime.Parse(model.EffectDate.ToString("yyyy-MM-dd"))});
            var num=OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public int Delete(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string sqlStr =
                @"Update FMS_SortingFee Set IsDeleted =1 ,
                                                        IsChange=1,
                                                        UpdateBy=:UpdateBy,
                                                        UpdateTime=sysDate,
                                                        Status = 4
                              where SortingFeeID = :SortingFeeID
                               ";
            OracleParameter[] parameters = {
                                              new OracleParameter(":UpdateBy", OracleDbType.Int32),
                                              new OracleParameter(":SortingFeeID", OracleDbType.Varchar2)
                                          };
            parameters[0].Value = model.UpdateBy;
            parameters[1].Value = model.SortingFeeID;

            var num = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public DataTable GetSoringFeeWait(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string sqlStr =
                @"Select fsfw.SortingFeeWaitID as ID,fsfw.SortingFeeID as FeeID ,d.DistributionName as SortingMerchant,ec.CompanyName as SortingCenter, mbi.MerchantName ,
                               c.CityName, (case  when fsfw.FareType =1 then '运输费用项目'
                                                  when fsfw.FareType =2 then '分拣到城市项目'
                                                  when fsfw.FareType =3 then '分拣到站点项目'  
                                                  when fsfw.FareType =4 then '逆向入库项目'
                                                  when fsfw.FareType =5 then '提货补偿项目' end) as FareTypeStr,
                             AccountFare, (case when fsfw.Status = 0 then '待审核'
                                                when fsfw.Status = 1 then '已审核'
                                                when fsfw.Status = 2 then '已置回'
                                                end) as Status,
                             fsfw.Status as StatusCode,
                             to_char(fsfw.EffectDate,'yyyy-MM-dd') EffectDate,
                             to_char(fsfw.CreateTime,'yyyy-MM-dd') CreateTime,to_char(fsfw.CreateTime,'yyyy-MM-dd') UpdateTime,e.EmployeeName as CreateBy, e1.EmployeeName as AuditBy
                 from FMS_SortingFeeWait fsfw Join PS_PMS.Distribution d on fsfw.SortingMerchantID = d.DistributionID
                      Join pS_PMS.Expresscompany ec on fsfw.sortingcenterid = ec.expresscompanyid
                 Left Join PS_PMS.City c on fsfw.CityID = c.CityID 
                      Join PS_PMS.Employee e on fsfw.CreateBy = e.EmployeeID
                 Left Join PS_PMS.Employee e1 on  fsfw.AuditBy = e1.EmployeeID
                      Join PS_PMS.MerchantBaseInfo mbi on fsfw.MerchantID = mbi.ID
                 where 1=1 and fsfw.CreateTime >= :StartTime and fsfw.CreateTime <= :EndTime and fsfw.IsDeleted =0 ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = model.StartTime });
            parameters.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = model.EndTime });

            if (!string.IsNullOrEmpty(model.CityIDs))
            {
                sqlStr += string.Format(@"and fsfw.CityID in ({0}) ", model.CityIDs);
            }

            if (!string.IsNullOrEmpty(model.SortingCenterIDs))
            {
                sqlStr += string.Format(@"and fsfw.SortingCenterID in ({0}) ", model.SortingCenterIDs);
            }

            if (!string.IsNullOrEmpty(model.SortingMerchantIDs))
            {
                sqlStr += string.Format(@"and fsfw.SortingMerchantID in ({0}) ", model.SortingMerchantIDs);
            }

            if (model.FareType != -1)
            {
                sqlStr += "and fsfw.FareType = :FareType ";
                parameters.Add(new OracleParameter(":FareType", OracleDbType.Int32) { Value = model.FareType });
            }

            if (model.Status != -1 && model.Status !=-2)
            {
                sqlStr += "and fsfw.Status = :Status ";
                parameters.Add(new OracleParameter(":Status", OracleDbType.Int32) { Value = model.Status });
            }

            if (model.Status == -2)
            {
                 sqlStr += "and fsfw.Status in (0,1) ";
            }
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray());
            return ds.Tables[0];

        }


        public int AddSortingFeeWait(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string strSql = @"
            Insert into FMS_SortingFeeWait 
                              ( 
                                SortingFeeWaitID,
                                SortingFeeID,
                                SortingMerchantID,
                                SortingCenterID,
                                CityID,
                                FareType,
                                MerchantID,
                                AccountFare,
                                Status,
                                WaybillCount,
                                IsAccountBill,
                                CreateBy,
                                AuditBy,
                                UpdateBy,
                                DistributionCode,
                                IsDeleted,
                                IsChange,
                                CreateTime,
                                UpdateTime,
                                EffectDate
                                
                               )Values(
                                :SortingFeeWaitID,
                                :SortingFeeID,
                                :SortingMerchantID,
                                :SortingCenterID,
                                :CityID,
                                :FareType,
                                :MerchantID,
                                :AccountFare,
                                :Status,
                                :WaybillCount,
                                :IsAccountBill,
                                :CreateBy,
                                :AuditBy,
                                :UpdateBy,
                                :DistributionCode,
                                :IsDeleted,
                                :IsChange,
                                 sysDate,
                                 sysDate,
                                :EffectDate
                               )";
            OracleParameter[] parameters = {
                                               new OracleParameter(":SortingFeeWaitID", OracleDbType.Varchar2){Value =model.SortingFeeWaitID},
                                               new OracleParameter(":SortingFeeID", OracleDbType.Varchar2){Value =model.SortingFeeID},
                                               new OracleParameter(":SortingMerchantID", OracleDbType.Int32){Value = model.SortingMerchantID},
                                               new OracleParameter(":SortingCenterID", OracleDbType.Int32){Value = model.SortingCenterID},
                                               new OracleParameter(":CityID", OracleDbType.Varchar2){Value = model.CityID},
                                               new OracleParameter(":FareType", OracleDbType.Int32){Value = model.FareType},
                                               new OracleParameter(":MerchantID", OracleDbType.Int32){Value = model.MerchantID},
                                               new OracleParameter(":AccountFare", OracleDbType.Varchar2){Value = model.AccountFare},
                                               new OracleParameter(":Status", OracleDbType.Int32){Value = model.Status},
                                               new OracleParameter(":WaybillCount", OracleDbType.Int32){Value = model.WaybillCount},
                                               new OracleParameter(":IsAccountBill", OracleDbType.Int32){Value = model.IsAccountBill},
                                               new OracleParameter(":CreateBy", OracleDbType.Int32){Value = model.CreateBy},
                                               new OracleParameter(":AuditBy", OracleDbType.Int32){Value = model.AuditBy},
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32){Value = model.UpdateBy},
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2){Value = string.IsNullOrEmpty(model.DistributionCode)? "":model.DistributionCode},
                                               new OracleParameter(":IsDeleted", OracleDbType.Int32){Value = model.IsDeleted?1:0},
                                               new OracleParameter(":IsChange", OracleDbType.Int32){Value = model.IsChange? 1:0},
                                               new OracleParameter(":EffectDate",OracleDbType.Date){Value = DateTime.Parse(model.EffectDate.ToString("yyyy-MM-dd"))},         
                                           };
           var num =OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);
           return Convert.ToInt32(num);
        }


        public int UpdateSortingFeeWait(MODEL.FinancialManage.FMS_SortingFeeModel model)
        {
            string sqlStr =
                @"Update FMS_SortingFeeWait Set         SortingMerchantID =:SortingMerchantID ,
                                                        SortingCenterID =:SortingCenterID,
                                                        CityID =:CityID,
                                                        MerchantID =:MerchantID,
                                                        FareType =:FareType,
                                                        AccountFare =:AccountFare,
                                                        WaybillCount =:WaybillCount,
                                                        IsAccountBill =:IsAccountBill,
                                                        UpdateBy =:UpdateBy,
                                                        UpdateTime =sysDate,
                                                        DistributionCode = :DistributionCode,
                                                        Status =0,
                                                        IsChange = 1,
                                                        EffectDate =:EffectDate
                               where SortingFeeWaitID = :SortingFeeWaitID
                    ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":SortingMerchantID", OracleDbType.Int32) { Value = model.SortingMerchantID });
            parameters.Add(new OracleParameter(":SortingCenterID", OracleDbType.Int32) { Value = model.SortingCenterID });
            parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2) { Value = model.CityID });
            parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Int32) { Value = model.MerchantID });
            parameters.Add(new OracleParameter(":FareType", OracleDbType.Int32) { Value = model.FareType });
            parameters.Add(new OracleParameter(":AccountFare", OracleDbType.Varchar2) { Value = model.AccountFare });
            parameters.Add(new OracleParameter(":WaybillCount", OracleDbType.Int32) { Value = model.WaybillCount });
            parameters.Add(new OracleParameter(":IsAccountBill", OracleDbType.Int32) { Value = model.IsAccountBill });
            parameters.Add(new OracleParameter(":UpdateBy", OracleDbType.Int32) { Value = model.UpdateBy });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = model.DistributionCode });
            parameters.Add(new OracleParameter(":SortingFeeWaitID", OracleDbType.Varchar2) { Value = model.SortingFeeWaitID });
            parameters.Add(new OracleParameter(":EffectDate", OracleDbType.Date) { Value = DateTime.Parse(model.EffectDate.ToString("yyyy-MM-dd")) });
            var num = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public DataTable GetSmallSortingFeeModel(string SortingFeeID)
        {
            string sqlStr = @"Select * from FMS_SortingFee where SortingFeeID =:SortingFeeID";
            OracleParameter[] parameter = {
                                              new OracleParameter(":SortingFeeID",OracleDbType.Varchar2){Value = SortingFeeID}
                                          };
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameter);
            return ds.Tables[0];
        }


        public int AuditSortingFee(FMS_SortingFeeModel model)
        {
            string strSql =
                @"Update FMS_SortingFee set AuditBy =:AuditBy,
                                                           Status = 1,
                                                           UpdateBy =:UpdateBy,
                                                           UpdateTime =sysDate
                             where SortingFeeID = :SortingFeeID " ;

            OracleParameter[] parameters = {
                                               new OracleParameter(":AuditBy", OracleDbType.Int32)
                                                   {Value = model.AuditBy},
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32)
                                                   {Value = model.UpdateBy},
                                               new OracleParameter(":SortingFeeID", OracleDbType.Varchar2)
                                                   {Value = model.SortingFeeID},
                                           };
            var num =OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);

            return Convert.ToInt32(num);
        }


        public int AuditSortingFeeWait(FMS_SortingFeeModel model)
        {
            string strSql =
                @"Update FMS_SortingFeeWait set AuditBy =:AuditBy,
                                                           Status = 1,
                                                           UpdateBy =:UpdateBy,
                                                           UpdateTime =sysDate
                             where SortingFeeWaitID = :SortingFeeWaitID ";

            OracleParameter[] parameters = {
                                               new OracleParameter(":AuditBy", OracleDbType.Int32)
                                                   {Value = model.AuditBy},
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32)
                                                   {Value = model.UpdateBy},
                                               new OracleParameter(":SortingFeeWaitID", OracleDbType.Varchar2)
                                                   {Value = model.SortingFeeWaitID},
                                           };
            var num = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);

            return Convert.ToInt32(num);
        }


        public int DeleteWait(FMS_SortingFeeModel model)
        {
            string sqlStr =
                @"Update FMS_SortingFeeWait set IsDeleted = 1,
                                                           UpdateBy = :UpdateBy,
                                                           UpdateTime = sysDate
                                                           where IsDeleted =0 and
                                                           SortingFeeID = :SortingFeeID ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32),
                                               new OracleParameter(":SortingFeeID", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = model.UpdateBy;
            parameters[1].Value = model.SortingFeeID;

            var num =OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            return Convert.ToInt32(num);
        }


        public int BackSortingFee(FMS_SortingFeeModel model)
        {
            string sqlStr =
              @"Update FMS_SortingFee set Status = 2,
                                                           UpdateBy = :UpdateBy,
                                                           AuditBy =:AuditBy,
                                                           UpdateTime = sysDate 
                                                           where SortingFeeID = :SortingFeeID ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32),
                                               new OracleParameter(":AuditBy",OracleDbType.Int32), 
                                               new OracleParameter(":SortingFeeID", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = model.UpdateBy;
            parameters[1].Value = model.AuditBy;
            parameters[2].Value = model.SortingFeeID;

            var num = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            return Convert.ToInt32(num);
        }


        public int BackSortingFeeWait(FMS_SortingFeeModel model)
        {
            string sqlStr =
             @"Update FMS_SortingFeeWait set Status = 2,
                                                           UpdateBy = :UpdateBy,
                                                           AuditBy = :AuditBy,
                                                           UpdateTime = sysDate 
                                                           where SortingFeeWaitID = :SortingFeeWaitID ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32),
                                               new OracleParameter(":AuditBy",OracleDbType.Int32), 
                                               new OracleParameter(":SortingFeeWaitID", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = model.UpdateBy;
            parameters[1].Value = model.AuditBy;
            parameters[2].Value = model.SortingFeeWaitID;

            var num = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            return Convert.ToInt32(num);
        }


        public DataTable GetSmallSortingFeeWaitModel(string SortingFeeWaitID)
        {
            string sqlStr = @"Select * from FMS_SortingFeeWait where SortingFeeWaitID =:SortingFeeWaitID";
            OracleParameter[] parameter = {
                                              new OracleParameter(":SortingFeeWaitID",OracleDbType.Varchar2){Value = SortingFeeWaitID}
                                          };
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameter);
            return ds.Tables[0];
        }


        public DataTable GetSoringFeeWait(int rowNum)
        {
            string strSql =
                @"select 
                                    fsfw.SortingFeeWaitID,
                                    fsfw.SortingMerchantID,
                                    fsfw.SortingCenterID,
                                    fsfw.CityID,
                                    fsfw.MerchantID,
                                    fsfw.FareType,
                                    fsfw.AccountFare,
                                    fsfw.WaybillCount,
                                    fsfw.IsAccountBill,
                                    fsfw.AuditBy,
                                    fsfw.CreateBy,
                                    fsfw.UpdateBy,
                                    fsfw.DistributionCode,
                                    fsfw.EffectDate,
                                    fsfw.SortingFeeID
                                    from FMS_SortingFeeWait fsfw where fsfw.Status =1 and fsfw.IsDeleted=0 and fsfw.EffectDate = to_date(to_char(sysdate,'yyyy-mm-dd'),'yyyy-mm-dd')
                                    and RowNum <=:RowCount
           ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":RowCount", OracleDbType.Int32) {Value = rowNum}
                                           };
           var ds= OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql,parameters);
           return ds.Tables[0];
        }


        public int UpdateSortingFeeWaitForEffect(FMS_SortingFeeModel model)
        {
            string strSql =
                @" Update FMS_SortingFeeWait 
                                     set IsDeleted =1,
                                         UpdateTime = sysDate
                              where SortingFeeWaitID = :SortingFeeWaitID 
                                                           ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":SortingFeeWaitID", OracleDbType.Varchar2)
                                                   {
                                                       Value = model.SortingFeeWaitID
                                                   }
                                           };
            var num =OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);
            return Convert.ToInt32(num);
        }


        public int UpdateSortingFeeForEffect(FMS_SortingFeeModel model)
        {
            string sqlStr =
            @"Update FMS_SortingFee Set SortingMerchantID =:SortingMerchantID ,
                                                        SortingCenterID =:SortingCenterID,
                                                        CityID =:CityID,
                                                        MerchantID =:MerchantID,
                                                        FareType =:FareType,
                                                        AccountFare =:AccountFare,
                                                        WaybillCount =:WaybillCount,
                                                        IsAccountBill =:IsAccountBill,
                                                        UpdateBy =:UpdateBy,
                                                        UpdateTime =sysDate,
                                                        DistributionCode = :DistributionCode,
                                                        Status =3,
                                                        IsChange = 1,
                                                        EffectDate =:EffectDate
                               where SortingFeeID = :SortingFeeID
                    ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":SortingMerchantID", OracleDbType.Int32) { Value = model.SortingMerchantID });
            parameters.Add(new OracleParameter(":SortingCenterID", OracleDbType.Int32) { Value = model.SortingCenterID });
            parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2) { Value = model.CityID });
            parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Int32) { Value = model.MerchantID });
            parameters.Add(new OracleParameter(":FareType", OracleDbType.Int32) { Value = model.FareType });
            parameters.Add(new OracleParameter(":AccountFare", OracleDbType.Varchar2) { Value = model.AccountFare });
            parameters.Add(new OracleParameter(":WaybillCount", OracleDbType.Int32) { Value = model.WaybillCount });
            parameters.Add(new OracleParameter(":IsAccountBill", OracleDbType.Int32) { Value = model.IsAccountBill });
            parameters.Add(new OracleParameter(":UpdateBy", OracleDbType.Int32) { Value = model.UpdateBy });
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = model.DistributionCode });
            parameters.Add(new OracleParameter(":SortingFeeID", OracleDbType.Varchar2) { Value = model.SortingFeeID });
            parameters.Add(new OracleParameter(":EffectDate", OracleDbType.Date) { Value = DateTime.Parse(model.EffectDate.ToString("yyyy-MM-dd")) });
            var num = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters.ToArray());
            return Convert.ToInt32(num);
        }


        public void AddSortingFeeHis(FMS_SortingFeeModel model)
        {
            string strSql =
                @"             
                              Update FMS_SortingFeeHis set
                                     IsDeleted = 1
                              where SortingFeeID =:SortingFeeID 
               ";
            OracleParameter []para = {
                                         new OracleParameter(":SortingFeeID",OracleDbType.Varchar2){Value = model.SortingFeeID} 
                                     };
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, para);
                        
            strSql =@"    
                           Insert into FMS_SortingFeeHis 
                              ( 
                                SortingFeeHisID,
                                SortingMerchantID,
                                SortingCenterID,
                                CityID,
                                FareType,
                                MerchantID,
                                AccountFare,
                                WaybillCount,
                                IsAccountBill,
                                CreateBy,
                                AuditBy,
                                UpdateBy,
                                DistributionCode,
                                CreateTime,
                                EffectDate,
                                SortingFeeID,
                                IsDeleted
                                
                               )Values(
                                :SortingFeeHisID,
                                :SortingMerchantID,
                                :SortingCenterID,
                                :CityID,
                                :FareType,
                                :MerchantID,
                                :AccountFare,
                                :WaybillCount,
                                :IsAccountBill,
                                :CreateBy,
                                :AuditBy,
                                :UpdateBy,
                                :DistributionCode,
                                 sysDate,
                                :EffectDate,
                                :SortingFeeID,
                                0
                               )";
            OracleParameter[] parameters = {
                                               new OracleParameter(":SortingFeeHisID", OracleDbType.Varchar2){Value =model.SortingFeeWaitID},
                                               new OracleParameter(":SortingMerchantID", OracleDbType.Int32){Value = model.SortingMerchantID},
                                               new OracleParameter(":SortingCenterID", OracleDbType.Int32){Value = model.SortingCenterID},
                                               new OracleParameter(":CityID", OracleDbType.Varchar2){Value = model.CityID},
                                               new OracleParameter(":FareType", OracleDbType.Int32){Value = model.FareType},
                                               new OracleParameter(":MerchantID", OracleDbType.Int32){Value = model.MerchantID},
                                               new OracleParameter(":AccountFare", OracleDbType.Varchar2){Value = model.AccountFare},
                                               new OracleParameter(":WaybillCount", OracleDbType.Int32){Value = model.WaybillCount},
                                               new OracleParameter(":IsAccountBill", OracleDbType.Int32){Value = model.IsAccountBill},
                                               new OracleParameter(":CreateBy", OracleDbType.Int32){Value = model.CreateBy},
                                               new OracleParameter(":AuditBy", OracleDbType.Int32){Value = model.AuditBy},
                                               new OracleParameter(":UpdateBy", OracleDbType.Int32){Value = model.UpdateBy},
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2){Value = string.IsNullOrEmpty(model.DistributionCode)? "":model.DistributionCode},
                                               new OracleParameter(":EffectDate",OracleDbType.Date){Value = DateTime.Parse(model.EffectDate.ToString("yyyy-MM-dd"))},      
                                               new OracleParameter(":SortingFeeID",OracleDbType.Varchar2){Value = model.SortingFeeID} 
                                           };
             OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);
            
        }


        public bool ExsitSortingFeeWait(FMS_SortingFeeModel model)
        {
            string strSql =
                @" Select count(1) from FMS_SortingFeeWait fsfw where fsfw.IsDeleted =0 and
                               SortingFeeID = :SortingFeeID ";
            OracleParameter[] parameter = {
                                              new OracleParameter(":SortingFeeID", OracleDbType.Varchar2)
                                                  {
                                                      Value = model.SortingFeeID
                                                  }
                                          };
           var ret= OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql, parameter);
            return Convert.ToInt32(ret) > 0;
        }


        public string GetAccountFareByMerchant(int SortingMerchantID, int SortingCenterID, string CityID, int MerchantID, DateTime SDate,int Type)
        {
            string sqlStr =
                @"Select AccountFare from FMS_SortingFeeHis 
                                               where EffectDate<= :EffectDate 
                                                     and SortingCenterID =:SortingCenterID 
                                                     and SortingMerchantID =:SortingMerchantID
                                                     and CityID =:CityID
                                                     and MerchantID = :MerchantID
                                                     and FareType =:Type
                                                     and rownum =1
                                              Order by EffectDate desc    
                              ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":EffectDate", OracleDbType.Date)
                                                   {Value = DateTime.Parse(SDate.ToString("yyyy-MM-dd"))},
                                               new OracleParameter(":SortingCenterID", OracleDbType.Int32)
                                                   {Value = SortingCenterID},
                                               new OracleParameter(":SortingMerchantID", OracleDbType.Int32)
                                                   {Value = SortingMerchantID},
                                               new OracleParameter(":CityID", OracleDbType.Varchar2) {Value = CityID},
                                               new OracleParameter(":MerchantID", OracleDbType.Int32)
                                                   {Value = MerchantID},
                                               new OracleParameter(":Type", OracleDbType.Int32) {Value = Type}
                                           };
            var AccountFare =OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            return AccountFare == null ? "":  
                AccountFare.ToString();

        }
    }
}
