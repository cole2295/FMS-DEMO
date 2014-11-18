using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;

namespace ServiceForSortingFee
{
    public partial class ServiceForSortingFee : ServiceBase
    {
        private readonly ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();

        /// <summary>
        /// 定义时间
        /// </summary>
       
        private System.Timers.Timer _timeEffect;
        private static bool isRuningEffect = false;
  
        public ServiceForSortingFee()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
       // public void OnStart()
        {
            StringBuilder sbService = new StringBuilder();
            try
            {
               
                sbService.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：");
                
                //生效服务
                if (Common.EffectIsRun)
                {
                    sbService.AppendLine("拣运生效服务");
                    _timeEffect = new System.Timers.Timer { Interval = Common.EffectInterval };
                    _timeEffect.Elapsed += EffectTimerElapsed;
                    _timeEffect.AutoReset = true;
                    _timeEffect.Enabled = true;
                }

                sbService.AppendLine("服务已启动");
                WriteText(sbService.ToString());
                Common.SendFailedMail(Common.SuccedSubject, sbService.ToString());
            }
            catch (Exception ex)
            {
                Common.SendFailedMail(Common.FailedSubject, ex.Message + ex.Source + ex.StackTrace);

                WriteText(ex.Message + ex.Source + ex.StackTrace);

                return;
            }
        }

        protected override void OnStop()
        {
            WriteText(Common.FailedSubject);

            Common.SendFailedMail(Common.FailedSubject, "拣运计算服务停止");
        }

        protected void EffectTimerElapsed(object sender, ElapsedEventArgs e)
        {

           // WriteText("Effect test"+DateTime.Now.ToString());

            if (Common.JudgeRunTime(Common.EffectRunHour))
                return;

            if (isRuningEffect) return;

            isRuningEffect = true;

            try
            {
               // string hmNow = DateTime.Now.ToString("HH:mm:ss");
                
                    //WriteText("Effect test");
                    string result =sortingFeeSrv.Dispposed(Common.RowCount);
                    if(!string.IsNullOrEmpty(result))
                    {
                        WriteText(result);
                    }
               
            }
            catch (Exception ex)
            {
                //WriteText("Effect test Exception");
                Common.SendFailedMail(Common.FailedSubject, ex.Message + "\n" + ex);
            }
            finally
            {
                isRuningEffect = false;
            }
        }

        public void WriteText(string tips)
        {
            string path = Application.StartupPath + "/日志";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fs = new FileStream(path + "/" + DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }
    }
}
