﻿using System;
using System.Reflection;
using log4net;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Utility;

namespace Manufacturing.DataCollector.Datasources.Simulation
{
    public class RandomDatasource : IDatasource
    {
        public event EventHandler<DataReceivedEventArgs<decimal>> DataReceived;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly double _min;
        private readonly double _max;

        public int Id { get; set; }
        public string[] Schedule { get; set; }

        public RandomDatasource(DataCollectorConfiguration configuration)
        {
            _min = (double)configuration.RandomDatasourceMin;
            _max = (double)configuration.RandomDatasourceMax;

            Id = configuration.RandomDatasourceDeviceID;
            Schedule = configuration.RandomDatasourceSchedule;//configuration.RandomDatasourceIntervalSeconds
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
            if (evt != null)
            {
                Log.DebugFormat("Generated simuated data value {0} for datasource {1}", value, Id);
                evt(this, new DataReceivedEventArgs<decimal>(value, Id, DateTime.UtcNow));
            }
        }
    }
}
