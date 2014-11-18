using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Model;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IUserService
    {
        RFD.FMS.Util.ControlHelper.UserTree<string> CreateDepartmentTree();
        System.Collections.Generic.KeyValuePair<int, string> GetCompanyByUser(int userId);
        System.Data.DataSet GetList(string where);
        Employee GetModel(int EmployeeID);
        string GetUserRoles(int userId);
        bool UserLogIn(Employee employee);
        DataSet GetSampList(Employee model);
        IDictionary<int, string> GetUserRoles(string userCode);
        DataSet GetSampList(string employCode);
    }
}
