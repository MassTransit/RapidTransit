namespace RapidTransit.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Reflection;


    public static class TypeExtensions
    {
        static readonly TypeNameFormatter _typeNameFormatter = new TypeNameFormatter();

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            return GetAllProperties(typeInfo);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
        {
            if (typeInfo.BaseType != null)
            {
                foreach (PropertyInfo prop in GetAllProperties(typeInfo.BaseType))
                    yield return prop;
            }

            List<PropertyInfo> properties = typeInfo.DeclaredMethods
                                                    .Where(
                                                        x => x.IsSpecialName && x.Name.StartsWith("get_") && !x.IsStatic)
                                                    .Select(
                                                        x =>
                                                        typeInfo.GetDeclaredProperty(x.Name.Substring("get_".Length)))
                                                    .ToList();

            if (typeInfo.IsInterface)
            {
                foreach (
                    PropertyInfo prop in
                        properties.Concat(
                            typeInfo.ImplementedInterfaces.SelectMany(x => x.GetTypeInfo().DeclaredProperties)))
                    yield return prop;

                yield break;
            }

            foreach (PropertyInfo info in properties)
                yield return info;
        }

        public static IEnumerable<PropertyInfo> GetAllStaticProperties(this Type type)
        {
            TypeInfo info = type.GetTypeInfo();

            if (info.BaseType != null)
            {
                foreach (PropertyInfo prop in GetAllStaticProperties(info.BaseType))
                    yield return prop;
            }

            IEnumerable<PropertyInfo> props = info.DeclaredMethods
                                                  .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && x.IsStatic)
                                                  .Select(x => info.GetDeclaredProperty(x.Name.Substring("get_".Length)));

            foreach (PropertyInfo propertyInfo in props)
                yield return propertyInfo;
        }

        public static IEnumerable<PropertyInfo> GetStaticProperties(this Type type)
        {
            TypeInfo info = type.GetTypeInfo();

            IEnumerable<PropertyInfo> props = info.DeclaredMethods
                                                  .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && x.IsStatic)
                                                  .Select(x => info.GetDeclaredProperty(x.Name.Substring("get_".Length)));

            foreach (PropertyInfo propertyInfo in props)
                yield return propertyInfo;
        }

        /// <summary>
        /// Determines if a type is neither abstract nor an interface and can be constructed.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type can be constructed, otherwise false.</returns>
        public static bool IsConcrete(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            return !typeInfo.IsAbstract && !typeInfo.IsInterface;
        }

        /// <summary>
        /// Determines if a type can be constructed, and if it can, additionally determines
        /// if the type can be assigned to the specified type.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <param name="assignableType">The type to which the subject type should be checked against</param>
        /// <returns>True if the type is concrete and can be assigned to the assignableType, otherwise false.</returns>
        public static bool IsConcreteAndAssignableTo(this Type type, Type assignableType)
        {
            return IsConcrete(type) && assignableType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Determines if a type can be constructed, and if it can, additionally determines
        /// if the type can be assigned to the specified type.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <typeparam name="T">The type to which the subject type should be checked against</typeparam>
        /// <returns>True if the type is concrete and can be assigned to the assignableType, otherwise false.</returns>
        public static bool IsConcreteAndAssignableTo<T>(this Type type)
        {
            return IsConcrete(type) && typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Determines if the type is a nullable type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be null</returns>
        public static bool IsNullable(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>).GetTypeInfo();
        }

        /// <summary>
        /// Determines if the type is a nullable type
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="underlyingType">The underlying type of the nullable</param>
        /// <returns>True if the type can be null</returns>
        public static bool IsNullable(this Type type, out Type underlyingType)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            bool isNullable = typeInfo.IsGenericType
                              && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>).GetTypeInfo();
            underlyingType = isNullable ? Nullable.GetUnderlyingType(type) : null;
            return isNullable;
        }

        /// <summary>
        /// Determines if the type is an open generic with at least one unspecified generic argument
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type is an open generic</returns>
        public static bool IsOpenGeneric(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericTypeDefinition || typeInfo.ContainsGenericParameters;
        }

        /// <summary>
        /// Determines if a type can be null
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be null</returns>
        public static bool CanBeNull(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            return !typeInfo.IsValueType || type.IsNullable() || typeInfo == typeof(string);
        }

        public static string GetTypeName(this Type type)
        {
            return _typeNameFormatter.GetTypeName(type);
        }
    }


    public static class TypeInfoExtensions
    {
        public static Type[] GetGenericArguments(this TypeInfo typeInfo)
        {
            return typeInfo.GenericTypeArguments;
        }

        public static Type[] GetInterfaces(this TypeInfo typeInfo)
        {
            return typeInfo.ImplementedInterfaces.ToArray();
        }

        public static MethodInfo GetGetMethod(this PropertyInfo typeInfo, bool includeNonPublic = false)
        {
            return typeInfo.GetMethod;
        }

        public static MethodInfo GetSetMethod(this PropertyInfo typeInfo, bool includeNonPublic = false)
        {
            return typeInfo.SetMethod;
        }
    }
}