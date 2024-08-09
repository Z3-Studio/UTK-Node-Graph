using System.Collections.Generic;
using System.Linq;

namespace Z3.NodeGraph.Core
{
    public static class NodeGraphExtensions
    {
        public static void SendEvent<T>(this GraphRunner graphRunner, string eventname, T value)
        {
            graphRunner.Events.SendCustomEvent(eventname, value);
        }

        public static void SendEvent(this GraphRunner graphRunner, string eventname, object value = null)
        {
            graphRunner.Events.SendCustomEvent(eventname, value);
        }

        public static void ReplaceDependencies<T>(this IList<T> dependencies, Dictionary<string, GraphSubAsset> subAssets) where T : GraphSubAsset
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                dependencies[i] = (T)subAssets[dependencies[i].Guid];
            }
        }

        public static void Parse<T>(this IList<T> dependencies, Dictionary<string, GraphSubAsset> subAssets) where T : GraphSubAsset
        {
            List<T> copy = dependencies.ToList();
            dependencies.Clear();

            foreach (T item in copy)
            {
                if (subAssets.TryGetValue(item.Guid, out GraphSubAsset value))
                {
                    dependencies.Add((T)value);
                }
            }
        }
    }
}
