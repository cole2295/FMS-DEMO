using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.BasicSetting
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
    public class ProvinceDao : SqlServerDao, IProvinceDao
    {
        /// <summary>
        /// 获取省份字典
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvinceList()
        {
            const string sqlGetProvinceList = "SELECT ProvinceID,ProvinceName FROM RFD_PMS.dbo.Province(nolock) WHERE ISDELETED=0";
            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList).Tables[0];
            return dataTable;
        }

        /// <summary>
        /// 获取省份字典
        /// </summary>
        /// <returns></returns>
        public string GetProvinceID(string Name)
        {
            string sqlGetProvinceList = string.Format("SELECT ProvinceID FROM RFD_PMS.dbo.Province(nolock) WHERE ProvinceName='{0}'", Name);
            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList).Tables[0];
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
            const string sqlGetProvinceList = "SELECT ProvinceID,ProvinceName FROM RFD_PMS.dbo.Province(nolock) WHERE ISDELETED=0  AND DistrictID=@DistrictID";

            SqlParameter[] parameters={
                                          new SqlParameter("@DistrictID",SqlDbType.NVarChar),
                                      };
            parameters[0].Value=strDistrictID;
            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList, parameters).Tables[0];
            return dataTable;
        }

        public string GetDistrictId(string ProvinceID)
        {
            string sqlGetProvinceList = string.Format("SELECT DistrictID FROM RFD_PMS.dbo.Province(nolock) WHERE ProvinceID='{0}'", ProvinceID);
            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetProvinceList).Tables[0];
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
FROM   RFD_PMS.dbo.Province p(NOLOCK)
       JOIN RFD_PMS.dbo.City c(NOLOCK)
            ON  c.ProvinceID = p.ProvinceID
       JOIN RFD_PMS.dbo.Area a(NOLOCK)
            ON  a.CityID = c.CityID
WHERE  p.IsDeleted = 0
       AND c.IsDeleted = 0
       AND a.IsDeleted = 0
";

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetCityNoSiteNo(int expressCompanyId)
        {
            var strSql = @"SELECT  SITENO ,
        DQcode
FROM    RFD_PMS.dbo.ExpressCompany ex ( NOLOCK )
        JOIN RFD_PMS.dbo.City c ( NOLOCK ) ON ex.CityID = c.CityID
WHERE   ex.ExpressCompanyID = @ExpressCompanyID";

            SqlParameter[] parameters =
            {
                new SqlParameter("",SqlDbType.Int),
            };

            parameters[0].Value = expressCompanyId;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        #endregion
    }
}
