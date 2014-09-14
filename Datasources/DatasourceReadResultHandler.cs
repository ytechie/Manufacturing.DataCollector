using log4net;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Manufacturing.DataCollector.Datasources
{
    public interface IDatasourceReadResultHandler
    {
        void OnDataReceived(object sender, DataReceivedEventArgs<decimal> dataReceivedEventArgs);
    }

    public class DatasourceReadResultHandler : IDatasourceReadResultHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ILocalRecordRepository _recordRepository;

        public DatasourceReadResultHandler(ILocalRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public void OnDataReceived(object sender, DataReceivedEventArgs<decimal> dataReceivedEventArgs)
        {
            var record = new DatasourceRecord
            {
                Timestamp = dataReceivedEventArgs.Timestamp,
                DatasourceId = dataReceivedEventArgs.DeviceID,
            };
            record.SetDecimalValue(dataReceivedEventArgs.Value);            
            _recordRepository.Push(record);
        }
    }
}
