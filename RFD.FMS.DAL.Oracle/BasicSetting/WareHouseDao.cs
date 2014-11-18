using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using System.Data.SqlClient;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class WareHouseDao : OracleDao, IWareHouseDao
	{
        private string strSql = "";

        /// <summary>
        /// 分拣中心和仓库
        /// </summary>
        /// <returns></returns>
        public DataTable GetWareHouseSortCenter(string distributionCode)
        {
            if (distributionCode == "rfd")
            {
                strSql = @"WITH    t AS ( SELECT   w.WarehouseId ,
												w.WarehouseName
									   FROM     Warehouse w
									   WHERE    w.Enable = 1
									   UNION ALL
									   SELECT   'S_' || CAST(ExpressCompanyID as NVarchar2(20)),
												CompanyName
									   FROM     ExpressCompany  ec
									   WHERE    IsDeleted = 0
												AND CompanyFlag = 1
                                                AND ec.DistributionCode=:DistributionCode
									 )
							SELECT  *
							FROM    t";
            }
            else
            {
                strSql = @"SELECT   'S_'+cast(ExpressCompanyID as NVARCHAR2(20) ) WarehouseId,
												CompanyName WarehouseName
									   FROM     ExpressCompany  ec
									   WHERE    IsDeleted = 0
												AND CompanyFlag = 1
                                                AND ec.DistributionCode=:DistributionCode";
            }
            OracleParameter[] parameters ={
                                        new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 仓库
        /// </summary>
        /// <returns></returns>
        public DataTable GetWareHouse()
        {
            strSql = @"SELECT   w.WarehouseId ,
								w.WarehouseName
					   FROM     Warehouse w
					   WHERE    w.Enable = 1";

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        /// <summary>
        /// 分拣中心
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortCenter(string distributionCode)
        {
            strSql = @"SELECT   ExpressCompanyID as WarehouseId,
								CompanyName as WarehouseName
					   FROM     ExpressCompany  ec
					   WHERE    IsDeleted = 0
								AND CompanyFlag = 1 AND ParentID<>11 and DistributionCode=:DistributionCode";
            OracleParameter[] parameters ={
                                        new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }
	}
}
