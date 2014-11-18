using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Common.Logging;
using System.Configuration;
using RFD.FMS.Util;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.WEBLOGIC.Mail;
using RFD.FMS.DAL.FinancialManage;

namespace ServiceForRepairDedust
{
    public partial class ServiceForRepairDedust : ServiceBase
    {
        public ServiceForRepairDedust()
        {
            InitializeComponent();
        }

        private Timer _time;

        //public void OnStart()
        protected override void OnStart(string[] args)
        {
            Common.SendFailedMail(Common.FailedSubject, "提成数据修复服务启动");

            try
            {
                double sleeptime = Common.ServiceInterval;
                _time = new Timer { Interval = sleeptime };//实例化Timer类，设置间隔时间为10000毫秒； 
                _time.Elapsed += TimerElapsed;//到达时间的时候执行事件； 
                _time.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                _time.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 
            }
            catch (Exception ex)
            {
                return;
            }
        }

        protected override void OnStop()
        {
            Common.SendFailedMail(Common.FailedSubject, "提成数据修复服务停止");
        }

        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            RepairDedust();
        }

        public void RepairDedust()
        {
            try
            {
                FMS_DeductBaseInfoDao fMS_DeductBaseInfoDao = new FMS_DeductBaseInfoDao();

                IList<long> waybillNos = null;

                //Mail mail = new Mail();

                //删除重复计算的提成
                //waybillNos = fMS_DeductBaseInfoDao.CheckRepeatDeduct();

                //if (waybillNos.Count != 0)
                //{
                //    mail.SendMailToUser("提成重复计算提示", "取消重复计算的提成：" + DataConvert.ToDbIds(waybillNos), "gaopengxiang@vancl.cn;");
                //}

                ////提示没有计算的提成
                //waybillNos = fMS_DeductBaseInfoDao.ClearNotEvalDeduct();

                //if (waybillNos.Count != 0)
                //{
                //    mail.SendMailToUser("没有计算提成提示", "运单没有计算提成：" + DataConvert.ToDbIds(waybillNos), "gaopengxiang@vancl.cn;");
                //}

                //快递单已删除取消取件提成
                waybillNos = fMS_DeductBaseInfoDao.RepairExpressDelete();

                //if (waybillNos.Count != 0)
                //{
                //    mail.SendMailToUser("快递单删除取消取件提成提示", "无效快递单取消提成：" + DataConvert.ToDbIds(waybillNos), "gaopengxiang@vancl.cn;");
                //}

                ////修复快递时间
                waybillNos = fMS_DeductBaseInfoDao.RepairExpressDate();

                //if (waybillNos.Count != 0)
                //{
                //    mail.SendMailToUser("取件提成时间修复提示", "修复取件提成时间：" + DataConvert.ToDbIds(waybillNos), "gaopengxiang@vancl.cn;");
                //}

                //waybillNos = fMS_DeductBaseInfoDao.RepairFalseDelete();

                //if (waybillNos.Count != 0)
                //{
                //    mail.SendMailToUser("提成错误删除修复提示", "修复错误删除提成：" + DataConvert.ToDbIds(waybillNos), "gaopengxiang@vancl.cn;");
                //}
            }
            catch (Exception ex)
            {
                Common.SendFailedMail("提成数据修复服务异常", ex.Message);

                return;
            }
        }
    }
}