using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.BasicSetting
{
    public class AreaDao : SqlServerDao, IAreaDao
    {
        /// <summary>
        /// 获取区县信息
        /// </summary>
        /// <param name="area">查询条件</param>
        /// <returns>区县信息dataTable类型</returns>
        public DataTable GetAreaList(Area area)
        {
            string sqlGetAreaList = "SELECT AREAID,AREANAME FROM RFD_PMS.dbo.Area(NOLOCK) WHERE ISDELETED=0";

            //按照城市ID查询
            if (!string.IsNullOrEmpty(area.CityID))
            {
                sqlGetAreaList += string.Format(" AND CITYID={0}", area.CityID);
            }

            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetAreaList).Tables[0];

            return dataTable;
        }
    }
}
