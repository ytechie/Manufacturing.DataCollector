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
        private readonly double _CPUSampleRate;


        public int Id { get; set; }
        public string Schedule { get; set; }

        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public CPUDatasource(DataCollectorConfiguration configuration)
        {
            Id = configuration.CPUDeviceID;
            Schedule = configuration.CPUDeviceSchedule; //configuration.CPUSampleRateSeconds
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
                evt(this, new DataReceivedEventArgs<decimal>(value, Id, DateTime.UtcNow));
        }
    }
}
