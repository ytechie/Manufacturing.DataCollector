using System.Reflection;
using System.ServiceProcess;
using Bootstrap;
using Bootstrap.StructureMap;
using log4net;
using Manufacturing.DataCollector.Api;
using StructureMap;

namespace Manufacturing.DataCollector
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IContainer _container;
        private ApiHost _apiHost;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Loading Configuration...");
            Bootstrapper
                .With.StructureMap()
                .LookForTypesIn.ReferencedAssemblies().Including.Assembly(Assembly.GetAssembly(typeof(DataCollectorContainer)))
                .Start();

            _container = (IContainer)Bootstrapper.Container;

            Log.Info("Starting DataCollector...");


            Log.Info("Starting DataCollector API...");
            _apiHost = _container.GetInstance<ApiHost>();
            _apiHost.Run();
        }

        protected override void OnStop()
        {
            _apiHost.Reset();
        }
    }
}
