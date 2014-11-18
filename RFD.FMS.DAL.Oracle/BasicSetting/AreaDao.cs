using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.ApplicationBlocks.Data;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class AreaDao : OracleDao, IAreaDao
    {
        /// <summary>
        /// 获取区县信息
        /// </summary>
        /// <param name="area">查询条件</param>
        /// <returns>区县信息dataTable类型</returns>
        public DataTable GetAreaList(Area area)
        {
            string sqlGetAreaList = "SELECT AREAID,AREANAME FROM Area WHERE ISDELETED=0";

            //按照城市ID查询
            if (!string.IsNullOrEmpty(area.CityID))
            {
                sqlGetAreaList += string.Format(" AND CITYID={0}", area.CityID);
            }

            DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetAreaList).Tables[0];

            return dataTable;
        }
    }
}
