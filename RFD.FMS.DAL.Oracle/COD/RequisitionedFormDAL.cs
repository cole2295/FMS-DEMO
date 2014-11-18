using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.COD;
using Oracle.DataAccess.Client;

namespace RFD.FMS.DAL.Oracle.COD
{
    public class RequisitionedFormDAL : OracleDao, IRequisitionedFormDAL
	{
        public string SqlStr { get; set; }

        public DataTable GetRequisitionedOrderList(string expressCompanyId, string dateStr, string dateEnd)
        {
            #region old
            //            SqlStr = @"SELECT  w.WaybillNO ,
            //        ec1.CompanyName ,
            //        w.DeliverTime ,
            //        Weight = CASE WHEN NVL(wi.WayBillInfoWeight, 0) > NVL(wi.WayBillInfoVolumeWeight,
            //                                                              0)
            //                                 THEN NVL(wi.WayBillInfoWeight, 0)
            //                                 ELSE NVL(wi.WayBillInfoVolumeWeight, 0)
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
            //       DeliveryFare=(SELECT fcbi.Fare FROM FMS_CODBaseInfo AS fcbi 
            //						WHERE fcbi.IsFare=1 AND fcbi.IsDeleted=0 AND fcbi.WaybillNO=w.WaybillNO ORDER BY fcbi.DeliverTime DESC)
            //FROM    Waybill AS w
            //        JOIN ExpressCompany AS ec ON w.DeliverStationID = ec.ExpressCompanyID
            //        JOIN ExpressCompany AS ec1 ON ec1.ExpressCompanyID = ec.TopCODCompanyID
            //        JOIN WaybillInfo AS wi ON wi.WaybillNO = w.WaybillNO
            //        JOIN warehouse w1 ON w1.WarehouseId = w.WarehouseId
            //        JOIN WaybillTakeSendInfo AS wtsi ON w.WaybillNO = wtsi.WaybillNO
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
        FROM FMS_CODBaseInfo fbci1
        JOIN (
			SELECT WaybillNO,
                MAX(fbci.DeliverTime) DeliverTime,
				MAX(INFOID) ID
			FROM FMS_CODBaseInfo fbci
			WHERE fbci.MerchantID=8 
				AND fbci.IsDeleted=0
				AND fbci.ComeFrom in (14,15,16) 
				{0}
            GROUP BY fbci.WaybillNO
	) aa ON fbci1.INFOID=aa.ID
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
        JOIN ExpressCompany ec1 ON t.TopCODCompanyID = ec1.ExpressCompanyID
        JOIN warehouse w1 ON w1.WarehouseId = t.WarehouseId
ORDER BY t.DeliverTime
";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND fbci.TopCODCompanyID=:TopCODCompanyID ");
                parameters.Add(new OracleParameter(":TopCODCompanyID", OracleDbType.Decimal) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
                sbWhere.Append(" AND fbci.DeliverTime>=:DeliverTimeStr ");
                parameters.Add(new OracleParameter(":DeliverTimeStr", OracleDbType.Date) { Value = DataConvert.ToDateTime(dateStr) });
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                sbWhere.Append(" AND fbci.DeliverTime<=:DeliverTimeEnd ");
                parameters.Add(new OracleParameter(":DeliverTimeEnd", OracleDbType.Date) { Value = DataConvert.ToDateTime(dateEnd) });
            }

            SqlStr = string.Format(SqlStr, sbWhere.ToString());

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, ToParameters(parameters.ToArray()));
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetRequisitionedOrderListV2(string expressCompanyId, string dateStr, string dateEnd)
        {
            throw new Exception("oracle 无查询V2");
        }
	}
}
