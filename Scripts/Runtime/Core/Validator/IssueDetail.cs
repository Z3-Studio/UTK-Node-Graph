using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    public enum AnalysisType
    {
        CorruptedAsset,
        NullSubAsset,
        MissingNode,
        MissingSubAsset,
        MissingBinding,
        Other
    }

    public enum AnalysisCriticality
    {
        /// <summary> It must to be fixed </summary>
        Error,

        /// <summary> High probability of generating error and inconsistency in logic </summary>
        Warning,

        /// <summary> This will not generate an error, but it may cause inconsistency in your logic. </summary>
        Info
    }

    public class IssueDetail
    {
        public GraphSubAsset Context { get; }
        public AnalysisType Type { get; }
        public AnalysisCriticality Criticality { get; }
        public string Description { get; }

        public IssueDetail(GraphSubAsset context, AnalysisType type, AnalysisCriticality criticality, string description)
        {
            Context = context;
            Type = type;
            Criticality = criticality;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Type.ToStringBold()}: {Description}";
        }
    }
}
