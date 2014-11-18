using System;
using System.Data;
using System.Web.Services;

using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model;

/// <summary>
/// BasicSettingWebService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
[System.Web.Script.Services.ScriptService]
public class BasicSettingWebService : System.Web.Services.WebService
{
    /// <summary>
    /// 获取站点信息
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public ExpressCompany GetStationInfo(string SimpleSpell)
    {
        //实例化业务层
        ExpressCompany model = new ExpressCompany();
        var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
        DataTable dataTable = expressCompanyService.GetStationListBySimpleSpell(SimpleSpell.ToUpper());
        //没找到活找到多条
        if (dataTable.Rows.Count == 0 || dataTable.Rows.Count > 1)
        {
            return null;
        }
        else
        {
            model.ExpressCompanyID = Convert.ToInt32(dataTable.Rows[0]["ExpressCompanyID"].ToString());
            model.CompanyName = dataTable.Rows[0]["CompanyName"].ToString();
            return model;
        }
    }

    /// <summary>
    /// 获取员工信息 zjw 2011-06-12
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public Employee GetEmployeeInfo(string employeeCode)
    {
        //实例化业务层
        Employee model = new Employee();
        var userService = ServiceLocator.GetService<IUserService>();
        DataTable dataTable = userService.GetSampList(employeeCode.ToUpper()).Tables[0];
        //没找到活找到多条
        if (dataTable.Rows.Count == 0 || dataTable.Rows.Count > 1)
        {
            return null;
        }
        else
        {
            model.ID = Convert.ToInt32(dataTable.Rows[0]["EmployeeID"].ToString());
            model.EmployeeName = dataTable.Rows[0]["EmployeeName"].ToString();
            model.EmployeeCode = dataTable.Rows[0]["EmployeeCode"].ToString();

            return model;
        }
    }
    /// <summary>
    /// 获取汉字的第一个字母并返回
    /// </summary>
    /// <param name="Name">源汉字字符</param>
    /// <returns>第一个字母组合</returns>
    [WebMethod]
    public string GetSimpleSpell(string Name)
    {
        return ChsSpell.GetChsSpell(Name.Trim());
    }

    /// <summary>
    /// 清理Cookie
    /// </summary>
    [WebMethod]
    public void ClearCookie()
    {
        CookieUtil.ClearCookie("RFDLMSUserID");
        CookieUtil.ClearCookie("RFDLMSExpressID");
        CookieUtil.ClearCookie("RFDLMSUserID");
        CookieUtil.ClearCookie("RFDLMSUserID");
    }
}
