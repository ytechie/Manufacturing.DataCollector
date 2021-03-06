﻿using System;
using Manufacturing.Framework.Datasource;

namespace Manufacturing.DataCollector
{
    public interface IDatasource
    {
        event EventHandler<DataReceivedEventArgs<decimal>> DataReceived;

        void StartRead();

        int Id { get; set; }
        string[] Schedule { get; set; }
    }
}
