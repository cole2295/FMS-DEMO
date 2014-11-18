using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IGoodsCategoryService
    {
        DataTable GetAllGoods();

        string GetNameByCode(string code);

        DataTable GetGoodsCategoryByMerchantID(int merchantId, string distributionCode);
    }
}
