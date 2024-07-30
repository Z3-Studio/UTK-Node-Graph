using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Variables)]
    [NodeDescription("Switch the value variable A to B")]
    public class SwitchVariables<T> : ActionTask {

        /*[RequiredField]*/ public Parameter<T> variableA;
        /*[RequiredField]*/ public Parameter<T> variableB;

        protected override void StartAction() {
            T aux = variableA.Value;
            variableA.Value = variableB.Value;
            variableB.Value = aux;
            EndAction(true);
        }
    }
}