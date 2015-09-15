using System;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace AnnotationGenerator.Notes
{
    /// <summary>
    /// Build type and method names in a way compatible with R# Annotations XML format
    /// </summary>
    static class ResharperNamesBuilder
    {
        public static string GetAssemblyNameString([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            return assembly.FullName;
        }

        public static string GetMemberNameString([NotNull] MemberInfo member)
        {
            var methodInfo = member as MethodBase;
            if (methodInfo != null)
            {
                return GetMethodNameString(methodInfo);
            }

            throw new ArgumentException("Member type not supported : " + member.MemberType, nameof(member));
        }

        public static string GetMethodNameString([NotNull] MethodBase methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

            var declaringType = methodInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentException("The method is required to have a declaring type", nameof(methodInfo));
            }

            var methodName = methodInfo.IsConstructor ? "#ctor" : methodInfo.Name;
            var parameterString = GetParametersString(methodInfo);
            var typeParameterCount = methodInfo.IsGenericMethod ? "``" + methodInfo.GetGenericArguments().Length : "";

            return $"M:{declaringType.FullName}.{methodName}{typeParameterCount}{parameterString}";
        }

        private static string GetParameterTypeName(Type type)
        {
            var builder = new StringBuilder();
            AddParameterTypeName(type, builder);
            return builder.ToString();
        }

        private static void AddParameterTypeName(Type type, StringBuilder builder)
        {
            if (type.IsGenericType)
            {
                var fullName = type.GetGenericTypeDefinition().FullName;
                var typeName = fullName.Substring(0, fullName.LastIndexOf("`", StringComparison.Ordinal));
                builder.Append(typeName);
                builder.Append("{");
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    AddParameterTypeName(genericArgument, builder);
                }
                builder.Append("}");
            }
            else if (type.IsGenericParameter)
            {
                builder.Append(type.DeclaringMethod != null ? "``" : "`");
                builder.Append(type.GenericParameterPosition);
            }
            else
            {
                builder.Append(type.FullName);
            }
        }

        private static string GetParametersString([NotNull] MethodBase methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length <= 0)
            {
                return "";
            }

            var parametersSeparated = string.Join(",",
                methodInfo.GetParameters().Select(p => GetParameterTypeName(p.ParameterType)));

            return $"({parametersSeparated})";
        }
    }
}