using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.COD;
using RFD.FMS.Service.COD;

namespace RFD.FMS.WEBLOGIC.COD
{
	public class RequisitionedForm:IRequisitionedForm
	{
        private IRequisitionedFormDAL _requisitionedFormDAL;
        private IRequisitionedForm OracleService;

        public List<RequisitionedFormModel> GetRequisitionedOrderListV2(string expressCompanyId, string dateStr, string dateEnd)
        {
            if (OracleService != null)
            {
                return OracleService.GetRequisitionedOrderListV2(expressCompanyId, dateStr, dateEnd);
            }
            DataTable dt = _requisitionedFormDAL.GetRequisitionedOrderListV2(expressCompanyId, dateStr, dateEnd);

            return TransformToRequisitionedModel(dt);
        }

		public List<RequisitionedFormModel> GetRequisitionedOrderList(string expressCompanyId ,string dateStr,string dateEnd)
		{
            if (OracleService != null)
            {
                return OracleService.GetRequisitionedOrderList(expressCompanyId, dateStr, dateEnd);
            }
            DataTable dt = _requisitionedFormDAL.GetRequisitionedOrderList(expressCompanyId, dateStr, dateEnd);

			return TransformToRequisitionedModel(dt);
		}

		private List<RequisitionedFormModel> TransformToRequisitionedModel(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;

			List<RequisitionedFormModel> rList = new List<RequisitionedFormModel>();
			foreach (DataRow dr in dt.Rows)
			{
				RequisitionedFormModel r = new RequisitionedFormModel();
				
				r.WaybillNO = dr["WaybillNO"].ToString();
				if (dr["Weight"] != DBNull.Value)
					r.Weight = dr["Weight"].ToString().TryGetDecimal();
				if (dr["WarehouseId"] != DBNull.Value)
					r.WarehouseId = dr["WarehouseId"].ToString();
				if (dr["WarehouseName"] != DBNull.Value)
					r.WarehouseName = dr["WarehouseName"].ToString();
				if (dr["DeliverTime"] != DBNull.Value)
					r.DeliverTime = dr["DeliverTime"].ToString().TryGetDateTime();
				if (dr["CompanyName"] != DBNull.Value)
					r.CompanyName = dr["CompanyName"].ToString();
				if (dr["DeliveryFare"] != DBNull.Value)
					r.DeliveryFare = dr["DeliveryFare"].ToString().TryGetDecimal();
				if (dr["ADDRESS"] != DBNull.Value)
					r.Address = dr["ADDRESS"].ToString();
				rList.Add(r);
			}

			return rList;
		}
	}
}
