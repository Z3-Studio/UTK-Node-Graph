using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{

    /// <summary>
    /// Display all types derived from TType
    /// </summary>
    public class TypeSelectorPopup<TType> : SelectorPopup<Type>
    {
        private TypeSelectorPopup(Action<string, Type> onSelectType, string title) : base(onSelectType, title)
        {
            Type baseType = typeof(TType);
            Type[] types = TypeCache.GetTypesDerivedFrom(baseType).Where(t => !t.IsAbstract).ToArray(); 

            foreach (Type type in types)
            {
                NodeCategoryAttribute category = type.GetCustomAttribute<NodeCategoryAttribute>();

                string name = string.Empty;
                if (category != null)
                {
                    name = category.Path + '/';
                }

                Items.Add(new(name + type.Name.GetNiceString(), type));
            }
        }

        public new static void OpenWindow(Action<string, Type> onSelectType)
        {
            OpenWindow(onSelectType, $"Add New {typeof(TType).Name.GetNiceString()}");
        }

        public static void OpenWindow(Action<string, Type> onSelectType, string title) 
        {
            TypeSelectorPopup<TType> instance = new(onSelectType, title);
            instance.OpenGenericPopup();
        }
    }
}