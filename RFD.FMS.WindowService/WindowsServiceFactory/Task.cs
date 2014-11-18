using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Reflection;
using System.Threading;
using Common.Logging;
using WindowsServiceInterface;

namespace WindowsServiceFactory
{
    /// <summary>
    /// 任务
    /// </summary>
    public class Task
    {
        #region 变量
        System.Timers.Timer _timer; //每个任务自己的timer
        public TaskModel taskModel; //每个任务配置
        public bool stop = true;
        /// <summary>
        /// 文本文件记录日志对象
        /// </summary>
        protected readonly ILog Logger = AppConfig.GetFactoryLogger();
        /// <summary>
        /// 邮件
        /// </summary>
        protected readonly ILog LoggerEMail = AppConfig.GetFactoryEmail("WindowsService");
        #endregion

        public Task(TaskModel wsDTO)
        {
            this.taskModel = wsDTO;
        }

        #region 方法
        /// <summary>
        /// 刷新函数，计算timer下次运行时间
        /// </summary>
        /// <param name="taskModel"></param>
        public void ReFlash(TaskModel taskModel)
        {
            //Logger.Info("服务:" + taskModel.Name + "刷新开始！");
            if (!taskModel.IsRun)//服务停止
            {
                _timer.Enabled = false;
                return;
            }
            //Logger.Info("服务:" + taskModel.Name + "刷新结束！");
            if (taskModel.RunType != this.taskModel.RunType)//运行类型不同时
            {
                if (taskModel.RunType == 0)
                {
                    _timer.Interval = taskModel.Interval;
                }
                else
                {
                    //计算下次运行时间
                    TimeSpan sp = taskModel.FixTime - System.DateTime.Now;
                    if (sp.TotalMilliseconds < 0)
                    {
                        sp = taskModel.FixTime.AddDays(1) - System.DateTime.Now;
                    }
                    _timer.Interval = sp.TotalMilliseconds;
                }
                this.taskModel = taskModel;
                return;
            }
            if (taskModel.RunType == 0)
            {
                if (taskModel.Interval != this.taskModel.Interval)
                {
                    _timer.Interval = taskModel.Interval;
                }
            }
            else
            {
                if (taskModel.FixTime != this.taskModel.FixTime)
                {
                    TimeSpan sp = taskModel.FixTime - System.DateTime.Now;
                    if (sp.TotalMilliseconds < 0)
                    {
                        sp = taskModel.FixTime.AddDays(1) - System.DateTime.Now;
                    }
                    _timer.Interval = sp.TotalMilliseconds;
                }
            }
            if(_timer.Enabled ==false)
            {
                _timer.Enabled = true;
                Logger.Info("服务:" + taskModel.Name + "启动！");
            }
           
            this.taskModel = taskModel;
        }
        /// <summary>
        /// 任务停止
        /// </summary>
        public void Stop()
        {
            this._timer.Enabled = false;
        }
        /// <summary>
        /// 任务开始
        /// </summary>
        public void Start()
        {
            _timer = new System.Timers.Timer();
            if (taskModel.RunType == 0)
            {
                _timer.Interval = taskModel.Interval;
            }
            else
            {
                TimeSpan sp = taskModel.FixTime - System.DateTime.Now;
                if (sp.TotalMilliseconds < 0)
                {
                    sp = taskModel.FixTime.AddDays(1) - System.DateTime.Now;
                }
                _timer.Interval = sp.TotalMilliseconds;
            }
            _timer.Enabled = true;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
        }
        /// <summary>
        /// timer elapsed 事件触发方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.stop == false)
            {
                return;
            }
            this.stop = false;
            Logger.Info("服务:" + taskModel.Name + "开始！");
            try
            {
                IService service = Assembly.Load(taskModel.AssemblyName).CreateInstance(taskModel.ClassFullName) as IService;
                if (service == null)
                {
                    Logger.Error("服务:" + taskModel.Name + "反射失败！");//写日志
					LoggerEMail.Error("服务:" + taskModel.Name + "反射失败！");
                    return;
                }
                service.DealDetail(taskModel);
            }
            catch (Exception ex)
            {
                Logger.Error("服务:" + taskModel.Name + "异常:" + ex.Message);//写日志
				LoggerEMail.Error("服务:" + taskModel.Name + "异常:" + ex.Message + "\n" + ex);
                //写日志
            }
            finally
            {
                if (taskModel.RunType == 1)
                {
                    TimeSpan sp = taskModel.FixTime - System.DateTime.Now;
                    if (sp.TotalMilliseconds < 0)
                    {
                        sp = taskModel.FixTime.AddDays(1) - System.DateTime.Now;
                    }
                    _timer.Interval = sp.TotalMilliseconds;
                }
                Logger.Info("服务:" + taskModel.Name + "完成！");
                this.stop = true;
            }
        }
        #endregion
    }
}
