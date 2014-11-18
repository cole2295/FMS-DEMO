using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using RFD.Message;

namespace RFD.ServicesManager.Quartz
{
    public class QuartzManager
    {
        IScheduler _sched = null;

        public void Start()
        {
            try
            {
                if (null == _sched)
                {
                    NameValueCollection properties = new NameValueCollection();
                    properties["quartz.scheduler.instanceName"] = "XmlConfiguredInstance";
                    properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                    properties["quartz.threadPool.threadCount"] = "1";
                    properties["quartz.threadPool.threadPriority"] = "Normal";
                    properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.JobInitializationPlugin, Quartz";
                    properties["quartz.plugin.xml.fileNames"] = "~/JobConfig.xml";

                    ISchedulerFactory sf = new StdSchedulerFactory(properties);
                    _sched = sf.GetScheduler();
                }

                _sched.Start();

                MessageCollector.Instance.Collect(GetType(), "服务启动");
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect(GetType(), ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                if (null != _sched)
                {
                    _sched.Shutdown(true);
                }

                MessageCollector.Instance.Collect(GetType(), "服务停止");
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect(GetType(), ex.ToString());
            }
        }
    }
}
