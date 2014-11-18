using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.WEBLOGIC
{
    public class RuleResolveFee : IRuleResolve
    {
        #region IRuleResolve 成员

        public string DoResolve(string condition)
        {
            return condition;
        }

        public string UnDoResolve(string sqlCondition)
        {
            return sqlCondition;
        }

        #endregion
    }
}
