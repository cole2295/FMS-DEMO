using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.COD;

namespace RFD.FMS.DAL.COD
{
    public class AccountOperatorLogDao : SqlServerDao, IAccountOperatorLogDao
	{
        public string strSql { get; set; }

        //2、商家配送费，3、商家基础信息，4商家基础信息待生效，5、商家配送费待生效
        public bool AddOperatorLogLog(string PK_NO, int createBy, string logText, int logType)
        {
            strSql = @"INSERT INTO RFD_FMS.dbo.FMS_CODOperatorLog
							   (PK_NO
							   ,CreateBy
							   ,CreateTime
							   ,LogText
							   ,LogType
                               ,IsChange)
						 VALUES
							   (@PK_NO
							   ,@CreateBy
							   ,GETDATE()
							   ,@LogText
							   ,@LogType
                               ,@IsChange)";
            SqlParameter[] parameters ={
										   new SqlParameter("@PK_NO",SqlDbType.NVarChar,20),
										   new SqlParameter("@CreateBy",SqlDbType.Int),
										   new SqlParameter("@LogText",SqlDbType.NVarChar,250),
										   new SqlParameter("@LogType",SqlDbType.Int),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = PK_NO;
            parameters[1].Value = createBy;
            parameters[2].Value = logText;
            parameters[3].Value = logType;
            parameters[4].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataTable GetOperatorLogLog(string pk_No, int logType)
        {
            string sql = @"
SELECT fcol.LogID,PK_NO,e.EmployeeName,fcol.CreateTime,fcol.LogText
 FROM dbo.FMS_CODOperatorLog fcol(NOLOCK)
 JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON e.EmployeeID=fcol.CreateBy
 WHERE PK_NO=@PK_NO AND LogType=@LogType ORDER BY fcol.CreateTime DESC
";
            SqlParameter[] parameters ={
                                           new SqlParameter("@PK_NO",SqlDbType.NVarChar,20),
                                           new SqlParameter("@LogType",SqlDbType.Int),
                                      };
            parameters[0].Value = pk_No;
            parameters[1].Value = logType;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
	}
}
