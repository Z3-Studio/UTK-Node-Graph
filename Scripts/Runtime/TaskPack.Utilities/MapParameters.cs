using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Auto Mapper copy all Properties from 'data' and paste to 'ReferenceVariables'")]
    public class MapParameters : ActionTask
    {
        [SerializeField] protected Parameter<ScriptableObject> data;

        public override string Info => $"Map Parameters {data}";

        protected override void StartAction()
        {
            //if (!Mapper.HasMap(data.GenericType, GraphRunner.ReferenceVariables.GetType()))
            //{

            //}

            PropertyInfo[] originProperties = data.Value.GetType().GetProperties();

            Dictionary<string, PropertyInfo> propertyMap = new();
            foreach (PropertyInfo destinationProperty in originProperties)
            {
                MapToAttribute mapTo = destinationProperty.GetCustomAttribute<MapToAttribute>();

                if (mapTo == null)
                {
                    propertyMap[destinationProperty.Name] = destinationProperty;
                }
                else
                {
                    propertyMap[mapTo.Parameter] = destinationProperty;
                }
            }

            List<FieldInfo> originFields = ReflectionUtils.GetAllFields(data.Value);

            Dictionary<string, FieldInfo> fieldMap = new();
            foreach (FieldInfo destinationProperty in originFields)
            {
                MapToAttribute mapTo = destinationProperty.GetCustomAttribute<MapToAttribute>();

                if (mapTo == null)
                {
                    fieldMap[destinationProperty.Name] = destinationProperty;
                }
                else
                {
                    fieldMap[mapTo.Parameter] = destinationProperty;
                }
            }

            foreach (VariableInstance variable in GraphController.ReferenceVariables.Values)
            {
                if (propertyMap.TryGetValue(variable.Name, out PropertyInfo propertyInfo) && variable.OriginalType == propertyInfo.PropertyType)
                {
                    variable.Value = propertyInfo.GetValue(data.Value);
                    continue;
                }
                else if (fieldMap.TryGetValue(variable.Name, out FieldInfo fieldInfo) && variable.OriginalType == fieldInfo.FieldType)
                {
                    variable.Value = fieldInfo.GetValue(data.Value);
                }
            }

            EndAction();
        }
    }

    public class Mapper
    {
        private static readonly Dictionary<(Type, Type), List<(PropertyInfo, PropertyInfo)>> mapping = new();

        public static bool HasMap(Type sourceType, Type destionationType) => mapping.ContainsKey((sourceType, destionationType));

        public static bool HasMap<TSource, TDestination>() => HasMap(typeof(TSource), typeof(TDestination));

        public static void CreateMap<TSource, TDestination>() where TDestination : new()
        {
            CreateMap(typeof(TSource), typeof(TSource));
        }

        public static void CreateMap(Type sourceType, Type destionationType)
        {
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            PropertyInfo[] destinationProperties = destionationType.GetProperties();

            List<(PropertyInfo, PropertyInfo)> propertyMapping = new();

            Dictionary<string, PropertyInfo> destionationMap = new();
            foreach (PropertyInfo destinationProperty in destinationProperties) 
            {
                MapToAttribute mapTo = destinationProperty.GetCustomAttribute<MapToAttribute>();

                if (mapTo == null)
                {
                    destionationMap[destinationProperty.Name] = destinationProperty;
                }
                else
                {
                    destionationMap[mapTo.Parameter] = destinationProperty;
                }
            }

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                if (!destionationMap.TryGetValue(sourceProperty.Name, out PropertyInfo destinationProperty))
                    continue;

                // Simple implementation, you can improve it creating converter system
                if (sourceProperty.PropertyType != destinationProperty.PropertyType)
                    continue;

                propertyMapping.Add((sourceProperty, destinationProperty));
            }

            mapping[(sourceType, destionationType)] = propertyMapping;
        }

        public static TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            TDestination destination = Activator.CreateInstance<TDestination>();

            List<(PropertyInfo, PropertyInfo)> propertyMapping = mapping[(typeof(TSource), typeof(TDestination))];

            foreach ((PropertyInfo itemToGet, PropertyInfo itemToSet) in propertyMapping)
            {
                itemToSet.SetValue(destination, itemToGet.GetValue(source));
            }

            return destination;
        }
    }
}
