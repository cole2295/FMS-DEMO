using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.WEBLOGIC
{
    public interface IRuleUI
    {
        event Action<RuleParameter> DoSave;

        int NodeId { get; set; }

        int RuleType { get; set; }

        string Condition { get; set; }

        void InitUI();
    }

    public class RuleParameter
    {
        public int NodeId { get; set; }
        public int RuleType { get; set; }
        public string Condition { get; set; }
    }
}
