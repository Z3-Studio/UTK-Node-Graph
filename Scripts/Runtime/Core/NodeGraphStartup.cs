namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Initialize depedencies in runtime
    /// </summary>
    public static class NodeGraphStartup
    {
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
#endif
        public static void Init()
        {
            TypeResolver.Init();
        }
    }
}
