﻿using System.Collections.Generic;
using Bootstrap.StructureMap;
using Manufacturing.Framework.Datasource;
using StructureMap;

namespace Manufacturing.DataCollector
{
    public class DataCollectorContainer : IStructureMapRegistration
    {
        public void Register(IContainer container)
        {
            container.Configure(x => x.Scan(y =>
            {
                y.TheCallingAssembly();
                y.ExcludeType<DatasourceAggregator>();
                y.AddAllTypesOf<IDatasource>();
                y.SingleImplementationsOfInterface().OnAddedPluginTypes(z => z.LifecycleIs(InstanceScope.Unique));
            }));
        }

        public static IEnumerable<IDatasource> GetAllDatasources(IContainer container)
        {
            return container.GetAllInstances<IDatasource>();
        }

        public void Register(System.ComponentModel.IContainer container)
        {
        }
    }
}
