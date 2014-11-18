using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.BasicSetting
{
    public class ExpressCompanyDeductDao : SqlServerDao
    {
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public IDictionary<int, RFD.FMS.MODEL.BasicSetting.ExpressCompanyDeduct> GetModel(List<int> expressCompanyIds)
        {
            IDictionary<int,RFD.FMS.MODEL.BasicSetting.ExpressCompanyDeduct> dictionary = new Dictionary<int, MODEL.BasicSetting.ExpressCompanyDeduct>();

            string strSql = String.Format(@"select ID,ExpressCompanyID,ExpressReceiveDeduct,ExpressSendDeduct,ProgramReceiveDeduct,ProgramSendDeduct,UseDate,CreateTime,CreateBy,UpdateTime,UpdateBy,IsDeleted,ReceiveBasicCommission,ReceiveBASICWEIGHT,ReceiveWEIGHTADDCOMMISSION,SendBasicCommission,SendBASICWEIGHT,SendWEIGHTADDCOMMISSION,Status from rfd_pms.dbo.ExpressCompanyDeduct (nolock)
                          where ExpressCompanyID in ({0}) ", DataConvert.ToDbIds(expressCompanyIds));

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text,strSql.ToString());

            ExpressCompanyDeduct model = null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                dictionary.Add(model.ExpressCompanyID, DataRowToObject(row));
            }

            return dictionary;
        }

        private ExpressCompanyDeduct DataRowToObject(DataRow row)
        {
            ExpressCompanyDeduct model = new ExpressCompanyDeduct();

            model.ExpressCompanyID = DataConvert.ToInt(row["ExpressCompanyID"]);
            model.ExpressReceiveDeduct = DataConvert.ToString(row["ExpressReceiveDeduct"]);
            model.ExpressSendDeduct = DataConvert.ToString(row["ExpressSendDeduct"]);
            model.ReceiveBASICWEIGHT = DataConvert.ToDecimal(row["ReceiveBASICWEIGHT"]);
            model.ReceiveWEIGHTADDCOMMISSION = DataConvert.ToDecimal(row["ReceiveWEIGHTADDCOMMISSION"]);
            model.SendBASICWEIGHT = DataConvert.ToDecimal(row["SendBASICWEIGHT"]);
            model.SendWEIGHTADDCOMMISSION = DataConvert.ToDecimal(row["SendWEIGHTADDCOMMISSION"].ToString());

            return model;
        }
    }
}