using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Extensions.WindowsAzure.Eventing.ServiceModel.Bus {
    /// <summary>
    /// Represents an event that has been published to a queue
    /// </summary>
    public class EnqueuedEvent {
        private Guid eventSourceId;
        /// <summary>
        /// The id of the event source
        /// </summary>
        public Guid EventSourceId {
            get { return eventSourceId; }
            set { eventSourceId = value; }
        }
        private long eventVersion;
        /// <summary>
        /// The version of the event
        /// </summary>
        public long EventVersion {
            get { return eventVersion; }
            set { eventVersion = value; }
        }

        private string _tableName;
        /// <summary>
        /// The table name the event is stored in
        /// </summary>
        public string TableName {
            get { return _tableName; }
            set { _tableName = value; }
        }

    }
}
