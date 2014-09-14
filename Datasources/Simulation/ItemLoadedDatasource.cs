using System;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Utility;

namespace Manufacturing.DataCollector.Datasources.Simulation
{
    public class ItemLoadedDatasource : IDatasource
    {
        public event EventHandler<DataReceivedEventArgs<decimal>> DataReceived;

        private readonly ITimer _timer;
        private readonly double _variation;
        private readonly double _rate;
        private readonly double _maxitems;
        private Random _rand;

        public int Id { get; set; }
        public string[] Schedule {  get; set; }

        public ItemLoadedDatasource(ITimer timer, DataCollectorConfiguration configuration)
        {
            _timer = timer;

            _variation = configuration.ItemProducedTimeVariation;
            _rate = configuration.ItemProducedSampleRate;
            _rand = new Random();
            _maxitems = configuration.ItemProducedMax;

            _timer.Tick += (sender, args) => StartRead();

            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_rate)
                + TimeSpan.FromSeconds((_rand.NextDouble() - 0.5) * _variation));

            Id = configuration.ItemProducedDeviceID;
        }
        
        public void StartRead()
        {
            decimal itemsProduced = (decimal) (_rand.NextDouble() * (_maxitems + (10 * (_rand.Next(0, 10) - 5))));

            RaiseDataReceivedEvent(itemsProduced);
            var period = TimeSpan.FromSeconds(_rate)
                 + TimeSpan.FromSeconds((_rand.NextDouble() - 0.5) * _variation);
            _timer.Change(period, period);
        }

        private void RaiseDataReceivedEvent(decimal value)
        {
            var evt = DataReceived;
            if(evt != null)
                evt(this, new DataReceivedEventArgs<decimal>(value, Id, DateTime.UtcNow));
        }
    }
}
