using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;
using RFD.FMS.DAL.FinancialManage;
using Microsoft.JScript.Vsa;
using Vancl.TML.ServiceInterface;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.FinancialManage;
using LMS.Util;
using System.Data;
using RFD.FMS.MODEL;
using LMS.AdoNet.UnitOfWork;

namespace RFD.FMS.Service.Reduct
{
    public interface IDeductService
    {
        void DealDetail();
    }
}
