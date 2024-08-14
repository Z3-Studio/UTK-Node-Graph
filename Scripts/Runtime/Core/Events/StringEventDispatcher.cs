using System;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public interface IStringEvent<T> : IStringEvent
    {
        T Payload { get; }
    }

    public interface IStringEvent
    {
        object Sender { get; }
        string EventName { get; }
    }

    /// <summary>
    /// Register and Invoke events in Component
    /// </summary>
    public class StringEventDispatcher : MonoBehaviour
    {
        private Action<IStringEvent> callbacks;
        private readonly Dictionary<Type, Action<IStringEvent>> callbackWithPayload = new();

        #region Local Events
        public void SendEvent(IStringEvent evt)
        {
            callbacks?.Invoke(evt);
        }

        public void SendEvent<T>(IStringEvent<T> evt)
        {
            Type eventType = typeof(T);

            callbacks?.Invoke(evt);

            if (callbackWithPayload.ContainsKey(eventType))
            {
                callbackWithPayload[eventType]?.Invoke(evt);
            }
        }

        public void RegisterCallback(Action<IStringEvent> callback)
        {
            callbacks += callback;
        }

        public void UnregisterCallback(Action<IStringEvent> callback)
        {
            callbacks -= callback;
        }

        public void RegisterCallback<T>(Action<IStringEvent<T>> callback)
        {
            Type eventType = typeof(T);
            if (callbackWithPayload.ContainsKey(eventType))
            {
                callbackWithPayload[eventType] += e => callback((IStringEvent<T>)e);
            }
            else
            {
                callbackWithPayload[eventType] = e => callback((IStringEvent<T>)e);
            }
        }

        public void UnregisterCallback<T>(Action<IStringEvent<T>> callback)
        {
            Type eventType = typeof(T);
            if (!callbackWithPayload.ContainsKey(eventType))
                return;

            callbackWithPayload[eventType] -= e => callback((IStringEvent<T>)e);
            if (callbackWithPayload[eventType] == null)
            {
                callbackWithPayload.Remove(eventType);
            }
        }
        #endregion

        #region Utils methods
        public void SendSelfEvent(string eventname)
        {
            StringEvent newEvent = new StringEvent(gameObject, eventname);
            SendEvent(newEvent);
        }

        public void SendSelfEvent<T>(string eventname, T value)
        {
            StringEvent<T> newEvent = new StringEvent<T>(gameObject, eventname, value);
            SendEvent(newEvent);
        }

        public void SendEvent(object sender, string eventName)
        {
            StringEvent newEvent = new StringEvent(sender, eventName);
            SendEvent(newEvent);
        }

        public void SendEvent<T>(object sender, string eventName, T payload)
        {
            StringEvent<T> newEvent = new StringEvent<T>(sender, eventName, payload);
            SendEvent(newEvent);
        }

        public static void SendEvent<T>(object sender, GameObject target, string eventName, T payload)
        {
            StringEvent<T> newEvent = new StringEvent<T>(sender, eventName, payload);
            StringEventDispatcher dispatcher = target.GetComponent<StringEventDispatcher>();

            if (!dispatcher)
                return;

            dispatcher.SendEvent(newEvent);
        }

        public static void SendEvent(object sender, GameObject target, string eventName)
        {
            StringEvent newEvent = new StringEvent(sender, eventName);
            StringEventDispatcher dispatcher = target.GetComponent<StringEventDispatcher>();

            if (!dispatcher)
                return;

            dispatcher.SendEvent(newEvent);
        }
        #endregion
    }
}
