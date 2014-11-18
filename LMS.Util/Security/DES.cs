using System;
using System.Security.Cryptography;
using System.Text;

namespace RFD.FMS.Util.Security
{
    /// <summary>
    /// 3DES����/����
    /// </summary>
    public class DES
    {
		private const string DES_KEY = "FDSFIojslsk;fjlk;)*(+nmjdsf$#@dsf54641#&*(()";

        /// <summary>
        /// 3des�����ַ���
        /// </summary>
        /// <param name="plainText">����</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>���ܺ󲢾�base63������ַ���</returns>
        /// <remarks>���أ�ָ�����뷽ʽ</remarks>
        public static string Encrypt3DES(string plainText, Encoding encoding)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;
            var DES = new
                TripleDESCryptoServiceProvider();
            var hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(encoding.GetBytes(DES_KEY));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = encoding.GetBytes(plainText);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock
                                              (Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// 3des�����ַ���
        /// </summary>
        /// <param name="plainText">����</param>
        /// <returns>���ܺ󲢾�base63������ַ���</returns>
        public static string Encrypt3DES(string plainText)
        {
            return Encrypt3DES(plainText, Encoding.Default);
        }

        /// <summary>
        /// 3des�����ַ���
        /// </summary>
        /// <param name="entryptText">����</param>
        /// <returns>���ܺ���ַ���</returns>
        public static string Decrypt3DES(string entryptText)
        {
            return Decrypt3DES(entryptText, Encoding.Default);
        }

        /// <summary>
        /// 3des�����ַ���
        /// </summary>
        /// <param name="entryptText">����</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>���ܺ���ַ���</returns>
        /// <remarks>��̬������ָ�����뷽ʽ</remarks>
        public static string Decrypt3DES(string entryptText, Encoding encoding)
        {
            var DES = new
                TripleDESCryptoServiceProvider();
            var hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(encoding.GetBytes(DES_KEY));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result;
            try
            {
                byte[] Buffer = Convert.FromBase64String(entryptText);
                result = encoding.GetString(DESDecrypt.TransformFinalBlock
                                                (Buffer, 0, Buffer.Length));
            }
            catch (Exception e)
            {
                throw (new Exception("Invalid Key or input string is not a valid base64 string", e));
            }

            return result;
        }
    }
}