using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.COD;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Util;
using System.ServiceModel.Activation;
using RFD.FMS.Service;
using RFD.FMS.Service.Mail;

namespace RFD.FMS.ServiceImpl.COD
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DistributionFeeContractService : IDistributionFeeContractService
    {
        public DistributionFeeDTO GetistributionFee(DistributionFeeDTO searchModel)
        {
            try
            {
                IDistributionFeeService service = ServiceLocator.GetService<IDistributionFeeService>();

                return service.GetistributionFee(searchModel);
            }
            catch (Exception ex)
            {
                IMail mail = ServiceLocator.GetService<IMail>();
                StringBuilder sbStr = new StringBuilder();
                sbStr.Append("FormCode:" + searchModel.FormCode+";");
                sbStr.Append("ExpressCompanyID:" + searchModel.ExpressCompanyID + ";");
                sbStr.Append("Status:" + searchModel.Status + ";");
                sbStr.Append("MerchantID:" + searchModel.MerchantID + "\n");
                sbStr.Append("Exception:"+ex.Message+"\n"+ex.InnerException);
                mail.SendMailToUser("配送费接口异常", sbStr.ToString(), "zengwei@vancl.cn");
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = "配送费查询异常";
                searchModel.Fare = 0M;
                return searchModel;
            }
        }
    }
}
