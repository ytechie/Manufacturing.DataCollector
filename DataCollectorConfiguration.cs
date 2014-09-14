namespace Manufacturing.DataCollector
{
    public class DataCollectorConfiguration
    {
        public int Port { get; set; }
        public string MsmqQueueName { get; set; }

        #region Datasource options that should probably be refactored somewhere else

        public decimal RandomDatasourceMin { get; set; }
        public decimal RandomDatasourceMax { get; set; }
        public int RandomDatasourceDeviceID { get; set; }
        public string[] RandomDatasourceSchedule { get; set; }

        public int CPUDeviceID { get; set; }
        public string[] CPUDeviceSchedule { get; set; }

        public double ItemProducedSampleRate { get; set; }
        public double ItemProducedTimeVariation { get; set; }
        public double ItemProducedMax { get; set; }
        public int ItemProducedDeviceID { get; set; }
        public string ItemProducedSchedule { get; set; }

        public double MaterialLoadedSampleRate { get; set; }
        public double MaterialLoadedTimeVariation { get; set; }
        public double MaterialLoadedMax { get; set; }
        public int MaterialLoadedDeviceID { get; set; }

        #endregion
    }
}
