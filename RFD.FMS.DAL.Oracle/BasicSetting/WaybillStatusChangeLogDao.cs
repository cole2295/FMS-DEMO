using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;
using Oracle.ApplicationBlocks.Data;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class WaybillStatusChangeLogDao : OracleDao, IWaybillStatusChangeLogDao
    {
        #region IWaybillStatusChangeLogDao 成员

        public bool UpdateSynStatus(long id)
        {
            string sql = String.Format("update LMS_WaybillStatusChangeLog set IsSynFms=1,IsChange=3  where WAYBILLSTATUSCHANGELOGID=:ID");

            OracleParameter[] parameters = 
            {
                new OracleParameter(":ID",id)
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);

            return true;
        }

        public DataTable GetSynWaybillLogs(int count)
        {
            string sql =
                @"select 
WAYBILLSTATUSCHANGELOGID AS ID,
WaybillNo,
CurNode,
Status,
SubStatus,
MerchantID,
DistributionCode,
DeliverStationID,
CreateTime,
CreateBy,CREATESTATION,
NOTE AS Description,
IsSyn,
CustomerOrder 
from PS_LMS.LMS_WaybillStatusChangeLog
where CreateTime > sysdate-15 and CreateTime < sysdate-15/24/60 
and (IsSynFms=0 OR IsSynFms is null) and rownum<=:SqlNum
";
            OracleParameter[] parameters = { new OracleParameter(":SqlNum", OracleDbType.Decimal) };
            parameters[0].Value = count;
            DataTable table =new DataTable();
            try
            {
                var ds = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql, parameters);
                if (ds!=null&&ds.Tables.Count>0)
                {
                    table = ds.Tables[0];
                }
                // = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql).Tables[0];
            }
            catch (Exception)
            {

                return null;
            }
            

            return table;
        }

        public List<WaybillStatusChangeLog> GetWaybillStatus(List<string> ids)
        {
            string sql = String.Format(@"select LMS_WaybillStatusChangeLogRecord as ID,WaybillNo,CurNode,Status,SubStatus,MerchantID,DistributionCode,DeliverStationID,CreateTime,CreateBy,
                                                CreateStation,Description,IsSyn,CustomerOrder 
                                        from LMS_WaybillStatusChangeLog where ID in ({0})", DataConvert.ToDbIds(ids));

            DataTable table = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
//            string sql = String.Format(@"select LMS_WaybillStatusChangeLogRecord as ID,WaybillNo,CurNode,Status,SubStatus,MerchantID,DistributionCode,DeliverStationID,CreateTime,CreateBy,
//                                                CreateStation,Description,IsSyn,CustomerOrder 
//                                        from LMS_WaybillStatusChangeLog where 1=1 ", DataConvert.ToDbIds(ids));
//            sql += Util.Common.GetOracleInParameterWhereSql(" WAYBILLSTATUSCHANGELOGID", "WAYBILLSTATUSCHANGELOGID", false, false);
//            OracleParameter[] parameters =
//            {
//             new OracleParameter(":WAYBILLSTATUSCHANGELOGID",OracleDbType.Decimal){Value =DataConvert.ToDbIds(ids).Replace(" ", "")}, 
//            }
//            ;
//            DataTable table = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            
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
            strSql.Append("insert into LMS_WaybillStatusChangeLogRecord(");
            strSql.Append("ClassKey,LogID,SynTimes,IsDeleted,CreateTime,UpdateTime,IsChange)");
            strSql.Append(" values (");
            strSql.Append(":ClassKey,:LogID,:SynTimes,:IsDeleted,sysdate,sysdate,3)");
            OracleParameter[] parameters = 
            {
                new OracleParameter(":ClassKey", OracleDbType.Varchar2,200) { Value=key },
                new OracleParameter(":LogID", OracleDbType.Decimal) { Value=logId },
                new OracleParameter(":SynTimes", OracleDbType.Decimal) { Value=1 },
                new OracleParameter(":IsDeleted", OracleDbType.Decimal) { Value=false }
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
        }

        public DataTable GetFailseLogByClassKey(string classkey)
        {
            string sql = @"select
	                        clog.WAYBILLSTATUSCHANGELOGID as ID,
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
                        from LMS_WaybillStatusChangeLog clog
	                        inner join LMS_WaybillStatusChangeLogRecord record on clog.ID=record.LogID and record.IsDeleted=0
                        where record.ClassKey=:ClassKey and rownum<51";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":ClassKey", OracleDbType.Varchar2,200) { Value=classkey }
            };

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters).Tables[0];
        }

        public void UpdateFailseLog(long id)
        {
            string sql = "update LMS_WaybillStatusChangeLogRecord set IsDeleted=1,IsChange=3  where WAYBILLSTATUSCHANGELOGID=:ID";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":ID", OracleDbType.Decimal) { Value=id }
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
        }
    }
}
