using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProjectAction.AutoAttributes
{
    public readonly struct AutoFieldMetadata
    {
        public AutoFieldMetadata(
            FieldInfo field,
            AutoComponentAttribute attribute,
            Type fieldType,
            Type elementType,
            bool isList)
        {
            Field = field;
            Attribute = attribute;
            FieldType = fieldType;
            ElementType = elementType;
            IsList = isList;
        }

        public FieldInfo Field { get; }
        public AutoComponentAttribute Attribute { get; }
        public Type FieldType { get; }
        public Type ElementType { get; }
        public bool IsList { get; }
    }

    public static class AutoComponentBinder
    {
        private const BindingFlags INSTANCE_FIELD_FLAGS =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Dictionary<Type, IReadOnlyDictionary<string, AutoFieldMetadata>> FIELD_CACHE = new();

        public static T Resolve<T>(MonoBehaviour owner, T currentValue, string fieldName) where T : class
        {
            if (owner == null || currentValue != null || string.IsNullOrWhiteSpace(fieldName))
            {
                return currentValue;
            }

            var metadataMap = GetFieldMetadataMap(owner.GetType());
            var metadata = GetMetadata(metadataMap, fieldName);
            if (metadata == null)
            {
                return currentValue;
            }

            var resolved = ResolveFieldValue(owner, metadata.Value) as T;
            if (resolved == null)
            {
                return currentValue;
            }

            metadata.Value.Field.SetValue(owner, resolved);
            return resolved;
        }

        public static int Bind(MonoBehaviour owner, bool overwriteExisting)
        {
            if (owner == null)
            {
                return 0;
            }

            var updateCount = 0;
            var metadataMap = GetFieldMetadataMap(owner.GetType());
            metadataMap.Values.ForEach(metadata =>
            {
                var currentValue = metadata.Field.GetValue(owner);
                if (!overwriteExisting && HasValue(currentValue))
                {
                    return;
                }

                var resolved = ResolveFieldValue(owner, metadata);
                if (!HasValue(resolved))
                {
                    return;
                }

                metadata.Field.SetValue(owner, resolved);
                updateCount++;
            });

            return updateCount;
        }

        public static bool HasAutoFields(Type targetType)
        {
            return GetFieldMetadataMap(targetType).Count > 0;
        }

        private static object ResolveFieldValue(MonoBehaviour owner, AutoFieldMetadata metadata)
        {
            return metadata.Attribute.Mode switch
            {
                AutoComponentLookupMode.GetComponent =>
                    owner.GetComponent(metadata.ElementType),
                AutoComponentLookupMode.GetComponentInParent =>
                    owner.GetComponentInParent(metadata.ElementType, metadata.Attribute.IncludeInactive),
                AutoComponentLookupMode.GetComponentInChildren =>
                    owner.GetComponentInChildren(metadata.ElementType, metadata.Attribute.IncludeInactive),
                AutoComponentLookupMode.GetComponentsInChildren =>
                    ResolveChildrenCollection(owner, metadata),
                _ => null
            };
        }

        private static object ResolveChildrenCollection(MonoBehaviour owner, AutoFieldMetadata metadata)
        {
            var components = owner.GetComponentsInChildren(metadata.ElementType, metadata.Attribute.IncludeInactive);
            if (components == null || components.Length == 0)
            {
                return null;
            }

            var orderedComponents = Enumerable.Range(0, components.Length)
                .Select(index => components[index])
                .ToArray();

            if (metadata.FieldType.IsArray)
            {
                var array = Array.CreateInstance(metadata.ElementType, orderedComponents.Length);
                Enumerable.Range(0, orderedComponents.Length)
                    .ForEach(index => array.SetValue(orderedComponents[index], index));
                return array;
            }

            if (!metadata.IsList || Activator.CreateInstance(metadata.FieldType) is not IList list)
            {
                return null;
            }

            orderedComponents.ForEach(component => list.Add(component));
            return list;
        }

        private static IReadOnlyDictionary<string, AutoFieldMetadata> GetFieldMetadataMap(Type targetType)
        {
            var cached = FIELD_CACHE.ContainsKey(targetType)
                ? FIELD_CACHE[targetType]
                : null;
            if (cached != null)
            {
                return cached;
            }

            var metadataMap = targetType
                .GetFields(INSTANCE_FIELD_FLAGS)
                .Select(CreateMetadata)
                .Where(metadata => metadata != null)
                .Select(metadata => metadata.Value)
                .ToDictionary(metadata => metadata.Field.Name, metadata => metadata, StringComparer.Ordinal);

            FIELD_CACHE[targetType] = metadataMap;
            return metadataMap;
        }

        private static AutoFieldMetadata? CreateMetadata(FieldInfo field)
        {
            var attribute = field.GetCustomAttribute<AutoComponentAttribute>();
            if (attribute == null)
            {
                return null;
            }

            var fieldType = field.FieldType;
            var elementType = ResolveElementType(fieldType);
            if (elementType == null || !typeof(Component).IsAssignableFrom(elementType))
            {
                return null;
            }

            var isList = IsComponentList(fieldType) != null;
            var isCollection = fieldType.IsArray || isList;
            if (attribute.Mode == AutoComponentLookupMode.GetComponentsInChildren && !isCollection)
            {
                return null;
            }

            if (attribute.Mode != AutoComponentLookupMode.GetComponentsInChildren && isCollection)
            {
                return null;
            }

            return new AutoFieldMetadata(field, attribute, fieldType, elementType, isList);
        }

        private static Type ResolveElementType(Type fieldType)
        {
            if (fieldType.IsArray)
            {
                return fieldType.GetElementType();
            }

            var listElementType = IsComponentList(fieldType);
            return listElementType ?? fieldType;
        }

        private static Type IsComponentList(Type fieldType)
        {
            if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(List<>))
            {
                return null;
            }

            var elementType = fieldType.GetGenericArguments().FirstOrDefault();
            if (elementType == null || !typeof(Component).IsAssignableFrom(elementType))
            {
                return null;
            }

            return elementType;
        }

        private static AutoFieldMetadata? GetMetadata(
            IReadOnlyDictionary<string, AutoFieldMetadata> metadataMap,
            string fieldName)
        {
            return metadataMap.ContainsKey(fieldName)
                ? metadataMap[fieldName]
                : null;
        }

        private static bool HasValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is UnityEngine.Object unityObject && unityObject == null)
            {
                return false;
            }

            return value switch
            {
                Array array => array.Length > 0,
                IList list => list.Count > 0,
                _ => true
            };
        }
    }
}
