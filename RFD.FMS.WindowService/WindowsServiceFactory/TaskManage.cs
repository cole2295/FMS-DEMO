using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using System.Data;
using System.Configuration;
using System.Threading;
using WindowsServiceInterface;

namespace WindowsServiceFactory
{
    public class TaskManage
    {
        /// <summary>
        /// 文本文件记录日志对象
        /// </summary>
        protected readonly ILog Logger = AppConfig.GetFactoryLogger();
        protected readonly ILog LoggerEMail = AppConfig.GetFactoryEmail();
        /// <summary>
        /// 获取所有需要运行的WindowsService
        /// </summary>
        /// <returns></returns>
        public IList<TaskModel> GetAllRunServices()
        {
            string serviceFilePath = GetServicesConfigFilePath();

            //Logger.Error("WindowsService File Path:" + serviceFilePath);

            //读取配置文件信息到DataSet
            var ds = new DataSet();
            ds.ReadXml(serviceFilePath);
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                Logger.Info("服务启动...,共有任务数量：0，请进行配置后再重新启动");
                LoggerEMail.Info("服务启动...,共有任务数量：0，请进行配置后再重新启动");
            }

            List<TaskModel> windowsServiceDtos = new List<TaskModel>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                try
                {
                    TaskModel taskModel = new TaskModel();
                    taskModel.Id = row["ID"].ToString();
                    taskModel.Name = row["Name"].ToString();
                    taskModel.AssemblyName = row["AssemblyName"].ToString();
                    taskModel.ClassFullName = row["ClassFullName"].ToString();
                    taskModel.RunType = int.Parse(row["RunType"].ToString());
                    taskModel.IsRun = bool.Parse(row["IsRun"].ToString());
                    taskModel.FixTime = DateTime.Parse(row["FixTime"].ToString());
                    taskModel.Interval = int.Parse(row["Interval"].ToString());
                    taskModel.Method = row["Method"].ToString().ToUpper();
                    taskModel.topNum = row["topNum"].ToString();
                    taskModel.insertNum = int.Parse(row["insertNum"].ToString());
                    taskModel.SyncStartTime = row["SyncStartTime"].ToString();
                    taskModel.EmailNotify = bool.Parse(row["EmailNotify"].ToString());
                    taskModel.log = row["log"].ToString();
                    taskModel.email = row["email"].ToString();
                    windowsServiceDtos.Add(taskModel);
                }
                catch (Exception exception)
                {
                    Logger.Error("服务配置错误:"+exception.Message,exception);
                    LoggerEMail.Error("服务配置错误:" + exception.Message, exception);
                }
            }



            return windowsServiceDtos;
        }
        public void ServiceReflash(IList<TaskModel> list,Task task)
        {
            foreach (TaskModel taskModel in list)
            {
                if (taskModel.Id == task.taskModel.Id)
                {
                    task.ReFlash(taskModel);
                    list.Remove(taskModel);
                    break;
                }
            }
        }
        /// <summary>
        /// 获取所有服务所在配置文件路径
        /// </summary>
        /// <returns></returns>
        private  string GetServicesConfigFilePath()
        {
            string ServiceInstallPath = Thread.GetDomain().BaseDirectory.Trim();
            string serviceFilePath = ConfigurationManager.AppSettings["TasksFilePath"];
            return ServiceInstallPath + serviceFilePath;
        }
    }
}
