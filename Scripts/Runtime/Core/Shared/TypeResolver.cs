using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;
using Z3.Utils.ExtensionMethods;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Z3.NodeGraph.Core
{
    public class Converter 
    {
        public string Description { get; set; }
        public ConvertionType type { get; set; }

        public Func<object, object> Method { get; set; } = _ => throw new NotImplementedException();
        public Type InType { get; set; }
        public Type OutType { get; set; }

        public Converter(ConvertionType convertionType)
        {
            type = convertionType;
            Description = convertionType.ToString().ToItalic();
        }
    }

    public enum ConvertionType
    {
        IsAssignableFrom,
        TypeConverter,
        Null
    }

    public static class TypeResolver
    {
        /// <summary> Key: (TOut, TIn) </summary>
        private static readonly Dictionary<(Type, Type), Converter> converters = new();

        private static Dictionary<string, List<Type>> userTypes;
        private static Dictionary<string, List<Type>> UserTypes
        {
            get
            {
                if (userTypes == null)
                {
                    SetUserTypes(new());
                }

                return userTypes;
            }
        }

        private static List<(string, Type)> cachedVariables;
        public static List<(string, Type)> CachedVariables
        {
            get
            {
                if (cachedVariables == null)
                {
                    List<(string, Type)> types = new();
                    IncludeTypes(types, "System", SystemTypes);
                    IncludeTypes(types, "Unity Engine", UnityTypes);

                    foreach ((string name, List<Type> customTypes) in UserTypes)
                    {
                        IncludeTypes(types, name, customTypes);
                    }

                    cachedVariables = types;
                }

                return cachedVariables.ToList();
            }
        }

        #region Variable List
        public static Dictionary<string, Type> SystemTypes { get; } = new()
        {
            { "Bool" , typeof(bool) },
            { "Int" , typeof(int) },
            { "Float", typeof(float) },
            { "String" , typeof(string) },
            //{ "Double" , typeof(double) }, // Deserialiation error
            //{ "Long" , typeof(long) }, // Deserialiation error
             //typeof(object), // Missing field view
             //typeof(Type),  // Missing field view
             // Array, Structs/Property, 
        };

        public static List<Type> UnityTypes { get; } = new()
        {
             // Custom Structs
             typeof(BoundsInt),
             typeof(Bounds),
             typeof(Color),
             typeof(LayerMask),
             typeof(Matrix4x4),
             typeof(Quaternion),
             typeof(RectInt),
             typeof(Rect),
             typeof(Vector2Int),
             typeof(Vector2),
             typeof(Vector3Int),
             typeof(Vector3),
             typeof(Vector4),
             //typeof(ExposedReference),
             //typeof(ManagedReference),
             // Basic
             typeof(UnityEngine.Object),
             typeof(ScriptableObject),
             typeof(GameObject),
             typeof(Transform),
             typeof(Component),
             typeof(MonoBehaviour),
             // Animation
             typeof(Animator),
             typeof(AnimationClip),
             typeof(AnimationCurve),
             // Physics
             typeof(Rigidbody),
             typeof(Rigidbody2D),
             typeof(CharacterController),
             typeof(Collider),
             typeof(Collider2D),
             typeof(RaycastHit),
             typeof(RaycastHit2D),
             // Renderer
             typeof(Camera),
             typeof(Light),
             typeof(Material),
             typeof(Texture),
             typeof(Texture2D),
             typeof(Renderer),
             typeof(Sprite),
             typeof(SpriteRenderer),
             typeof(MeshRenderer),
             typeof(SkinnedMeshRenderer),
             // Audio
             typeof(AudioMixer),
             typeof(AudioListener),
             typeof(AudioClip),
             typeof(AudioSource),
             // AI
             typeof(NavMeshAgent),
             // UI
             typeof(Button),
             typeof(Slider),
             typeof(Text),
             // Others
             typeof(TextAsset),
        };
        #endregion

        public static void SetUserTypes(List<string> namespaces)
        {
            Dictionary<string, List<Type>> types = new Dictionary<string, List<Type>>();
            

            // Get all assemblies loaded in the current AppDomain
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Loop through each assembly
            foreach (Assembly assembly in assemblies)
            {
                // Find assembly that match with the name
                string matchName = namespaces.FirstOrDefault(ns => assembly.FullName.StartsWith(ns));
                if (string.IsNullOrEmpty(matchName))
                    continue;

                List<Type> assemblyTypes = new List<Type>();

                // Filter by valid types in the namespace
                foreach (Type type in assembly.GetTypes())
                {
                    // Check if the namespace starts with the base namespace
                    if (type.IsGenericTypeDefinition || type.Namespace == null || !type.Namespace.StartsWith(matchName) || type.MemberType != MemberTypes.TypeInfo)
                        continue;

                    assemblyTypes.Add(type);
                }

                if (assemblyTypes.Count == 0)
                    continue;
                
                // Check if the path exist, and add types
                if (types.TryGetValue(matchName, out List<Type> items))
                {
                    items.AddRange(assemblyTypes);
                }
                else
                {
                    types.Add(matchName, assemblyTypes);
                }
            }

            userTypes = types;
            cachedVariables = null;
        }

        #region Validator
        public static bool CanConvert(IParameter parameter, IVariable variable)
        {
            Converter get = GetGetConverterType(parameter, variable);

            if (get.type != ConvertionType.Null)
                return true;

            Converter set = GetSetConverterType(parameter, variable);
            return set.type != ConvertionType.Null;
        }

        public static Converter GetGetConverterType(IParameter parameter, IVariable variable) // TODO: Review return
        {
            if (parameter.GenericType.IsAssignableFrom(variable.OriginalType))
                return new Converter(ConvertionType.IsAssignableFrom);

            if (TryGetConverter(parameter.GenericType, variable.OriginalType, out Converter converter))
                return converter;

            return new Converter(ConvertionType.Null);
        }

        public static Converter GetSetConverterType(IParameter parameter, IVariable variable) // TODO: Review return
        {
            if (parameter.GenericType.IsAssignableFrom(variable.OriginalType))
                return new Converter(ConvertionType.IsAssignableFrom);

            if (TryGetConverter(variable.OriginalType, parameter.GenericType, out Converter converter))
                return converter;

            return new Converter(ConvertionType.Null);
        }
        #endregion

        #region Converter
        public static Func<object> CreateGet(IParameter p, IVariable v)
        {
            if (p.GenericType.IsAssignableFrom(v.OriginalType))
            {
                return () => v.Value;
            }

            // Custom converters
            if (TryGetConverter(p.GenericType, v.OriginalType, out Converter converter))
            {
                return () => converter.Method(v.Value);
            }
            
            return null;
        }

        public static Action<object> CreateSet(IParameter p, IVariable v)
        {
            if (v.OriginalType.IsAssignableFrom(p.GenericType))
            {
                return (newValue) => v.Value = newValue;
            }

            // Custom converters
            if (TryGetConverter(v.OriginalType, p.GenericType, out Converter converter))
            {
                return (newValue) => v.Value = converter.Method(newValue);
            }

            return null;
        }

        public static Func<object, object> CreateConverter(IParameter p, IVariable v)
        {
            if (v.OriginalType.IsAssignableFrom(p.GenericType))
            {
                return (newValue) => v.Value = newValue;
            }

            // Custom converters
            if (TryGetConverter(v.OriginalType, p.GenericType, out Converter converter))
            {
                return (newValue) => v.Value = converter.Method(newValue);
            }

            return null;
        }
        
        public static bool TryGetConverter(Type typeOut, Type typeIn, out Converter converter)
        {
            converter = GetConverter(typeOut, typeIn);
            return converter != null;
        }

        public static Converter GetConverter(Type typeOut, Type typeIn)
        {
            if (typeOut == null || typeIn == null)
                return null;

            (Type, Type) key = (typeOut, typeIn);
            if (converters.TryGetValue(key, out Converter converter))
                return converter;
            
            // Next time it will be finded by dictionary
            Dictionary<(Type, Type), Converter> clone = new(converters);
            foreach (((Type keyOut, Type keyIn), Converter converterValue) in clone)
            {
                if (keyOut.IsAssignableFrom(typeOut) && keyIn.IsAssignableFrom(typeIn))
                {
                    // Found valid convertion
                    converters[key] = converterValue; 
                    return converterValue;
                }
            }

            // There is not convertion
            converters[key] = null;
            return null;
        }
        #endregion

        #region Conversor Registry
        public static void GetAndCreateMaps(ITypeConverter converter)
        {
            Type objectType = converter.GetType();

            foreach (Type interfaceType in objectType.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                    continue;

                if (interfaceType.GetGenericTypeDefinition() == typeof(ITypeConverter<,>))
                {
                    InterfaceMapping map = objectType.GetInterfaceMap(interfaceType);

                    Type[] genericArguments = interfaceType.GetGenericArguments();

                    (Type, Type) key = (genericArguments[0], genericArguments[1]);
                    MethodInfo method = map.TargetMethods[0]; // Could exist a better way

                    AddConverter(converter, key, method);
                }
                else if (interfaceType.GetGenericTypeDefinition() == typeof(ITypeConveterCreator<,>))
                {
                    InterfaceMapping map = objectType.GetInterfaceMap(interfaceType);

                    Type[] genericArguments = interfaceType.GetGenericArguments();

                    (Type, Type) key = (genericArguments[0], genericArguments[1]);
                    MethodInfo method = map.TargetMethods[0]; // Could exist a better way

                    AddConverter(converter, key, method);
                }
            }
        }

        public static void CreateMap<TOut, TIn>(ITypeConverter<TOut, TIn> converter)
        {
            (Type, Type) key = (typeof(TOut), typeof(TIn));
            MethodInfo method = typeof(ITypeConverter<TOut, TIn>).GetMethod(nameof(ITypeConverter<TOut, TIn>.Convert));

            AddConverter(converter, key, method);
        }

        private static void AddConverter(ITypeConverter converter, (Type, Type) key, MethodInfo method)
        {
            DescriptionAttribute attribute = method.GetCustomAttribute<DescriptionAttribute>();

            converters[key] = new Converter(ConvertionType.TypeConverter)
            {
                Method = (input) => method.Invoke(converter, new object[] { input }),
                Description = attribute != null ? attribute.Description : "Empty description",
                OutType = key.Item1,
                InType = key.Item2
            };
        }

        #endregion

        private static void IncludeTypes(List<(string, Type)> types, string subName, List<Type> typesList)
        {
            foreach (Type type in typesList)
            {
                types.Add(($"{subName}/{type.GetTypeNiceString()}", type));
            }
        }
        private static void IncludeTypes(List<(string, Type)> types, string subName, Dictionary<string, Type> typesList)
        {
            foreach ((string name, Type type) in typesList)
            {
                types.Add(($"{subName}/{name}", type));
            }
        }
    }
}