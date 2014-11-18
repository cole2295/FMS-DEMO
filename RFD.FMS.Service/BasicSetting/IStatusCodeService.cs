using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;
using System.Data;
using System.Web.UI.WebControls;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IStatusCodeService
    {
        IList<string> GetAllType();

        DataTable GetCodeItemByCodeType(string codeType, string distributionCode, bool isEnabled);

        IList<StatusCodeInfo> GetListByType(string codeType, string distributionCode, bool isEnabled);

        int Exists(string codeType, string codeNo, string distributionCode);

        bool Add(StatusCodeInfo codeInfo);

        bool Update(StatusCodeInfo codeInfo);

        bool Delete(string codeType, string codeNo, string updateBy);

        StatusCodeInfo GetModel(string codeType, string codeNo, string distributionCode, bool isEnabled);

        void BindDropDownListByCodeType(DropDownList dropDownList, string text, string value, string codeType, string distributionCode);

        bool AddByRelationType(StatusCodeInfo codeInfo);
    }
}
