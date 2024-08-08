using System;

namespace Z3.NodeGraph.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MustBindAttribute : Attribute
    {
        public MustBindAttribute() { }
    }
}
