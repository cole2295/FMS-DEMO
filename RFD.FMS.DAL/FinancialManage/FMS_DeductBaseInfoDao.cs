using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public class FMS_DeductBaseInfoDao : SqlServerDao, IFMS_DeductBaseInfoDao
    {
        /// <summary>
        /// 添加提成信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long Add(FMS_DeductSubBaseInfo model)
        {
            string sql = @"
if not exists (select 1 from LMS_RFD.dbo.FMS_DeductSubBaseInfo(nolock) where WaybillNO=@WaybillNO and IsDeleted=0 and DeductType=@DeductType)
   begin
insert into LMS_RFD.dbo.FMS_DeductSubBaseInfo (WaybillNO,Formula,DeductType,StationId,DeliverUser,BasicCommission,
                    AreaCommission,WeightCommission,CreateTime,UpdateTime,IsDeleted,AdjustCommission,IsChange)
                Values(@WaybillNO,@Formula,@DeductType,@StationId,@DeliverUser,@BasicCommission,
                    @AreaCommission,@WeightCommission,@CreateTime,@UpdateTime,@IsDeleted,@AdjustCommission,2)
                ;select @@IDENTITY
end";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",model.WaybillNO),
                new SqlParameter("@Formula",model.Formula),
                new SqlParameter("@DeductType",model.DeductType),
                new SqlParameter("@StationId",model.StationId),
                new SqlParameter("@DeliverUser",model.DeliverUser),
                new SqlParameter("@BasicCommission",model.BasicCommission),
                new SqlParameter("@AreaCommission",model.AreaCommission),
                new SqlParameter("@WeightCommission",model.WeightCommission),
                new SqlParameter("@CreateTime",model.CreateTime),
                new SqlParameter("@UpdateTime",model.UpdateTime),
                new SqlParameter("@IsDeleted",model.IsDeleted),
                new SqlParameter("@AdjustCommission",model.AdjustCommission)
            };

            return DataConvert.ToLong(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql, parameters));
        }

        public bool IsCalculateOK(long id)
        {
            string sql = "update LMS_RFD.dbo.FMS_DeductBaseInfo set IsDeductAcount=1,UpdateTime=getdate() ,IsChange=2  where DeductID=@DeductID";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@DeductID",id)
            };

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters.ToArray());

            return rows == 1;
        }

        /// <summary>
        /// 查询快递取件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetExpressReceiveModel(string DistributionCodeList)
        {
            string sql =
                String.Format(@"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,bInfo.ReceiveStationID,
                    bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,bInfo.IsDeductAcount,
                    bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,bInfo.MerchantID,
                    bInfo.BackStationTime,wtsi.SendAddress ,wtsi.ReceiveAddress
                from LMS_RFD.dbo.FMS_DeductBaseInfo bInfo(nolock) 
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON wtsi.WaybillNO = bInfo.WaybillNO
                    inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                    inner join RFD_PMS.dbo.ExpressCompany ecp on bInfo.ReceiveStationID=ecp.ExpressCompanyID
                where bInfo.IsDeductAcount=0 and bInfo.IsDeleted=0
                    and mInfo.IsExpressDelivery=1 
                    and bInfo.Status in ('-3','-5','0')
                    and bInfo.CreateTime > DATEADD(d,-2,getdate())
                    and bInfo.CreateTime < DATEADD(n,-10,getdate())
                    and ecp.DistributionCode in ({0})", DistributionCodeList);

            List<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> list = new List<MODEL.FinancialManage.FMS_DeductBaseInfo>();

            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 查询快递派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetExpressSendModel(string DistributionCodeList)
        {
            string sql = String.Format(@"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,
                    bInfo.ReceiveStationID,bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,
                    bInfo.IsDeductAcount,bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,
                    bInfo.MerchantID,bInfo.BackStationTime,wtsi.ReceiveAddress ,wtsi.SendAddress
                from LMS_RFD.dbo.FMS_DeductBaseInfo bInfo(nolock) 
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON wtsi.WaybillNO=bInfo.WaybillNO
                    inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                    inner join RFD_PMS.dbo.ExpressCompany ecp on bInfo.DeliverStationID=ecp.ExpressCompanyID
                where bInfo.IsDeductAcount=0 and bInfo.IsDeleted=0
                    and mInfo.IsExpressDelivery=1 
                    and bInfo.Status='3'
                    and bInfo.CreateTime > DATEADD(d,-2,getdate())
                    and bInfo.CreateTime < DATEADD(n,-10,getdate())
                    and ecp.DistributionCode IN ({0}) ", DistributionCodeList);

            List<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> list = new List<MODEL.FinancialManage.FMS_DeductBaseInfo>();

            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 查询快递派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetExpressSendModelByWaybillNO(long WaybillNO)
        {
            string sql =@"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,
                    bInfo.ReceiveStationID,bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,
                    bInfo.IsDeductAcount,bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,
                    bInfo.MerchantID,bInfo.BackStationTime,wtsi.ReceiveAddress ,wtsi.SendAddress
                from LMS_RFD.dbo.FMS_DeductBaseInfo bInfo(nolock) 
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON wtsi.WaybillNO=bInfo.WaybillNO
                    inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                    inner join RFD_PMS.dbo.ExpressCompany ecp on bInfo.DeliverStationID=ecp.ExpressCompanyID
                where  bInfo.IsDeleted=0
                    and mInfo.IsExpressDelivery=1 
                    and bInfo.Status='3'                    
                    and ecp.DistributionCode IN ('rfd','bswlhf','bswlfz','bswlxm','bswlkm','bswltj','bswlfs','bswldg','bswlcs','bswlty','bswlcd','bswlhz','bswlnb',
'bswlwx','bswlcz','bswlzs','bswlzh','bswlnj','bswlsz','bswlwh','bswlxa','bswlshz','bswl') 
and bInfo.WaybillNO=@WaybillNO
                        ";

            List<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> list = new List<MODEL.FinancialManage.FMS_DeductBaseInfo>();
            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",WaybillNO)
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 查询项目单派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetProjectSendModel(string DistributionCodeList)
        {
            string sql = String.Format(@"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,
                    bInfo.ReceiveStationID,bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,
                    bInfo.IsDeductAcount,bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,
                    bInfo.MerchantID,binfo.BackStationTime,wtsi.ReceiveAddress,wtsi.SendAddress 
                from LMS_RFD.dbo.FMS_DeductBaseInfo bInfo(nolock) 
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON wtsi.WaybillNO=bInfo.WaybillNO
                    inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                    inner join RFD_PMS.dbo.ExpressCompany ecp 
on bInfo.DeliverStationID=ecp.ExpressCompanyID
                where bInfo.IsDeductAcount=0 and bInfo.IsDeleted=0 
                    and bInfo.Status='3'
                    and mInfo.IsExpressDelivery!=1 
                    and bInfo.CreateTime > DATEADD(d,-2,getdate())
                    and bInfo.CreateTime < DATEADD(n,-10,getdate())
                    and ecp.DistributionCode IN ({0})", DistributionCodeList);

            List<FMS_DeductBaseInfo> list = new List<FMS_DeductBaseInfo>();

            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }



        /// <summary>
        /// 查询项目单派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        public IList<FMS_DeductBaseInfo> GetProjectSendModelByWaybillNO(long WaybillNO)
        {
            string sql = @"select top 400 bInfo.DeductID,bInfo.WaybillNO,bInfo.DeliverStationID,
                    bInfo.ReceiveStationID,bInfo.DeliverManID,bInfo.ReceiveDeliverManID,bInfo.WayBillInfoWeight,
                    bInfo.IsDeductAcount,bInfo.WaybillCategory,bInfo.DeductMoney,bInfo.DeductNotes,
                    bInfo.MerchantID,binfo.BackStationTime,wtsi.ReceiveAddress,wtsi.SendAddress 
                from LMS_RFD.dbo.FMS_DeductBaseInfo bInfo(nolock) 
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON wtsi.WaybillNO=bInfo.WaybillNO
                    inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                    inner join RFD_PMS.dbo.ExpressCompany ecp 
on bInfo.DeliverStationID=ecp.ExpressCompanyID
                where bInfo.WaybillNO=@WaybillNO";

            List<FMS_DeductBaseInfo> list = new List<FMS_DeductBaseInfo>();

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",WaybillNO)
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataRowToObject(row));
            }

            return list;
        }

        /// <summary>
        /// 获取站点快递派件公式
        /// </summary>
        /// <param name="stationIds">站点编号集合</param>
        /// <returns>站点公式集合</returns>
        public DataTable GetExpressSendFormula(string stationIds)
        {
            string sql = String.Format(@"select ExpressCompanyID StationId,SendFormula Formula from RFD_PMS.dbo.DeductStationBaseInfo(nolock)
            where ExpressCompanyID in ({0}) and ReductType=2 and IsDeleted=0 and UseStatus=1", stationIds);

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 获取站点快递取件公式
        /// </summary>
        /// <param name="stationIds">站点编号集合</param>
        /// <returns>站点公式集合</returns>
        public DataTable GetExpressReceiveFormula(string stationIds)
        {
            string sql = String.Format(@"select ExpressCompanyID StationId,ReceiveFormula Formula from RFD_PMS.dbo.DeductStationBaseInfo(nolock)
            where ExpressCompanyID in ({0}) and ReductType=2 and IsDeleted=0 and UseStatus=1",stationIds);

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 获取站点项目派件公式
        /// </summary>
        /// <param name="stationIds">站点集合</param>
        /// <returns>站点公式集合</returns>
        public DataTable GetProjectSendFormula(string stationIds, string categoryCodes)
        {
            string sql = String.Format(@"select ExpressCompanyID StationId,GoodsCategoryCode,SendFormula Formula from RFD_PMS.dbo.DeductStationBaseInfo(nolock)
            where ExpressCompanyID in ({0}) and GoodsCategoryCode in ({1}) and ReductType=1 and IsDeleted=0 and UseStatus=1", stationIds,categoryCodes);

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        private FMS_DeductBaseInfo DataRowToObject(DataRow row)
        {
            FMS_DeductBaseInfo model = new FMS_DeductBaseInfo();

            model.DeductID = DataConvert.ToLong(row["DeductID"]);
            model.WaybillNO = DataConvert.ToLong(row["WaybillNO"]);
            model.DeliverStationID = DataConvert.ToInt(row["DeliverStationID"]);
            model.ReceiveStationID = DataConvert.ToInt(row["ReceiveStationID"]);
            model.DeliverManID = DataConvert.ToInt(row["DeliverManID"]);
            model.ReceiveDeliverManID = DataConvert.ToInt(row["ReceiveDeliverManID"]);
            model.WayBillInfoWeight = DataConvert.ToDecimal(row["WayBillInfoWeight"]);
            model.IsDeductAcount = DataConvert.ToInt(row["IsDeductAcount"]);
            model.WaybillCategory = DataConvert.ToString(row["WaybillCategory"]);
            model.AreaSendDeduct = DataConvert.ToDecimal(row["DeductMoney"], -1000);
            model.AreaReceiveDeduct = DataConvert.ToDecimal(row["DeductNotes"],-1000);
            model.MerchantID = DataConvert.ToInt(row["MerchantID"]);
            model.ReceiveAddress = DataConvert.ToString(row["ReceiveAddress"]);
            model.SendAddress = DataConvert.ToString(row["SendAddress"]);
            model.AdjustCommission = DataConvert.ToDecimal(row["DeductMoney"], -1000) > 0 ? DataConvert.ToDecimal(row["DeductMoney"], -1000) : DataConvert.ToDecimal(row["DeductNotes"], -1000);
            if (!string.IsNullOrEmpty(row["BackStationTime"].ToString().Trim()))
            {
                model.BackStationTime = DataConvert.ToDateTime(row["BackStationTime"]);
            }

            return model;
        }

        public decimal GetStationDefaultAreaDeduct(int stationId)
        {
            string sql = @"select DeductValue from RFD_PMS.dbo.DeductStationAreaCommission dsac(nolock) where IsDeleted=0 and StationId=@StationId and IsDefault=1";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@StationId",stationId)
            };

            return DataConvert.ToDecimal(SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public void DeleteOldDeduct(long waybillNO, int deductType)
        {
            string sql = "update FMS_DeductSubBaseInfo set IsDeleted=1,UpdateTime=getdate(),IsChange=2 where waybillno=@waybillno and DeductType=@DeductType and IsDeleted=0";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@waybillno",waybillNO),
                new SqlParameter("@DeductType",deductType)
            };

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }

        public IList<long> CheckRepeatDeduct()
        {
            string sql = @"select a.WaybillNO,a.DeductType from FMS_DeductSubBaseInfo a(nolock),FMS_DeductSubBaseInfo b(nolock) 
                where a.WaybillNO=b.WaybillNO and a.DeductType=b.DeductType and a.IsDeleted=0
                and b.IsDeleted=0 and a.ID!=b.ID and a.CreateTime >= '2012-10-01 00:00:00'";

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];

            long waybill = -1;
            int deductType = -1;

            DataRow row = null;

            IList<long> waybillNos = new List<long>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                waybill = DataConvert.ToLong(row["WaybillNO"]);
                deductType = DataConvert.ToInt(row["DeductType"]);

                //ClearErrorDeduct(waybill,deductType);

                waybillNos.Add(waybill);
            }

            return waybillNos;
        }

        public IList<long> ClearNotEvalDeduct()
        {
            string sql = @"select wb.WaybillNO from Waybill wb(nolock) 
                inner join RFD_PMS.dbo.ExpressCompany ecp(nolock) on wb.DeliverStationID=ecp.ExpressCompanyID
                left join FMS_DeductSubBaseInfo subDeduct(nolock) on wb.WaybillNO=subDeduct.WaybillNO
                where wb.Status='3' and subDeduct.ID is null and wb.CreatTime > '2012-10-01 00:00:00' and wb.BackStationTime < DATEADD(mi, -60, GETDATE()) 
                and ecp.DistributionCode='rfd'";

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];

            DataRow row = null;

            long waybillNO = -1;

            IList<long> waybillNos = new List<long>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                waybillNO = DataConvert.ToLong(row["WaybillNO"]);

                waybillNos.Add(waybillNO);
            }

            return waybillNos;
        }

        public IList<long> RepairExpressDelete()
        {
            string sql = @"select subInfo.ID,subInfo.WaybillNO from LMS_RFD.dbo.ExpressOrder eo(nolock) 
                inner join LMS_RFD.dbo.FMS_DeductSubBaseInfo subInfo(nolock) on eo.WaybillNo=subInfo.WaybillNO
                where eo.IsDelete=1 and subInfo.IsDeleted=0 and subInfo.DeductType=1";

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];

            DataRow row = null;

            string sqlUpdate = @"update LMS_RFD.dbo.FMS_DeductSubBaseInfo set IsDeleted=1,UpdateTime=getdate(),IsChange=2 where ID={0}";

            long id = -1;
            long waybillNo = -1;

            IList<long> waybillNos = new List<long>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToLong(row["ID"]);
                waybillNo = DataConvert.ToLong(row["WaybillNO"]);

                SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, String.Format(sqlUpdate,id));

                waybillNos.Add(waybillNo);
            }

            return waybillNos;
        }

        public IList<long> RepairExpressDate()
        {
            IList<long> ids = new List<long>();

            string sql = @"select info.ID,eo.CreateTime from LMS_RFD.dbo.FMS_DeductSubBaseInfo info(nolock) 
                inner join LMS_RFD.dbo.ExpressOrder eo(nolock) on info.WaybillNO=eo.WaybillNo
                where info.DeductType=1 
                and CONVERT(varchar(100), info.CreateTime, 23) != CONVERT(varchar(100), eo.CreateTime, 23)
                and eo.ComplementTime > '2012-10-01 00:00:00' and eo.IsDelete=0";

            DataTable table = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];

            DataRow row = null;

            long id = -1;
            string expressTime = null;

            string updateSql = "update LMS_RFD.dbo.FMS_DeductSubBaseInfo set CreateTime='{0}' ,IsChange=2   where ID={1}";

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToLong(row["ID"]);
                expressTime = DataConvert.ToString(row["CreateTime"]);

                ids.Add(id);

                SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, String.Format(updateSql,expressTime,id));
            }

            return ids;
        }

        public IList<long> RepairFalseDelete()
        {
            IList<long> waybillNos = new List<long>();

            string sql = @"select wb.WaybillNO from Waybill wb(nolock) where wb.Status='3' 
		        and wb.WaybillNO in
		        (
			        select WaybillNO from FMS_DeductSubBaseInfo f1(nolock)
			         where f1.IsDeleted=1 
				        and f1.CreateTime > '2012-10-01 00:00:00'   
				        and WaybillNO in (
			        select WaybillNO from (
			        select WaybillNO,COUNT(1) as num from FMS_DeductSubBaseInfo f(nolock)
			        where (1=1)
			        and f.CreateTime > '2012-10-01 00:00:00'  
			        group by WaybillNO) aa
			        where aa.num=1)
		        )";

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];

            string updateSql = "update FMS_DeductSubBaseInfo set IsDeleted=0,IsChange=2 where WaybillNO={0}";

            DataRow row = null;

            string waybill = "";

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                waybill = DataConvert.ToString(row["WaybillNo"]);

                waybillNos.Add(DataConvert.ToLong(waybill));

                SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, String.Format(updateSql, waybill));
            }

            return waybillNos;
        }

        private void ClearErrorDeduct(long waybill, int deduct)
        {
            string sql = "select ID from FMS_DeductSubBaseInfo(nolock) where WaybillNO=@WaybillNO and DeductType=@DeductType and IsDeleted=0";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybill),
                new SqlParameter("@DeductType",deduct)
            };

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];

            DataRow row = null;

            string sqlUpdate = "update FMS_DeductSubBaseInfo set IsDeleted=1,UpdateTime=getdate(),IsChange=2 where ID=";

            for (int i = 0; i < table.Rows.Count - 1; i++)
            {
                row = table.Rows[i];

                SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlUpdate + DataConvert.ToLong(row["ID"]));
            }
        }

        private void UpdateBackStation(long id, DateTime? backStaionTime)
        {
            string sql = "update FMS_DeductSubBaseInfo set CreateTime=@CreateTime,IsChange=2 where ID=@ID";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@CreateTime",backStaionTime),
                new SqlParameter("@ID",id)
            };

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }

        public bool IsDeleted(long deductId)
        {
            string sql = "select count(1) from FMS_DeductBaseInfo(nolock) where DeductId=@DeductId and IsDeleted=1";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@DeductId",deductId)
            };

            return DataConvert.ToInt(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql, parameters)) > 0;
        }

        //public void DeleteOldDeduct(long waybillNO, int deductType)
        //{
        //    string sql = "update FMS_DeductSubBaseInfo set IsDeleted=1 where waybillno=:waybillno and DeductType=:DeductType and IsDeleted=0";

        //    SqlParameter[] parameters = 
        //    {
        //        new SqlParameter(":waybillno",waybillNO),
        //        new SqlParameter(":DeductType",deductType)
        //    };

        //    SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        //}

        public void UpdateSubAreaDeduct(FMS_DeductSubBaseInfo model)
        {
            string sql = "update FMS_DeductSubBaseInfo SET AREACOMMISSION=@AREACOMMISSION where waybillno=@waybillno and DeductType=@DeductType and IsDeleted=0";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@AREACOMMISSION",model.AreaCommission),
                new SqlParameter("@waybillno",model.WaybillNO),
                new SqlParameter("@DeductType",model.DeductType)
            };

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }
    }
}
