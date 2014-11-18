using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.Trace
{
    public enum TraceLevel
    {
        [Description("Error")]
        Error,
        [Description("Fail")]
        Fail,
        [Description("Ok")]
        Ok,
        [Description("Success")]
        Success,
        [Description("Warning")]
        Warning
    }
}
