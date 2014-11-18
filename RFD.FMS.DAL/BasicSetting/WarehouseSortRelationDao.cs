using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.BasicSetting;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.BasicSetting
{
    public class WarehouseSortRelationDao : SqlServerDao, IWarehouseSortRelationDao
    {
        public DataTable GetWarehouseSortRelation(WarehouseSort model)
        {
            var strSql = new StringBuilder();
            var field = model.isExport ? @"(ROW_NUMBER() OVER (ORDER BY ec.ExpressCompanyID)) AS '序号',
                        CompanyName '分拣中心', ProvinceName '省份', CityName '城市', DistrictName '区域', 
						ISNULL(w.WarehouseName,'无仓库') '关联仓库',
						(CASE WHEN wsr.IsDeleted IS NULL OR wsr.IsDeleted = 1 THEN '未启用'
                              ELSE '已启用' END) '是否启用'" :
                        @"CompanyName, ProvinceName, CityName, DistrictName, 
						ISNULL(w.WarehouseName,'无仓库') 'WarehouseName',
						(CASE WHEN wsr.IsDeleted IS NULL OR wsr.IsDeleted = 1 THEN '未启用'
                              ELSE '已启用' END) 'IsDeleted',
                        ec.ExpressCompanyID,ISNULL(w.WarehouseId,-1) 'WarehouseId'";
            strSql.AppendFormat(@"
				SELECT {0} 
				FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) 
				JOIN RFD_PMS.dbo.Province p(NOLOCK) ON ec.ProvinceID = p.ProvinceID
				JOIN RFD_PMS.dbo.City c(NOLOCK) ON ec.CityID = c.CityID
				JOIN RFD_PMS.dbo.District d(NOLOCK) ON ec.DistrictID = d.DistrictID
				LEFT JOIN RFD_PMS.dbo.WarehouseSortRelation wsr(NOLOCK) ON ec.ExpressCompanyID = wsr.SortationID
				LEFT JOIN RFD_PMS.dbo.Warehouse w(NOLOCK) ON wsr.WarehouseID = w.WarehouseId
				WHERE CompanyFlag = 1 AND ec.IsDeleted = 0", field);
            //可选参数
            var paramList = new List<SqlParameter>();
            if (model.SortationID != -1)
            {
                strSql.Append(" AND ec.ExpressCompanyID=@ExpressCompanyID ");
                paramList.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = model.SortationID });
            }
            if (model.HasWarehouse)
            {
                strSql.Append(" AND wsr.WarehouseId IS NOT NULL ");
            }
            if (model.IsDelete)
            {
                strSql.Append(" AND wsr.IsDeleted = 0 ");
            }
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>());
            return ds != null ? ds.Tables[0] : null;
        }

        public DataTable GetSortationList(string DistributionCode)
        {
            var strSql = @"SELECT ExpressCompanyID, CompanyName 
                             FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) 
                            WHERE ec.CompanyFlag = 1 AND ec.IsDeleted = 0";
            var paramList = new List<SqlParameter>();
            if (!String.IsNullOrEmpty(DistributionCode))
            {
                strSql += " AND ec.DistributionCode=@DistributionCode ";
                paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = DistributionCode });
            }
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, paramList.ToArray<SqlParameter>());
            return ds != null ? ds.Tables[0] : null;
        }

        public DataTable GetWarehouseList()
        {
            var strSql = @"SELECT WarehouseId, WarehouseName FROM RFD_PMS.dbo.Warehouse w(NOLOCK)";
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
            return ds != null ? ds.Tables[0] : null;
        }

        public bool InsertWarehouseSortRelation(WarehouseSort model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
                IF EXISTS(SELECT * FROM RFD_PMS.dbo.WarehouseSortRelation wsr(NOLOCK) WHERE SortationID=@SortationID)
                UPDATE RFD_PMS.dbo.WarehouseSortRelation SET WarehouseID=@WarehouseID,IsDeleted=@IsDeleted,
                UpdateBy=@UpdateBy,UpdateTime=@UpdateTime,UpdateStation=@UpdateStation
                WHERE SortationID=@SortationID
                ELSE
                INSERT INTO RFD_PMS.dbo.WarehouseSortRelation
                (SortationID,WarehouseID,IsDeleted,CreateBy,CreateTime,CreateStation)
				VALUES
                (@SortationID,@WarehouseID,@IsDeleted,@CreateBy,@CreateTime,@CreateStation)");
            //可选参数
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@SortationID", SqlDbType.Int) { Value = model.SortationID });
            paramList.Add(new SqlParameter("@WarehouseID", SqlDbType.NVarChar, 20) { Value = model.WarehouseID });
            paramList.Add(new SqlParameter("@IsDeleted", SqlDbType.Bit) { Value = model.IsDelete });
            paramList.Add(new SqlParameter("@CreateBy", SqlDbType.Int) { Value = model.CreateBy });
            paramList.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = model.CreateTime });
            paramList.Add(new SqlParameter("@CreateStation", SqlDbType.Int) { Value = model.CreateStation });
            paramList.Add(new SqlParameter("@UpdateBy", SqlDbType.Int) { Value = model.UpdateBy });
            paramList.Add(new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = model.UpdateTime });
            paramList.Add(new SqlParameter("@UpdateStation", SqlDbType.Int) { Value = model.UpdateStation });
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        public bool UpdateWarehouseSortRelation(WarehouseSort model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
                UPDATE RFD_PMS.dbo.WarehouseSortRelation SET IsDeleted=@IsDeleted,
                UpdateBy=@UpdateBy,UpdateTime=@UpdateTime,UpdateStation=@UpdateStation
                WHERE SortationID=@SortationID AND WarehouseID=@WarehouseID");
            //可选参数
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@SortationID", SqlDbType.Int) { Value = model.SortationID });
            paramList.Add(new SqlParameter("@WarehouseID", SqlDbType.NVarChar, 20) { Value = model.WarehouseID });
            paramList.Add(new SqlParameter("@IsDeleted", SqlDbType.Bit) { Value = model.IsDelete });
            paramList.Add(new SqlParameter("@UpdateBy", SqlDbType.Int) { Value = model.UpdateBy });
            paramList.Add(new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = model.UpdateTime });
            paramList.Add(new SqlParameter("@UpdateStation", SqlDbType.Int) { Value = model.UpdateStation });
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        public string GetWarehouseBySortation(string sortid)
        {
            var strSql = @"
				SELECT ISNULL(wsr.WarehouseId,-1) 'WarehouseId'
				  FROM RFD_PMS.dbo.WarehouseSortRelation wsr(NOLOCK) 
                 WHERE wsr.IsDeleted = 0 AND wsr.SortationID = @SortationID";
            var house = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql,
                new SqlParameter[]{
				new SqlParameter("@SortationID", sortid)});
            return house != null ? house.ToString() : string.Empty;
        }
        /// <summary>
        /// 通过Vjia仓库ID得到如风达仓库ID add by wangyongc 2012-03-02
        /// </summary>
        /// <param name="VjiaID">Vjia仓库ID</param>
        /// <returns></returns>
        public string GetWarehouseIDByVjiaID(string VjiaID)
        {
            var strSql =
                @"SELECT wh.WarehouseId
                    FROM RFD_PMS.dbo.Warehouse wh(NOLOCK)  WHERE wh.VjiaWareHouseID=@VjiaWareHouseID";
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@VjiaWareHouseID", SqlDbType.NVarChar, 20) { Value = VjiaID });
            var retrun = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>());
            return retrun != null ? retrun.ToString() : VjiaID;
        }
    }
}
