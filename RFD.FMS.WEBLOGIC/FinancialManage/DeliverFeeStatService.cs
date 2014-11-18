using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Model;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    [DataObject]
    public class DeliverFeeStatService : RFD.FMS.Service.FinancialManage.IDeliverFeeStatService
    {
        private IDeliverFeeStatDao _deliverFeeStatDao;
        private readonly IProvinceDao procinceDao = ServiceLocator.GetService<IProvinceDao>();
        private readonly ICityDao cityDao = ServiceLocator.GetService<ICityDao>();

        public DataTable GetProvince()
        {
            return procinceDao.GetProvinceList();
        }
        public DataTable GetCity(string strProvinceID)
        {
            City c=new City();
            c.ProvinceID = strProvinceID;
            return cityDao.GetCityList(c);
        }
        public  DataTable GetStationList(string strCityID)
        {
            return _deliverFeeStatDao.GetStationList(strCityID);
        }

        public DataTable GetQueryDataV2(Hashtable ht)
        {
            return _deliverFeeStatDao.GetQueryDataV2(ht);
        }

        public DataTable GetQueryData(Hashtable ht)
        {
            return _deliverFeeStatDao.GetQueryData(ht);
        }

         public DataTable GetDetail(Hashtable ht)
         {
             return _deliverFeeStatDao.GetDetail(ht);
         }
    }
}
