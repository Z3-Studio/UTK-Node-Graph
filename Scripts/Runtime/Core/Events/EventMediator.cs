using System;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Register and Invoke events using ScriptableObject
    /// </summary>
    [CreateAssetMenu(menuName = GraphPath.Graph + "Event Mediator", fileName = "New" + nameof(EventMediator))]
    public class EventMediator : ScriptableObject
    {
        private Action listeners;
        private readonly Dictionary<Type, Action<object>> genericCallbacks = new();

        public void Invoke()
        {
            listeners?.Invoke();
        }

        public void RegisterCallback(Action callback)
        {
            listeners += callback;
        }

        public void UnregisterCallback(Action callback)
        {
            listeners -= callback;
        }

        public void Invoke<T>(T payload)
        {
            Type eventType = typeof(T);
            if (!genericCallbacks.ContainsKey(eventType))
                return;

            genericCallbacks[eventType]?.Invoke(payload);
        }

        public void RegisterCallback<T>(Action<T> callback)
        {
            Type eventType = typeof(T);
            if (genericCallbacks.ContainsKey(eventType))
            {
                genericCallbacks[eventType] += obj => callback((T)obj);
            }
            else
            {
                genericCallbacks[eventType] = obj => callback((T)obj);
            }
        }

        public void UnregisterCallback<T>(Action<T> callback)
        {
            Type eventType = typeof(T);
            if (!genericCallbacks.ContainsKey(eventType))
                return;

            genericCallbacks[eventType] -= obj => callback((T)obj);
            if (genericCallbacks[eventType] == null)
            {
                genericCallbacks.Remove(eventType);
            }
        }

        public static EventMediator operator +(EventMediator eventMediator, Action action)
        {
            eventMediator.listeners += action;
            return eventMediator;
        }

        public static EventMediator operator -(EventMediator eventMediator, Action action)
        {
            eventMediator.listeners -= action;
            return eventMediator;
        }

        public static implicit operator Action(EventMediator gameEvent) => gameEvent.Invoke;
    }
}
