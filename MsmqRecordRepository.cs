using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Reflection;
using log4net;
using log4net.Repository;
using Manufacturing.Framework.Datasource;
using Manufacturing.Framework.Dto;
using Manufacturing.Framework.Utility;

namespace Manufacturing.DataCollector
{
    public class MsmqRecordRepository : ILocalRecordRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDatasourceRecordSerializer _recordSerializer;
        private readonly string _queueName;
        private static readonly object MsmqCreateMutex = new Object();
        private readonly DataCollectorConfiguration _dataCollectorConfiguration;

        public MsmqRecordRepository(IDatasourceRecordSerializer recordSerializer, DataCollectorConfiguration configuration)
        {
            _recordSerializer = recordSerializer;
            _queueName = string.Format(".\\private$\\{0}", configuration.MsmqQueueName.ToLower());
            _dataCollectorConfiguration = configuration;
        }

        public void Push(DatasourceRecord record)
        {
            Push(new List<DatasourceRecord> { record });
        }

        public void Push(IEnumerable<DatasourceRecord> records)
        {
            var queue = GetMessageQueue();

            //Log.Debug("Persisting message to the MSMQ service");
            using (var tx = new MessageQueueTransaction())
            {
                try
                {
                    tx.Begin();

                    foreach (var record in records)
                    {
                        using (var msg = new Message())
                        {
                            msg.BodyStream = _recordSerializer.Serialize(new List<DatasourceRecord> {record});
                            queue.Send(msg, tx);
                        }
                    }
                    tx.Commit();
                }
                catch (Exception)
                {
                    tx.Abort();
                    throw;
                }
            }

            //Log.Debug("Message persisted successfully");
        }

        private MessageQueue GetMessageQueue()
        {
            MessageQueue queue;
            lock (MsmqCreateMutex)
            {
                if (MessageQueue.Exists(_queueName))
                {
                    queue = new MessageQueue(_queueName);
                }
                else
                {
                    Log.InfoFormat("Creating message queue '{0}'", _queueName);
                    queue = MessageQueue.Create(_queueName, true);
                }
            }

            return queue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor">
        /// The Action that should be used to process the dequeued records. If
        /// an exception is thrown during processing, the transaction will be rolled back.
        /// </param>
        /// <param name="batchSize"></param>
        public void ProcessRecords(Action<IEnumerable<DatasourceRecord>> processor, int batchSize)
        {
            if (batchSize < 1)
                throw new ArgumentOutOfRangeException("batchSize", "batch size must be >= 1");

            var queue = GetMessageQueue();

            var tx = new MessageQueueTransaction();
            tx.Begin();

            var queueEnum = queue.GetMessageEnumerator2();
            var batch = new List<Message>();

            var keepProcessing = true;
            var queueHasItems = queueEnum.MoveNext();
            if (!queueHasItems)
                return;

            // TODO: This next line is occasionally getting an exception where the item has been removed before we get here
            var current = queueEnum.Current;
            do
            {
                //var start = DateTime.UtcNow; //for insturmentation

                if (current != null && batch.Count < batchSize)
                {
                    try
                    {
                        current = queueEnum.RemoveCurrent(tx);
                        batch.Add(current);
                    }
                    catch (MessageQueueException)
                    {
                        current = null; //No way to check for an empty queue, just have to eat this exception
                    }
                }
                else
                {
                    //We're either out of records, or we have our batch. Either way, process the batch.

                    var records = new List<DatasourceRecord>();
                    foreach (var msg in batch)
                    {
                        var deserialized = _recordSerializer.Deserialize(msg.BodyStream).ToList();
                        if(deserialized.Count > 1)
                            Log.Warn("You have a MSMQ message with multiple records. This is not supported and may bomb.");
                        records.AddRange(deserialized);
                    }

                    try
                    {
                        //Log.DebugFormat("Batched {0} MSMQ records in {1}ms", records.Count, (DateTime.UtcNow - start).TotalMilliseconds);
                        processor(records);
                    }
                    catch (Exception ex)
                    {
                        tx.Abort();
                        Log.Error("Record processor threw an exception, so the Msmq transaction was rolled back", ex);
                        return;
                    }
                    tx.Commit(); //We did a successful handoff, we don't need the messages any more

                    batch.Clear();

                    if (current != null)
                    {
                        //We need to start a new transaction for the next batch
                        tx = new MessageQueueTransaction();
                        tx.Begin();

                        try
                        {
                            current = queueEnum.RemoveCurrent(tx);
                            batch.Add(current);
                        }
                        catch (MessageQueueException)
                        {
                            //Empty queue, just eat it
                            current = null;
                        }
                    }

                    if (current == null)
                    {
                        keepProcessing = false;
                    }
                }

            } while (keepProcessing);
        }
    }
}
