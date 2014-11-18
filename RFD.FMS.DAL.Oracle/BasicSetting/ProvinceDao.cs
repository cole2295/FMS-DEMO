using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    /*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：省份字典（数据层）
 * 说明：查询省份信息等
 * 作者：杨来旺
 * 创建日期：2011-02-30 13:42:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */
    public class ProvinceDao : OracleDao, IProvinceDao
    {
        /// <summary>
        /// 获取省份字典
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvinceList()
        {
            const string sqlGetProvinceList = "SELECT ProvinceID,ProvinceName FROM Province WHERE ISDELETED=0";
            DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList).Tables[0];
            return dataTable;
        }

        /// <summary>
        /// 获取省份字典
        /// </summary>
        /// <returns></returns>
        public string GetProvinceID(string Name)
        {
            string sqlGetProvinceList = string.Format("SELECT ProvinceID FROM Province WHERE ProvinceName='{0}'", Name);
            DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList).Tables[0];
            if (dataTable.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                return dataTable.Rows[0]["ProvinceID"].ToString();
            }
        }

        #region IProvinceDao 成员


        public DataTable GetProvinceList(string strDistrictID)
        {
            const string sqlGetProvinceList = "SELECT ProvinceID,ProvinceName FROM Province WHERE ISDELETED=0  AND DistrictID=:DistrictID";

            OracleParameter[] parameters={
                                          new OracleParameter(":DistrictID",SqlDbType.NVarChar),
                                      };
            parameters[0].Value=strDistrictID;
            DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList, parameters).Tables[0];
            return dataTable;
        }

        public string GetDistrictId(string ProvinceID)
        {
            string sqlGetProvinceList = string.Format("SELECT DistrictID FROM Province WHERE ProvinceID='{0}'", ProvinceID);
            DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList).Tables[0];
            if (dataTable.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                return dataTable.Rows[0]["DistrictID"].ToString();
            }
        }

        public DataTable GetAllPCA()
        {
            string sql = @"
SELECT p.ProvinceID,
       c.CityID,
       a.AreaID,
       p.ProvinceName,
       c.CityName,
       a.AreaName
FROM   Province p
       JOIN City c
            ON  c.ProvinceID = p.ProvinceID
       JOIN Area a
            ON  a.CityID = c.CityID
WHERE  p.IsDeleted = 0
       AND c.IsDeleted = 0
       AND a.IsDeleted = 0
";

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetCityNoSiteNo(int expressCompanyId)
        {
            var strSql = @"SELECT  SITENO ,
        DQcode
FROM    ExpressCompany ex
        JOIN City c ON ex.CityID = c.CityID
WHERE   ex.ExpressCompanyID = :ExpressCompanyID";

            OracleParameter[] parameters =
            {
                new OracleParameter("",OracleDbType.Decimal),
            };

            parameters[0].Value = expressCompanyId;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        #endregion
    }
}
