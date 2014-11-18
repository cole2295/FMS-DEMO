using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using RFD.ServicesManager.Quartz;
using RFD.Sync.Impl;

namespace RFD.SyncData.WinService
{
    public partial class Service1 : ServiceBase
    {
        private QuartzManager _smgr = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Mail mail = new Mail();

            mail.SendMailToUser("服务启动","FMS同步数据服务已启动","gaopengxiang@vancl.cn,zengwei@vancl.cn");

            try
            {
                if (null == _smgr)
                {
                    _smgr = new QuartzManager();
                }

                _smgr.Start();
            }
            catch (Exception ex)
            {
                mail.SendMailToUser("服务启动异常", ex.Message, "gaopengxiang@vancl.cn,zengwei@vancl.cn");
            }
        }

        protected override void OnStop()
        {
            if (null != _smgr)
            {
                _smgr.Stop();
            }
        }
    }
}
