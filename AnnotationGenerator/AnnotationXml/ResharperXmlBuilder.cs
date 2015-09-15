using System;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AnnotationGenerator.Notes
{
    /// <summary>
    /// Build XML nodes for the R# Annotations XML format
    /// </summary>
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
            new Lazy<string>(() => ResharperNamesBuilder.GetMethodNameString(GetEmptyCtor<NotNullAttribute>()));

        private static readonly Lazy<string> canBeNullCtor =
            new Lazy<string>(() => ResharperNamesBuilder.GetMethodNameString(GetEmptyCtor<CanBeNullAttribute>()));

        private static readonly Lazy<string> stringFormatMethodCtor =
            new Lazy<string>(() => ResharperNamesBuilder.GetMethodNameString(GetSingleArgCtor<StringFormatMethodAttribute, string>()));

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
    }
}
