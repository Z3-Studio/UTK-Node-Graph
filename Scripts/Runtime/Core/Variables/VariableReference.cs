using System;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Utility class, make easy to get and set variables <see cref="VariableInstanceList"/>
    /// </summary>
    public class VariableReference<T> : IVariable
    {
        public string Name => variable.Name;
        public T Value { get => (T)variable.Value; set => variable.Value = value; }
        public string Guid => variable.Guid;
        public Type OriginalType => variable.OriginalType;

        object IVariable.Value { get => variable.Value; set => variable.Value = variable; }

        private readonly VariableInstance variable;

        public VariableReference(VariableInstance variable)
        {
            this.variable = variable;
        }

        public static implicit operator VariableInstance(VariableReference<T> a) => a.variable;

        public override bool Equals(object obj)
        {
            if (obj is IVariable variable)
                return this.variable.Equals(variable);

            return false;
        }

        public override int GetHashCode() => variable.GetHashCode();
    }
}
