using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using RFD.FMS.Proxy.LMSRoleProxy;

using RFD.FMS.Util;
using LMS.AdoDao.BasicSetting;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.WEBLOGIC
{
    public class StatusCodeService
    {
        private static StatusCodeInfoDao codeInfoDao = new StatusCodeInfoDao();

        public static IList<string> GetAllType()
        {
            return codeInfoDao.GetAllTypes();
        }

        public static DataTable GetCodeItemByCodeType(string codeType)
        {
            return codeInfoDao.GetStatusInfoByCodeType(codeType);
        }

        public static int Exists(string codeType, string codeNo)
        {
            return codeInfoDao.Exists(codeType,codeNo);
        }

        public static bool Add(StatusCodeInfo codeInfo)
        {
            return codeInfoDao.Add(codeInfo);
        }

        public static bool Update(StatusCodeInfo codeInfo)
        {
            return codeInfoDao.Update(codeInfo);
        }

        public static bool Delete(string codeType, string codeNo)
        {
            return codeInfoDao.Delete(codeType, codeNo);
        }

        public static StatusCodeInfo GetModel(string codeType, string codeNo)
        {
            return codeInfoDao.GetModel(codeType, codeNo);
        }
    }
}
