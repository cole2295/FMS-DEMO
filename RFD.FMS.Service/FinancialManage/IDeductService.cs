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
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet.UnitOfWork;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IDeductService
    {
        void DealDetail();
        bool DealProjectSendByWaybillNO(long WaybillNO);

        /// <summary>
        /// 处理快递派件
        /// </summary>
        bool DealExpressSendByWaybillNO(long WaybillNo);
    }
}
