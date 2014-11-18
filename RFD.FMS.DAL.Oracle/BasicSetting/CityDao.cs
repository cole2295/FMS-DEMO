using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Oracle.ApplicationBlocks.Data;

using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Model;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    /*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：城市字典（数据层）
 * 说明：查询城市信息等
 * 作者：杨来旺
 * 创建日期：2011-03-02 09:10:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */
    public class CityDao : OracleDao, ICityDao
    {
        /// <summary>
        /// 根据条件查询城市信息
        /// </summary>
        /// <param name="city">查询条件</param>
        /// <returns>城市信息dataTable类型</returns>
        public DataTable GetCityList(City city)
        {
            string sqlGetCityList = "SELECT CityID,CityName FROM City WHERE ISDELETED=0";
            //城市ID
            if (!string.IsNullOrEmpty(city.CityID))
            {
                sqlGetCityList += string.Format(" And CityID='{0}'", city.CityID);
            }
            //城市名称
            if (!string.IsNullOrEmpty(city.CityName))
            {
                sqlGetCityList += string.Format(" And CityName='{0}'", city.CityName);
            }
            //所在省份
            if (!string.IsNullOrEmpty(city.ProvinceID))
            {
                sqlGetCityList += string.Format(" And ProvinceID='{0}'", city.ProvinceID);
            }
            //城市级别
            if(!string.IsNullOrEmpty(city.Level))
            {
                if(city.Level == "1")
                {
                    sqlGetCityList += " And CityID like '%0100'";  
                }   
            }
            DataTable dataTable = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetCityList).Tables[0];
            return dataTable;
        }
    }
}
