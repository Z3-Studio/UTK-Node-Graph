using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Used to serialize in <see cref="GraphData"/>
    /// </summary>
    [Serializable]
    public class Variable : ISerializationCallbackReceiver, IVariable
    {
        // Serialized fields
        public string name;
        public string guid;
        public string type = "";
        public string serializedValue;
        public List<Object> serializedObjects;

        // Interface
        public string Name => name;
        public object Value { get => value; set => this.value = value; }
        public string Guid => guid;
        public Type OriginalType
        {
            get
            {
                originalType ??= Type.GetType(type);
                return originalType;
            }
        }

        // Deserialized value
        private object value;
        private Type originalType;

        private object lastValue; // TEMP

        public void SetType(Type newType)
        {
            originalType = newType;
            type = newType.AssemblyQualifiedName;
            value = newType.GetDefaultValueForType();
        }

        public void OnAfterDeserialize()
        {
            value = Serializer.FromJson(serializedValue, OriginalType, serializedObjects);
            lastValue = value;
        }

        public void OnBeforeSerialize()
        {
            if (lastValue == value) // TODO: Fix it
                return;

            ForceSave();
        }

        public void ForceSave() // TODO: Review it
        {
            lastValue = value;
            serializedValue = Serializer.ToJson(value, OriginalType, serializedObjects);
        }

        public static Variable Clone(Variable variable)
        {
            return new()
            {
                name = variable.name,
                guid = variable.guid,
                type = variable.type,
                value = variable.value,
                serializedObjects = variable.serializedObjects,
                serializedValue = variable.serializedValue,
            };
        }

        public static Variable Clone(Variable variable, OverrideVariable overrideVariable)
        {
            Variable newVariable = Clone(variable);
            newVariable.value = overrideVariable.value;
            return newVariable;
        }

        public static implicit operator bool(Variable variable)
        {
            return variable is not null && !string.IsNullOrEmpty(variable.guid);
        }

        #if UNITY_EDITOR
        public static Variable CreateVariable(ScriptableObject asset, Type type, List<Variable> targetList, string variableName = "NewVariable")
        {
            const int MaxInteractions = 100;
            string name = variableName;

            for (int i = 1; i < MaxInteractions; i++)
            {
                if (!targetList.Any(p => p.name == name))
                    break;

                name = $"{variableName} [{i + 1}]";
            }

            Variable newVariable = new()
            {
                name = name,
                guid = UnityEditor.GUID.Generate().ToString(),
                type = type.AssemblyQualifiedName,
                value = type.GetDefaultValueForType()
            };

            targetList.Add(newVariable);

            UnityEditor.EditorUtility.SetDirty(asset);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(asset);
            return newVariable;
        }
        #endif
    }
}
