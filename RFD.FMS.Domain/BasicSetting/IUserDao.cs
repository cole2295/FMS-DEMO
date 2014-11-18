using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Model;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IUserDao
    {
        bool DeleteOldData();
        bool Exists(string EmployeeCode);
        System.Data.DataSet GetList(string strWhere);
        Employee GetModel(int EmployeeID);
        System.Data.DataSet GetSampList(Employee model);
        System.Data.DataSet GetSampList(string employCode);
        System.Data.DataTable GetUserByDepartmentID(int departmentID);
        System.Data.DataTable GetUserRoles(int userId);
        System.Collections.Generic.IDictionary<int, string> GetUserRoles(string userCode);
        bool UpdateIsChangeStatus();
        System.Data.DataTable UserLogIn(Employee employee);
    }
}
