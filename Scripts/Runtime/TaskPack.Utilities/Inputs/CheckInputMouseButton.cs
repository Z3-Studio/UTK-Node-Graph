#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    [NodeCategory(Categories.Inputs)]
    public class CheckInputMouseButton : ConditionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<MouseButton> mouseButton = MouseButton.Left;

        [ParameterDefinition(AutoBindType.FindSameVariable)]
        [SerializeField] private Parameter<float> inputRetentionDuration;

        public override string InfoC => $"Check Key Pressed: {mouseButton}";

        private float buffer;

        public override void StartCondition()
        {
            buffer = float.MinValue;
        }

        public override bool CheckCondition()
        {
            if (Mouse.current == null)
                return false;

            bool isPressed = mouseButton.Value switch
            {
                MouseButton.Left => Mouse.current.leftButton.isPressed,
                MouseButton.Right => Mouse.current.rightButton.isPressed,
                MouseButton.Middle => Mouse.current.middleButton.isPressed,
                MouseButton.Forward => Mouse.current.forwardButton.isPressed,
                MouseButton.Back => Mouse.current.backButton.isPressed,
                _ => false
            };

            if (isPressed)
            {
                buffer = Time.time + DeltaTime + inputRetentionDuration.Value;
            }

            return buffer >= Time.time;
        }
    }
}
#endif
