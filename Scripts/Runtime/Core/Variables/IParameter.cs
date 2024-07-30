using System;

namespace Z3.NodeGraph.Core
{
    public interface IParameter
    {
        object Value { get; set; }
        string Guid { get; }
        Type GenericType { get; }
        bool IsBinding { get; } // TODO: Binded

        ///<summary> The variable is not null </summary>
        bool IsDefined { get; }
        bool IsSelfBind { get; }
        Variable Variable { get; }

        // Runtime methods
        void SetupDependencies(GraphController graphController);

        // Editor Methods
        void Invalid();
        void SelfBind();
        void Bind(Variable variable);
        void Unbind();
    }

    public interface IParameter<T> : IParameter
    {
        new T Value { get; set; }
    }
}
