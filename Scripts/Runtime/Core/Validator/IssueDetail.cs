using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    public enum AnalysisType
    {
        /// <summary> Missing script, typically due to deletion or mismatch with meta file. </summary>
        CorruptedAsset,

        /// <summary> Null field in the SubAssets list. </summary>
        NullAsset,

        /// <summary> Item not present in the SubAssets list. </summary>
        NotIncludedAsset,

        /// <summary> Invalid, empty, or already used GUID for a sub-asset. </summary>
        InvalidAssetGuid,

        /// <summary> Asset present in the SubAssets list, but is not a Node and not referenced by other assets. </summary>
        UnreferencedAsset,

        /// <summary> Parameter has a GUID but does not match any Variable. </summary>
        MissingBinding,

        /// <summary> SubAsset appears more than once in the SubAsset list. </summary>
        DuplicateAsset,

        /// <summary> The graph contains Nodes, but no definition for the StartNode is set. </summary>
        StartNodeNotDefined,

        /// <summary> SubAsset name does not match type and guid </summary>
        /// <remarks> Not implemented </remarks>
        InvalidAssetName,

        /// <summary> Other types of issues not classified. </summary>
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
