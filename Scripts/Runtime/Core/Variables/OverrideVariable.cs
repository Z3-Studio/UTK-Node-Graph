using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    [Serializable]
    public class OverrideVariable : ISerializationCallbackReceiver, IVariable
    {
        public string guid;
        public object value;

        public UnityEngine.Object serializedObject;
        public string serializedValue;

        public string Name => throw new NotImplementedException();
        public object Value { get => value; set => this.value = value; }
        public string Guid => guid;
        public Type OriginalType => throw new NotImplementedException(); // Check Variable.cs implementation

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(serializedValue))
            {
                value = Serializer.FromJson(serializedValue);
            }
            else
            {
                value = serializedObject;
            }
        }

        public void OnBeforeSerialize()
        {
            if (IsUnityObject(value))
            {
                serializedObject = value as UnityEngine.Object;
                serializedValue = null;
            }
            else
            {
                serializedValue = Serializer.ToJson(value);
                serializedObject = null;
            }
        }

        private bool IsUnityObject(object obj) => obj != null && typeof(UnityEngine.Object).IsAssignableFrom(obj.GetType());

        public static void Validate(List<Variable> originalVariables, List<OverrideVariable> overrideVariables) 
        {
            // Remove invalid variables
            foreach (OverrideVariable overrideVariable in overrideVariables.ToList())
            {
                Variable variable = originalVariables.FirstOrDefault(v => v == overrideVariable);
                if (!variable || !variable.OriginalType.IsValidSubType(overrideVariable.value))
                {
                    overrideVariables.Remove(overrideVariable);
                }
            }
        }

        public static implicit operator bool(OverrideVariable overrideVariable)
        {
            return overrideVariable is not null && !string.IsNullOrEmpty(overrideVariable.guid);
        }

        public static bool operator ==(OverrideVariable overrideVariable, Variable variable)
        {
            if (!overrideVariable || !variable)
                return false;

            return overrideVariable.guid == variable.guid;
        }

        public static bool operator !=(OverrideVariable a, Variable b) => !(a == b);
        public static bool operator ==(Variable b, OverrideVariable a) => a == b;
        public static bool operator !=(Variable b, OverrideVariable a) => !(a == b);
    }
}
