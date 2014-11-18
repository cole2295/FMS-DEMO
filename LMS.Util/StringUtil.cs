using System.Text.RegularExpressions;
using System.Web;
using System;
using System.Data;
using System.Text;

namespace RFD.FMS.Util
{
    public static class StringUtil
    {
        /// <summary>
        /// �����ַ�
        /// </summary>
        public const string SpecialChar = @"[~!@#$%&*':?/.\\|}{)(=]";

        /// <summary>
        /// �ж��Ƿ�Ϊ��ȷ�Ĺ̶��绰����
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsValidPhone(string phone)
        {
            //string pattern = @"^(\d{7,8}|(\(|\��)\d{3,4}(\)|\��)\d{7,8}|\d{3,4}(-?)\d{7,8})(((-|ת)\d{1,9})?)$ ";
            if (string.IsNullOrEmpty(phone)) return false;
            const string pattern = @"\b\d{7,16}";
            return Regex.Match(phone, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ��ȷ���ֻ���
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;
            const string pattern = @"\b1(3|5|8)\d{9}\b";
            return Regex.Match(mobile, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ��ȷ����������
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsValidPostalCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            const string pattern = @"\b\d{6}\b";
            return Regex.Match(code, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ��ȷ��Email��ַ
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            const string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            return Regex.Match(email, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// �Ƿ������ַ���
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCnString(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            const string pattern = @"[\u4e00-\uf900]";
            return Regex.Match(str, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// �Ƿ��������ַ�
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsCnString(char ch)
        {
            return IsCnString(ch.ToString());
        }

        /// <summary>
        /// �Ƿ�����ȷ�Ķ�����
        /// </summary>
        /// <param name="formcode"></param>
        /// <returns></returns>
        public static bool IsValidFormCode(string formcode)
        {
            if (string.IsNullOrEmpty(formcode)) return false;
            const string pattern = @"^\b(1|2|3|4|5|7|8|9)\d{11,14}\b$";
            return Regex.Match(formcode, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// �Ƿ���������ַ�
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsContainSpecialChar(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return Regex.Match(str, SpecialChar, RegexOptions.Compiled).Success;
        }

        public static void Alert(string Message)
        {
            string js = @"<Script language='JavaScript'>
                            alert('" + Message + "');</Script>";

            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1
                = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");
            return reg1.IsMatch(str);
        }
        /// <summary>
        /// �Ƿ������ֻ���ĸ�Լ��»������
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumericOrLetter(string str)
        {
            System.Text.RegularExpressions.Regex reg1
               = new System.Text.RegularExpressions.Regex(@"^\w+$");
            return reg1.IsMatch(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutString(string str, int length)
        {
            string newString = string.Empty;
            if (str != string.Empty)
            {
                if (str.Length > length)
                {
                    newString = str.Substring(0, length);
                }
                else
                {
                    newString = str;
                }
            }
            return newString;
        }
        /// <summary>
        /// �ж��Ƿ�Ϊ������
        /// </summary>
        /// <typeparam name="T">��ǰ����</typeparam>
        /// <param name="current">��ǰ����</param>
        /// <returns></returns>
        public static bool IsNullData<T>(this T current)
        {
            if (current == null || (object)current == DBNull.Value) return true;
            return String.IsNullOrEmpty(current.ToString().Trim());
        }
        /// <summary>
        /// ��ʽ�����ҽ�����
        /// </summary>
        /// <param name="money">��ǰ���</param>
        /// <returns></returns>
        public static string FormatMoney(object money)
        {
            if (money.IsNullData()) return string.Empty;
            return Convert.ToDecimal(money.ToString()).ToString("#,##0.###");
        }

        /// <summary>
        /// ��ʽ��ʱ�����
        /// </summary>
        /// <param name="date">��ǰʱ��</param>
        /// <param name="datepart">ѡ����ȡ����</param>
        /// <example>sd:�����ڣ�st:�����ڣ�ld:��ʱ�䣻lt:��ʱ��; all: ��������ʱ��</example>
        /// <returns></returns>
        public static string FormatDateTime(object date, string datepart)
        {
            var result = DateTime.Now;
            if (date.IsNullData() || !DateTime.TryParse(date.ToString(), out result))
                return string.Empty;
            var datetime = Convert.ToDateTime(date.ToString().Trim());
            switch (datepart.ToLower())
            {
                case "":
                case "sd":
                    return datetime.ToShortDateString();
                case "st":
                    return datetime.ToShortTimeString();
                case "all":
                    return datetime.ToString();
                case "ld":
                    return datetime.ToLongDateString();
                case "lt":
                    return datetime.ToLongTimeString();
                default:
                    return datetime.ToString(datepart);
            }
        }
        /// <summary>
        /// ��ʽ��ʱ�����(Ĭ�ϸ�ʽ��Ϊ������)
        /// </summary>
        /// <param name="date">��ǰʱ��</param>
        /// <returns></returns>
        public static string FormatDateTime(object date)
        {
            return FormatDateTime(date, "ld");
        }

        /// <summary>
        /// ��ʽ��ʱ�����(Ĭ�ϸ�ʽ��Ϊ������)
        /// </summary>
        /// <param name="date">��ǰʱ��</param>
        /// <returns></returns>
        public static DateTime FormatDateTime(object date, DateTime defaultValue)
        {
            var datetime = FormatDateTime(date);
            return datetime.IsNullData() ? defaultValue : Convert.ToDateTime(datetime);
        }

        public static string FormatDataRow(DataRow dr, string field)
        {
            return dr[field].IsNullData() ? "" : dr[field].ToString().Trim();
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ����
        /// </summary>
        /// <param name="dr">��ǰ��������</param>
        /// <returns></returns>
        public static bool IsEmptyDataRow(DataRow dr)
        {
            var result = true;
            var items = dr.ItemArray;
            foreach (var item in items)
            {
                if (!item.IsNullData())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// �жϵ�ǰ���Ƿ�Ϊģ���Ӧ��
        /// </summary>
        /// <param name="dr">��ǰ��</param>
        /// <param name="template">ģ��</param>
        /// <param name="index">ģ��������</param>
        /// <returns></returns>
        public static bool IsTemplateRow(this DataRow dr, DataTable template, int index)
        {
            var result = false;
            var items = dr.ItemArray;
            for (var i = 0; i < template.Columns.Count; i++)
            {
                var dc = template.Columns[i];
                var cell = items[i].IsNullData() ? "" : items[i].ToString().Trim();
                var col = FormatDataRow(template.Rows[index], dc.ColumnName);
                if (cell != col)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// ����ǰ�ַ�������ָ���ķָ������е���
        /// </summary>
        /// <param name="current">��ǰ�ַ���</param>
        /// <param name="appendStr">��Ҫ���ӵ��ַ���</param>
        /// <param name="separator">�ָ���</param>
        /// <returns></returns>
        public static string Append(this string current, string appendStr, char separator)
        {
            current += current.IsNullData() ? appendStr.Trim() : separator + appendStr.Trim();
            return current;
        }
        /// <summary>
        /// ����ǰ�ַ�������ָ���ķָ������е��ӣ�Ĭ�ϰ��ո���ӣ�
        /// </summary>
        /// <param name="current">��ǰ�ַ���</param>
        /// <param name="appendStr">��Ҫ���ӵ��ַ���</param>
        /// <returns></returns>
        public static string Append(this string current, string appendStr)
        {
            current += Append(current, appendStr, ' ');
            return current;
        }
        /// <summary>
        /// ����ǰ����ת��Ϊ����(δת���ɹ���ȡĬ��ֵ)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns></returns>
        public static int ConvertToInt<T>(this T current, int defaultValue)
        {
            var result = 0;
            return current.IsNullData() || !int.TryParse(current.ToString(), out result) ? defaultValue : result;
        }

        /// <summary>
        /// ����ǰ����ת��Ϊ����(δת���ɹ���ȡĬ��ֵ)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns></returns>
        public static long ConvertToLong<T>(this T current, long defaultValue)
        {
            long result = 0;
            return current.IsNullData() || !long.TryParse(current.ToString(), out result) ? defaultValue : result;
        }

        /// <summary>
        /// ����ǰ����ת��Ϊ����(δת���ɹ���ȡĬ��ֵ)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns></returns>
        public static bool ConvertToBool<T>(this T current, bool defaultValue)
        {
            var result = false;
            return current.IsNullData() || !bool.TryParse(current.ToString(), out result) ? defaultValue : result;
        }

        /// <summary>
        /// ����ǰ����ת��Ϊ����(δת���ɹ���ȡfalse)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <returns></returns>
        public static bool ConvertToBool<T>(this T current)
        {
            return current.ConvertToBool(false);
        }

        /// <summary>
        /// ����ǰ����ת��Ϊ����(δת���ɹ���ȡ0)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <returns></returns>
        public static int ConvertToInt<T>(this T current)
        {
            return current.ConvertToInt(0);
        }

        /// <summary>
        /// ����ǰ����ת��Ϊ����(δת���ɹ���ȡ0)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <returns></returns>
        public static long ConvertToLong<T>(this T current)
        {
            return current.ConvertToLong(0);
        }

        /// <summary>
        /// ����ǰ����ת��ΪС��(δת���ɹ���ȡĬ��ֵ)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal<T>(this T current, decimal defaultValue)
        {
            var result = 0.0m;
            return current.IsNullData() || !decimal.TryParse(current.ToString(), out result) ? defaultValue : result;
        }
        /// <summary>
        /// ����ǰ����ת��ΪС��(δת���ɹ���ȡ0)
        /// </summary>
        /// <param name="current">��ǰ����</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal<T>(this T current)
        {
            return current.ConvertToDecimal(0m);
        }

        /// <summary>
        /// �����ַ��ָ��ַ�����ȡ�ַ�����ǰ��λ��Ǻ���
        /// </summary>
        /// <param name="strValue">ԭʼ�ַ���</param>
        /// <param name="splitChar">���˷ָ���ַ�</param>
        /// <param name="isGetLeft">ȡ�ָ����ַ������滹������</param>
        /// <returns>�ָ��Ľ��</returns>
        public static string SubString(string strValue, char splitChar, bool isGetLeft)
        {
            if (isGetLeft == true) return strValue.Substring(0, strValue.LastIndexOf(splitChar));

            return strValue.Substring(strValue.LastIndexOf(splitChar) + 1);
        }

        public static string GetLongDate(string date, bool isEnd)
        {
            if (date.IsNullData()) return String.Empty;
            var year = date.Substring(0, 4).ConvertToInt();
            var month = date.Substring(date.Length - 2, 2);
            var seperate = date.Substring(date.Length - 3, 1);
            if (!isEnd) return date + seperate + "01";
            switch (month)
            {
                case "01":
                case "03":
                case "05":
                case "07":
                case "08":
                case "10":
                case "12":
                    return date + seperate + "31";
                case "02":
                    var isReap = ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0);
                    return date + seperate + (isReap ? "29" : "28");
                case "04":
                case "06":
                case "09":
                case "11":
                    return date + seperate + "30";
            }
            return String.Empty;
        }
    }
}