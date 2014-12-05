using System;
using System.Reflection;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Bootstrap.Extensions.StartupTasks;
using log4net;
using Manufacturing.DataCollector.DependencyResolution;
using StructureMap;

namespace Manufacturing.DataCollector.Api
{
    public class ApiHost : IStartupTask
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DataCollectorConfiguration _configuration;
        private readonly IContainer _controllerResolver;
        private static HttpSelfHostServer _server;

        public ApiHost(DataCollectorConfiguration configuration, IContainer controllerResolver)
        {
            _configuration = configuration;
            _controllerResolver = controllerResolver;
        }

        public void Run()
        {
            Log.Info("DataCollector API starting...");

            var baseAddress = new UriBuilder("http", "localhost", _configuration.Port);

            // Set up server configuration
            var config = new HttpSelfHostConfiguration(baseAddress.Uri)
            {
                DependencyResolver = new StructureMapWebApiDependencyResolver(_controllerResolver)
            };

            //This tells it to use Unity to create the controllers

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Create server
            _server = new HttpSelfHostServer(config);

            // Start listening
            Log.InfoFormat("Starting self-hosted API on {0}", baseAddress.Uri);
            try
            {
                _server.OpenAsync().Wait();
            }
            catch (AggregateException ae)
            {
                if (ae.InnerExceptions[0].GetType() == typeof (AddressAccessDeniedException))
                {
                    //People don't follow directions to get this up and running, so this will
                    //help them fix this exception.
                    throw new AddressAccessDeniedException("Unable to open the required selfhost port. Please make sure you ran the install script (https://github.com/ytechie/Manufacturing.DevRunner/blob/master/scripts/installDev.ps1)");    
                }

                throw;
            }
            
            Log.InfoFormat("API listening on {0}", baseAddress.Uri);
        }

        public void Reset()
        {
            Log.Info("Stopping API Host...");
            _server.CloseAsync().Wait();
            Log.Debug("API Host Stopped");
        }
    }
}
