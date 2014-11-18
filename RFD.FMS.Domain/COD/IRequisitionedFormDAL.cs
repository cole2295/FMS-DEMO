using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.COD
{
    public interface IRequisitionedFormDAL
    {
        System.Data.DataTable GetRequisitionedOrderList(string expressCompanyId, string dateStr, string dateEnd);
        System.Data.DataTable GetRequisitionedOrderListV2(string expressCompanyId, string dateStr, string dateEnd);
    }
}
