using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public abstract class VariablesAsset : ScriptableObject 
    {
        public abstract GraphVariables GetOriginal();
        public abstract List<Variable> GetVariables();
        public List<Variable> GetOriginalVariables() => GetOriginal().Variables;
    }
}
