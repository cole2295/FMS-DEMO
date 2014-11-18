using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IWaybillStatusForSortingTransfer
    {
        int InsertIntoSortingTransferDetail(WaybillStatusChangeLog model);
    }
}
