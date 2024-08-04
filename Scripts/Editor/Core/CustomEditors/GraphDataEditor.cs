using UnityEditor;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    // Note: Inside of BehaviourTreeDataEditor, There was a repair method for Composite, where it removed repeated items using Distinct().
    // Might be interested in a repair interface like IConnectedNode, containing a list and a boolean

    [CustomEditor(typeof(GraphData), true)]
    public class GraphDataEditor : NGEditor<GraphData>
    {
        public override void CreateInspector()
        {
            base.CreateInspector();
            VariableList variableList = new VariableList("Local Variables", Target, Target.LocalVariables);
            Add(variableList, true);
        }
    }
}