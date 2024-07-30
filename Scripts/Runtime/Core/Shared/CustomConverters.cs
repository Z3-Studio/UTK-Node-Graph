using System;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;
using Description = System.ComponentModel.DescriptionAttribute;

namespace Z3.NodeGraph.Core
{
    public class CustomConverters :
        // Primitive
        ITypeConverter<float, string>,
        ITypeConverter<int, string>,
        ITypeConverter<bool, string>,
        ITypeConverter<string, object>,

        // Unity two-way conversion
        ITypeConverter<GameObject, Component>, ITypeConverter<Component, GameObject>,
        ITypeConverter<Vector3, Vector2>, ITypeConverter<Vector2, Vector3>,
        ITypeConverter<Vector3, Quaternion>, ITypeConverter<Quaternion, Vector3>,

        // Unity one-way conversion
        ITypeConverter<Quaternion, GameObject>, 
        ITypeConverter<Quaternion, Component>,
        ITypeConverter<Vector3, GameObject>, 
        ITypeConverter<Vector3, Component>,
        ITypeConverter<Vector2, GameObject>, 
        ITypeConverter<Vector2, Component>,
        ITypeConverter<bool, Object>,
        ITypeConverter<Transform, GameObject>,
        ITypeConverter<Transform, Component>,

        // Custom Func
        ITypeConveterCreator<IConvertible, IConvertible>,
        ITypeConveterCreator<Component, Component>
    {
        #region Primitive
        [Description(nameof(float.TryParse))]
        float ITypeConverter<float, string>.Convert(string source)
        {
            return source.ToFloat();
        }

        [Description(nameof(int.TryParse))]
        int ITypeConverter<int, string>.Convert(string source)
        {
            return source.ToInt();
        }

        [Description(nameof(bool.TryParse))]
        bool ITypeConverter<bool, string>.Convert(string source)
        {
            return source.ToBool();
        }

        [Description(nameof(object.ToString))]
        string ITypeConverter<string, object>.Convert(object source)
        {
            return $"{source}";
        }
        #endregion

        #region Unity two-way conversion
        [Description(nameof(Component.GetComponent))]
        Component ITypeConverter<Component, GameObject>.Convert(GameObject source)
        {
            return source.GetComponent<Component>();
        }

        [Description("component.gameObject")]
        GameObject ITypeConverter<GameObject, Component>.Convert(Component source)
        {
            return source.gameObject;
        }

        [Description("quaternion.eulerAngles")]
        Vector3 ITypeConverter<Vector3, Quaternion>.Convert(Quaternion source)
        {
            return source.eulerAngles;
        }

        [Description(nameof(Quaternion.Euler))]
        Quaternion ITypeConverter<Quaternion, Vector3>.Convert(Vector3 source)
        {
            return Quaternion.Euler(source);
        }

        [Description("implicit operator")]
        Vector2 ITypeConverter<Vector2, Vector3>.Convert(Vector3 source)
        {
            return source;
        }

        [Description("implicit operator")]
        Vector3 ITypeConverter<Vector3, Vector2>.Convert(Vector2 source)
        {
            return source;
        }
        #endregion

        #region Unity one-way conversion
        [Description("implicit operator")]
        bool ITypeConverter<bool, Object>.Convert(Object source)
        {
            return source;
        }

        [Description("gameObject.transform.rotation")]
        Quaternion ITypeConverter<Quaternion, GameObject>.Convert(GameObject source)
        {
            return source.transform.rotation;
        }

        [Description("component.transform.rotation")]
        Quaternion ITypeConverter<Quaternion, Component>.Convert(Component source)
        {
            return source.transform.rotation;
        }

        [Description("gameObject.transform.position")]
        Vector3 ITypeConverter<Vector3, GameObject>.Convert(GameObject source)
        {
            return source.transform.position;
        }

        [Description("component.transform.position")]
        Vector3 ITypeConverter<Vector3, Component>.Convert(Component source)
        {
            return source.transform.position;
        }

        [Description("gameObject.transform.position")]
        Vector2 ITypeConverter<Vector2, GameObject>.Convert(GameObject source)
        {
            return source.transform.position;
        }

        [Description("component.transform.position")]
        Vector2 ITypeConverter<Vector2, Component>.Convert(Component source)
        {
            return source.transform.position;
        }

        [Description("gameObject.transform")]
        Transform ITypeConverter<Transform, GameObject>.Convert(GameObject source)
        {
            return source.transform;
        }

        [Description("component.transform")]
        Transform ITypeConverter<Transform, Component>.Convert(Component source)
        {
            return source.transform;
        }

        [Description("fromType.GetComponent(toType)")]
        Func<Component, Component> ITypeConveterCreator<Component, Component>.CreateConverter(Type fromType, Type toType)
        {
            Component previousVariableValue = null;
            Component cachedComponent = null;

            // Cache component to avoid unnecessary GetComponents calls
            return (value) =>
            {
                if (previousVariableValue != value)
                {
                    previousVariableValue = value;
                    cachedComponent = null;
                }

                if (cachedComponent == null)
                {
                    cachedComponent = value ? value.GetComponent(toType) : null;
                }

                return cachedComponent;
            };
        }

        [Description("fromType.GetComponent(toType)")]
        Func<IConvertible, IConvertible> ITypeConveterCreator<IConvertible, IConvertible>.CreateConverter(Type fromType, Type toType)
        {
            return (value) =>
            {
                try
                {
                    return (IConvertible)Convert.ChangeType(value, toType);
                }
                catch
                {
                    return default;
                }
            };
        }
        #endregion
    }
}