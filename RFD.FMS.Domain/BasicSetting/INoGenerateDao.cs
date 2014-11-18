using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface INoGenerateDao
    {
        string GetLastNo(int NoType, string dateStr);

        string GetLastNo(int noType);

        string GetLastNo(string tableName, string columnName, out DateTime? dbDate, out string tabColCode);
    }
}
