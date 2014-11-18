using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using log4net;

namespace RFD.FMS.Util
{
    public class LogHelper
    {
        private string logType;

        public LogHelper()
        {

        }
        public LogHelper(string logType)
        {
            this.logType = logType;
        }
        public LogHelper(string logType, string dicName)
        {
            this.logType = logType;
            this.dicName = dicName;
        }

        private string _dicName;
        public string dicName
        {
            get
            {
                if (_dicName == null)
                    _dicName = HttpRuntime.AppDomainAppPath + "log\\";
                return _dicName;
            }
            set
            {
                _dicName = value;
            }
        }
        public void WriteTest(string tips, string logType)
        {
            try
            {
                var dicStr = dicName + "/" + logType + "/";
                if (!Directory.Exists(dicStr))
                {
                    Directory.CreateDirectory(dicStr);
                }
                var fs =
                    new FileStream(
                        dicStr + DateTime.Now.ToString("yyyy年MM月dd") +
                        "logData.txt", FileMode.OpenOrCreate, FileAccess.Write);
                var mStreamWriter = new StreamWriter(fs);
                mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                mStreamWriter.WriteLine(tips);
                mStreamWriter.Flush();
                mStreamWriter.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public void WriteTest(string tips)
        {
            WriteTest(tips, this.logType);
        }
        public void ErrorFormat(string tips, params object[] prams)
        {
            WriteTest(string.Format("<-----" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + tips + "----->", prams), this.logType);
        }
        public void WriteException(Exception ex)
        {
            string s = string.Format("异常信息：{0},source:{1},trace:{2}", ex.Message, ex.Source, ex.StackTrace);
            if (ex.InnerException != null)
            {
                s += string.Format("内部异常：{0},", ex);
            }
            WriteTest(s);

        }

        private ILog _log;

        public ILog Log
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }
    }
}