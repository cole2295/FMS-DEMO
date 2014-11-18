using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.COD;
using System.Data.SqlClient;

namespace RFD.FMS.DAL.COD
{
    public class RequisitionedFormDAL : SqlServerDao, IRequisitionedFormDAL
	{
        public string SqlStr { get; set; }

        public DataTable GetRequisitionedOrderList(string expressCompanyId, string dateStr, string dateEnd)
        {
            #region old
            //            SqlStr = @"SELECT  w.WaybillNO ,
            //        ec1.CompanyName ,
            //        w.DeliverTime ,
            //        Weight = CASE WHEN ISNULL(wi.WayBillInfoWeight, 0) > ISNULL(wi.WayBillInfoVolumeWeight,
            //                                                              0)
            //                                 THEN ISNULL(wi.WayBillInfoWeight, 0)
            //                                 ELSE ISNULL(wi.WayBillInfoVolumeWeight, 0)
            //                            END ,
            //        w.WarehouseId ,
            //        w1.WarehouseName ,
            //        ADDRESS = ( CASE wtsi.ReceiveProvince
            //                      WHEN '北京' THEN ''
            //                      WHEN '天津' THEN ''
            //                      WHEN '上海' THEN ''
            //                      WHEN '重庆' THEN ''
            //                      ELSE ''
            //                    END ) + wtsi.ReceiveCity + wtsi.ReceiveArea
            //        + wtsi.ReceiveAddress,
            //       DeliveryFare=(SELECT TOP 1 fcbi.Fare FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi(NOLOCK) 
            //						WHERE fcbi.IsFare=1 AND fcbi.IsDeleted=0 AND fcbi.WaybillNO=w.WaybillNO ORDER BY fcbi.DeliverTime DESC)
            //FROM    LMS_RFD.dbo.Waybill AS w ( NOLOCK )
            //        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON w.DeliverStationID = ec.ExpressCompanyID
            //        JOIN RFD_PMS.dbo.ExpressCompany AS ec1 ( NOLOCK ) ON ec1.ExpressCompanyID = ec.TopCODCompanyID
            //        JOIN LMS_RFD.dbo.WaybillInfo AS wi ON wi.WaybillNO = w.WaybillNO
            //        JOIN RFD_PMS.dbo.warehouse w1 ( NOLOCK ) ON w1.WarehouseId = w.WarehouseId
            //        JOIN LMS_RFD.dbo.WaybillTakeSendInfo AS wtsi ( NOLOCK ) ON w.WaybillNO = wtsi.WaybillNO
            //        WHERE w.MerchantID=8 AND w.IsDelete=0 AND w.ComeFrom in (14,15,16) {0}
            //		ORDER BY w.DeliverTime";

            #endregion

            SqlStr = @"
    WITH t AS (
	SELECT fbci1.WaybillNo,
		fbci1.AccountWeight Weight,
        fbci1.WarehouseId ,
        fbci1.ADDRESS,
        fbci1.TopCODCompanyID,
        fbci1.DeliverTime,
        fbci1.Fare DeliveryFare
        FROM FMS_CODBaseInfo fbci1(NOLOCK)
        JOIN (
			SELECT WaybillNO,
                MAX(fbci.DeliverTime) DeliverTime,
				MAX(ID) ID
			FROM FMS_CODBaseInfo fbci(NOLOCK)
			WHERE fbci.MerchantID=8 
				AND fbci.IsDeleted=0
				AND fbci.ComeFrom in (14,15,16) 
				{0}
            GROUP BY fbci.WaybillNO
	) aa ON fbci1.ID=aa.ID
)
SELECT	t.WaybillNO,
		ec1.CompanyName ,
		t.DeliverTime,
		t.Weight,
		t.WarehouseId,
		w1.WarehouseName,
		t.Address,
		t.DeliveryFare
	FROM t
        JOIN RFD_PMS.dbo.ExpressCompany AS ec1 ( NOLOCK ) ON t.TopCODCompanyID = ec1.ExpressCompanyID
        JOIN RFD_PMS.dbo.warehouse w1 ( NOLOCK ) ON w1.WarehouseId = t.WarehouseId
ORDER BY t.DeliverTime
";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND fbci.TopCODCompanyID=@TopCODCompanyID ");
                parameters.Add(new SqlParameter("@TopCODCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
                sbWhere.Append(" AND fbci.DeliverTime>=@DeliverTimeStr ");
                parameters.Add(new SqlParameter("@DeliverTimeStr", SqlDbType.DateTime) { Value = dateStr });
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                sbWhere.Append(" AND fbci.DeliverTime<=@DeliverTimeEnd ");
                parameters.Add(new SqlParameter("@DeliverTimeEnd", SqlDbType.DateTime) { Value = dateEnd });
            }

            SqlStr = string.Format(SqlStr, sbWhere.ToString());

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray());
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetRequisitionedOrderListV2(string expressCompanyId, string dateStr, string dateEnd)
        {
            //            SqlStr = @"SELECT  w.WaybillNO ,
            //        ec1.CompanyName ,
            //        w.DeliverTime ,
            //        Weight = CASE WHEN ISNULL(wi.WayBillInfoWeight, 0) > ISNULL(wi.WayBillInfoVolumeWeight,
            //                                                              0)
            //                                 THEN ISNULL(wi.WayBillInfoWeight, 0)
            //                                 ELSE ISNULL(wi.WayBillInfoVolumeWeight, 0)
            //                            END ,
            //        w.WarehouseId ,
            //        w1.WarehouseName ,
            //        ADDRESS = ( CASE wtsi.ReceiveProvince
            //                      WHEN '北京' THEN ''
            //                      WHEN '天津' THEN ''
            //                      WHEN '上海' THEN ''
            //                      WHEN '重庆' THEN ''
            //                      ELSE ''
            //                    END ) + wtsi.ReceiveCity + wtsi.ReceiveArea
            //        + wtsi.ReceiveAddress,
            //       DeliveryFare=(SELECT TOP 1 fcbi.Fare FROM FMS_CODBaseInfo AS fcbi(NOLOCK) 
            //						WHERE fcbi.IsFare=1 AND fcbi.IsDeleted=0 AND fcbi.WaybillNO=w.WaybillNO ORDER BY fcbi.DeliverTime DESC)
            //FROM    LMS_RFD.dbo.Waybill AS w ( NOLOCK )
            //        JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON w.DeliverStationID = ec.ExpressCompanyID
            //        JOIN RFD_PMS.dbo.ExpressCompany AS ec1 ( NOLOCK ) ON ec1.ExpressCompanyID = ec.TopCODCompanyID
            //        JOIN LMS_RFD.dbo.WaybillInfo AS wi ON wi.WaybillNO = w.WaybillNO
            //        JOIN RFD_PMS.dbo.warehouse w1 ( NOLOCK ) ON w1.WarehouseId = w.WarehouseId
            //        JOIN LMS_RFD.dbo.WaybillTakeSendInfo AS wtsi ( NOLOCK ) ON w.WaybillNO = wtsi.WaybillNO
            //        WHERE w.MerchantID=8 AND w.IsDelete=0 AND w.ComeFrom in (14,15,16) {0}
            //		ORDER BY w.DeliverTime";

            SqlStr = @"
    WITH t AS (
	SELECT fbci1.WaybillNo,
		fbci1.AccountWeight Weight,
        fbci1.WarehouseId ,
        fbci1.ADDRESS,
        fbci1.TopCODCompanyID,
        fbci1.DeliverTime,
        fbci1.Fare DeliveryFare
        FROM FMS_CODBaseInfo fbci1(NOLOCK)
        JOIN (
			SELECT WaybillNO,
                MAX(fbci.DeliverTime) DeliverTime,
				MAX(ID) ID
			FROM FMS_CODBaseInfo fbci(NOLOCK)
			WHERE fbci.MerchantID=8 
				AND fbci.IsDeleted=0
				AND fbci.ComeFrom in (14,15,16) 
				{0}
            GROUP BY fbci.WaybillNO
	) aa ON fbci1.ID=aa.ID
)
SELECT	t.WaybillNO,
		ec1.CompanyName ,
		t.DeliverTime,
		t.Weight,
		t.WarehouseId,
		w1.WarehouseName,
		t.Address,
		t.DeliveryFare
	FROM t
        JOIN RFD_PMS.dbo.ExpressCompany AS ec1 ( NOLOCK ) ON t.TopCODCompanyID = ec1.ExpressCompanyID
        JOIN RFD_PMS.dbo.warehouse w1 ( NOLOCK ) ON w1.WarehouseId = t.WarehouseId
ORDER BY t.DeliverTime
";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND fbci.TopCODCompanyID=@TopCODCompanyID ");
                parameters.Add(new SqlParameter("@TopCODCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
                sbWhere.Append(" AND fbci.DeliverTime>=@DeliverTimeStr ");
                parameters.Add(new SqlParameter("@DeliverTimeStr", SqlDbType.DateTime) { Value = dateStr });
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                sbWhere.Append(" AND fbci.DeliverTime<=@DeliverTimeEnd ");
                parameters.Add(new SqlParameter("@DeliverTimeEnd", SqlDbType.DateTime) { Value = dateEnd });
            }

            SqlStr = string.Format(SqlStr, sbWhere.ToString());

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray());
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }
	}
}
