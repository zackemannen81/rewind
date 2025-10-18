
using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Lightweight event bus to keep systems decoupled while allowing structured communication.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> s_subscribers = new();

        public static void Subscribe<T>(Action<T> callback) where T : struct
        {
            if (callback == null)
            {
                return;
            }

            var eventType = typeof(T);
            if (!s_subscribers.TryGetValue(eventType, out var listeners))
            {
                listeners = new List<Delegate>();
                s_subscribers[eventType] = listeners;
            }

            if (!listeners.Contains(callback))
            {
                listeners.Add(callback);
            }
        }

        public static void Unsubscribe<T>(Action<T> callback) where T : struct
        {
            if (callback == null)
            {
                return;
            }

            var eventType = typeof(T);
            if (!s_subscribers.TryGetValue(eventType, out var listeners))
            {
                return;
            }

            listeners.Remove(callback);

            if (listeners.Count == 0)
            {
                s_subscribers.Remove(eventType);
            }
        }

        public static void Publish<T>(T eventToPublish) where T : struct
        {
            var eventType = typeof(T);
            if (!s_subscribers.TryGetValue(eventType, out var listeners))
            {
                return;
            }

            // Iterate backwards so subscribers can safely unsubscribe while events are processed.
            for (var i = listeners.Count - 1; i >= 0; i--)
            {
                if (listeners[i] is Action<T> typedListener)
                {
                    typedListener.Invoke(eventToPublish);
                }
            }
        }
    }
}
