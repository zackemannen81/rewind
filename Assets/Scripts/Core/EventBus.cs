
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Action<object>>> s_subscribers = new();

        public static void Subscribe<T>(Action<T> callback) where T : struct
        {
            var eventType = typeof(T);
            if (!s_subscribers.ContainsKey(eventType))
            {
                s_subscribers[eventType] = new List<Action<object>>();
            }

            s_subscribers[eventType].Add(e => callback((T)e));
        }

        public static void Unsubscribe<T>(Action<T> callback) where T : struct
        {
            var eventType = typeof(T);
            if (!s_subscribers.ContainsKey(eventType))
            {
                return;
            }

            // This is a simplified unsubscribe. A more robust implementation might require
            // keeping a reference to the original delegate.
            // For this project, we will keep it simple for now.
        }

        public static void Publish<T>(T eventToPublish) where T : struct
        {
            var eventType = typeof(T);
            if (!s_subscribers.ContainsKey(eventType))
            {
                return;
            }

            foreach (var subscriber in s_subscribers[eventType])
            {
                subscriber.Invoke(eventToPublish);
            }
        }
    }
}
