using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.FMS.Model;
using RFD.FMS.MODEL;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEBLOGIC.RuleObject;

namespace RFD.FMS.RULE
{
	public class FeeRule : IUserRule
	{
        #region IUserRule 成员

        public int Invoke(FMS_FlowProcess process, int ruleId)
        {
            LMS.FMS.Model.FMS_FeeApply feeApply = DBService.GetModel<LMS.FMS.Model.FMS_FeeApply, int>(process.BizOrderId.Value);

            RuleService ruleService = new RuleService();

            RuleObjectFee ruleObject = ruleService.GetRuleFeePropertyValue(process.FlowPointId.Value, ruleId);

            if (feeApply.AllFee.Value > ruleObject.FeeStart && feeApply.AllFee.Value <= ruleObject.FeeStop) return ruleObject.ToNode;

            return -1;
        }

        #endregion
    }
}
