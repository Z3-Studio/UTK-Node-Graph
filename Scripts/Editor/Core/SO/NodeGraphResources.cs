using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Editor
{
    public class NodeGraphResources : ScriptableObject // TODO: Dictionary attribute
    {
        [Title("Visual Tree")]
        [SerializeField] private VisualTreeAsset analyzerWindowVT;
        [SerializeField] private VisualTreeAsset analizerSubAssetInfoVT;
        [SerializeField] private VisualTreeAsset validatorWindowVT;
        [SerializeField] private VisualTreeAsset validatorLogVT;
        [SerializeField] private VisualTreeAsset popupVT;
        [SerializeField] private VisualTreeAsset nodeGraphVT;
        [SerializeField] private VisualTreeAsset parameterVT;
        [SerializeField] private VisualTreeAsset btNodeVT;
        [SerializeField] private VisualTreeAsset smNodeVT;
        [SerializeField] private VisualTreeAsset smTransitionLabel;

        [Title("Styles")]
        [SerializeField] private StyleSheet nodeGraphSS;

        [Title("Icons")]
        [SerializeField] private Texture2D arrowTransition;
        [SerializeField] private Texture2D multipleArrowTransition;

        [Title("Icons")]
        [SerializeField] private Texture2D noIcon;
        [SerializeField] private Texture2D interruptor;
        [SerializeField] private Texture2D exclamation;
        [SerializeField] private Texture2D interrogation;
        [SerializeField] private Texture2D rightArrow;
        [SerializeField] private Texture2D upArrow;
        [SerializeField] private Texture2D downArrow;
        [SerializeField] private Texture2D repeat;
        [SerializeField] private Texture2D graphRunner;
        [SerializeField] private Texture2D subGraph;
        [SerializeField] private Texture2D parallel;
        [SerializeField] private Texture2D forEach;
        [SerializeField] private Texture2D invert;
        [SerializeField] private Texture2D remap;
        [SerializeField] private Texture2D waitUntil;
        [SerializeField] private Texture2D selector;
        [SerializeField] private Texture2D timeOut;
        [SerializeField] private Texture2D random;

        public static Texture2D ArrowTransition => Instance.arrowTransition;
        public static Texture2D MultipleArrowTransition => Instance.multipleArrowTransition;
        public static VisualTreeAsset AnalizerWindowVT => Instance.analyzerWindowVT;
        public static VisualTreeAsset AnalizerSubAssetInfoVT => Instance.analizerSubAssetInfoVT;
        public static VisualTreeAsset ValidatorWindowVT => Instance.validatorWindowVT;
        public static VisualTreeAsset ValidatorLogVT => Instance.validatorLogVT;
        public static VisualTreeAsset ParameterVT => Instance.parameterVT;
        public static VisualTreeAsset NodeGraphVT => Instance.nodeGraphVT;
        public static VisualTreeAsset BTNodeVT => Instance.btNodeVT;
        public static VisualTreeAsset SMNodeVT => Instance.smNodeVT;
        public static VisualTreeAsset SmTransitionLabel => Instance.smTransitionLabel;
        public static VisualTreeAsset PopupVT => Instance.popupVT;
        public static StyleSheet NodeGraphSS => Instance.nodeGraphSS;

        private static NodeGraphResources _Instance { get; set; }
        private static NodeGraphResources Instance
        {
            get
            {
                if (!_Instance)
                {
                    string path = $"Packages/{GraphPath.FullPackageName}/{nameof(NodeGraphResources)}.asset";
                    _Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<NodeGraphResources>(path);
                    //_Instance = GraphPath.LoadNgAssetPath<NodeGraphResources>();
                }

                return _Instance;
            }
        }

        public static Texture2D GetIGraphIcon(GraphIcon iconType) => Instance.GetIcon(iconType);

        private Texture2D GetIcon(GraphIcon iconType)
        {
            return iconType switch
            {
                GraphIcon.Sequencer => rightArrow,
                GraphIcon.ConditionTask => interrogation,
                GraphIcon.ActionTask => exclamation,
                GraphIcon.Repeater => repeat,
                GraphIcon.GraphRunner => graphRunner,
                GraphIcon.SubGraph => subGraph,
                GraphIcon.Enter => downArrow,
                GraphIcon.Exit => upArrow,
                GraphIcon.ForEach => forEach,
                GraphIcon.Invert => invert,
                GraphIcon.Remap => remap,
                GraphIcon.WaitUntil => waitUntil,
                GraphIcon.Parallel => parallel,
                GraphIcon.Selector => selector,
                GraphIcon.TimeOut => timeOut,
                GraphIcon.Interruptor => interruptor,
                GraphIcon.Random => random,
                _ => noIcon,
            };
        }
    }
}