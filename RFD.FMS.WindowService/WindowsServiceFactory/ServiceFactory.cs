using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Timers;
using Common.Logging;
using WindowsServiceInterface;

namespace WindowsServiceFactory
{
    public partial class ServiceFactory : ServiceBase
    {
        public ServiceFactory()
        {
            InitializeComponent();
            //Thread.CurrentThread.IsBackground = true;
        }

        #region 变量
        TaskManage taskManage=new TaskManage();//任务参数获得
        IList<TaskModel> _taskModels = null;
        IList<Task> taskList = new List<Task>();
        System.Timers.Timer _timerRefiah;
        /// <summary>
        /// 文本文件记录日志对象
        /// </summary>
        protected readonly ILog Logger = AppConfig.GetFactoryLogger();
        /// <summary>
        /// 邮件
        /// </summary>
		protected readonly ILog LoggerEMail = AppConfig.GetFactoryEmail("WindowsService");
        #endregion


        #region
        protected override void OnStart(string[] args)
        {
			StringBuilder sbStr = new StringBuilder();
            _taskModels = taskManage.GetAllRunServices();
            foreach (TaskModel taskModel in _taskModels)
            {
                if (taskModel.IsRun)
                {
                    Task task = new Task(taskModel);
                    taskList.Add(task);
                    Thread thread = new Thread(new ThreadStart(task.Start));
                    thread.Start();
					sbStr.Append("服务【" + taskModel.Name + "】启动成功\r\n");
                }
            }
            _timerRefiah = new System.Timers.Timer();
            _timerRefiah.Enabled = true;
            _timerRefiah.Interval = 60000;
            _timerRefiah.Elapsed+=new ElapsedEventHandler(_timerRefiah_Elapsed);
			Logger.Info("如风达财务windows服务启动。IP：" + AppConfig.GetIP() + "\r\n" + sbStr.ToString());
			LoggerEMail.Info("如风达财务windows服务启动。IP：" + AppConfig.GetIP() + "\r\n" + sbStr.ToString());
        }
        /// <summary>
        ///任务刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timerRefiah_Elapsed(object sender, ElapsedEventArgs e)
        {
            _taskModels = taskManage.GetAllRunServices();
            foreach (Task task in taskList)
            {
                taskManage.ServiceReflash(_taskModels, task);
            }
            foreach (TaskModel taskModel in _taskModels)
            {
                if (taskModel.IsRun)
                {
                    Task task = new Task(taskModel);
                    taskList.Add(task);
                    Thread thread = new Thread(new ThreadStart(task.Start));
                    thread.Start();
                    Logger.Info("服务【" + taskModel.Name + "】启动成功。IP：" + AppConfig.GetIP());
                    LoggerEMail.Info("服务【" + taskModel.Name + "】启动成功。IP：" + AppConfig.GetIP());
                }
            }
        }

        protected override void OnStop()
        {
			StringBuilder sbStr = new StringBuilder();
            foreach (Task task in taskList)
            {
                task.Stop();
				sbStr.Append("服务【" + task.taskModel.Name + "】停止成功\r\n");
            }
			Logger.Info("如风达财务windows服务停止。IP：" + AppConfig.GetIP() + "\r\n" + sbStr.ToString());
			LoggerEMail.Info("如风达财务windows服务停止。IP：" + AppConfig.GetIP() + "\r\n" + sbStr.ToString());
			Thread.Sleep(10000);
        }
        #endregion
    }
}
