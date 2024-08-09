using System.Collections.Generic;
using UnityEngine;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Core
{
    public abstract class GraphSubAsset : ScriptableObject
    {
        [SerializeField] protected string title;

        [ReadOnly, HideInGraphInspector]
        [SerializeField] private string guid;

        /// <summary> Genered by using UnityEditor.GUID </summary>
        public string Guid => guid;

        private string cachedInfo; // TODO: Delete this after compilation, if you rename the class a new bug will appear
        public virtual string Info
        {
            get
            {
                if (string.IsNullOrEmpty(cachedInfo))
                {
                    cachedInfo = this.GetTypeNiceString();
                }

                return cachedInfo;
            }
        }

        public float DeltaTime => GraphController.DeltaTime;
        protected GraphController GraphController { get; private set; }

        public void SetGuid(string newGuid, string parent = "")
        {
            guid = newGuid;
            name = parent + $"{GetType().Name} [{newGuid}]";
        }

        public void SetupDependencies(GraphController graphController, Dictionary<string, GraphSubAsset> subAssets)
        {
            GraphController = graphController;

            // Bind parameters dependencies
            foreach (IParameter parameter in this.GetAllFieldValuesTypeOf<IParameter>())
            {
                parameter.SetupDependencies(graphController);
            }

            SetupDependencies(subAssets);
        }

        /// <summary>
        /// Called at initialization to replace dependencies
        /// </summary>
        /// <param name="subAssets"> Key: GraphSubAsset.Guid, Value: Clone </param>
        protected virtual void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets) { }

        public virtual void Parse(Dictionary<string, GraphSubAsset> copies) { }

        public sealed override string ToString() => !string.IsNullOrEmpty(title) ? title : Info;
    }
}
