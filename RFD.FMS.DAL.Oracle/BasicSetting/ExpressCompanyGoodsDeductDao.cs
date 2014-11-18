using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class ExpressCompanyGoodsDeductDao : OracleDao
    {
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public IDictionary<string, ExpressCompanyGoodsDeduct> GetModel(List<int> deliverStationsIds)
        {
            IDictionary<string, ExpressCompanyGoodsDeduct> dictionary = new Dictionary<string, ExpressCompanyGoodsDeduct>();

            string strSql = String.Format(@"
                    select ID,ExpressCompanyID,GoodsCategoryCode,BasicCommission,WeightCommission,ExtraCommission,UseDate,CreateTime,CreateBy,UpdateTime,UpdateBy,IsDeleted 
                    from ExpressCompanyGoodsDeduct 
                    where ExpressCompanyID in ({0}) ",DataConvert.ToDbIds(deliverStationsIds));

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());

            ExpressCompanyGoodsDeduct model = null;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                model = DataRowToObject(row);

                if (dictionary.ContainsKey(model.ExpressCompanyID + "[" + model.GoodsCategoryCode + "]") == false)
                {
                    dictionary.Add(model.ExpressCompanyID + "[" + model.GoodsCategoryCode + "]", model);
                }
            }

            return dictionary;
        }

        private ExpressCompanyGoodsDeduct DataRowToObject(DataRow row)
        {
            ExpressCompanyGoodsDeduct model = new ExpressCompanyGoodsDeduct();

            model.ID = DataConvert.ToInt(row["ID"]);
            model.ExpressCompanyID = DataConvert.ToInt(row["ExpressCompanyID"]);
            model.GoodsCategoryCode = DataConvert.ToString(row["GoodsCategoryCode"]);
            model.BasicCommission = DataConvert.ToDecimal(row["BasicCommission"]);
            model.WeightCommission = DataConvert.ToDecimal(row["WeightCommission"]);
            model.ExtraCommission = DataConvert.ToDecimal(row["ExtraCommission"]);
            model.UseDate = DataConvert.ToDateTime(row["UseDate"]);
            model.CreateTime = DataConvert.ToDateTime(row["CreateTime"]);
            model.CreateBy = DataConvert.ToInt(row["CreateBy"]);
            model.UpdateTime = DataConvert.ToDateTime(row["UpdateTime"]);
            model.UpdateBy = DataConvert.ToInt(row["UpdateBy"]);
            model.IsDeleted = DataConvert.ToInt(row["IsDeleted"]);

            return model;
        }
    }
}
