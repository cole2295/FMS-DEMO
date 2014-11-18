using System;
using System.Data;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Data.SqlClient;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using System.Collections.Generic;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class GoodsCategoryDao : OracleDao, IGoodsCategoryDao
    {
        public DataTable GetAllGoods()
        {
            string sql = "select GoodsCategoryCode Code,GoodsCategoryName Name from GoodsCategory gcy where IsDeleted=0";

            DataTable table = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];

            return table;
        }

        public string GetNameByCode(string code)
        {
            string sql = "select GoodsCategoryName from GoodsCategory gcy where GoodsCategoryCode=:GoodsCategoryCode";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":GoodsCategoryCode",code)
            };

            return DataConvert.ToString(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql,parameters));
        }

        public DataTable GetGoodsCategoryByMerchantID(int merchantId, string distributionCode)
        {
            string sql = @"SELECT mgcr.RelationID,mbi.ID as MerchantID,mbi.MerchantName,mgcr.GoodsCategoryCode as Code,gc.GoodsCategoryName as Name
 FROM MerchantBaseInfo mbi
JOIN DistributionMerchantRelation dmr ON dmr.MerchantId=mbi.ID
JOIN MerchantGoodsCategoryRelation mgcr ON mbi.ID=mgcr.MerchantID
JOIN GoodsCategory gc ON gc.GoodsCategoryCode=mgcr.GoodsCategoryCode
WHERE dmr.IsDeleted=0  AND mgcr.IsDeleted=0  AND gc.IsDeleted=0 {0}";

            StringBuilder sbSqlWhere = new StringBuilder();
            List<OracleParameter> parameterList = new List<OracleParameter>();

            if (merchantId > 0)
            {
                sbSqlWhere.Append(" AND mbi.ID=:MerchantId ");
                parameterList.Add(new OracleParameter(":MerchantId", OracleDbType.Decimal) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbSqlWhere.Append(" AND dmr.DistributionCode=:DistributionCode ");
                parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 20) { Value = distributionCode });
            }

            sql = string.Format(sql, sbSqlWhere.ToString());

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray())).Tables[0];
        }
    }
}
