using System;
using Quartz;
using Quartz.Impl;

namespace ServiceForStationDaily
{
    public class JobManager
    {
        public void OnStart()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();
            CreateStationDailyJob(ref sched);
            sched.Start();
        }
        /// <summary>
        /// 配送报表
        /// </summary>
        /// <param name="sched"></param>
        private static void CreateStationDailyJob(ref IScheduler sched)
        {
            var job = new JobDetail("JobNotifyStationDaily", null, typeof(JobForStationDaily));
            var trigger = TriggerUtils.MakeDailyTrigger(Common.StartHour, Common.StartMinute);
            trigger.Name = "NotifyTrigerStationDaily";
            trigger.StartTimeUtc = TriggerUtils.GetEvenSecondDateBefore(DateTime.UtcNow);
            sched.ScheduleJob(job, trigger);
        }
    }
}
