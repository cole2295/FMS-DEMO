using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IMerchantService
    {
        DataTable GetMerchants(string disCode);

        DataTable GetAllMerchants(string disCode);

        DataTable GetMerchantsNoExpress(string disCode);

        string GetMerchantName(long waybillNo);

        string GetMerchantCategory(int merchantId);
    }
}
