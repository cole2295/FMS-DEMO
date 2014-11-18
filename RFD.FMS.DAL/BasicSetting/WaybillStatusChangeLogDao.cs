using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.BasicSetting
{
    public class WaybillStatusChangeLogDao : SqlServerDao, IWaybillStatusChangeLogDao
    {
        #region IWaybillStatusChangeLogDao 成员

        public bool UpdateSynStatus(long id)
        {
            string sql = String.Format("update LMS_RFD.dbo.LMS_WaybillStatusChangeLog set IsSynFms=1,IsChange=2 where ID=@ID");

            SqlParameter[] parameters = 
            {
                new SqlParameter("@ID",id)
            };

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);

            return true;
        }

        public DataTable GetSynWaybillLogs(int count)
        {
            string sql = String.Format(@"select top {0} ID,WaybillNo,CurNode,Status,SubStatus,MerchantID,DistributionCode,DeliverStationID,CreateTime,CreateBy,CreateStation,Description,IsSyn,CustomerOrder from LMS_RFD.dbo.LMS_WaybillStatusChangeLog with(nolock,forceseek) where CreateTime > DATEADD(day,-1,getdate()) and CreateTime < DATEADD(n,-30,getdate()) and (IsSynFms=0 OR IsSynFms is null)", count);

            DataTable table = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];

            return table;
        }

        public List<WaybillStatusChangeLog> GetWaybillStatus(List<string> ids)
        {
            string sql = String.Format("select ID,WaybillNo,CurNode,Status,SubStatus,MerchantID,DistributionCode,DeliverStationID,CreateTime,CreateBy,CreateStation,Description,IsSyn,CustomerOrder from LMS_RFD.dbo.LMS_WaybillStatusChangeLog(nolock) where ID in ({0})", DataConvert.ToDbIds(ids));

            DataTable table = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];

            List<WaybillStatusChangeLog> models = new List<WaybillStatusChangeLog>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                models.Add(DataRowToObject(table.Rows[i]));
            }

            return models;
        }

        #endregion

        public WaybillStatusChangeLog DataRowToObject(DataRow dataRow)
        {
            WaybillStatusChangeLog model = new WaybillStatusChangeLog();

            model.ID = DataConvert.ToLong(dataRow["ID"]);
            model.WaybillNo = DataConvert.ToLong(dataRow["WaybillNo"]);
            model.CurNode = DataConvert.ToInt(dataRow["CurNode"]);
            model.Status = DataConvert.ToString(dataRow["Status"]);
            model.SubStatus = DataConvert.ToInt(dataRow["SubStatus"]);
            model.MerchantID = DataConvert.ToInt(dataRow["MerchantID"]);
            model.DistributionCode = DataConvert.ToString(dataRow["DistributionCode"]);
            model.DeliverStationID = DataConvert.ToInt(dataRow["DeliverStationID"]);
            model.CreateTime = DataConvert.ToDateTime(dataRow["CreateTime"]);
            model.CreateBy = DataConvert.ToInt(dataRow["CreateBy"]);
            model.CreateStation = DataConvert.ToInt(dataRow["CreateStation"]);
            model.IsSyn = DataConvert.ToInt(dataRow["IsSyn"]) == 1;
            model.Description = DataConvert.ToString(dataRow["Description"]);
            model.CustomerOrder = DataConvert.ToString(dataRow["CustomerOrder"]);

            return model;
        }


        public void RecordFailseLog(long logId, string key)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into LMS_RFD.dbo.LMS_WaybillStatusChangeLogRecord(");
            strSql.Append("ClassKey,LogID,SynTimes,IsDeleted,CreateTime,UpdateTime,IsChange)");
            strSql.Append(" values (");
            strSql.Append("@ClassKey,@LogID,@SynTimes,@IsDeleted,@CreateTime,@UpdateTime,2)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = 
            {
                new SqlParameter("@ClassKey", SqlDbType.VarChar,100) { Value=key },
                new SqlParameter("@LogID", SqlDbType.BigInt) { Value=logId },
                new SqlParameter("@SynTimes", SqlDbType.Int ,4) { Value=1 },
                new SqlParameter("@IsDeleted", SqlDbType.BigInt) { Value=false },
			    new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value=DateTime.Now },
                new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value=DateTime.Now }
            };

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
        }

        public DataTable GetFailseLogByClassKey(string classkey)
        {
            string sql = @"select top 50
	                        clog.ID,
	                        clog.WaybillNo,
	                        clog.CurNode,
	                        clog.Status,
	                        clog.SubStatus,
	                        clog.MerchantID,
	                        clog.DistributionCode,
	                        clog.DeliverStationID,
	                        clog.CreateTime,
	                        clog.CreateBy,
	                        clog.CreateStation,
	                        clog.Description,
	                        clog.IsSyn,
	                        clog.CustomerOrder,
	                        record.ID RecordID
                        from LMS_RFD.dbo.LMS_WaybillStatusChangeLog clog(nolock)
	                        inner join LMS_RFD.dbo.LMS_WaybillStatusChangeLogRecord record(nolock) on clog.ID=record.LogID and record.IsDeleted=0
                        where record.ClassKey=@ClassKey";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@ClassKey", SqlDbType.VarChar,100) { Value=classkey }
            };

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];
        }

        public void UpdateFailseLog(long id)
        {
            string sql = "update LMS_RFD.dbo.LMS_WaybillStatusChangeLogRecord set IsDeleted=1,IsChange=2 where ID=@ID";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@ID", SqlDbType.BigInt) { Value=id }
            };

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }
    }
}
