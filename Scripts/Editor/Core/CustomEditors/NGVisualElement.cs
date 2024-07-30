using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// <see cref="Core.HideInGraphInspectorAttribute"/>
    /// </summary>
    public class NGVisualElement : VisualElement
    {
        public virtual object Target { get; }

        private List<VisualElement> objectToHide = new();

        public void Add(VisualElement element, bool hideInGraph)
        {
            Add(element);
            if (hideInGraph)
            {
                objectToHide.Add(element);
            }
        }

        public void HideEditor()
        {
            foreach (VisualElement element in objectToHide)
            {
                element.style.SetDisplay(false);
            }
        }
    }

    public class NGVisualElement<T> : NGVisualElement where T : Object
    {
        private T target;

        public override object Target => target;

        internal void AddTarget(T target)
        {
            this.target = target;
        }
    }

    public abstract class NGEditor<TObject> : Z3Editor<TObject> where TObject : Object
    {

        /// <summary> Z3VisualElement<Object> </summary>
        private NGVisualElement<TObject> root;

        public sealed override VisualElement CreateInspectorGUI()
        {
            root = new NGVisualElement<TObject>();
            root.AddTarget(Target);
            CreateInspector();
            return root;
        }

        public virtual void CreateInspector()
        {
            // Create the base editor, but don't need to hide
            Add(CreateBase(), false);
        }

        protected VisualElement CreateBase() => base.CreateInspectorGUI();

        protected void Add(VisualElement element, bool hideInGraph) => root.Add(element, hideInGraph);
    }
}