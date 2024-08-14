using System;
using System.Linq;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    [Serializable]
    public struct StringDispatcherReference
    {
        public string keyName;
        public StringEventDispatcher eventDispatcher;
    }

    /// <summary>
    /// Utility class to make easier the dispatcher process
    /// </summary>
    public class StringDispatcherReferences : MonoBehaviour
    {
        [SerializeField] private StringDispatcherReference[] references;

        public StringEventDispatcher[] GetAllDispatcher() => references.Select(r => r.eventDispatcher).ToArray();

        public StringEventDispatcher GetDispatcher(string name)
        {
            return references.First(r => r.keyName == name).eventDispatcher;
        }

        public void SendEventTo(string key, object sender, string eventName)
        {
            StringEventDispatcher target = GetDispatcher(key);
            target.SendEvent(sender, eventName);
        }

        public void SendEventTo<T>(string key, object sender, string eventName, T payload)
        {
            StringEventDispatcher target = GetDispatcher(key);
            target.SendEvent(sender, eventName, payload);
        }

        public void SendEventAll(object sender, string eventName)
        {
            foreach (StringEventDispatcher target in GetAllDispatcher())
            {
                target.SendEvent(sender, eventName);
            }
        }

        public void SendEventAll<T>(object sender, string eventName, T payload)
        {
            foreach (StringEventDispatcher target in GetAllDispatcher())
            {
                target.SendEvent(sender, eventName, payload);
            }
        }
    }
}
