using System;

namespace Z3.NodeGraph.Core
{
    public interface IVariable
    {
        string Name { get; }
        object Value { get; set; } // TODO: Review set
        string Guid { get; }
        Type OriginalType { get; }
    }
}
