using System;
using System.Collections.Generic;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.DataAccess.Client;
using Oracle.ApplicationBlocks.Data;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    class DistributionDao :OracleDao, IDistributionDao
    {
        public DataTable GetDistribution(Distribution model)
        {
            string sqlStr =@"Select DistributionID, DistributionName,DistributionCode 
                              from PS_PMS.Distribution d where IsDelete =0";
            if(!string.IsNullOrEmpty(model.DistributionCode))
            {
                sqlStr += "and d.DistributionCode =:DistributionCode ";
            }

            if(model.DistributionID>0)
            {
                sqlStr += "and d.DistributionID =:DistributionID ";
            }

            if(!string.IsNullOrEmpty(model.DistributionName))
            {
                sqlStr += "and d.DistributionName =:DistributionName ";
            }

            OracleParameter[] parameters = {
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2),
                                               new OracleParameter(":DistributionID", OracleDbType.Int32),
                                               new OracleParameter(":DistributionName", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = model.DistributionCode;
            parameters[1].Value = model.DistributionID;
            parameters[2].Value = model.DistributionName;

            var ds =OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters);
            
            return ds.Tables[0];
        }


        public DataTable GetDistributionCodeByID(string Ids)
        {
            string sqlStr =
               string.Format(
                   @" Select distinct DistributionCode from PS_PMS.ExpressCompany ec
                                             where ec.ExpressCompanyID in ({0})",
                   Ids);
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);

            return ds.Tables[0];
        }
    }
}
