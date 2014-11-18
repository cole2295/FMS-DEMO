using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.DAL.Interface
{
    public interface IPager
    {
        String GetQueryString(int type, string condition);
    }
}
