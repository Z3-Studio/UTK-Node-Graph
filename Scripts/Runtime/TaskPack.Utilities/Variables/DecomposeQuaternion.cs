using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Gets a transform rotation")]
    public class DecomposeQuaternion : ActionTask
    {
        [Header("Input")]
        public Parameter<Quaternion> quaternionInput;
        
        [Header("Full Rotation")]
        public Parameter<Vector3> euler;

        [Header("Decomposed Euler")]
        public Parameter<float> eulerX;
        public Parameter<float> eulerY;
        public Parameter<float> eulerZ;
        
        [Header("Decomposed Quaternion")]
        public Parameter<float> quaternionX;
        public Parameter<float> quaternionY;
        public Parameter<float> quaternionZ;
        public Parameter<float> quaternionW;

        [Header("Parameters")]
        [Tooltip("Returns a negative angle equivalent if it's greater than 180 degrees")]
        public Parameter<bool> useSignedEuler;

        public override string Info => $"Decompose {quaternionInput}";

        protected override void StartAction()
        {
            Quaternion quaternion = quaternionInput.Value;
            Vector3 eulerAngles = quaternion.eulerAngles;
            
            euler.Value = eulerAngles;
            
            eulerX.Value = useSignedEuler.Value ? GetSignedEuler(eulerAngles.x) : eulerAngles.x;
            eulerY.Value = useSignedEuler.Value ? GetSignedEuler(eulerAngles.y) : eulerAngles.y;
            eulerZ.Value = useSignedEuler.Value ? GetSignedEuler(eulerAngles.z) : eulerAngles.z;
            
            quaternionX.Value = quaternion.x;
            quaternionY.Value = quaternion.y;
            quaternionZ.Value = quaternion.z;
            quaternionW.Value = quaternion.w;
            
            EndAction(true);
        }

        /// <summary>
        /// Returns relative signed angles
        /// </summary>
        /// <param name="euler">Euler angle in degrees</param>
        /// <returns></returns>
        private static float GetSignedEuler(float euler)
        {
            return euler > 180 ? euler - 360 : euler;
        }
    }
}