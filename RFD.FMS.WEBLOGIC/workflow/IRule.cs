using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.FMS.Model;

namespace RFD.FMS.WEBLOGIC
{
    public interface IRule
    {
        IRuleResolve Resolve { get; set; }

        /// <summary>
        /// 根据条件脚本执行规则得到审批下一节点。
        /// </summary>
        /// <param name="condition">条件脚本</param>
        /// <returns>返回下一审批节点的编号</returns>
        int DoHandler(FMS_FlowProcess flowProcess, string condition);
    }
}
