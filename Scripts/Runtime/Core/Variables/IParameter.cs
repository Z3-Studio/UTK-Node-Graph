using System;

namespace Z3.NodeGraph.Core
{
    public interface IParameter
    {
        object Value { get; set; }
        string Guid { get; }
        Type GenericType { get; }
        bool IsBinding { get; } // TODO: Binded

        bool IsSelfBinding { get; }
        Variable Variable { get; }

        ///<summary> Variable is not null </summary>
        bool IsDefined { get; }

        ///<summary> IsBinding && !IsSelfBinding </summary>
        bool IsVariableBinding { get; }

        // Runtime methods
        void SetupDependencies(GraphController graphController);

        // Editor Methods
        void Invalid();
        void SelfBind();
        void Bind(Variable variable);
        void Unbind();

        void SetBinding(string newGuid);
        void CopyParameter(IParameter otherParameter);
    }

    public interface IParameter<T> : IParameter
    {
        new T Value { get; set; }
    }
}
