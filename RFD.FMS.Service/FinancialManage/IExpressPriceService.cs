﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IExpressPriceService
    {
        DataTable GetAreaInfo(string strCityID, string strCityName);
    }
}
