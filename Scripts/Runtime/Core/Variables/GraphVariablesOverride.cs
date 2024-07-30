using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Used to make variant of GraphVariables
    /// </summary>
    [CreateAssetMenu(menuName = GraphPath.Graph + "Graph Variables Override", fileName = "New" + nameof(GraphVariables))]
    public sealed class GraphVariablesOverride : VariablesAsset
    {
        [SerializeField] private VariablesAsset baseVariablesAsset;
        [Hide]
        [SerializeField] private List<OverrideVariable> overrideVariables = new();

        public VariablesAsset BaseVariablesAsset => baseVariablesAsset;

        private void OnValidate()
        {
            // Remove override variables
            if (baseVariablesAsset == null)
            {
                overrideVariables.Clear();
                return;
            }

            // Calling GetOrinalVariable will Validate baseVariables
            List<Variable> originalVariables = GetOriginalVariables();

            OverrideVariable.Validate(originalVariables, overrideVariables);
        }

        public List<OverrideVariable> GetOverrideVariables()
        {
            OnValidate();
            return overrideVariables;
        }

        public List<Variable> GetBaseVariables()
        {
            if (baseVariablesAsset == null)
                return new();

            return baseVariablesAsset.GetVariables();
        }

        public override List<Variable> GetVariables()
        {
            // Clone List
            List<Variable> cloneList = new();

            foreach (Variable variable in GetBaseVariables())
            {
                OverrideVariable overrideVariable = GetOverrideVariables().FirstOrDefault(ov => ov == variable);

                if (overrideVariable)
                {
                    cloneList.Add(Variable.Clone(variable, overrideVariable));
                }
                else
                {
                    cloneList.Add(Variable.Clone(variable));
                }
            }

            return cloneList;
        }

        public override GraphVariables GetOriginal()
        {
            VariablesAsset asset = baseVariablesAsset;
            while (asset != this)
            {
                if (asset is GraphVariables graphVariable)
                {
                    return graphVariable;
                }

                if (asset == null)
                    break;

                GraphVariablesOverride graphVariablesOverride = asset as GraphVariablesOverride;
                asset = graphVariablesOverride.baseVariablesAsset;
            }

            baseVariablesAsset = null;

            if (asset == this)
                throw new InvalidCastException("The base asset is recursive");
            else
                throw new InvalidCastException("The base asset is an OverrideVariable and does not have a baseVariablesAsset");
        }
    }
}
