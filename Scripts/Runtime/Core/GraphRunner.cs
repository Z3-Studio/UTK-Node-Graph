using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.Core
{
    public enum UpdateMethod
    {
        Update,
        FixedUpdate,
        LateUpdate,
        Manual
    }

    public enum ActivationMethod
    {
        AwakeDestroy,
        EnableDisable,
        Manual
    }

    /// <summary>
    /// Generic implementation to run any GraphData
    /// </summary>
    [AddComponentMenu(GraphPath.ComponentMenu + "Graph Runner")]
    public sealed class GraphRunner : MonoBehaviour, IGraphRunner
    {
        [SerializeField] private GraphData graphData;
        [SerializeField] private GraphVariablesComponent graphVariablesComponent;
        [SerializeField] private ActivationMethod activationMethod = ActivationMethod.EnableDisable;
        [SerializeField] private UpdateMethod updateMethod = UpdateMethod.FixedUpdate;

        // Editor properties
        public GraphData GraphData => graphData;
        public GraphController RootController { get; private set; }

        // Interface properties
        public Component Component => this;
        public VariableInstanceList ReferenceVariables => graphVariablesComponent.ReferenceVariables;
        public CachedComponents CachedComponents { get; private set; }
        public float OwnerActivationTime { get; private set; }
        public float DeltaTime { get ; private set; }
        public bool Active { get ; private set; }

        private void Reset() => TryGetComponent(out graphVariablesComponent);

        private void Awake()
        {
            CachedComponents = new CachedComponents(this);
            graphVariablesComponent.InitReferenceVariables();
            RootController = graphData.CreateInstance(this);

            if (activationMethod != ActivationMethod.AwakeDestroy)
                return;

            ManualActivation();
        }

        private void OnDestroy()
        {
            if (activationMethod != ActivationMethod.AwakeDestroy)
                return;

            ManualDeactivation();
        }

        private void OnEnable()
        {
            if (activationMethod != ActivationMethod.EnableDisable)
                return;

            ManualActivation();
        }

        private void OnDisable()
        {
            if (activationMethod != ActivationMethod.EnableDisable)
                return;

            ManualDeactivation();
        }

        private void FixedUpdate()
        {
            if (updateMethod != UpdateMethod.FixedUpdate)
                return;

            ManualFixedUpdate();
        }

        private void Update()
        {
            if (updateMethod != UpdateMethod.Update)
                return;

            ManualUpdate();
        }

        private void LateUpdate()
        {
            if (updateMethod != UpdateMethod.LateUpdate)
                return;

            ManualUpdate();
        }

        public void ManualActivation()
        {
            Active = true;
            OwnerActivationTime = Time.time;
            RootController.StartGraph();
        }

        public void ManualDeactivation()
        {
            Active = false;
            RootController.StopGraph();
        }

        public void ManualFixedUpdate() => ManualUpdate(Time.fixedDeltaTime);

        public void ManualUpdate() => ManualUpdate(Time.deltaTime);

        public void ManualUpdate(float delta)
        {
            DeltaTime = delta;
            RootController.OnUpdate();
        }
    }
}
