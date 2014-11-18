using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing.Imaging;
using System.IO;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.UserControl
{
	/// <summary>
	/// Validate ��ժҪ˵����
	/// </summary>
    public partial class Validate : BasePage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
		    ;
            if (CookieUtil.ExistCookie("timeStamp")&&!string.IsNullOrEmpty(Request["timeStamp"]))
            {
                System.TimeSpan span = DateTime.Parse(Request["timeStamp"].ToString()) -
                                       DateTime.Parse(CookieUtil.GetCookie("timeStamp"));
                if (span.TotalSeconds < 2)
                {
                    return;
                }
                CookieUtil.AddCookie("timeStamp", Request["timeStamp"]);
            }
		    string Num = RandNum(4);
            //if (CookieUtil.ExistCookie("ChangeValidate"))
            //{
            //    if (CookieUtil.GetCookie("ChangeValidate") == "NO" && CookieUtil.ExistCookie("Validate"))
            //    {
            //        Num = CookieUtil.GetCookie("Validate");
            //    }
            //}


            CookieUtil.AddCookie("Validate", Num);
		    CookieUtil.AddCookie("ChangeValidate","YES");
            ValidateCode(Num, 40, 20, "����", 10, "#FFFFFF");        

		}
        /// <summary>
        /// �÷�����������ָ��λ���������
        /// </summary>
        /// <param name="VcodeNum">�������������λ��</param>
        /// <returns>����һ��������ַ���</returns>
        private string RandNum(int VcodeNum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');//��ֳ�����
            string VNum = "";
            int temp = -1;//��¼�ϴ������ֵ�������ܱ�����������һ���������

            Random rand = new Random();
            //����һ���򵥵��㷨�Ա�֤����������Ĳ�ͬ
            for (int i = 0; i < VcodeNum; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }

                int t = rand.Next(VcArray.Length - 1);
                if (temp != -1 && temp == t)
                {
                    return RandNum(VcodeNum);

                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }

        /// <summary>
        /// ����ͼƬ��д���ַ�
        /// </summary>
        /// <param name="VNum">Ŀ���ַ�</param>
        /// <param name="w">��</param>
        /// <param name="h">��</param>
        /// <param name="font">�����ļ�</param>
        /// <param name="fontSize">�����С</param>
        /// <param name="bgColor">ͼƬ������ɫ</param>
        private void ValidateCode(string VNum, int w, int h, string font, int fontSize, string bgColor)
        {
            Bitmap Img = new Bitmap(w, h);//����ͼ���ʵ��
            Graphics g = Graphics.FromImage(Img);//��Img���������µ�Graphics����
            g.Clear(ColorTranslator.FromHtml(bgColor));//������ɫ
            Font f = new Font(font, fontSize);//����Font���ʵ��
            SolidBrush s = new SolidBrush(Color.Black);//���ɱ�ˢ���ʵ��
            g.DrawString(VNum, f, s, 3, 3);//��VNumд��ͼƬ��
            Random random = new Random();
            //��ͼƬ��ǰ�����ŵ�
            for (int i = 0; i < 50; i++)
            {
                int x = random.Next(Img.Width);
                int y = random.Next(Img.Height);
                Img.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, Img.Width - 1, Img.Height - 1);

            Img.Save(Response.OutputStream, ImageFormat.Jpeg);//����ͼ����Jpegͼ���ļ��ĸ�ʽ���浽����
            Response.ContentType = "image/Jpeg";
            //������Դ
            g.Dispose();
            Img.Dispose();
            Response.End();
        }


	}
}
