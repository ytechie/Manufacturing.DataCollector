using System;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Utility;
using System.Diagnostics;

namespace Manufacturing.DataCollector.Datasources.Simulation
{
    public class CPUDatasource : IDatasource
    {
        public event EventHandler<DataReceivedEventArgs<decimal>> DataReceived;

        private readonly ITimer _timer;
        private readonly double _min;
        private readonly double _max;
        private readonly int _deviceid;
        private readonly double _CPUSampleRate;

        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public CPUDatasource(ITimer timer, DataCollectorConfiguration configuration)
        {
            _timer = timer;

            _timer.Tick += (sender, args) => StartRead();
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(configuration.CPUSampleRateSeconds));
            _deviceid = configuration.CPUDeviceID;
        
            
        }
        
        public void StartRead()
        {

            decimal usage = (decimal)cpuCounter.NextValue();

            RaiseDataReceivedEvent(usage);
        }

        private void RaiseDataReceivedEvent(decimal value)
        {
            var evt = DataReceived;
            if(evt != null)
                evt(this, new DataReceivedEventArgs<decimal>(value, _deviceid, DateTime.UtcNow));
        }
    }
}
