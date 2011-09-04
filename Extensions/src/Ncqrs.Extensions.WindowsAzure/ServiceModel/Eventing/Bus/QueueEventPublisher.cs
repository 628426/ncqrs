using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Ncqrs.Eventing.ServiceModel.Bus;
using Microsoft.WindowsAzure.StorageClient;
using Ncqrs.Extensions.WindowsAzure.Events.Storage;
using Ncqrs.Eventing;

namespace Ncqrs.Extensions.WindowsAzure.Eventing.ServiceModel.Bus {
    public class QueueEventPublisher : QueueEventBus {
         /// <summary>
        /// Creates a new Queue Event Bus
        /// </summary>
        /// <param name="account">The storage account</param>
        public QueueEventPublisher(CloudStorageAccount account) : base(account, null, null) {
        }
        /// <summary>
        /// Creates a new Queue Event Bus
        /// </summary>
        /// <param name="account">The storage account</param>
        /// <param name="tablePrefix">The storage table prefix</param>
        public QueueEventPublisher(CloudStorageAccount account, string tablePrefix) : base(account, tablePrefix, null) {

        }
        /// <summary>
        /// Creates a new Queue Event Bus
        /// </summary>
        /// <param name="account">The storage account</param>
        /// <param name="tablePrefix">The storage table prefix</param>
        /// <param name="queueName">The queue prefix</param>
        public QueueEventPublisher(CloudStorageAccount account, string tablePrefix, string queueName) : base(account, tablePrefix, queueName) {
        }
        /// <summary>
        /// Publishes the event to the specified handlers
        /// </summary>
        /// <param name="eventMessage">The event to publish</param>
        /// <param name="eventMessageType">The event message type</param>
        /// <param name="handlers">The handlers</param>
        protected override void PublishToHandlers(Ncqrs.Eventing.ServiceModel.Bus.IPublishableEvent eventMessage, Type eventMessageType, IEnumerable<Action<Ncqrs.Eventing.ServiceModel.Bus.PublishedEvent>> handlers) {
            System.Diagnostics.Contracts.Contract.Requires<ArgumentNullException>(handlers != null);
           
            var publishedEventClosedType = typeof(PublishedEvent<>).MakeGenericType(eventMessage.Payload.GetType());
            var publishedEvent = (PublishedEvent)Activator.CreateInstance(publishedEventClosedType, eventMessage);

            foreach (var handler in handlers) {
                handler(publishedEvent);
            }
        }

        public bool Process() {
            CloudQueueMessage message = GetQueue().GetMessage(new TimeSpan(0, 0, 20));
            if (message != null) {
                try {
                    EnqueuedEvent msg = (EnqueuedEvent)Utility.DeJsonize(message.AsString,
                        typeof(EnqueuedEvent));
                    if (msg != null) {
                        EventEntity entity = Utility.GetContext(_account, _tableName).CreateQuery<EventEntity>(_tableName).
                            Where(e => e.PartitionKey == msg.EventSourceId.ToString() && e.RowKey == Utility.GetRowKey(msg.EventVersion)).FirstOrDefault();

                        if (entity != null) {
                            CommittedEvent evnt = new CommittedEvent(entity.CommitId,
                                entity.EventIdentifier,
                                entity.EventSourceId,
                                entity.EventSequence,
                                entity.EventTimeStamp,
                                Utility.DeJsonize(entity.Payload, entity.Name),
                                System.Version.Parse(entity.EventVersion)
                                );

                            Publish(evnt);

                        }
                    }
                } finally {
                    GetQueue().DeleteMessage(message);
                    
                }
                return true;
            }

            return false;

        }
    }
}
