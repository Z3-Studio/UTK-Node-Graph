using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.Utils;
using Z3.Utils.Editor;

namespace Z3.NodeGraph.Editor
{
    // TODO: ProjectPreferences to avoid nameSpaces ignored by git
    // Review it: consider using NodeGraph.Editor namespace
    public class UserPreferences : ScriptableObject
    {
        [SerializeField] private bool openWelcome = true;
        [SerializeField] private AutoBindType defaultAutoBindType = AutoBindType.FindSimilarVariable;
        [SerializeField] private List<string> nameSpaces;

        [ReadOnly]
        [SerializeField] private List<GraphPreference> graphPreferences = new();

        public static AutoBindType DefaultAutoBindType => Instance.defaultAutoBindType;

        public static bool OpenWelcome
        {
            get => Instance.openWelcome;
            set
            {
                Instance.openWelcome = value;
                EditorUtility.SetDirty(Instance);
            }
        }

        private static UserPreferences Instance { get; set; }

        public static void Init()
        {
            if (Instance == null)
            {
                string path = $"{GraphPath.PublicResources}/Z3/{nameof(UserPreferences)}.asset";
                Instance = EditorUtils.LoadOrCreateAsset<UserPreferences>(path); 

                //Instance = GraphPath.LoadNgAssetPath<UserPreferences>();
                Instance.Validate();
            }

            RefreshTypes();
        }

        [Button]
        private static void RefreshTypes()
        {
            // TODO: Create a editor inspector class, to find and include types, and call this method
            List<string> namespaces = Instance.nameSpaces
               .Where(n => !string.IsNullOrEmpty(n))
               .ToList();

            TypeResolver.SetUserTypes(namespaces);
        }

        public static GraphPreference GetGraphPreference(GraphData graph)
        {
            GraphPreference preferences = Instance.graphPreferences.FirstOrDefault(p => p.Graph == graph);
           
            if (preferences == null && graph)
            {
                preferences = new GraphPreference(graph);
                Instance.graphPreferences.Add(preferences);
                SetInstanceDirty();

            }

            return preferences;
        }

        private void Validate()
        {
            // Remove empty slots
            int nullCount = CollectionUtils.ClearNullItems(graphPreferences, (i) => i.Graph == null);

            if (nullCount > 0)
            {
                Debug.Log($"Removed '{nullCount}' null assets from {nameof(UserPreferences)}");
                EditorUtility.SetDirty(this);
            }
        }

        public static void SetInstanceDirty()
        {
            EditorUtility.SetDirty(Instance);
        }
    }
}