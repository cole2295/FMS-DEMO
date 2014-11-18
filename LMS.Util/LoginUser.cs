using System;

namespace RFD.FMS.Util
{
    public class LoginUser
    {
        public const string CookieUserID = "RFDLMSUserID";
        public const string CookieUserCode = "RFDLMSUserCode";
        public const string CookieUserName = "RFDLMSUserName";
        public const string CookieExpressId = "RFDLMSExpressID";
        public const string CookieExpressCode = "RFDLMSExpressCode";
        public const string CookieExpressName = "RFDLMSExpressName";
        public const string CookieDistributionCode = "DistributionCode";

        /// <summary>
        /// 员工ID
        /// </summary>
        public static int Userid
        {
            get
            {
                int userId = CookieUtil.GetCookie<int>(CookieUserID);

                if (userId == 0)
                {
                    Exception ex = new Exception("帐号异常,用户Cookies丢失");

                    throw ex;
                }

                return userId;
            }
            set
            {
                CookieUtil.AddCookie<int>(CookieUserID, value);
            }
        }

        /// <summary>
        /// 员工代号
        /// </summary>
        public static string UserCode
        {
            get
            {
                return CookieUtil.GetCookie<string>(CookieUserCode);
            }
            set
            {
                CookieUtil.AddCookie<string>(CookieUserCode, value);
            }
        }

        /// <summary>
        /// 员工名称
        /// </summary>
        public static string UserName
        {
            get
            {
                string userName = CookieUtil.GetCookie<string>(CookieUserName);

                if (userName == null)
                {
                    return "未设置Cookie";
                }

                return userName;
            }
            set
            {
                CookieUtil.AddCookie<string>(CookieUserName, value);
            }
        }

        /// <summary>
        /// 单位编码
        /// </summary>
        public static int ExpressId
        {
            get
            {
                return CookieUtil.GetCookie<int>(CookieExpressId);
            }
            set
            {
                CookieUtil.AddCookie<int>(CookieExpressId, value);
            }
        }

        /// <summary>
        /// 单位代码
        /// </summary>
        public static string ExpressCode
        {
            get
            {
                return CookieUtil.GetCookie<string>(CookieExpressCode);
            }
            set
            {
                CookieUtil.AddCookie<string>(CookieExpressCode, value);
            }
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public static string ExpressName
        {
            get
            {
                return CookieUtil.GetCookie<string>(CookieExpressName);
            }
            set
            {
                CookieUtil.AddCookie<string>(CookieExpressName, value);
            }
        }


        /// <summary>
        /// 配送商Code
        /// </summary>
        public static string DistributionCode
        {
            get
            {
                return CookieUtil.GetCookie<string>(CookieDistributionCode);
            }
            set
            {
                CookieUtil.AddCookie<string>(CookieDistributionCode, value);
            }
        }
    }
}
