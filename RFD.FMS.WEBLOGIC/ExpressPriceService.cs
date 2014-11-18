using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.DAL.BasicSetting;

namespace RFD.FMS.WEBLOGIC
{
    [DataObject]
    public class ExpressPriceService
    {
        private readonly ExpressPriceDao dao=new ExpressPriceDao();

         /// <summary>
        /// 查询所有区域运费信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAreaInfo(string strCityID,string strCityName)
         {
             return dao.GetAreaInfo(strCityID, strCityName);
         }
    }
}
