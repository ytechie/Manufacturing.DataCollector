using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manufacturing.DataCollector.Datasources
{
    public class DatasourceReadJob: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var map = context.JobDetail.JobDataMap;
            var dataSource = (IDatasource)map.Get("Datasource");

            dataSource.StartRead();
        }
    }
}
