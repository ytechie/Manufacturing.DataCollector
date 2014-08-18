using System;
using System.Reflection;
using Bootstrap.Extensions.StartupTasks;
using log4net;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;

namespace Manufacturing.DataCollector.Datasources
{
    public class DatasourceScheduler : IStartupTask
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private DatasourceAggregator _aggregator;
        private ILocalRecordRepository _recordRepository;

        public DatasourceScheduler(DatasourceAggregator aggregator, ILocalRecordRepository recordRepository)
        {
            _aggregator = aggregator;
            _recordRepository = recordRepository;
        }

        public void Run()
        {
            Log.Info("Starting Datasource Scheduler");

            _aggregator.DataReceived += AggregatorOnDataReceived;
        }

        private void AggregatorOnDataReceived(object sender, DataReceivedEventArgs<decimal> dataReceivedEventArgs)
        {
            var record = new DatasourceRecord
            {
                Timestamp = dataReceivedEventArgs.Timestamp,
                DatasourceId = dataReceivedEventArgs.DeviceID,
            };
            record.SetDecimalValue(dataReceivedEventArgs.Value);
            _recordRepository.Push(record);
        }

        public void Reset()
        {
            Log.Info("Stopping Datasource Scheduler");
        }
    }
}
