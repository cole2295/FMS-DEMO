using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using Common.Logging;

namespace WindowsServiceInterface
{
    /// <summary>
    /// 任务相关参数
    /// </summary>
    public class TaskModel
    {
        /// <summary>
        /// 服务Id唯一标识
        /// </summary>
        public string Id { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 类库名称
        /// </summary>
        public string AssemblyName { set; get; }

        /// <summary>
        /// 类全名
        /// </summary>
        public string ClassFullName { set; get; }

        /// <summary>
        /// 要运行的方法
        /// </summary>
        public string Method { set; get; }
        /// <summary>
        /// 是否启动运行
        /// </summary>
        public bool IsRun { set; get; }

        /// <summary>
        /// 0固定时间运行，1固定时间间隔
        /// </summary>
        public int RunType { set; get; }

        /// <summary>
        /// 运行间隔时间(以秒为单位)
        /// </summary>
        public int Interval { set; get; }


        /// <summary>
        /// 固定运行时间列表
        /// </summary>
        public DateTime FixTime { get; set; }


        /// <summary>
        /// 是否发邮件通知
        /// </summary>
        public bool EmailNotify { set; get; }


        ///<summary>
        /// 前多少条
        ///</summary>
        public string topNum { get; set; }


        ///<summary>
        /// 一次操作多少条
        ///</summary>
        public int insertNum { get; set; }

        ///<summary>
        /// 同步起始时间
        ///</summary>
        public string SyncStartTime { get; set; }

        private string _log = "default_Log";
        private string _email = "default_EMail";

        public string log
        {
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    _log = value;
                }
            }
        }
        public string email
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _email = value;
                }
            }
        }
        /// <summary>
        /// 日志
        /// </summary>
        public ILog Logger
        {
            get
            {
                return GetExceptionLogger(_log);
            }
        }

        /// <summary>
        /// 邮件
        /// </summary>
        public ILog Loggeremail
        {
            get
            {
                return GetExceptionLoggerEmail(_email);
            }
        }

        private ILog GetExceptionLogger(string log)
        {
            return LogManager.GetLogger(log);
        }
        private ILog GetExceptionLoggerEmail(string email)
        {
            return LogManager.GetLogger(email);
        }
        
    }
}
