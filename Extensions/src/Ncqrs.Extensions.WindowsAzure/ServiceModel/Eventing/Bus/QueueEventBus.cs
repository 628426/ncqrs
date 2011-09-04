using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Ncqrs.Eventing.ServiceModel.Bus;


namespace Ncqrs.Extensions.WindowsAzure.Eventing.ServiceModel.Bus {
    /// <summary>
    /// Publishes registered events to Azure Queues
    /// </summary>
    public class QueueEventBus : HandlingBus {

        protected CloudStorageAccount _account = null;
        protected string _tableName = "NcqrsEvents";
        private string _queueName = "NcqrsQueue".ToLowerInvariant();
        private string _blobContainer = "NcqrsSnapshots".ToLowerInvariant();
        private static IList<string> _createdQueues = new List<string>();

        /// <summary>
        /// Creates a new Queue Event Bus
        /// </summary>
        /// <param name="account">The storage account</param>
        public QueueEventBus(CloudStorageAccount account) : this(account, null, null) {
        }
        /// <summary>
        /// Creates a new Queue Event Bus
        /// </summary>
        /// <param name="account">The storage account</param>
        /// <param name="tablePrefix">The storage table prefix</param>
        public QueueEventBus(CloudStorageAccount account, string tablePrefix) : this(account, tablePrefix, null) {

        }
        /// <summary>
        /// Creates a new Queue Event Bus
        /// </summary>
        /// <param name="account">The storage account</param>
        /// <param name="tablePrefix">The storage table prefix</param>
        /// <param name="queueName">The queue prefix</param>
        public QueueEventBus(CloudStorageAccount account, string tablePrefix, string queueName) {
            _account = account;
            _tableName = tablePrefix + _tableName;
            _queueName = queueName.ToLowerInvariant() + _queueName.ToLowerInvariant();
        }
        /// <summary>
        /// Publishes the event to the specified handlers
        /// </summary>
        /// <param name="eventMessage">The event to publish</param>
        /// <param name="eventMessageType">The event message type</param>
        /// <param name="handlers">The handlers</param>
        protected override void PublishToHandlers(IPublishableEvent eventMessage, Type eventMessageType, IEnumerable<Action<PublishedEvent>> handlers) {
            Enqueue(eventMessage);
        }

        protected CloudQueue GetQueue() {
            CloudQueueClient client = _account.CreateCloudQueueClient();
            if (!_createdQueues.Contains(_queueName)) {
                lock (_createdQueues) {
                    if (!_createdQueues.Contains(_queueName)) {
                        client.GetQueueReference(_queueName).CreateIfNotExist();
                        _createdQueues.Add(_queueName);
                    }
                }
            }

            return client.GetQueueReference(_queueName);
        }

        private void Enqueue(IPublishableEvent evnt) {


            CloudQueueMessage message = new CloudQueueMessage(
                Utility.Jsonize(new EnqueuedEvent() {
                    EventSourceId = evnt.EventSourceId,
                    EventVersion = evnt.EventSequence,
                    TableName = _tableName
                }, typeof(EnqueuedEvent)));

            GetQueue().AddMessage(message);        
        }
    }
}
