using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IStatusCodeInfoDao
    {
        int Exists(string codeType, string codeNo, string distributionCode);

        DataTable GetStatusInfoByCodeType(string codeType, string distributionCode, bool isEnabled);

        StatusCodeInfo GetModel(string codeType, string codeNo, string distributionCode, bool isEnabled);

        IList<string> GetAllTypes();

        IList<StatusCodeInfo> GetListByType(string codeType, string distributionCode,bool isEnabled);

        bool Add(StatusCodeInfo codeInfo);

        bool Update(StatusCodeInfo codeInfo);

        bool Delete(string codeType, string codeNo, string updateBy);

        void GetMaxNoAndOrderBy(string codeType, out string codeNo, out int orderBy);
    }
}
