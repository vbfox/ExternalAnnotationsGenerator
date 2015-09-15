using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AnnotationGenerator.Notes
{
    static class ResharperXmlBuilder
    {
        public static XElement BuildAttribute([NotNull] string ctor, params object[] args)
        {
            if (ctor == null) throw new ArgumentNullException(nameof(ctor));

            var element = new XElement("attribute", new XAttribute("ctor", ctor));

            foreach (var arg in args)
            {
                element.Add(new XElement(arg.ToString()));
            }

            return element;
        }

        private static readonly Lazy<string> notNullCtor =
            new Lazy<string>(() => GetMethodNameString(GetEmptyCtor<NotNullAttribute>()));

        private static readonly Lazy<string> canBeNullCtor =
            new Lazy<string>(() => GetMethodNameString(GetEmptyCtor<CanBeNullAttribute>()));

        private static readonly Lazy<string> stringFormatMethodCtor =
            new Lazy<string>(() => GetMethodNameString(GetSingleArgCtor<StringFormatMethodAttribute, string>()));

        private static ConstructorInfo GetSingleArgCtor<TType, TArg>()
        {
            return typeof(TType).GetConstructor(new [] { typeof(TArg) });
        }

        private static ConstructorInfo GetEmptyCtor<TType>()
        {
            return typeof (TType).GetConstructor(Type.EmptyTypes);
        }

        public static XElement BuilStringFormatMethodAttribute([NotNull] string parameterName)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            return BuildAttribute(stringFormatMethodCtor.Value, parameterName);
        }

        public static XElement BuilNotNullAttribute()
        {
            return BuildAttribute(notNullCtor.Value);
        }

        public static XElement BuilCanBeNullAttribute()
        {
            return BuildAttribute(canBeNullCtor.Value);
        }

        public static XElement BuildParameterElement([NotNull] string parameterName)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            return new XElement("parameter", new XAttribute("name", parameterName));
        }

        public static string GetAssemblyNameString([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            return assembly.FullName;
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

            return $"M:{declaringType.FullName}.{methodName}{parameterString}";
        }

        private static string GetTypeName(Type type)
        {
            var builder = new StringBuilder();
            AddTypeName(type, builder);
            return builder.ToString();
        }

        private static void AddTypeName(Type type, StringBuilder builder)
        {
            if (type.IsConstructedGenericType)
            {
                var typeName = type.FullName.Substring(0, type.FullName.LastIndexOf("`", StringComparison.Ordinal) - 1);
                builder.Append(typeName);
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
                methodInfo.GetParameters().Select(p => p.ParameterType.FullName));

            return $"({parametersSeparated})";
        }
    }
}
