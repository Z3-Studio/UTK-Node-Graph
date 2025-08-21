using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Core
{
    [Serializable]
    public class OverrideVariable : ISerializationCallbackReceiver, IVariable
    {
        // Serialized fields
        [SerializeField] public string name;
        [SerializeField] public string guid;
        [SerializeField] public string type = "";
        [SerializeField] public string serializedValue;
        [SerializeField] public List<Object> serializedObjects = new();

        [Obsolete]
        public Object serializedObject;

        // Interface
        public string Name => throw new InvalidOperationException("Get from original");
        public object Value { get => value; set => this.value = value; }
        public string Guid => guid;
        public Type OriginalType => throw new InvalidOperationException("Get from original"); // Check Variable.cs implementation

        [Obsolete]
        private Type TempOriginalType
        {
            get
            {
                originalType ??= Type.GetType(type);
                return originalType;
            }
        }

        // Deserialized value
        private object value;
        [Obsolete]
        private Type originalType;

        [Obsolete("TEMP: Used to improve optimization in inspector")]
        private object lastValue;

        public OverrideVariable(Variable original)
        {
            guid = original.guid;
            Value = original.Value;
            type = original.type;
        }

        public void SetType(Variable original)
        {
            type = original.type;
        }

        public void OnAfterDeserialize()
        {
            value = Serializer.FromJson(serializedValue, TempOriginalType, serializedObjects);
        }

        public void OnBeforeSerialize()
        {
            if (lastValue == value)
                return;

            lastValue = value;
            serializedValue = Serializer.ToJson(value, TempOriginalType, serializedObjects);
        }

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

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
