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
	/// Validate 的摘要说明。
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
            ValidateCode(Num, 40, 20, "黑体", 10, "#FFFFFF");        

		}
        /// <summary>
        /// 该方法用于生成指定位数的随机数
        /// </summary>
        /// <param name="VcodeNum">参数是随机数的位数</param>
        /// <returns>返回一个随机数字符串</returns>
        private string RandNum(int VcodeNum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');//拆分成数组
            string VNum = "";
            int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数

            Random rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同
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
        /// 生成图片并写入字符
        /// </summary>
        /// <param name="VNum">目标字符</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="font">字体文件</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="bgColor">图片背景颜色</param>
        private void ValidateCode(string VNum, int w, int h, string font, int fontSize, string bgColor)
        {
            Bitmap Img = new Bitmap(w, h);//生成图像的实例
            Graphics g = Graphics.FromImage(Img);//从Img对象生成新的Graphics对象
            g.Clear(ColorTranslator.FromHtml(bgColor));//背景颜色
            Font f = new Font(font, fontSize);//生成Font类的实例
            SolidBrush s = new SolidBrush(Color.Black);//生成笔刷类的实例
            g.DrawString(VNum, f, s, 3, 3);//将VNum写入图片中
            Random random = new Random();
            //画图片的前景干扰点
            for (int i = 0; i < 50; i++)
            {
                int x = random.Next(Img.Width);
                int y = random.Next(Img.Height);
                Img.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, Img.Width - 1, Img.Height - 1);

            Img.Save(Response.OutputStream, ImageFormat.Jpeg);//将此图像以Jpeg图像文件的格式保存到流中
            Response.ContentType = "image/Jpeg";
            //回收资源
            g.Dispose();
            Img.Dispose();
            Response.End();
        }


	}
}
