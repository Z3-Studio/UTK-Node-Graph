using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils;

namespace Z3.NodeGraph.Editor
{
    public class HideInGraphBuilder //: Z3AttributeDrawer<HideInGraphInspectorAttribute>
    {
        internal static void HideObjects(object target, VisualElement root)
        {
            IEnumerable<FieldInfo> query = ReflectionUtils.GetAllMembersByAttribute<FieldInfo>(target, typeof(HideInGraphInspectorAttribute), "UnityEngine");

            // Checar um field por um, até que acabe a lista ou a classe impleemente readonly
            foreach (FieldInfo field in query)
            {
                VisualElement element = root.GetProperty(field.Name);
                if (element != null)
                {
                    element.parent.Remove(element);
                }
            }

            NGVisualElement z3element = root.Q<NGVisualElement>();
            if (z3element != null)
            {
                z3element.HideEditor();
            }
        }
    }
}