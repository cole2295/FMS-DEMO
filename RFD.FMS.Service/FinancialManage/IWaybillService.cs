using System.Data;
using RFD.FMS.DAL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IWaybillService
    {
        DataTable GetThirdWaybillDetails(ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref PageInfo pi, out DataTable amount);

        DataTable GetThirdWaybillStat(ThirdPartyWaybillSearchConditons conditions, out DataTable amount);
    }
}
