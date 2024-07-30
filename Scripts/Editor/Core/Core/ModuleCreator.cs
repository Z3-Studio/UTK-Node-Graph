using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    [InitializeOnLoad]
    public static class ModuleCreator
    {
        /// <summary>
        /// Key: Graph, Value Module
        /// </summary>
        public static Dictionary<Type, Type> Modules;

        static ModuleCreator()
        {
            List<Type> types = TypeCache.GetTypesDerivedFrom(typeof(NodeGraphModule<>)).Where(t => !t.IsAbstract).ToList();

            Modules = new();
            foreach (Type t in types)
            {
                Type baseType = t.BaseType;

                while (baseType != null)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(NodeGraphModule<>).GetGenericTypeDefinition())
                    {
                        Type graphType = baseType.GetGenericArguments()[0];
                        Modules[graphType] = t;
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }
        }

        public static NodeGraphModule GetModule(GraphData graphData, NodeGraphReferences references)
        {
            Type type = graphData.GetType();
            NodeGraphModule newModule = Activator.CreateInstance(Modules[type]) as NodeGraphModule;
            newModule.Construct(references);
            return newModule;
        }
    }
}