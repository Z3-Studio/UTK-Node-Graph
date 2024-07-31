using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.Core
{
    public class GraphPath
    {
        public const string Graph = Z3Path.ScriptableObjects + "Node Graph/"; // TODO: Move to 

        public const string MenuPath = Z3Path.MenuPath + "Node Graph/";

        public const string ComponentMenu = "Z3 Node Graph/";
        public static string PublicResources => "Assets/Editor";
        public static string Resources => "Packages/" + FullPackageName;
        public static string FullPackageName => Z3Path.PackageCompanyName + "." + PackageName;
        public static string PackageName => "node-graph";

        public static T LoadNgAssetPath<T>() where T : ScriptableObject
        {
            return Z3Path.LoadAssetPath<T>("NodeGraph");
        }
    }
}
