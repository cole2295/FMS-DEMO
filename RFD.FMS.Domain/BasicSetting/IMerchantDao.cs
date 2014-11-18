using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IMerchantDao
    {
        DataTable GetMerchants(string disCode);

        DataTable GetMerchantsNoExpress(string disCode);

        DataTable GetAllMerchants(string disCode);

        string GetMerchantName(long waybillNo);

        string GetMerchantCategory(int merchantId);

        int GetMerchantDeliverFee(int merchantId);

        decimal GetVolumeParmer(int MerchantID);

        DataTable GetMerchantNameByID(string ids);
    }
}
