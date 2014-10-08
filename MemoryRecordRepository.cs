using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Manufacturing.Framework.Dto;

namespace Manufacturing.DataCollector
{
    public class MemoryRecordRepository : ILocalRecordRepository
    {
        private static readonly List<DatasourceRecord> Records = new List<DatasourceRecord>();

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Push(DatasourceRecord record)
        {
            lock (Records)
            {
                Records.Add(record);
            }
        }

        public void Push(IEnumerable<DatasourceRecord> records)
        {
            lock (Records)
            {
                Records.AddRange(records);
            }
        }

        public void ProcessRecords(Action<IEnumerable<DatasourceRecord>> processor, int batchSize)
        {
            lock (Records)
            {
                var batch = Records.Take(batchSize);
                try
                {
                    processor(batch);
                }
                catch (Exception ex)
                {
                    Log.Error("Error processing record batch", ex);
                    throw;
                }
                
                Records.RemoveRange(0, Math.Min(batchSize, Records.Count));
            }
        }
    }
}
