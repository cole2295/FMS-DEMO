using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.BasicSetting
{
    public class WareHouseDao : SqlServerDao,IWareHouseDao
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
									   FROM     RFD_PMS.dbo.Warehouse w ( NOLOCK )
									   WHERE    w.[Enable] = 1
									   UNION ALL
									   SELECT   'S_'+CONVERT(NVARCHAR(20),ExpressCompanyID ),
												CompanyName
									   FROM     RFD_PMS.dbo.ExpressCompany (NOLOCK) ec
									   WHERE    IsDeleted = 0
												AND CompanyFlag = 1
                                                AND ec.DistributionCode=@DistributionCode
									 )
							SELECT  *
							FROM    t";
            }
            else
            {
                strSql = @"SELECT   'S_'+CONVERT(NVARCHAR(20),ExpressCompanyID ) WarehouseId,
												CompanyName WarehouseName
									   FROM     RFD_PMS.dbo.ExpressCompany (NOLOCK) ec
									   WHERE    IsDeleted = 0
												AND CompanyFlag = 1
                                                AND ec.DistributionCode=@DistributionCode";
            }
            SqlParameter[] parameters ={
                                        new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 仓库
        /// </summary>
        /// <returns></returns>
        public DataTable GetWareHouse()
        {
            strSql = @"SELECT   w.WarehouseId ,
								w.WarehouseName
					   FROM     RFD_PMS.dbo.Warehouse w ( NOLOCK )
					   WHERE    w.[Enable] = 1";

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        /// <summary>
        /// 分拣中心
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortCenter(string distributionCode)
        {
            strSql = @"SELECT   ExpressCompanyID as WarehouseId,
								CompanyName as WarehouseName
					   FROM     RFD_PMS.dbo.ExpressCompany (NOLOCK) ec
					   WHERE    IsDeleted = 0
								AND CompanyFlag = 1 AND ParentID<>11 and DistributionCode=@DistributionCode";
            SqlParameter[] parameters ={
                                        new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }
	}
}
