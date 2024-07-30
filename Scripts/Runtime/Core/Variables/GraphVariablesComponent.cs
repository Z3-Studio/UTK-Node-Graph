using System;
using System.Collections.Generic;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Core
{
    public class GraphVariablesComponent : MonoBehaviour
    {
        [SerializeField] private VariablesAsset baseVariablesAsset;
        [ReadOnly, HideInInspector]
        [SerializeField] private List<OverrideVariable> overrideVariables = new();

        public VariableInstanceList ReferenceVariables { get; set; }

        public void InitReferenceVariables()
        {
            if (ReferenceVariables != null)
                return;

            ReferenceVariables = VariableInstanceList.CloneVariables(GetBaseVariables(), GetOverrideVariables());
        }

        private void OnValidate()
        {
            // Remove override variables
            if (baseVariablesAsset == null)
            {
                overrideVariables.Clear();
                return;
            }

            OverrideVariable.Validate(baseVariablesAsset.GetOriginalVariables(), overrideVariables);
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

        public bool HasAsset() => baseVariablesAsset != null;

        public bool AssetIsValid()
        {
            try
            {
                OnValidate();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
