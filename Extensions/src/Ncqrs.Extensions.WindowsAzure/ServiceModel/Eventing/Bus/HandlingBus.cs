using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Extensions.WindowsAzure.Eventing.ServiceModel.Bus {
    /// <summary>
    /// Represents a basic bus with Registration functionality
    /// </summary>
    public abstract class HandlingBus : IEventBus {
        private readonly Dictionary<Type, List<Action<PublishedEvent>>> _handlerRegister = new Dictionary<Type, List<Action<PublishedEvent>>>();
        /// <summary>
        /// Returns the handlers associated with the supplied event
        /// </summary>
        /// <param name="eventMessage">The supplied event</param>
        /// <returns>A set of handlers</returns>
        protected IEnumerable<Action<PublishedEvent>> GetHandlersForEvent(IPublishableEvent eventMessage) {
            if (eventMessage == null)
                return null;

            var dataType = eventMessage.Payload.GetType();
            var result = new List<Action<PublishedEvent>>();

            foreach (var key in _handlerRegister.Keys) {
                if (key.IsAssignableFrom(dataType)) {
                    var handlers = _handlerRegister[key];
                    result.AddRange(handlers);
                }
            }

            return result;
        }
        /// <summary>
        /// Registers a specific handler for events of the associated type
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="handler">The handler</param>
        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) {
            var eventDataType = typeof(TEvent);

            Action<PublishedEvent> act = evnt => handler.Handle((IPublishedEvent<TEvent>)evnt);
            RegisterHandler(eventDataType, act);
        }
        /// <summary>
        /// Registers a specific handler for events of the associated type
        /// </summary>
        /// <param name="eventDataType">The event type</param>
        /// <param name="handler">The handler</param>
        public void RegisterHandler(Type eventDataType, Action<PublishedEvent> handler) {
            List<Action<PublishedEvent>> handlers = null;
            if (!_handlerRegister.TryGetValue(eventDataType, out handlers)) {
                handlers = new List<Action<PublishedEvent>>(1);
                _handlerRegister.Add(eventDataType, handlers);
            }

            handlers.Add(handler);
        }
        /// <summary>
        /// Publishes the events
        /// </summary>
        /// <param name="eventMessages">Events to publish</param>
        public void Publish(IEnumerable<IPublishableEvent> eventMessages) {
            foreach (var eventMessage in eventMessages) {
                Publish(eventMessage);
            }
        }
        /// <summary>
        /// Publishes the event
        /// </summary>
        /// <param name="eventMessage">The event to publish</param>
        public void Publish(IPublishableEvent eventMessage) {
            var eventMessageType = eventMessage.GetType();

            IEnumerable<Action<PublishedEvent>> handlers = GetHandlersForEvent(eventMessage);

            if (handlers.Count() == 0) {
                
            } else {
                 PublishToHandlers(eventMessage, eventMessageType, handlers);
            }
        }
        /// <summary>
        /// Publishes the event to the specified handlers
        /// </summary>
        /// <param name="eventMessage">The event to publish</param>
        /// <param name="eventMessageType">The event message type</param>
        /// <param name="handlers">The handlers</param>
        protected abstract void PublishToHandlers(IPublishableEvent eventMessage, Type eventMessageType, IEnumerable<Action<PublishedEvent>> handlers);

    }
}
