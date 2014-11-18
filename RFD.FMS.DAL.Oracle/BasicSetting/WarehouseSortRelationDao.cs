using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.BasicSetting;
using System.Data.SqlClient;
using Oracle.ApplicationBlocks.Data;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class WarehouseSortRelationDao : OracleDao, IWarehouseSortRelationDao
    {
        public DataTable GetWarehouseSortRelation(WarehouseSort model)
        {
            var strSql = new StringBuilder();
            var field = model.isExport ? @"(ROW_NUMBER() OVER (ORDER BY ec.ExpressCompanyID)) AS '序号',
                        CompanyName '分拣中心', ProvinceName '省份', CityName '城市', DistrictName '区域', 
						NVL(w.WarehouseName,'无仓库') '关联仓库',
						(CASE WHEN wsr.IsDeleted IS NULL OR wsr.IsDeleted = 1 THEN '未启用'
                              ELSE '已启用' END) '是否启用'" :
                        @"CompanyName, ProvinceName, CityName, DistrictName, 
						NVL(w.WarehouseName,'无仓库') 'WarehouseName',
						(CASE WHEN wsr.IsDeleted IS NULL OR wsr.IsDeleted = 1 THEN '未启用'
                              ELSE '已启用' END) 'IsDeleted',
                        ec.ExpressCompanyID,NVL(w.WarehouseId,-1) 'WarehouseId'";
            strSql.AppendFormat(@"
				SELECT {0} 
				FROM ExpressCompany ec 
				JOIN Province p ON ec.ProvinceID = p.ProvinceID
				JOIN City c ON ec.CityID = c.CityID
				JOIN District d ON ec.DistrictID = d.DistrictID
				LEFT JOIN WarehouseSortRelation wsr ON ec.ExpressCompanyID = wsr.SortationID
				LEFT JOIN Warehouse w ON wsr.WarehouseID = w.WarehouseId
				WHERE CompanyFlag = 1 AND ec.IsDeleted = 0", field);
            //可选参数
            var paramList = new List<OracleParameter>();
            if (model.SortationID != -1)
            {
                strSql.Append(" AND ec.ExpressCompanyID=:ExpressCompanyID ");
                paramList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = model.SortationID });
            }
            if (model.HasWarehouse)
            {
                strSql.Append(" AND wsr.WarehouseId IS NOT NULL ");
            }
            if (model.IsDelete)
            {
                strSql.Append(" AND wsr.IsDeleted = 0 ");
            }
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>());
            return ds != null ? ds.Tables[0] : null;
        }

        public DataTable GetSortationList(string DistributionCode)
        {
            var strSql = @"SELECT ExpressCompanyID, CompanyName 
                             FROM ExpressCompany ec 
                            WHERE ec.CompanyFlag = 1 AND ec.IsDeleted = 0";
            var paramList = new List<OracleParameter>();
            if (!String.IsNullOrEmpty(DistributionCode))
            {
                strSql += " AND ec.DistributionCode=:DistributionCode ";
                paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100) { Value = DistributionCode });
            }
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, paramList.ToArray<OracleParameter>());
            return ds != null ? ds.Tables[0] : null;
        }

        public DataTable GetWarehouseList()
        {
            var strSql = @"SELECT WarehouseId, WarehouseName FROM Warehouse w";
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
            return ds != null ? ds.Tables[0] : null;
        }

        public bool InsertWarehouseSortRelation(WarehouseSort model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
                IF EXISTS(SELECT * FROM WarehouseSortRelation wsr WHERE SortationID=:SortationID)
                UPDATE WarehouseSortRelation SET WarehouseID=:WarehouseID,IsDeleted=:IsDeleted,
                UpdateBy=:UpdateBy,UpdateTime=:UpdateTime,UpdateStation=:UpdateStation
                WHERE SortationID=:SortationID
                ELSE
                INSERT INTO WarehouseSortRelation
                (SortationID,WarehouseID,IsDeleted,CreateBy,CreateTime,CreateStation)
				VALUES
                (:SortationID,:WarehouseID,:IsDeleted,:CreateBy,:CreateTime,:CreateStation)");
            //可选参数
            var paramList = new List<OracleParameter>();
            paramList.Add(new OracleParameter(":SortationID", OracleDbType.Decimal) { Value = model.SortationID });
            paramList.Add(new OracleParameter(":WarehouseID", OracleDbType.Varchar2,40) { Value = model.WarehouseID });
            paramList.Add(new OracleParameter(":IsDeleted", SqlDbType.Bit) { Value = model.IsDelete });
            paramList.Add(new OracleParameter(":CreateBy", OracleDbType.Decimal) { Value = model.CreateBy });
            paramList.Add(new OracleParameter(":CreateTime", SqlDbType.DateTime) { Value = model.CreateTime });
            paramList.Add(new OracleParameter(":CreateStation", OracleDbType.Decimal) { Value = model.CreateStation });
            paramList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateBy });
            paramList.Add(new OracleParameter(":UpdateTime", SqlDbType.DateTime) { Value = model.UpdateTime });
            paramList.Add(new OracleParameter(":UpdateStation", OracleDbType.Decimal) { Value = model.UpdateStation });
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>()) > 0;
        }

        public bool UpdateWarehouseSortRelation(WarehouseSort model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
                UPDATE WarehouseSortRelation SET IsDeleted=:IsDeleted,
                UpdateBy=:UpdateBy,UpdateTime=:UpdateTime,UpdateStation=:UpdateStation
                WHERE SortationID=:SortationID AND WarehouseID=:WarehouseID");
            //可选参数
            var paramList = new List<OracleParameter>();
            paramList.Add(new OracleParameter(":SortationID", OracleDbType.Decimal) { Value = model.SortationID });
            paramList.Add(new OracleParameter(":WarehouseID", OracleDbType.Varchar2,40) { Value = model.WarehouseID });
            paramList.Add(new OracleParameter(":IsDeleted", SqlDbType.Bit) { Value = model.IsDelete });
            paramList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateBy });
            paramList.Add(new OracleParameter(":UpdateTime", SqlDbType.DateTime) { Value = model.UpdateTime });
            paramList.Add(new OracleParameter(":UpdateStation", OracleDbType.Decimal) { Value = model.UpdateStation });
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>()) > 0;
        }

        public string GetWarehouseBySortation(string sortid)
        {
            var strSql = @"
				SELECT NVL(wsr.WarehouseId,-1) 'WarehouseId'
				  FROM WarehouseSortRelation wsr 
                 WHERE wsr.IsDeleted = 0 AND wsr.SortationID = :SortationID";
            var house = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql,
                new OracleParameter[]{
				new OracleParameter(":SortationID", sortid)});
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
                    FROM Warehouse wh  WHERE wh.VjiaWareHouseID=:VjiaWareHouseID";
            var paramList = new List<OracleParameter>();
            paramList.Add(new OracleParameter(":VjiaWareHouseID", OracleDbType.Varchar2,40) { Value = VjiaID });
            var retrun = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>());
            return retrun != null ? retrun.ToString() : VjiaID;
        }
    }
}
