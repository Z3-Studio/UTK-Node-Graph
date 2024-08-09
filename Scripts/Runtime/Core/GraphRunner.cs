﻿using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public enum UpdateMethod
    {
        Update,
        FixedUpdate,
        LateUpdate,
        Manual
    }

    /// <summary>
    /// Implementatio to run any GraphData
    /// </summary>
    [AddComponentMenu(GraphPath.ComponentMenu + "Graph Runner")]
    public sealed class GraphRunner : MonoBehaviour, IGraphRunner
    {
        [SerializeField] private GraphData graphData;
        [SerializeField] private GraphVariablesComponent graphVariablesComponent;
        [SerializeField] private UpdateMethod updateMethod = UpdateMethod.FixedUpdate;

        // Editor variables
        public GraphData GraphData => graphData;
        public VariableInstanceList ReferenceVariables => graphVariablesComponent.ReferenceVariables;
        public Component Component => this;

        /// <summary> Instance created from <see cref="graphData"/></summary>
        public GraphController RootController { get; private set; }
        public GraphEvents Events { get; private set; }
        public float OwnerActivationTime { get; private set; }
        public float DeltaTime { get ; private set; }

        private void Awake()
        {
            Events = new(this);
            graphVariablesComponent.InitReferenceVariables();
            RootController = graphData.CreateInstance(this);
        }

        private void OnEnable()
        {
            OwnerActivationTime = Time.time;
            RootController.StartGraph();
        }

        private void OnDisable()
        {
            RootController.StopGraph();
        }

        public void ManualFixedUpdate() => ManualUpdate(Time.fixedDeltaTime);

        public void ManualUpdate() => ManualUpdate(Time.deltaTime);

        public void ManualUpdate(float delta)
        {
            if (updateMethod != UpdateMethod.Manual)
                return;

            UpdateGraph(delta);
        }

        private void FixedUpdate()
        {
            if (updateMethod != UpdateMethod.FixedUpdate)
                return;

            UpdateGraph(Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (updateMethod != UpdateMethod.Update)
                return;

            UpdateGraph(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (updateMethod != UpdateMethod.LateUpdate)
                return;

            UpdateGraph(Time.deltaTime);
        }

        private void UpdateGraph(float deltaTime)
        {
            DeltaTime = deltaTime;
            RootController.OnUpdate();
        }
    }
}
