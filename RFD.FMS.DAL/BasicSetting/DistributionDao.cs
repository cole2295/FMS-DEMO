using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.DAL.BasicSetting
{
    public class DistributionDao : SqlServerDao,IDistributionDao 
    {
        public DataTable GetDistribution(Distribution model)
        {
            string sqlStr = @"Select DistributionID, DistributionName,DistributionCode 
                              from RFD_PMS.dbo.Distribution d(nolock) where IsDelete =0";
            if (!string.IsNullOrEmpty(model.DistributionCode))
            {
                sqlStr += "and d.DistributionCode =@DistributionCode ";
            }

            if (model.DistributionID > 0)
            {
                sqlStr += "and d.DistributionID =@DistributionID ";
            }

            if (!string.IsNullOrEmpty(model.DistributionName))
            {
                sqlStr += "and d.DistributionName =@DistributionName ";
            }

            SqlParameter[] parameters = {
                                               new SqlParameter("@DistributionCode", SqlDbType.NVarChar),
                                               new SqlParameter("@DistributionID", SqlDbType.Int),
                                               new SqlParameter("@DistributionName", SqlDbType.NVarChar)
                                           };
            parameters[0].Value = model.DistributionCode;
            parameters[1].Value = model.DistributionID;
            parameters[2].Value = model.DistributionName;

            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);

            return ds.Tables[0];
        }


        public DataTable GetDistributionCodeByID(string Ids)
        {
            string sqlStr =
                string.Format(
                    @" Select distinct DistributionCode from RFD_PMS.dbo.ExpressCompany ec(nolock)
                                             where ec.ExpressCompanyID in ({0})",
                    Ids);
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);

            return ds.Tables[0];
        }
    }
}
