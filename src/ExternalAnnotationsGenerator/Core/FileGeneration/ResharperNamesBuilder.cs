using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Core.FileGeneration
{
    /// <summary>
    /// Build type and method names in a way compatible with R# Annotations XML format
    /// </summary>
    public static class ResharperNamesBuilder
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

            var builder = new StringBuilder("M:");
            AppendRootTypeName(declaringType, builder);
            builder.Append(".");
            builder.Append(methodInfo.IsConstructor ? "#ctor" : methodInfo.Name);
            builder.Append(methodInfo.IsGenericMethod ? "``" + methodInfo.GetGenericArguments().Length : "");
            AppendParametersString(methodInfo, builder);
            return builder.ToString();
        }

        private static void AppendRootTypeName(Type type, StringBuilder builder)
        {
            if (type.IsNested)
            {
                AppendRootTypeName(type.DeclaringType, builder);
                builder.Append("+");
                builder.Append(type.Name);
            }
            else
            {
                builder.Append(type.Namespace);
                builder.Append(".");
                builder.Append(type.Name);
            }
        }

        private static void AppendParameterTypeName(Type type, StringBuilder builder)
        {
            if (type.IsGenericType)
            {
                var fullName = type.GetGenericTypeDefinition().FullName;
                var typeName = fullName.Substring(0, fullName.LastIndexOf("`", StringComparison.Ordinal));
                builder.Append(typeName);
                builder.Append("{");
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    AppendParameterTypeName(genericArgument, builder);
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

        private static void AppendParametersString([NotNull] MethodBase methodBase, StringBuilder builder)
        {
            var methodInfo = methodBase as MethodInfo;
            if (methodInfo != null && methodInfo.IsGenericMethod)
            {
                methodBase = methodInfo.GetGenericMethodDefinition();
            }

            var parameters = methodBase.GetParameters();
            if (parameters.Length == 0)
            {
                return;
            }

            builder.Append("(");
            var first = true;
            foreach (var parameter in parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(",");
                }
                AppendParameterTypeName(parameter.ParameterType, builder);
            }
            builder.Append(")");
        }
    }
}