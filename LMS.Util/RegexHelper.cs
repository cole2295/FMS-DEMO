using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace LTP.Common
{
    public class RegexHelper
    {
        #region Replace

        public static MatchCollection Matches(string input, string pattern)
        {
            Regex rex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

            MatchCollection mcs = rex.Matches(input);

            return mcs;
        }

        public static string Replace(string input, string pattern,string replacement)
        {
            return Regex.Replace(input,pattern, replacement,RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// �滻Html ��:��link���� ����link��
        /// </summary>
        /// <param name="label">��ǩ����,��:a,br,table,title��;</param>
        /// <param name="input">Դ�ִ�</param>
        /// <param name="replacement">�滻��Ŀ���ִ�</param>
        /// <returns></returns>
        public static string ReplaceHtml(string label, string input, string replacement)
        {
            string pattern = string.Format("(<{0}[^>]*>)|</[^>]*{0}>", label);

            string res = Replace(input, pattern, replacement);

            return res.Replace("&nbsp;", " ");
        }

        /// <summary>
        /// �滻HTML��ǩ��������ǩ������,��:��link��XXX����XX ����link��
        /// </summary>
        /// <param name="label">��ǩ����,��:a,br,table,title��;</param>
        /// <param name="input">Դ�ִ�</param>
        /// <param name="replacement">�滻��Ŀ���ִ�</param>
        /// <returns></returns>
        public static string ReplaceHtmlLabel(string label, string input, string replacement)
        {
            string pattern = string.Format("(<{0}[^>]*>).*?</[^>]*{0}>", label);

            string res = Replace(input, pattern, replacement);

            return res;
        }

        #endregion

        #region MatchsHtml notes

        public static HtmlLinkLabel[] MatchLinks(string input)
        {
            string pattern = "<a[^>]+href=(\"(?<href>[^\"]*)\"|'(?<href>[^']*)'|(?<href>\\S*))[^>]*>(?<innerText>.*?)</a>";

            MatchCollection mc = Matches(input, pattern);

            HtmlLinkLabel[] arrLinks = new HtmlLinkLabel[mc.Count];

            for (int i = 0; i < mc.Count; i++)
            {
                Match m = mc[i];

                HtmlLinkLabel link = new HtmlLinkLabel();

                link.Href = m.Groups["href"].Value;

                link.InnerText = m.Groups["innerText"].Value;

                link.LabelText = m.Value;

                arrLinks[i] = link;
            }

            return arrLinks;
        }

        #endregion

        public static bool IsEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9] {1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\)?]$");
        }
    }

    public class HtmlLinkLabel
    {
        public string LabelText = string.Empty;
        public string InnerText = string.Empty;
        public string Href = string.Empty;
    }

}
