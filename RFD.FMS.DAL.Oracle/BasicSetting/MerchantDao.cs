using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class MerchantDao : OracleDao, IMerchantDao
    {
        /// <summary>
        /// 获取相应的商家信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetMerchants(string disCode)
        {
            var strSql = new StringBuilder();
            if (disCode == "rfd")
            {
                strSql.Append(@"
			        SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0 AND m.Sources = 2
AND dmr.DistributionCode=:DistributionCode 
ORDER BY m.CreatTime");
            }
            else
            {
                strSql.Append(@"SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=:DistributionCode
ORDER BY m.CreatTime");
            }

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = disCode;

            var ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            return ds != null ? ds.Tables[0] : null;
        }

        /// <summary>
        /// 获取相应的商家信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetMerchantsNoExpress(string disCode)
        {
            var strSql = new StringBuilder();
            if (disCode == "rfd")
            {
                strSql.Append(@"
			        SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0 AND m.Sources = 2
AND dmr.DistributionCode=:DistributionCode  AND m.IsPeriodAccount=0 AND m.ID<>30
ORDER BY m.CreatTime");
            }
            else
            {
                strSql.Append(@"SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=:DistributionCode
ORDER BY m.CreatTime");
            }

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = disCode;

            var ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            return ds != null ? ds.Tables[0] : null;
        }


        public DataTable GetAllMerchants(string disCode)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
			    SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=:DistributionCode
ORDER BY m.CreatTime");

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = disCode;

            var ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            return ds != null ? ds.Tables[0] : null;
        }


        public string GetMerchantName(long waybillNo)
        {
            string sql = @"select mbi.MerchantName from Waybill wb 
                inner join MerchantBaseInfo mbi on wb.MerchantID=mbi.ID
                where wb.WaybillNO=:WaybillNO";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillNO",waybillNo)
            };

            return DataConvert.ToString(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public string GetMerchantCategory(int merchantId)
        {
            string sql = @"select mgc.GoodsCategoryCode from MerchantGoodsCategoryRelation mgc 
                where mgc.MerchantID=:MerchantID and not mgc.GoodsCategoryCode is null and mgc.GoodsCategoryCode!=''";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":MerchantID",merchantId)
            };

            return DataConvert.ToString(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        /// 业务调用 AddExpressOrder.aspx 快递单计算重量
        public int GetMerchantDeliverFee(int merchantId)
        {
            var strSql = @"SELECT WeightValueRule FROM FMS_MerchantDeliverFee WHERE MerchantID=:MerchantId AND Status=2 and WeightType=3";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":MerchantId",OracleDbType.Decimal),
            };

            parameters[0].Value = merchantId;

            DataTable dt = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            if (dt == null || dt.Rows.Count <= 0)
                return -1;
            return string.IsNullOrEmpty(dt.Rows[0]["WeightValueRule"].ToString()) ? -1 : int.Parse(dt.Rows[0]["WeightValueRule"].ToString());
        }

        /// 业务调用 多个地方调用
        public decimal GetVolumeParmer(int MerchantID)
        {
            string sql = "select VolumeParmer from FMS_MerchantDeliverFee where MerchantID={0} and RowNum=1";
            object o = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, String.Format(sql, MerchantID));
            decimal parmer = 0;
            if (o != null)
                decimal.TryParse(o.ToString(), out parmer);
            return parmer;
        }

        public DataTable GetMerchantNameByID(string ids)
        {
            if (string.IsNullOrEmpty(ids)) return null;
            string sql = "SELECT MerchantName FROM MerchantBaseInfo WHERE (1=1) {0}";

            sql = string.Format(sql, Util.Common.GetOracleInParameterWhereSql("ID", "ids", false, false));

            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":ids", OracleDbType.NVarchar2, 2000) { Value = ids });
            return OracleHelper.ExecuteDataset(ReadOnlyConnection,CommandType.Text,sql,ToParameters(parameterList.ToArray())).Tables[0];
        }
    }
}
