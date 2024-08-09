using System.Collections.Generic;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    public enum VariableScope
    {
        Reference,
        Local,
        SelfBind,
        Undefined
    }

    public class VariableAnalyzer
    {
        public string VariableName { get; }
        public Variable Variable { get; }
        public List<GraphSubAsset> Depedencies { get; } = new();
        public VariableScope VariableScope { get; }

        public string VariableGuid { get; }
        public bool Exist => Variable != null;

        public VariableAnalyzer(Variable variable, VariableScope variableScope)
        {
            Variable = variable;
            VariableName = variable.Name;
            VariableGuid = variable.Guid;
            VariableScope = variableScope;
        }

        /// <summary> Used  when cannot find the variable or is Self Bind </summary>
        public VariableAnalyzer(string variableName, string variableGuid, VariableScope variableScope)
        {
            VariableName = variableName;
            VariableGuid = variableGuid;
            VariableScope = variableScope;
        }

        public void AddDependency(GraphSubAsset subAsset, IParameter _)
        {
            Depedencies.Add(subAsset);
        }
    }
}
