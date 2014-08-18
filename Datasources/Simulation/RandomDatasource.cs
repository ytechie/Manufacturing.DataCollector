using System;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Utility;

namespace Manufacturing.DataCollector.Datasources.Simulation
{
    public class RandomDatasource : IDatasource
    {
        public event EventHandler<DataReceivedEventArgs<decimal>> DataReceived;

        private readonly ITimer _timer;
        private readonly double _min;
        private readonly double _max;
        private readonly int _deviceID;

        public RandomDatasource(ITimer timer, DataCollectorConfiguration configuration)
        {
            _timer = timer;

            _timer.Tick += (sender, args) => StartRead();
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(configuration.RandomDatasourceIntervalSeconds));

            _min = (double)configuration.RandomDatasourceMin;
            _max = (double)configuration.RandomDatasourceMax;
            _deviceID = configuration.RandomDatasourceDeviceID;
        }
        
        public void StartRead()
        {
            var r = new Random();
            var d = r.NextDouble();
            var dec = (decimal)(_min + ((_max - _min) * d));
            RaiseDataReceivedEvent(dec);
        }

        private void RaiseDataReceivedEvent(decimal value)
        {
            var evt = DataReceived;
            if(evt != null)
                evt(this, new DataReceivedEventArgs<decimal>(value, _deviceID, DateTime.UtcNow));
        }
    }
}
