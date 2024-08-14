using System;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    [AddComponentMenu(GraphPath.ComponentMenu + "Mono Event Dispatcher")]
    public class MonoEventDispatcher : MonoBehaviour
    {
        // Trigger 2D
        public event Action<Collider2D> OnTriggerEnter2DEvent;
        public event Action<Collider2D> OnTriggerStay2DEvent;
        public event Action<Collider2D> OnTriggerExit2DEvent;

        // Collision 2D
        public event Action<Collision2D> OnCollisionEnter2DEvent;
        public event Action<Collision2D> OnCollisionStay2DEvent;
        public event Action<Collision2D> OnCollisionExit2DEvent;

        // Trigger
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerStayEvent;
        public event Action<Collider> OnTriggerExitEvent;

        // Collision
        public event Action<Collision> OnCollisionEnterEvent;
        public event Action<Collision> OnCollisionStayEvent;
        public event Action<Collision> OnCollisionExitEvent;

        // Particle
        public event Action<GameObject> OnParticleCollisionEvent;
        public event Action OnParticleTriggerEvent;

        // Visibility
        public event Action OnBecameInvisibleEvent;
        public event Action OnBecameVisibleEvent;

        // Activation
        public event Action OnEnableEvent;
        public event Action OnDisableEvent;
        public event Action OnDestroyEvent;

        // Trigger 2D
        private void OnTriggerEnter2D(Collider2D other) => OnTriggerEnter2DEvent?.Invoke(other);
        private void OnTriggerStay2D(Collider2D other) => OnTriggerStay2DEvent?.Invoke(other);
        private void OnTriggerExit2D(Collider2D other) => OnTriggerExit2DEvent?.Invoke(other);

        // Collision 2D
        private void OnCollisionEnter2D(Collision2D collision) => OnCollisionEnter2DEvent?.Invoke(collision);
        private void OnCollisionStay2D(Collision2D collision) => OnCollisionStay2DEvent?.Invoke(collision);
        private void OnCollisionExit2D(Collision2D collision) => OnCollisionExit2DEvent?.Invoke(collision);

        // Trigger
        private void OnTriggerEnter(Collider other) => OnTriggerEnterEvent?.Invoke(other);
        private void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);
        private void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);

        // Collision
        private void OnCollisionEnter(Collision collision) => OnCollisionEnterEvent?.Invoke(collision);
        private void OnCollisionStay(Collision collision) => OnCollisionStayEvent?.Invoke(collision);
        private void OnCollisionExit(Collision collision) => OnCollisionExitEvent?.Invoke(collision);

        // Particle
        private void OnParticleCollision(GameObject other) => OnParticleCollisionEvent?.Invoke(other);
        private void OnParticleTrigger() => OnParticleTriggerEvent?.Invoke();

        // Visibility
        private void OnBecameInvisible() => OnBecameInvisibleEvent?.Invoke();
        private void OnBecameVisible() => OnBecameVisibleEvent?.Invoke();

        // Activation
        private void OnEnable() => OnEnableEvent?.Invoke();
        private void OnDisable() => OnDisableEvent?.Invoke();
        private void OnDestroy() => OnDestroyEvent?.Invoke();

        public static MonoEventDispatcher ValidateEmmiter(MonoEventDispatcher monoEvents, Component component)
        {
            return ValidateEmmiter(monoEvents, component.gameObject);
        }

        public static MonoEventDispatcher ValidateEmmiter(MonoEventDispatcher monoEvents, GameObject gameObject)
        {
            if (!monoEvents || monoEvents.gameObject != gameObject)
            {
                monoEvents = gameObject.GetOrAddComponent<MonoEventDispatcher>();
            }

            return monoEvents;
        }
    }
}