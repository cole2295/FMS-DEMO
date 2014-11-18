using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.DataAccess.Client;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using System.Data;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class AccountPeriodDao : OracleDao,IAccountPeriodDao
    {
        public bool AddAccountPeriod(AccountPeriod ap)
        {
            string sql = @"INSERT INTO FMS_ACCOUNTPERIOD(
                                ACCOUNTPERIODKID,PERIODTYPE,PERIODTYPECHILD,PERIODSTART,ISMONTHPERIOD,ISEXPRESS,EXPRESSIDS,ISDELETED,CREATEBY,CREATETIME,
                                UPDATEBY,UPDATETIME,DISTRIBUTIONCODE,PERIODRELATIONNAME,PERIODNAME,INTERVALNUM 
                            )
                           VALUES(
                                :ACCOUNTPERIODKID,:PERIODTYPE,:PERIODTYPECHILD,:PERIODSTART,:ISMONTHPERIOD,:ISEXPRESS,:EXPRESSIDS,:ISDELETED,:CREATEBY,SYSDATE,
                                :UPDATEBY,SYSDATE,:DISTRIBUTIONCODE,:PERIODRELATIONNAME,:PERIODNAME,:INTERVALNUM
                            )";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":ACCOUNTPERIODKID", OracleDbType.Varchar2) { Value = ap .AccountPeriodKid});
            parameterList.Add(new OracleParameter(":PERIODTYPE", OracleDbType.Decimal) { Value = ap.PeriodType });
            parameterList.Add(new OracleParameter(":PERIODTYPECHILD", OracleDbType.Decimal) { Value = ap.PeriodTypeChild });
            parameterList.Add(new OracleParameter(":PERIODSTART", OracleDbType.Varchar2) { Value = ap.PeriodStart });
            parameterList.Add(new OracleParameter(":ISMONTHPERIOD", OracleDbType.Decimal) { Value = ap.IsMonthPeriod });
            parameterList.Add(new OracleParameter(":ISEXPRESS", OracleDbType.Decimal) { Value = ap.IsExpress });
            parameterList.Add(new OracleParameter(":EXPRESSIDS", OracleDbType.Varchar2) { Value = ap.ExpressIds });
            parameterList.Add(new OracleParameter(":ISDELETED", OracleDbType.Decimal) { Value = ap.IsDeleted });
            parameterList.Add(new OracleParameter(":CREATEBY", OracleDbType.Decimal) { Value = ap.CreateBy });
            parameterList.Add(new OracleParameter(":UPDATEBY", OracleDbType.Decimal) { Value = ap.UpdateBy });
            parameterList.Add(new OracleParameter(":DISTRIBUTIONCODE", OracleDbType.Varchar2) { Value = ap.DistributionCode });
            parameterList.Add(new OracleParameter(":PERIODRELATIONNAME", OracleDbType.Varchar2) { Value = ap.PeriodRelationName });
            parameterList.Add(new OracleParameter(":PERIODNAME", OracleDbType.Varchar2) { Value = ap.PeriodName });
            parameterList.Add(new OracleParameter(":INTERVALNUM", OracleDbType.Decimal) { Value = ap.IntervalNum });
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, ToParameters(parameterList.ToArray()))>0;
        }

        public DataTable SearchAccountPeriod(AccountPeriodCondition apc)
        {
            string sql = @"SELECT ACCOUNTPERIODKID,PERIODTYPE,PERIODTYPECHILD,PERIODSTART,ISMONTHPERIOD,ISEXPRESS,EXPRESSIDS,ISDELETED,CREATEBY,CREATETIME,
                                UPDATEBY,UPDATETIME,DISTRIBUTIONCODE,PERIODRELATIONNAME,PERIODNAME,INTERVALNUM 
                            FROM FMS_ACCOUNTPERIOD WHERE (1=1) {0}";
            StringBuilder sqlWhere = new StringBuilder();
            List<OracleParameter> parameterList = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(apc.PeriodRelationName))
            {
                sqlWhere.Append(" AND PERIODRELATIONNAME=:PERIODRELATIONNAME ");
                parameterList.Add(new OracleParameter(":PERIODRELATIONNAME", OracleDbType.Varchar2) { Value = apc.PeriodRelationName });
            }

            if (!string.IsNullOrEmpty(apc.AccountPeriodKid))
            {
                sqlWhere.Append(" AND ACCOUNTPERIODKID=:ACCOUNTPERIODKID ");
                parameterList.Add(new OracleParameter(":ACCOUNTPERIODKID", OracleDbType.Varchar2) { Value = apc.AccountPeriodKid });
            }

            if (!string.IsNullOrEmpty(apc.IsDeleted))
            {
                sqlWhere.Append(" AND ISDELETED=:ISDELETED ");
                parameterList.Add(new OracleParameter(":ISDELETED", OracleDbType.Decimal) { Value = apc.IsDeleted });
            }

            sql = string.Format(sql,sqlWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray())).Tables[0];
        }

        public bool UpdateAccountPeriod(AccountPeriod ap)
        {
            string sql = @"UPDATE FMS_ACCOUNTPERIOD SET 
                                PERIODNAME=:PERIODNAME,
                                ISDELETED=:ISDELETED,
                                UPDATEBY=:UPDATEBY,
                                UPDATETIME=SYSDATE 
                            WHERE ACCOUNTPERIODKID=:ACCOUNTPERIODKID";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":ACCOUNTPERIODKID", OracleDbType.Varchar2) { Value = ap.AccountPeriodKid });
            parameterList.Add(new OracleParameter(":PERIODNAME", OracleDbType.Varchar2) { Value = ap.PeriodName });
            parameterList.Add(new OracleParameter(":ISDELETED", OracleDbType.Decimal) { Value = ap.IsDeleted });
            parameterList.Add(new OracleParameter(":UPDATEBY", OracleDbType.Decimal) { Value = ap.UpdateBy });
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, ToParameters(parameterList.ToArray())) > 0;
        }
    }
}
