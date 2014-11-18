using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.FMS.Model;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using LMS.Util;

namespace RFD.FMS.RULE
{
	public class DemoLogicPoint : IUserRule
	{
        #region IUserRule 成员

        public int Invoke(FMS_FlowProcess process,int ruleId)
        {
            LMS.FMS.DAL.FMS_FeeApply feeApplyDao = new LMS.FMS.DAL.FMS_FeeApply();

            bool isHave = feeApplyDao.HasBGYPSubject(process.BizOrderId.Value);

            if (isHave == false) return -1;

            RuleService ruleService = new RuleService();

            string strToNode = ruleService.GetRuleOfficeSuppliesValue(process.FlowPointId.Value, ruleId);

            return DataConvert.ToInt(strToNode, -1);
        }

        #endregion
    }
}
