using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Service.BasicSetting;
namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class GoodsCategoryService : IGoodsCategoryService
    {
        private IGoodsCategoryDao Dao;

        public DataTable GetAllGoods()
        {
            return Dao.GetAllGoods();
        }

        public string GetNameByCode(string code)
        {
            return Dao.GetNameByCode(code);
        }

        public DataTable GetGoodsCategoryByMerchantID(int merchantId, string distributionCode)
        {
            IGoodsCategoryDao Dao = new RFD.FMS.DAL.Oracle.BasicSetting.GoodsCategoryDao();
            return Dao.GetGoodsCategoryByMerchantID(merchantId, distributionCode);
        }
    }
}
