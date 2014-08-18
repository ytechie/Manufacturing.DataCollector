using System;
using System.Collections.Generic;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;

namespace Manufacturing.DataCollector
{
    public interface ILocalRecordRepository
    {
        void Push(DatasourceRecord record);

        void Push(IEnumerable<DatasourceRecord> records);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor">
        /// The method that will process the <see cref="DatasourceRecord"/> list. If this call completes successfully, the
        /// records will be removed from the repository. Throw an exception if you do not have reliable storage of the
        /// received records.
        /// </param>
        /// <param name="batchSize"></param>
        void ProcessRecords(Action<IEnumerable<DatasourceRecord>> processor, int batchSize);
    }
}
