using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    [CreateAssetMenu(menuName = GraphPath.Graph + "Graph Variables", fileName = "New" + nameof(GraphVariables))]
    public sealed class GraphVariables : ScriptableObject
    {
        [SerializeField] private List<GraphVariables> inheritedVariables = new();
        [HideInInspector]
        [SerializeField] private List<OverrideVariable> overrideVariables = new();
        [HideInInspector]
        [SerializeField] private List<Variable> declaredVariables = new();

        public List<Variable> DeclaredVariables => declaredVariables;
        public List<OverrideVariable> OverrideVariables => overrideVariables;

        public void OnValidate()
        {
            // Remove missing override variables
            List<Variable> originalVariables = GetBaseVariables();
            OverrideVariable.Validate(originalVariables, overrideVariables);
        }

        /// <summary> All variables with Final Value </summary>
        /// <remarks> Useful to runtime </remarks>
        public List<Variable> CloneAllVariables()
        {
            List<Variable> allDeclaredVariables = GetAllVariables();
            return CopyVariables(allDeclaredVariables);
        }

        /// <summary> All variables with Declared Value </summary>
        /// <remarks> Useful to validate </remarks>
        public List<Variable> GetAllVariables()
        {
            List<Variable> variableList = new();
            GetAllDeclaredVariables(new(), variableList);
            return variableList;
        }

        /// <summary> Only base variables with Final Value </summary>
        /// <remarks> Useful to display possible overrides in the inspector </remarks>
        public List<Variable> CloneBaseVariables()
        {
            List<Variable> allDeclaredVariables = GetBaseVariables();
            return CopyVariables(allDeclaredVariables);
        }

        /// <summary> Only base variables with Declared Value </summary>
        /// <remarks> Private operations </remarks>
        private List<Variable> GetBaseVariables()
        {
            List<Variable> allDeclaredVariables = GetAllVariables();
            allDeclaredVariables.RemoveRange(0, declaredVariables.Count);
            return allDeclaredVariables;
        }

        private List<Variable> CopyVariables(List<Variable> allDeclaredVariables)
        {
            List<Variable> cloneList = new();

            foreach (Variable variable in allDeclaredVariables)
            {
                // Try to find any override
                OverrideVariable overrideVariable = GetOverrideVariable(new(), variable);

                // Clone override or original 
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

        private void GetAllDeclaredVariables(List<GraphVariables> graphVariableList, List<Variable> variableList)
        {
            // Safe stackoverflow
            if (graphVariableList.Contains(this))
                return;

            graphVariableList.Add(this);
            variableList.AddRange(declaredVariables);

            foreach (GraphVariables baseGraphVariables in inheritedVariables.Where(a => a))
            {
                baseGraphVariables.GetAllDeclaredVariables(graphVariableList, variableList);
            }
        }

        private OverrideVariable GetOverrideVariable(List<GraphVariables> graphVariableList, Variable variable)
        {
            // Safe stackoverflow
            if (graphVariableList.Contains(this))
                return null;

            graphVariableList.Add(this);
            OverrideVariable overrideVariable = overrideVariables.FirstOrDefault(ov => ov == variable);

            if (overrideVariable)
                return overrideVariable;

            foreach (GraphVariables graphVariables in inheritedVariables.Where(a => a))
            {
                overrideVariable = graphVariables.GetOverrideVariable(graphVariableList, variable);

                if (overrideVariable)
                    return overrideVariable;
            }

            return null;
        }
    }
}
