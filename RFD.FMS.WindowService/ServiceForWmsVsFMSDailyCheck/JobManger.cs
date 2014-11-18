using System;
using Quartz;
using Quartz.Impl;
using RFD.FMS.Util;

namespace ServiceForWmsVsFMSDailyCheck
{
    public class JobManager
    {
        public void OnStart()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            var job = new JobDetail("JobWmsVsFMSDaily", null, typeof(JobForWmsVsFMSDailyCheck));

            var trigger = TriggerUtils.MakeDailyTrigger("JobWmsVsFMSDaily",7,30);
            trigger.StartTimeUtc = DataConvert.ToDateTime("1999-06-28T18:15:00+02:00").Value;

            sched.ScheduleJob(job, trigger);

            sched.Start();
        }
    }
}