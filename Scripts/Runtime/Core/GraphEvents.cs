using System;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public class GraphEvents
    {
        public event Action<string, object, Component> OnCustomEvent;

        public event Action<Collider2D> OnTriggerEnter2D;
        public event Action<Collider2D> OnTriggerStay2D;
        public event Action<Collider2D> OnTriggerExit2D;
        public event Action<Collision2D> OnCollisionEnter2D;
        public event Action<Collision2D> OnCollisionStay2D;
        public event Action<Collision2D> OnCollisionExit2D;

        public event Action<Collider> OnTriggerEnter;
        public event Action<Collider> OnTriggerStay;
        public event Action<Collider> OnTriggerExit;
        public event Action<Collision> OnCollisionEnter;
        public event Action<Collision> OnCollisionStay;
        public event Action<Collision> OnCollisionExit;

        public event Action<GameObject> OnParticleCollision;
        public event Action OnParticleTrigger;

        public event Action OnBecameInvisible;
        public event Action OnBecameVisible;

        public event Action OnEnable;
        public event Action OnDisable;
        public event Action OnDestroy;

        public event Action OnTransformChildrenChanged;
        public event Action OnTransformParentChanged;

        private GraphRunner owner;

        public GraphEvents(GraphRunner defaultSender)
        {
            owner = defaultSender;
        }

        internal void SendCustomEvent<T>(string eventname, T value)
        {
            OnCustomEvent?.Invoke(eventname, value, owner);
        }

        internal void SendCustomEvent(string eventname, object value)
        {
            OnCustomEvent?.Invoke(eventname, value, owner);
        }
    }
}
