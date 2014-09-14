using System;
using System.Collections.Generic;
using System.Linq;
using Manufacturing.Framework.Datasource;

namespace Manufacturing.DataCollector
{
    /// <summary>
    ///     Watches a set of IDatasource instances and aggregates their events
    /// </summary>
    /// 
    /// This isnt included in the flow right now, as dont think we want each individual datasource
    /// result to be exposed, as it causes double recordings
    /// Not sure if this is neally needed for anything going forward or was just initial mechanism to
    /// do reads before scheduling was implemented.  So leaving it around for now.
    public class DatasourceAggregator : IDatasource
    {
        private readonly List<IDatasource> _datasources;

        public int Id { get; set; }
        public string[] Schedule { get; set; }

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
