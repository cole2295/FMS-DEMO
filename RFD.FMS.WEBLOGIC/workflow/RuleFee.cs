using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.FMS.Model;
using System.Text;
using LMS.Util;

namespace RFD.FMS.WEBLOGIC
{
    public class RuleFee : IRule
    {
        #region IRule 成员

        public IRuleResolve Resolve
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int DoHandler(FMS_FlowProcess flowProcess, string condition)
        {
            FMS_FeeApply feeApply = DBService.GetModel<FMS_FeeApply,int>(flowProcess.BizOrderId.Value);

            string[] array = condition.Split(',');

            string item = "";
            string value = "";
            object objValue = null;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                item = array[i].Split('=')[0];
                value = array[i].Split('=')[1];

                stringBuilder = new StringBuilder();

                stringBuilder.Append("select count(1) from FMS_FeeApply where ");
                stringBuilder.Append(Resolve.DoResolve(item));
                stringBuilder.Append(" and ID = ");
                stringBuilder.Append(flowProcess.BizOrderId.Value);

                objValue = DBService.ExecuteScalar(stringBuilder.ToString());

                if (DataConvert.ToInt(objValue) > 0) return DataConvert.ToInt(value);
            }

            return -1;
        }

        #endregion
    }
}
