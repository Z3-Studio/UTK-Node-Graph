using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Variables)]
    [NodeDescription("Please describe what this ActionTask does.")]
    public class WithinOpeningAngle : ActionTask {

        [Header("Config")]
        public Parameter<float> angleDirection;
        public Parameter<float> openingAngle;

        [Header("Set")]
        public Parameter<float> currentAngle;

        public override string Info => $"Filter {currentAngle} into {openingAngle}";

        protected override void StartAction() {
            float halfAngle = openingAngle.Value / 2;
            float minAngle = (angleDirection.Value - halfAngle).NormalizeAngle();
            float maxAngle = (angleDirection.Value + halfAngle).NormalizeAngle();

            Vector2 angleRange = new Vector2(minAngle, maxAngle);
            if (minAngle > maxAngle) {
                if (angleRange.InsideRange(currentAngle.Value)) {

                    RecalculateAngle(angleRange);
                }
            }
            else {
                if (!angleRange.InsideRange(currentAngle.Value)) {

                    RecalculateAngle(angleRange);
                }
            }

            EndAction(true);
        }

        private void RecalculateAngle(Vector2 range) {
            
            
            float a = MathUtils.AngleDiference(currentAngle.Value, range.x);
            float b = MathUtils.AngleDiference(currentAngle.Value, range.y);
            currentAngle.Value = a < b ? range.x : range.y;
        }
    }
}