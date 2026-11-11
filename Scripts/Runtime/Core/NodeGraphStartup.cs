namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Initialize depedencies in runtime
    /// </summary>
    public static class NodeGraphStartup
    {
#if !UNITY_EDITOR
        // Note: This is called in editor by NodeGraphEditorStartup, but in runtime is by this attribute
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]
#endif
        public static void Init()
        {
            TypeResolver.Init();
        }
    }
}
