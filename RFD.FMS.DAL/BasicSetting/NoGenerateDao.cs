using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Data;
using RFD.FMS.DAL;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Util;


namespace RFD.FMS.DAL.BasicSetting
{
    public class NoGenerateDao : SqlServerDao, INoGenerateDao
	{
        /// <summary>
        /// 生成种子号
        /// </summary>
        /// <param name="NoType"></param>
        /// <returns></returns>
        public string GetLastNo(int NoType, string dateStr)
        {
            string sql =
                @"UPDATE LMS_RFD.dbo.NoGenerate
            SET     @LatestNo = LatestNo = LatestNo + 1,
                    IsChange=2
            WHERE   Date = @date
                    AND NoType = @NoType 
            IF ( @@rowcount = 0 ) 
                BEGIN	
                    UPDATE  LMS_RFD.dbo.NoGenerate
                    SET     @LatestNo = LatestNo = 1 ,
                            Date = @date,
                            IsChange=2
                    WHERE   NoType = @NoType 
                END
                SELECT @LatestNo
            ";

            var lastNo = new SqlParameter("@LatestNo", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            SqlParameter[] prams = new[]
			                       	{
			                       		 lastNo, new SqlParameter("@NoType", NoType),
			                       		new SqlParameter("@date", dateStr)
			                       	};


            if (SqlHelper.ExecuteNonQuery(Connection, System.Data.CommandType.Text, sql, prams) > 0)
            {
                return lastNo.Value.ToString();
            }
            return "0";

        }

        public string GetLastNo(int noType)
        {
            string sql = @"UPDATE  LMS_RFD.dbo.NoGenerate
							SET     @LatestNo = LatestNo = LatestNo + 1,
                                     IsChange=2
							WHERE   NoType = @NoType 
								SELECT @LatestNo
							";
            var lastNo = new SqlParameter("@LatestNo", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            SqlParameter[] prams = new[]
			                       	{
			                       		 lastNo, new SqlParameter("@NoType", noType)
			                       	};


            if (SqlHelper.ExecuteNonQuery(Connection, System.Data.CommandType.Text, sql, prams) > 0)
            {
                return lastNo.Value.ToString();
            }
            return "0";
        }

        public string GetLastNo(string tableName, string columnName, out DateTime? dbDate, out string tabColCode)
        {
            string sql =
                @" set @CurrentDate  = GETDATE()
                   
                   --获得编码
                   SELECT   @TabColCode = TabColCode
                   FROM     FMSTableColumnDic(NOLOCK)
                   WHERE    TableName = @TableName
                            AND ColumnName = @ColumnName
                   
                   --没有找到，直接返回0
                   IF ( @TabColCode IS NULL ) 
                    BEGIN
                        SELECT  0
                        RETURN
                    END

                   --插入新号
                   INSERT INTO FMS_NoGenerateExV2(CurrentDate) values (@CurrentDate);
                   set @LatestNo=SCOPE_IDENTITY();

                   SELECT @LatestNo,@CurrentDate,@TabColCode
                    ";

            var lastNo = new SqlParameter("@LatestNo", System.Data.SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            var dbSysDate = new SqlParameter("@CurrentDate", SqlDbType.Date)
            {
                Direction = ParameterDirection.Output
            };
            var tcCode = new SqlParameter("@TabColCode", SqlDbType.VarChar, 3)
            {
                Direction = ParameterDirection.Output
            };

            var prams = new[]
			{
			    lastNo,
                dbSysDate,
                tcCode,
                new SqlParameter("@TableName",SqlDbType.VarChar,50 ){ Value=tableName},
                new SqlParameter("@ColumnName",SqlDbType.VarChar,50 ){ Value=columnName}
			};

            int i = 0;
            try
            {
                i = SqlHelper.ExecuteNonQuery(Connection, System.Data.CommandType.Text, sql, prams);
            }
            catch (Exception ex)
            {
                throw new Exception("获得自增ID流水号失败:" + ex.Message);
            }
            if (i < 1)
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

            dbDate = ((DateTime)dbSysDate.Value).AddYears(50);
            tabColCode = tcCode.Value.ToString();

            return lastNo.Value.ToString();
        }
    }
}
