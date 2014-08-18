using System;
using System.Collections.Generic;
using System.Linq;
using Manufacturing.Framework.Datasource;

namespace Manufacturing.DataCollector
{
    /// <summary>
    ///     Watches a set of IDatasource instances and aggregates their events
    /// </summary>
    public class DatasourceAggregator : IDatasource
    {
        private readonly List<IDatasource> _datasources;

        public DatasourceAggregator(IEnumerable<IDatasource> datasources)
        {
            _datasources = datasources.ToList();
            SubscribeToAllChildren();
        }

        private void SubscribeToAllChildren()
        {
            foreach (var datasource in _datasources)
                datasource.DataReceived += RaiseDataReceived;
        }

        private void RaiseDataReceived(object source, DataReceivedEventArgs<decimal> args)
        {
            var evt = DataReceived;
            if (evt != null)
                evt(source, args);
        }

        public event EventHandler<DataReceivedEventArgs<decimal>> DataReceived;

        public void StartRead()
        {
            foreach (var datasource in _datasources)
                datasource.StartRead();
        }
    }
}
