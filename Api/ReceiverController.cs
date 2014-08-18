using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;

namespace Manufacturing.DataCollector.Api
{
    public class ReceiverController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ILocalRecordRepository _recordRepository;

        public ReceiverController(ILocalRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        // GET api/receiver
        public string Get()
        {
            return "Success";
        }

        /// <summary>
        ///     Receives a value posted to it, and saves that value to a local MessageQueue
        /// </summary>
        /// <param name="value"></param>
        public HttpResponseMessage Post([FromBody]DatasourceRecord value)
        {
            Log.DebugFormat("Received record ID '{0}'", value.DatasourceId);
            _recordRepository.Push(value);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        public HttpResponseMessage Post(int dummyRecords)
        {
            Log.DebugFormat("Queueing {0} dummy records", dummyRecords);

            var records = RandomDatasourceRecordGenerator.GenerateDummyData(dummyRecords);
            _recordRepository.Push(records);

            Log.Debug("Records queued");

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
