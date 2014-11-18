using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Service.BasicSetting
{
    public interface INoGenerate
    {
        string GetLastNo(int NoType);
        string GetLastNo(int NoType, string fmt);
        string GetLastNoWithSeed(int NoType);
        string GetOrderNo(int noType, int length, bool addDate, string fmt);
        string GetOrderNoShortDate(int noType, int length, bool addDate, string fmt);
        string GetOrderNoWithSeed(int noType, int length, string logo);
    }

    public interface IIDGenerateService
    {
        string NewId(string dbflag, string tableflag, string columnflag);
        string NewId(string tableflag, string columnflag);
        string ServiceNewId(string tableflag, string columnflag);
    }
}
