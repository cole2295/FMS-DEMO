using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Data;
using RFD.FMS.DAL;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Util;
using Oracle.DataAccess.Client;
using RFD.FMS.AdoNet.UnitOfWork;


namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class NoGenerateDao : OracleDao, INoGenerateDao
	{
        /// <summary>
        /// 生成种子号
        /// </summary>
        /// <param name="NoType"></param>
        /// <returns></returns>
        public string GetLastNo(int NoType, string dateStr)
        {
//            string sql =
//                @"
//            BEGIN
//                SELECT LatestNo + 1 into :LatestNo FROM FMS_NoGenerate WHERE   NOGENERATEDATE = :dateFlag AND NoType = :NoType;
//                UPDATE FMS_NoGenerate SET LatestNo =:LatestNo WHERE   NOGENERATEDATE = :dateFlag AND NoType = :NoType ;
//                IF SQL%ROWCOUNT = 0 then
//                    BEGIN	
//                        UPDATE  FMS_NoGenerate SET LatestNo = 1 , NOGENERATEDATE = :dateFlag WHERE NoType = :NoType;
//                        SELECT 1 into :LatestNo FROM dual;
//                    END;
//                end if; 
//            END;
//            ";
            string sql = @"
BEGIN
    UPDATE FMS_NoGenerate SET LatestNo =LatestNo+1 ,IsChange=3 WHERE   NOGENERATEDATE = :dateFlag AND NoType = :NoType;
    SELECT LatestNo INTO :LatestNo FROM FMS_NoGenerate WHERE   NOGENERATEDATE = :dateFlag AND NoType = :NoType;
    EXCEPTION
         WHEN NO_DATA_FOUND THEN
         begin
              UPDATE FMS_NoGenerate SET LatestNo =1,NOGENERATEDATE = :dateFlag,IsChange=3  WHERE NoType = :NoType;
              select LatestNo into :LatestNo from FMS_NoGenerate WHERE   NOGENERATEDATE = :dateFlag AND NoType = :NoType;
         end;
end;
";
            OracleParameter[] prams = 
			                       	{
			                       		  new OracleParameter(":LatestNo", OracleDbType.Decimal),
                                          new OracleParameter(":NoType", OracleDbType.Decimal),
			                       		  new OracleParameter(":dateFlag", OracleDbType.NChar,8)
			                       	};
            prams[0].Direction = ParameterDirection.Output;
            prams[1].Value = NoType;
            prams[2].Value = dateStr;

            OracleHelper.ExecuteNonQuery(Connection, System.Data.CommandType.Text, sql, prams);
            if (!string.IsNullOrEmpty(prams[0].Value.ToString()))
            {
                return prams[0].Value.ToString();
            }
            else
                return "0";
        }

        public string GetLastNo(int noType)
        {
            string sql = @"BEGIN
                            UPDATE  FMS_NoGenerate SET LatestNo=LatestNo + 1 WHERE   NoType = :NoType ;
							SELECT LatestNo INTO :LatestNo FROM FMS_NoGenerate WHERE NoType = :NoType ;
                          END;
							";
            var lastNo = new OracleParameter(":LatestNo", OracleDbType.Decimal)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            OracleParameter[] prams = new[]
			                       	{
			                       		 lastNo, new OracleParameter(":NoType", noType)
			                       	};


            OracleHelper.ExecuteNonQuery(Connection, System.Data.CommandType.Text, sql, prams);
            if (!string.IsNullOrEmpty(prams[0].Value.ToString()))
            {
                return prams[0].Value.ToString();
            }
            else
                return "0";
        }

        public string GetLastNo(string tableName, string columnName, out DateTime? dbDate, out string tabColCode)
        {
            string sql =
                @"begin
                   :CurrentDate := to_date(to_char(sysdate,'yyyy-mm-dd'),'yyyy-mm-dd');
                   --获得编码
                    SELECT   TabColCode into :TabColCode
                   FROM     FMSTableColumnDic
                   WHERE    TableName = :TableName
                            AND ColumnName = :ColumnName;
                    if :TabColCode IS NULL then
                     begin
                        dbms_output.put_line(0); 
                        RETURN;
                     end;
                     end if;

                   --插入新号
                   select seq_fms_nogenerateexv2.Nextval into :LatestNo from dual;
 
                   INSERT INTO FMS_NoGenerateExV2(generateid,CurrentDate) values (:LatestNo,:CurrentDate);
                   end;
                   ";

            var lastNo = new OracleParameter(":LatestNo", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };
            var dbSysDate = new OracleParameter(":CurrentDate", OracleDbType.Date)
            {
                Direction = ParameterDirection.Output
            };
            var tcCode = new OracleParameter(":TabColCode", OracleDbType.Varchar2, 6)
            {
                Direction = ParameterDirection.Output
            };

            var prams = new[]
			{
			    lastNo,
                dbSysDate,
                tcCode,
                new OracleParameter(":TableName",OracleDbType.Varchar2,100 ){ Value=tableName},
                new OracleParameter(":ColumnName",OracleDbType.Varchar2,100 ){ Value=columnName}
			};

            
            try
            {
                var i = OracleHelper.ExecuteNonQuery(Connection, System.Data.CommandType.Text, sql, prams);
            }
            catch (Exception ex)
            {
                throw new Exception("获得自增ID流水号失败:" + ex.Message);
            }
            if (DataConvert.ToLong(lastNo.Value) < 1)
            {
                throw new Exception(string.Format("获得自增ID流水号失败,可能是TableName:{0}ColumnName{1}在表里没有对应记录", tableName, columnName));
            }


            if (DBNull.Value == dbSysDate.Value
               || DBNull.Value == lastNo.Value
                || DBNull.Value == tcCode.Value
                )
            {
                throw new Exception(string.Format("获得自增ID流水号失败,可能是TableName:{0}ColumnName{1}在表里没有对应记录", tableName, columnName));
            }

            dbDate = Convert.ToDateTime(UnBoxOracleType(dbSysDate.Value)).AddYears(50);
            tabColCode = tcCode.Value.ToString();

            return lastNo.Value.ToString();
        }
    }
}
