using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.COD
{
    public interface IRequisitionedForm
    {
        List<RequisitionedFormModel> GetRequisitionedOrderListV2(string expressCompanyId, string dateStr, string dateEnd);

        List<RequisitionedFormModel> GetRequisitionedOrderList(string expressCompanyId ,string dateStr,string dateEnd);
    }
}
