using System;
using System.Reflection;
using Bootstrap.Extensions.StartupTasks;
using log4net;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;
using System.Linq;

namespace Manufacturing.DataCollector.Datasources
{
    public class DatasourceScheduler : IStartupTask, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IDatasourceReadResultHandler _readResultHandler;
        private IScheduler _scheduler;
        private IEnumerable<IDatasource> _datasources;
        private bool _disposed;

        public DatasourceScheduler(IEnumerable<IDatasource> datasources, IDatasourceReadResultHandler readResultHandler)
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _datasources = datasources;
            _readResultHandler = readResultHandler;
        }

        public void Run()
        {
            Log.Info("Starting Datasource Scheduler");

            // Go thru our datasources and schedule them based on their specified schedule
            foreach (var ds in _datasources.Where(ds => ds.Schedule != null && ds.Schedule.Length > 0))
            {
                // Thought could create a single job with multiple triggers, but Quartz doesn't seem to like that
                foreach (var schedStr in ds.Schedule)
                {
                    IJobDetail job = JobBuilder.Create<DatasourceReadJob>()
                    .WithIdentity("dsjob" + ds.Id + " " + schedStr, "datasources")
                    .Build();

                    job.JobDataMap.Add("Datasource", ds); // no method signature on JobBuilder allowing us to inject the ds there

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("dstrigger" + ds.Id + " " + schedStr, "datasources")
                        .StartNow()
                        .WithCronSchedule(schedStr)
                        .Build();

                    _scheduler.ScheduleJob(job, trigger);
                }
            }

            // Hook up the datasources to the handler (this doesnt quite seem like the right place for this...)
            foreach (var ds in _datasources)
            {
                ds.DataReceived += _readResultHandler.OnDataReceived;
            }

            // Handler is hooked up, start the scheduler
            _scheduler.Start();            
        }

        public void Reset()
        {
            Log.Info("Stopping Datasource Scheduler");
            _scheduler.Shutdown(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

         ~DatasourceScheduler()
        {
            Dispose(false);
        }

         protected virtual void Dispose(bool disposing)
         {
             if (_disposed)
                 return;

             _scheduler.Shutdown(false);
             
             _disposed = true;
         }
    }
}
