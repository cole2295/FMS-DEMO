using System;
using System.Data;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Data.SqlClient;
using RFD.FMS.AdoNet;
using System.Collections.Generic;

namespace RFD.FMS.DAL.BasicSetting
{
    public class GoodsCategoryDao : SqlServerDao, IGoodsCategoryDao
    {
        public DataTable GetAllGoods()
        {
            string sql = "select GoodsCategoryCode Code,GoodsCategoryName Name from RFD_PMS.dbo.GoodsCategory gcy(nolock) where IsDelete=0";

            DataTable table = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];

            return table;
        }

        public string GetNameByCode(string code)
        {
            string sql = "select GoodsCategoryName from RFD_PMS.dbo.GoodsCategory gcy(nolock) where GoodsCategoryCode=@GoodsCategoryCode";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@GoodsCategoryCode",code)
            };

            return DataConvert.ToString(SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql,parameters));
        }

        public DataTable GetGoodsCategoryByMerchantID(int merchantId, string distributionCode)
        {
            string sql = @"SELECT mgcr.RelationID,mbi.ID as MerchantId,mbi.MerchantName,mgcr.GoodsCategoryCode as code,gc.GoodsCategoryName as name
 FROM RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) 
JOIN RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK) ON dmr.MerchantId=mbi.ID
JOIN RFD_PMS.dbo.MerchantGoodsCategoryRelation mgcr(NOLOCK) ON mbi.ID=mgcr.MerchantID
JOIN RFD_PMS.dbo.GoodsCategory gc(NOLOCK) ON gc.GoodsCategoryCode=mgcr.GoodsCategoryCode
WHERE dmr.IsDeleted=0  AND mgcr.IsDelete=0  AND gc.IsDelete=0 {0}";

            StringBuilder sbSqlWhere = new StringBuilder();
            List<SqlParameter> parameterList = new List<SqlParameter>();

            if (merchantId > 0)
            {
                sbSqlWhere.Append(" AND mbi.ID=@MerchantId ");
                parameterList.Add(new SqlParameter("@MerchantId", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbSqlWhere.Append(" AND dmr.DistributionCode=@DistributionCode ");
                parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            sql = string.Format(sql, sbSqlWhere.ToString());

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray()).Tables[0];
        }
    }
}
