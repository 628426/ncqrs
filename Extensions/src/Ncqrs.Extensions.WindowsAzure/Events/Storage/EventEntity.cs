using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Ncqrs.Eventing;

namespace Ncqrs.Extensions.WindowsAzure.Events.Storage {
    /// <summary>
    /// Represents an event
    /// </summary>
    public class EventEntity : TableServiceEntity {
        /// <summary>
        /// CCreates a new event entity using the supplied event information
        /// </summary>
        /// <param name="uncomittedEvent">The event that has not yet been committed</param>
        public EventEntity(UncommittedEvent uncomittedEvent) : base(uncomittedEvent.EventSourceId.ToString(),
            Utility.GetRowKey(uncomittedEvent.EventSequence)) {
            _commitId = uncomittedEvent.CommitId;
            _eventIdentifier = uncomittedEvent.EventIdentifier;
            _eventSequence = uncomittedEvent.EventSequence;
            _eventSourceId = uncomittedEvent.EventSourceId;
            _eventTimeStamp = uncomittedEvent.EventTimeStamp;
            _eventVersion = uncomittedEvent.EventVersion.ToString();
            _name = uncomittedEvent.Payload.GetType().AssemblyQualifiedName;
            _payload = Utility.Jsonize(uncomittedEvent.Payload, uncomittedEvent.Payload.GetType());
        }
        /// <summary>
        /// Creates a new event
        /// </summary>
        public EventEntity() {
        }

        string _name;
        /// <summary>
        /// The name of this event
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        string _payload;
        /// <summary>
        /// The payload of this event
        /// </summary>
        public string Payload {
            get { return _payload; }
            set { _payload = value; }
        }
        string _eventVersion;
        /// <summary>
        /// The version of this event
        /// </summary>
        public string EventVersion {
            get { return _eventVersion; }
            set { _eventVersion = value; }
        }
        Guid _commitId;
        /// <summary>
        /// The commit id of this event
        /// </summary>
        public Guid CommitId {
            get { return _commitId; }
            set { _commitId = value; }
        }
        Guid _eventSourceId;
        /// <summary>
        /// The source id of this event
        /// </summary>
        public Guid EventSourceId {
            get { return _eventSourceId; }
            set { _eventSourceId = value; }
        }
        long _eventSequence;
        /// <summary>
        /// The sequence of this event
        /// </summary>
        public long EventSequence {
            get { return _eventSequence; }
            set { _eventSequence = value; }
        }
        Guid _eventIdentifier;
        /// <summary>
        /// The id of this event
        /// </summary>
        public Guid EventIdentifier {
            get { return _eventIdentifier; }
            set { _eventIdentifier = value; }
        }
        DateTime _eventTimeStamp;
        /// <summary>
        /// The timestamp of this event
        /// </summary>
        public DateTime EventTimeStamp {
            get { return _eventTimeStamp; }
            set { _eventTimeStamp = value; }
        }
    }
}
