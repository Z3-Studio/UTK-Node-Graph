#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    [NodeCategory(Categories.Inputs)]
    public class CheckInputKey : ConditionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Key> key = Key.Space;

        [ParameterDefinition(AutoBindType.FindSameVariable)]
        [SerializeField] private Parameter<float> inputRetentionDuration;

        public override string InfoC => $"Check Key Pressed: {key}";

        private float buffer;

        public override void StartCondition()
        {
            buffer = float.MinValue;
        }

        public override bool CheckCondition()
        {
            if (Keyboard.current == null)
                return false;

            if (Keyboard.current[key.Value].isPressed)
            {
                buffer = Time.time + DeltaTime + inputRetentionDuration.Value;
            }

            return buffer >= Time.time;
        }
    }
}
#endif
