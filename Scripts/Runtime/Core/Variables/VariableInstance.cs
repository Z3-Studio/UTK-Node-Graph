using System;
using System.Collections.Generic;
using System.Linq;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Runtime variable
    /// </summary>
    public class VariableInstance : IVariable
    {
        public string Name { get; }
        public object Value { get; set; }
        public string Guid { get; }
        public Type OriginalType { get; }

        public VariableInstance(Variable variable)
        {
            Name = variable.name;
            Value = variable.value;
            Guid = variable.guid;
            OriginalType = variable.OriginalType;
        }

        public VariableInstance(Variable variable, OverrideVariable overrideVariable) : this(variable)
        {
            Value = overrideVariable.value;
        }
    }

    /// <summary>
    /// Runtime variables list
    /// </summary>
    public class VariableInstanceList
    {
        public List<VariableInstance> Values => pair.Values.ToList();

        private readonly Dictionary<string, VariableInstance> pair = new(); 

        public VariableInstance this[string index]
        {
            get => pair[index];
            set => pair[index] = value;
        }

        public static VariableInstanceList CloneVariables(List<Variable> variablesToClone)
        {
            VariableInstanceList dic = new();
            foreach (Variable v in variablesToClone)
            {
                dic[v.guid] = new VariableInstance(v);
            }

            return dic;
        }

        public static VariableInstanceList CloneVariables(List<Variable> baseList, List<OverrideVariable> overrideList)
        {
            VariableInstanceList dic = new();
            foreach (Variable baseVariable in baseList)
            {
                OverrideVariable overrideVariable = overrideList.FirstOrDefault(ov => ov == baseVariable);
                VariableInstance newItem = overrideVariable is null ? new VariableInstance(baseVariable) : new VariableInstance(baseVariable, overrideVariable);
                dic[baseVariable.guid] = newItem;
            }

            return dic;
        }

        internal bool TryGetValue(string guid, out VariableInstance localVariable) => pair.TryGetValue(guid, out localVariable);
    }
}
