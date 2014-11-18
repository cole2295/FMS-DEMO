using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RFD.FMS.Util.WebExtension
{
    public class WebHttpContext
    {
        public static HttpContext GetCurrentHttpContext()
        {
            return HttpContext.Current;
        }
    }
}
