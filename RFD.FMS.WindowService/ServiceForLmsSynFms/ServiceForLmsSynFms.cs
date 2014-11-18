using System;
using System.Data;
using System.ServiceProcess;
using System.Configuration;
using System.IO;
using System.Timers;
using RFD.FMS.Util;
using RFD.FMS.Service.COD;

namespace ServiceForLmsSynFms
{
    public partial class ServiceForLmsSynFms : ServiceBase
    {
        public ServiceForLmsSynFms()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 定义时间
        /// </summary>
        private Timer _time;

        private Timer _time2;
        private Timer _time3;
        private static bool _isRuning = false;
        private static bool _isRuning2 = false;
        private static bool _isRuning3 = false;

        //public void OnStart()
        protected override void OnStart(string[] args)
        {
            try
            {
                Common.SendMail(Common.SuccessSubject, Common.StartSubject);
                double sleeptime = Common.ServiceInterval;
                _time = new Timer {Interval = sleeptime}; //实例化Timer类，设置间隔时间为10000毫秒； 
                _time.Elapsed += TimerElapsed; //到达时间的时候执行事件； 
                _time.AutoReset = true; //设置是执行一次（false）还是一直执行(true)； 
                _time.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；

                double sleeptime2 = Common.Service2Interval;
                _time2 = new Timer { Interval = sleeptime2 };
                _time2.Elapsed += TimerElapsed2;
                _time2.AutoReset = true;
                _time2.Enabled = true;

                double sleeptime3 = Common.Service3Interval;
                _time3 = new Timer { Interval = sleeptime3 };
                _time3.Elapsed += TimerElapsed3;
                _time3.AutoReset = true;
                _time3.Enabled = true;

            }
            catch (Exception ex)
            {
                Common.SendMail(Common.FailSubject, ex.Message + ex.Source + ex.StackTrace);

                return;
            }
        }

        protected override void OnStop()
        {
            Common.SendMail(Common.FailSubject, Common.StopSubject);
        }

        public string ChangeHtml(string[] con)
        {
            string htm = "<tr>";
            foreach (var s in con)
            {
                htm += ("<td bgcolor=\"yellow\" style=\"text-align:center;\">" + s + "</td>");
            }
            htm += "</tr>";
            return htm;
        }

        private void WriteTextForShip(string tips)
        {
            var fs = new FileStream(
                ConfigurationManager.AppSettings["BakFilePath"] + "/日志/" +
                DateTime.Now.ToString("yyyy年MM月dd") + "Ship.txt",
                FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            //写入日志的内容为tips
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }

        private void WriteTextForBack(string tips)
        {
            var fs = new FileStream(
                ConfigurationManager.AppSettings["BakFilePath"] + "/日志/" +
                DateTime.Now.ToString("yyyy年MM月dd") + "Back.txt",
                FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            //写入日志的内容为tips
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }



        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isRuning)
            {
                return;
            }
            _isRuning = true;

            try
            {
                var cod = ServiceLocator.GetService<IFMS_CODService>();
                var tip = cod.LmsSynFmsForShip(ConfigurationManager.AppSettings["TopNumForShip"], " 0 ");
                var tip2=cod.SynForDeliverTimeAndOutBountStation();
                //WriteTextForShip(tip);
                //if (tip.StartsWith("B") || tip.StartsWith("C") || tip.StartsWith("D"))
                if (tip.StartsWith("C") || tip.StartsWith("D"))
                {
                    Common.SendMail("财务数据同步报告", tip);
                }
                if (tip2.StartsWith("K") || tip2.StartsWith("J") || tip2.StartsWith("L"))
                {
                    Common.SendMail("财务数据同步报告", tip2);
                }
                //WriteTextForShip(tip2);
            }
            catch (Exception ex)
            {
                Common.SendMail(Common.FailSubject, DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
            finally
            {
                _isRuning = false;
            }
            
        }

        public void TimerElapsed2(object sender, ElapsedEventArgs e)
        {
            if (_isRuning2)
            {
                return;
            }
            _isRuning2 = true;

            try
            {
                var cod = ServiceLocator.GetService<IFMS_CODService>();
                var tip = cod.LmsSynFmsForBack(ConfigurationManager.AppSettings["TopNumForBack"], " 0 ");
                //WriteTextForBack(tip);
                //if (tip.StartsWith("F") || tip.StartsWith("G") || tip.StartsWith("I") || tip.StartsWith("H"))
                if (tip.StartsWith("F") || tip.StartsWith("I") || tip.StartsWith("H"))
                {
                    Common.SendMail("财务数据同步报告", tip);
                }
            }
            catch (Exception ex)
            {
                Common.SendMail(Common.FailSubject, DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
            finally
            {
                _isRuning2 = false;
            }
            
        }

        public void TimerElapsed3(object sender, ElapsedEventArgs e)
        {
            if (_isRuning3)
            {
                return;
            }
            _isRuning3 = true;

            try
            {
                var perNum = Convert.ToInt32(ConfigurationManager.AppSettings["IsSyn"]);
                var counter = 0;
                var cod = ServiceLocator.GetService<IFMS_CODService>();
                var ids2 =
                    cod.SearchAnyInfoOnMedium(
                        " SELECT  ID FROM LMS_RFD.dbo.LMS_Syn_FMS_COD syn (nolock) where syn.IsSyn = 2 and syn.StationID>0    ");
                if (!string.IsNullOrEmpty(ids2))
                {
                    string[] id = ids2.Split(',');
                    counter = id.Length/perNum;
                    for (int i = 0; i < counter; i++)
                    {
                        string minIds = "";
                        for (int j = perNum*i; j < perNum*(i + 1); j++)
                        {
                            minIds += ("," + Convert.ToInt64(id[j]));
                        }
                        minIds = minIds.Substring(1);
                        cod.UpDateMediumForSyn(minIds, 0);
                    }
                    if (id.Length%perNum != 0)
                    {
                        string minIds = "";
                        for (int k = perNum*counter; k < id.Length; k++)
                        {
                            minIds += ("," + Convert.ToInt64(id[k]));
                        }
                        minIds = minIds.Substring(1);
                        cod.UpDateMediumForSyn(minIds, 0);
                    }
                }

            }
            catch (Exception ex)
            {
                Common.SendMail(Common.FailSubject, DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
            finally
            {
                _isRuning3 = false;
            }
            
        }



      
    }
}
              
        
    

