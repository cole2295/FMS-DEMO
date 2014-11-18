using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.COD;

namespace RFD.FMS.DAL.Oracle.COD
{
	public class AccountOperatorLogDao : OracleDao, IAccountOperatorLogDao
	{
        public string strSql { get; set; }

        //2、商家配送费，3、商家基础信息，4商家基础信息待生效，5、商家配送费待生效
        public bool AddOperatorLogLog(string PK_NO, int createBy, string logText, int logType)
        {
            strSql = @"INSERT INTO FMS_CODOperatorLog
							   (logid,PK_NO
							   ,CreateBy
							   ,CreateTime
							   ,LogText
							   ,LogType
                               ,IsChange)
						 VALUES
							   (SEQ_FMS_CODOPERATORLOG.Nextval,:PK_NO
							   ,:CreateBy
							   ,SysDate
							   ,:LogText
								,:LogType
                                ,:IsChange)";
            OracleParameter[] parameters ={
										   new OracleParameter(":PK_NO",OracleDbType.Varchar2,40),
										   new OracleParameter(":CreateBy",OracleDbType.Decimal),
										   new OracleParameter(":LogText",OracleDbType.Varchar2,500),
										   new OracleParameter(":LogType",OracleDbType.Decimal),
                                           new OracleParameter(":IsChange",OracleDbType.Decimal),
									  };
            parameters[0].Value = PK_NO;
            parameters[1].Value = createBy;
            parameters[2].Value = logText;
            parameters[3].Value = logType;
            parameters[4].Value = 1;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataTable GetOperatorLogLog(string pk_No, int logType)
        {
            string sql = @"
SELECT fcol.LogID,PK_NO,e.EmployeeName,fcol.CreateTime,fcol.LogText
 FROM FMS_CODOperatorLog fcol
 JOIN Employee e ON e.EmployeeID=fcol.CreateBy
 WHERE PK_NO=:PK_NO AND LogType=:LogType  ORDER BY fcol.CreateTime DESC
";
            OracleParameter[] parameters ={
                                           new OracleParameter(":PK_NO",OracleDbType.Varchar2,40),
                                           new OracleParameter(":LogType",OracleDbType.Decimal),
                                      };
            parameters[0].Value = pk_No;
            parameters[1].Value = logType;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
	}
}
