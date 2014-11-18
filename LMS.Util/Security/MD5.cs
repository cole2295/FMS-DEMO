using System;
using System.Security.Cryptography;
using System.Text;

namespace RFD.FMS.Util.Security
{
    /// <summary>
    /// MD5����
    /// </summary>
    public class MD5
    {
        /// <summary>
        /// �ַ���MD5���ܣ����ش�д��ĸ
        /// </summary>
        /// <param name="plainText">����</param>
        /// <returns>���ģ���д��ĸ��</returns>
        public static string Encrypt(string plainText)
        {
            MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
            byte[] b = md.ComputeHash(Encoding.Default.GetBytes(plainText));
            string s = string.Empty;
            for (int i = 0; i < b.Length; i++)
            {
                s += (b[i].ToString("x2"));
            }
            return s.ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static String GetMD5Str(String info)
        {
            try
            {
                byte[] res = System.Text.Encoding.Default.GetBytes(info);
                MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
                byte[] result = md.ComputeHash(res);
                byte[] hash = md.ComputeHash(result);
                StringBuilder sbuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    int v = hash[i] & 0xFF;
                    if (v < 16) sbuilder.Append("0");
                    sbuilder.Append(Convert.ToString(v, 16).ToUpper());
                }
                return sbuilder.ToString();
            }
            catch
            {
                return null;
            }
        }
    }

}