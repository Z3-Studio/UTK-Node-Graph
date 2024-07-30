using System;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.Core
{
    public static class NodeGraphStartup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void Init()
        {
            foreach (Type item in ReflectionUtils.GetDeriveredConcreteTypes<ITypeConverter>())
            {
                ITypeConverter conversor = (ITypeConverter)Activator.CreateInstance(item);
                TypeResolver.GetAndCreateMaps(conversor);
            }

#if UNITY_EDITOR
            Validator.Init();
#endif
        }
    }
}
