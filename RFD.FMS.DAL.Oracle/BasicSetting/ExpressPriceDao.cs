using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    /*
   * (C)Copyright 2011-2012 如风达信息管理系统
   * 
   * 模块名称：订单收款和入库状态查询（数据层）
   * 说明：查询指定条件下的订单收款和入库状态信息
   * 作者：王勇
   * 创建日期：2011/07/25
   * 修改人：
   * 修改时间：
   * 修改记录：
   */
    public class ExpressPriceDao : OracleDao, IExpressPriceDao
    {
        #region SQLSTRING

        private const string strExpressList = @"SELECT a.AreaID,a.AreaName,a.CityID,a.PostCode,a.ZoneCode,a.TransferFee,a.Sorting,a.IsDeleted,
                                a.CreatBy,a.CreatTime,a.CreatStation,a.UpdateBy,a.UpdateTime,a.UpdateStation,c.cityname
                                FROM Area a inner join City c on a.CityID = c.cityid
                                WHERE a.CityID in (SELECT cityID FROM City c WHERE c.IsDeleted <> 1)";
        #endregion

        /// <summary>
        /// 查询所有区域运费信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAreaInfo(string strCityID, string strCityName)
        {
            string strSql = strExpressList;

            if(!String.IsNullOrEmpty(strCityID))
            {
                strSql += " and a.CityID like '" + strCityID + "%' ";
            }

            if (!String.IsNullOrEmpty(strCityName))
            {
                strSql += " and c.cityname like '" + strCityName + "%' ";
            }

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
            return dt;
        }
    }
}
