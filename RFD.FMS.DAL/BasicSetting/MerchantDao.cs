using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.BasicSetting
{
    public class MerchantDao : SqlServerDao, IMerchantDao
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
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0 AND m.Sources = 2
AND dmr.DistributionCode=@DistributionCode 
ORDER BY m.CreatTime");
            }
            else
            {
                strSql.Append(@"SELECT m.ID,m.MerchantName
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=@DistributionCode
ORDER BY m.CreatTime");
            }

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = disCode;

            var ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
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
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0 AND m.Sources = 2
AND dmr.DistributionCode=@DistributionCode  AND m.IsPeriodAccount=0 AND m.ID<>30
ORDER BY m.CreatTime");
            }
            else
            {
                strSql.Append(@"SELECT m.ID,m.MerchantName
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=@DistributionCode
ORDER BY m.CreatTime");
            }

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = disCode;

            var ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            return ds != null ? ds.Tables[0] : null;
        }


        public DataTable GetAllMerchants(string disCode)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
			    SELECT m.ID,m.MerchantName
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=@DistributionCode
ORDER BY m.CreatTime");

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = disCode;

            var ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            return ds != null ? ds.Tables[0] : null;
        }


        public string GetMerchantName(long waybillNo)
        {
            string sql = @"select mbi.MerchantName from LMS_RFD.dbo.Waybill wb(nolock) 
                inner join RFD_PMS.dbo.MerchantBaseInfo mbi(nolock) on wb.MerchantID=mbi.ID
                where wb.WaybillNO=@WaybillNO";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@WaybillNO",waybillNo)
            };

            return DataConvert.ToString(SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public string GetMerchantCategory(int merchantId)
        {
            string sql = @"select mgc.GoodsCategoryCode from RFD_PMS.dbo.MerchantGoodsCategoryRelation mgc(nolock) 
                where mgc.MerchantID=@MerchantID and not mgc.GoodsCategoryCode is null and mgc.GoodsCategoryCode!=''";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@MerchantID",merchantId)
            };

            return DataConvert.ToString(SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        /// 业务调用 AddExpressOrder.aspx 快递单计算重量
        public int GetMerchantDeliverFee(int merchantId)
        {
            var strSql = @"SELECT WeightValueRule FROM RFD_FMS.dbo.FMS_MerchantDeliverFee(nolock) WHERE MerchantID=@MerchantId AND Status=2 and WeightType=3";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@MerchantId",SqlDbType.Int),
            };

            parameters[0].Value = merchantId;

            DataTable dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            if (dt == null || dt.Rows.Count <= 0)
                return -1;
            return string.IsNullOrEmpty(dt.Rows[0]["WeightValueRule"].ToString()) ? -1 : int.Parse(dt.Rows[0]["WeightValueRule"].ToString());
        }

        /// 业务调用 多个地方调用
        public decimal GetVolumeParmer(int MerchantID)
        {
            string sql = "select top 1 VolumeParmer from rfd_fms.dbo.FMS_MerchantDeliverFee where MerchantID={0}";
            object o = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, String.Format(sql, MerchantID));
            decimal parmer = 0;
            if (o != null)
                decimal.TryParse(o.ToString(), out parmer);
            return parmer;
        }

        public DataTable GetMerchantNameByID(string ids)
        {
            if (string.IsNullOrEmpty(ids)) return null;
            string sql = "SELECT MerchantName FROM RFD_PMS.dbo.MerchantBaseInfo WHERE (1=1) AND ID in ({0})";

            sql = string.Format(sql, ids);

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }
    }
}
