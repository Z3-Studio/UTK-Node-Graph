using System;

namespace Z3.NodeGraph.Core
{
    public enum AutoBindType
    {
        None,
        SelfBind,
        FindSameVariable,
        FindSimilarVariable
    }

    /// <summary>
    /// Attribute is an automation for creating new GraphSubAssets
    /// </summary>
    /// <remarks>
    /// For self binding you also can set you parameter using <see cref="Parameter{T}.MakeSelfBinding"/>.
    /// This can be useful in newly created fields.
    /// <para>  Declaration example:  </para>
    /// <para>  
    /// [SerializeField] private <see cref="Parameter{T}"/> data = <see cref="Parameter{T}.MakeSelfBinding"/>;
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class ParameterDefinitionAttribute : Attribute 
    {
        public bool Get { get; }
        public bool Set { get; }
        public AutoBindType AutoBindType { get; }

        public ParameterDefinitionAttribute(AutoBindType autoBindType = AutoBindType.None, bool get = true, bool set = true)
        {
            AutoBindType = autoBindType;
            Get = get;
            Set = set;
        }
    }
}
