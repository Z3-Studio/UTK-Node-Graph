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
        [SerializeField] private Parameter<Quaternion> quaternionInput;
        
        [Header("Full Rotation")]
        [SerializeField] private Parameter<Vector3> euler;

        [Header("Decomposed Euler")]
        [SerializeField] private Parameter<float> eulerX;
        [SerializeField] private Parameter<float> eulerY;
        [SerializeField] private Parameter<float> eulerZ;
        
        [Header("Decomposed Quaternion")]
        [SerializeField] private Parameter<float> quaternionX;
        [SerializeField] private Parameter<float> quaternionY;
        [SerializeField] private Parameter<float> quaternionZ;
        [SerializeField] private Parameter<float> quaternionW;

        [Header("Parameters")]
        [Tooltip("Returns a negative angle equivalent if it's greater than 180 degrees")]
        [SerializeField] private Parameter<bool> useSignedEuler;

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
            
            EndAction();
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