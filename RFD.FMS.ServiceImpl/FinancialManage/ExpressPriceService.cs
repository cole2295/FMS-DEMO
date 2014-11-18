using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
    [DataObject]
    public class ExpressPriceService : IExpressPriceService
    {
        private IExpressPriceDao _expressPriceDao;

         /// <summary>
        /// 查询所有区域运费信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAreaInfo(string strCityID,string strCityName)
         {
             return _expressPriceDao.GetAreaInfo(strCityID, strCityName);
         }
    }
}
