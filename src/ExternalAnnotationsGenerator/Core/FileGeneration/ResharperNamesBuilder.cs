using System;
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

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return GetPropertyNameString(propertyInfo);
            }

            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return GetFieldNameString(fieldInfo);
            }

            throw new ArgumentException("Member type not supported : " + member.MemberType, nameof(member));
        }

        public static string GetMethodNameString([NotNull] MethodBase methodBase)
        {
            if (methodBase == null) throw new ArgumentNullException(nameof(methodBase));

            return GetMethodNameStringCore(GenericDefinitionHelper.GetGenericDefinition(methodBase));
        }

        public static string GetPropertyNameString([NotNull] PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            return GetPropertyNameStringCore(GenericDefinitionHelper.GetGenericDefinition(property));
        }

        private static string GetPropertyNameStringCore(PropertyInfo property)
        {
            var builder = new StringBuilder("P:");
            AppendMemberTypeAndName(property, builder);
            return builder.ToString();
        }

        public static string GetFieldNameString([NotNull] FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            return GetFieldNameStringCore(GenericDefinitionHelper.GetGenericDefinition(field));
        }

        private static string GetFieldNameStringCore(FieldInfo field)
        {
            var builder = new StringBuilder("F:");
            AppendMemberTypeAndName(field, builder);
            return builder.ToString();
        }

        // encodes dots with alternate # character
        private static string EncodeName(string name)
        {
            if (name.IndexOf('.') >= 0)
            {
                return name.Replace('.', '#');
            }

            return name;
        }

        private static void AppendMemberTypeAndName(MemberInfo member, StringBuilder builder)
        {
            var declaringType = member.DeclaringType;
            if (declaringType != null)
            {
                AppendRootTypeName(declaringType, builder);
                builder.Append(".");
                builder.Append(EncodeName(member.Name));
            }
        }

        static string GetMethodNameStringCore(MethodBase methodBase)
        {
            var builder = new StringBuilder("M:");

            AppendMemberTypeAndName(methodBase, builder);

            if (methodBase.IsGenericMethod)
            {
                builder.Append("``");
                builder.Append(methodBase.GetGenericArguments().Length);
            }
            AppendParametersString(methodBase, builder);
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