﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    [AddComponentMenu(GraphPath.ComponentMenu + "Graph Variables Component")]
    public class GraphVariablesComponent : MonoBehaviour
    {
        [SerializeField] private GraphVariables baseVariablesAsset;
        [SerializeField] private List<OverrideVariable> overrideVariables = new();

        public GraphVariables BaseVariablesAsset => baseVariablesAsset;
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

            OverrideVariable.Validate(baseVariablesAsset.GetAllVariables(), overrideVariables);
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

            return baseVariablesAsset.CloneAllVariables();
        }

        public bool AssetIsValid()
        {
            try
            {
                OnValidate();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
