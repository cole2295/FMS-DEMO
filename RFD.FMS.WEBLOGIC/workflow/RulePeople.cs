using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.FMS.Model;
using RFD.FMS.WEBLOGIC;
using System.Text;
using LMS.Util;

namespace RFD.FMS.WEBLOGIC
{
    public class RulePeople : IRule
    {
        #region IRule 成员

        public IRuleResolve Resolve
        {
            get { return resolve; }
            set { resolve = value; }
        }

        public int DoHandler(FMS_FlowProcess flowProcess, string condition)
        {
            string[] array = condition.Split('@');

            string item = "";
            string value = "";
            object objValue = null;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                item = array[i].Split('=')[0];
                value = array[i].Split('=')[1];

                stringBuilder = new StringBuilder();

                stringBuilder.Append("select count(1) from FMS_FlowOperatorLog where Operator in (");
                stringBuilder.Append(item);
                stringBuilder.Append(")");
                stringBuilder.Append(" and ID = ");
                stringBuilder.Append(flowProcess.BizOrderId.Value);

                objValue = DBService.ExecuteScalar(stringBuilder.ToString());

                if (DataConvert.ToInt(objValue) > 0) return DataConvert.ToInt(value);
            }

            return -1;
        }

        #endregion

        private IRuleResolve resolve;
    }
}
