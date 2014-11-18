using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util.Security;
using RFD.SSO.WebClient;
using System.Transactions;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Model;
using RFD.FMS.DAL.Oracle.BasicSetting;

namespace RFD.FMS.WEB
{
	public partial class Login : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                if (LoginType.IsSsoLogin)
                {
                    Response.Redirect("SsoAuthHandler.aspx", true);

                    return;
                }

                if (HttpContext.Current.Request.Cookies[LoginUser.CookieUserID] != null)
                    CookieUtil.ClearCookie(LoginUser.CookieUserID);
                if (HttpContext.Current.Request.Cookies[LoginUser.CookieExpressId] != null)
                    CookieUtil.ClearCookie(LoginUser.CookieExpressId);
                if (HttpContext.Current.Request.Cookies[LoginUser.CookieUserName] != null)
                    CookieUtil.ClearCookie(LoginUser.CookieUserName);
                if (HttpContext.Current.Request.Cookies[LoginUser.CookieUserCode] != null)
                    CookieUtil.ClearCookie(LoginUser.CookieUserCode);
				//设置自动登录
				AutoLogin();
			}
		}

		protected void btnlogin_Click(object sender, EventArgs e)
		{
            try
            {   
                
                LoginType.IsSsoLogin = false;

                CookieUtil.AddCookie("ChangeValidate", "NO");
                if (Request.Cookies["Validate"] == null)
                {
                    LoginError("验证码过期，请重新登陆！");
                    return;
                }
                string validate = CookieUtil.GetCookie("Validate");
                CookieUtil.ClearCookie("Validate");
                if (string.IsNullOrEmpty(validate) || txtValidate.Text != validate)
                {
                    LoginError("验证码输入错误！");
                    return;
                }
                //取值,赋值
                var user = new Employee
                {
                    EmployeeCode = TxtUserCode.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", ""),
                    PassWord = TxtPassWord.Text.Trim().Replace("'", "‘").Replace("-", "——").Replace(";", "")
                };
                //控制推广部门人员不能登录老系统
                //DisableLogin(user.EmployeeCode);
                //实例化业务层
                var userService = ServiceLocator.GetService<IUserService>();

                if (userService.UserLogIn(user))
                    Response.Redirect("index.aspx", true);
                else
                    LoginError("登录失败！");
                //ClientScript.RegisterStartupScript(this.GetType(), "__LoginResultInfo__", "alert(\"登录失败!\")", true);
            }
            catch (Exception ex)
            {
                LoginError("登陆失败<br>"+ex.Message);
            }
		}

        private void LoginError(string message)
        {
            ClientScript.RegisterStartupScript(GetType(), "__LoginResultInfo__", "alert(\"" + message + "\")", true);
        }

		private void DisableLogin(string employeeCode)
		{
			//实例化业务层
            var userService = ServiceLocator.GetService<IUserService>();
            var roles = userService.GetUserRoles(employeeCode);
			if (ContainsRole(roles, OracleFMS))
			{
				//LoginError("你所在的部门已经推广使用新系统！");
				Response.Redirect(ConfigurationManager.AppSettings["FMS_Oracle_URL"]);
			}
		}

		private void AutoLogin()
		{
			if (!Request.Url.Query.IsNullData())
			{
				//进入新系统
				var decryptText = HttpUtility.UrlDecode(Request.Url.Query.Replace("?", ""));
				var query = DES.Decrypt3DES(decryptText).Split('∞');				
				//var query = decryptText.Split('∞');
				//根据自动认证来判断
				if (!query[0].IsNullData())
				{
					var username = query[1];
					var password = query[2];
					var backurl = query[3];
					//实例化业务层
                    var userService = ServiceLocator.GetService<IUserService>();
					//取值,赋值
					var user = new Employee
					{
						EmployeeCode = username,
						PassWord = password
					};
					//用户登录系统
					if (userService.UserLogIn(user))
					{
						Response.Redirect("index.aspx", true);
					}
					else
					{
						Response.Redirect(backurl + "?r=false", true);
					}
				}
			}
		}

		private bool ContainsRole(IDictionary<int, string> roles, string[] loginRole)
		{
			foreach (int role in roles.Keys)
			{
				foreach (string r in loginRole)
				{
					if (role.ToString() == r)
					{
						return true;
					}
				}
			}
			return false;
		}

		public string[] OracleFMS
		{
			get
			{
				return GetRoles("Oracle_FMS");
			}
		}

		private string[] GetRoles(string configKey)
		{
			var roles = new List<string>();
			var list = ConfigurationManager.AppSettings[configKey].Split(',');
			foreach (string role in list)
			{
				if (roles.Contains(role)) continue;
				roles.Add(role);
			}
			return roles.ToArray();
		}
	}
}
