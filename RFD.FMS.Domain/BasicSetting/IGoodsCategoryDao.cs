using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IGoodsCategoryDao
    {
        DataTable GetAllGoods();

        string GetNameByCode(string code);

        DataTable GetGoodsCategoryByMerchantID(int merchantId, string distributionCode);
    }
}
