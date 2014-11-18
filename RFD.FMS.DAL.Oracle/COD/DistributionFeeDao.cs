using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.COD;
using System.Data;
using System.Data.SqlClient;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;

namespace RFD.FMS.DAL.Oracle.COD
{
    public class DistributionFeeDao : OracleDao, IDistributionFeeDao
    {
        public DataTable GetFareV2(MODEL.COD.DistributionFeeDTO dto)
        {
            string expressCompanyColumn = string.Empty;
            if (dto.Source == 0)
                expressCompanyColumn = "ExpressCompanyOldID";
            else if (dto.Source == 1)
                expressCompanyColumn = "ExpressCompanyVjiaOldID";
            else
                expressCompanyColumn = "ExpressCompanyID";

            string sqlStr = @"
                SELECT fcbi.WaybillNo,fcbi.IsFare,fcbi.Fare,fcbi.FareFormula,ec.AccountCompanyName,fcbi.MerchantID,fcbi.Flag,fcbi.OperateType,fcbi.CreateTime
                 FROM FMS_CODBaseInfo fcbi
                 JOIN ExpressCompany ec 
                 ON ec.ExpressCompanyID=fcbi.TopCODCompanyID
                WHERE (1=1)
                AND fcbi.IsDeleted=0
                AND fcbi.MerchantID=:MerchantID
                AND ec.{0}=:ExpressCompanyID
                AND fcbi.WaybillNO=:WaybillNO
                order by fcbi.CreateTime
                ";
            sqlStr = string.Format(sqlStr, expressCompanyColumn);
            OracleParameter[] parameters ={
                                           new OracleParameter(":MerchantID",OracleDbType.Decimal),
                                           new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
                                           new OracleParameter(":WaybillNO",SqlDbType.BigInt),

                                      };
            parameters[0].Value = dto.MerchantID;
            parameters[1].Value = dto.ExpressCompanyID;
            parameters[2].Value = dto.FormCode;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetFare(MODEL.COD.DistributionFeeDTO dto)
        {
            string expressCompanyColumn = string.Empty;
            if (dto.Source == 0)
                expressCompanyColumn = "ExpressCompanyOldID";
            else if (dto.Source == 1)
                expressCompanyColumn = "ExpressCompanyVjiaOldID";
            else
                expressCompanyColumn = "ExpressCompanyID";

            string sqlStr = @"
SELECT fcbi.WaybillNo,fcbi.IsFare,fcbi.Fare,fcbi.FareFormula,ec.AccountCompanyName,fcbi.MerchantID,fcbi.Flag,fcbi.OperateType,fcbi.CreateTime
 FROM FMS_CODBaseInfo fcbi
 JOIN ExpressCompany ec 
 ON ec.ExpressCompanyID=fcbi.TopCODCompanyID
WHERE (1=1)
AND fcbi.IsDeleted=0
AND fcbi.MerchantID=:MerchantID
AND ec.{0}=:ExpressCompanyID
AND fcbi.WaybillNO=:WaybillNO
order by fcbi.CreateTime
";
            sqlStr=string.Format(sqlStr,expressCompanyColumn);
            OracleParameter[] parameters ={
                                           new OracleParameter(":MerchantID",OracleDbType.Decimal),
                                           new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
                                           new OracleParameter(":WaybillNO",OracleDbType.Int64),

                                      };
            parameters[0].Value = dto.MerchantID;
            parameters[1].Value = dto.ExpressCompanyID;
            parameters[2].Value = dto.FormCode;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }
    }
}
