using System;
using UnityEngine;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Used to serialize a value or bind with a <see cref="Core.Variable"/>
    /// </summary>
    [Serializable]
    public class Parameter<T> : IParameter<T>
    {
        [SerializeField] private string guid; // TODO: rename to variableGuid
        [AssetOnly]
        [SerializeField] private T value;

        public bool NullValue => Value.ObjectNullCheck();

        // Editor
        public bool IsDefined => Variable != null;
        public bool IsBinding => !string.IsNullOrEmpty(guid);
        public bool IsSelfBind => guid == SelfBind;
        public Variable Variable { get; private set; }

        // Runtime
        string IParameter.Guid => guid;
        object IParameter.Value { get => Value; set => Value = (T)value; }
        public Type GenericType => typeof(T);
        public T Value
        {
            get
            {
                if (Get == null)
                {
                    return value;
                }
                return (T)Get();
            }
            set
            {
                if (Set == null)
                {
                    this.value = value;
                    return;
                }
                Set(value);
            }
        }

        private Func<object> Get { get; set; }
        private Action<object> Set { get; set; }

        public const string ValueField = nameof(value);
        public const string SelfBind = "SelfBind";

        void IParameter.SetupDependencies(GraphController graphController)
        {
            // TODO: Include editor directives
            BuildBind(graphController);
        }

        void IParameter.SelfBind()
        {
            guid = SelfBind;
            Variable = null;
        }

        void IParameter.Bind(Variable variable)
        {
            guid = variable.guid;
            Variable = variable;

            if (variable.OriginalType == null)
            {
                Debug.LogError(guid);
            }
        }

        void IParameter.Unbind()
        {
            guid = string.Empty;
            Variable = null;
        }

        void IParameter.Invalid()
        {
            Variable = null;
        }

        private void EditorBind(IParameter parameter)
        {
            // TODO: Reevaluate the guid and booleans
        }

        private void BuildBind(GraphController graphController)
        {
            if (IsSelfBind)
            {
                Get = graphController.CachedComponents.CreateGetter(GenericType);
                Set = (newValue) => throw new InvalidOperationException("You cannot set a component using SelfBind, is only get. Consider using GameObject.AddComponent<T>() or Destroy()");
                return;
            }

            if (!IsBinding)
                return;

            if (graphController.LocalVariables.TryGetValue(guid, out VariableInstance localVariable))
            {
                BindVariable(localVariable);
            }
            else if (graphController.ReferenceVariables.TryGetValue(guid, out VariableInstance referenceVariable))
            {
                BindVariable(referenceVariable);
            }
        }

        private void BindVariable(IVariable variable)
        {
            Get = TypeResolver.CreateGet(this, variable);
            Set = TypeResolver.CreateSet(this, variable);
        }

        public override string ToString()
        {
            if (IsBinding)
            {
                if (IsSelfBind)
                    return "Self".ToBold();

                if (IsDefined)
                    return '$' + Variable.name.ToBold();

                return "'Missing'".AddRichTextColor(Color.red);
            }

            // TODO: Draw a good name for collections
            if (NullValue)
                return "Null".ToBold();

            if (Value is Object obj)
                return obj.name.ToBold();

            return Value.ToString().ToBold();
        }

        /// <summary> Used to define the default value </summary>
        public static implicit operator Parameter<T>(T value)
        {
            return new Parameter<T> { Value = value };
        }

        /// <summary> Used to get Value </summary>
        public static implicit operator T(Parameter<T> parameter)
        {
            return parameter.Value;
        }

        /// <summary> Used to set Value </summary>
        /// <remarks> It's not intuitive, but it simplifies the set </remarks>
        public static Parameter<T> operator &(Parameter<T> a, T b)
        {
            a.Value = b;
            return a;
        }
    }
}
