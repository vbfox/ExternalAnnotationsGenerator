using System.Xml.Linq;

namespace AnnotationGenerator.Notes
{
    static class ResharperXmlBuilder
    {
        public const string StringFormatMethodCtor =
            "M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)";

        public const string NotNullCtor = "M:JetBrains.Annotations.NotNullAttribute.#ctor";
        public const string CanBeNullCtor = "M:JetBrains.Annotations.CanBeNullAttribute.#ctor";

        public static XElement BuildAttribute(string ctor, params object[] args)
        {
            var element = new XElement("attribute", new XAttribute("ctor", ctor));

            foreach (var arg in args)
            {
                element.Add(new XElement(arg.ToString()));
            }

            return element;
        }

        public static XElement BuilStringFormatMethodAttribute(string parameterName)
        {
            return BuildAttribute(StringFormatMethodCtor, parameterName);
        }

        public static XElement BuilNotNullAttribute()
        {
            return BuildAttribute(NotNullCtor);
        }

        public static XElement BuilCanBeNullAttribute()
        {
            return BuildAttribute(CanBeNullCtor);
        }

        public static XElement BuildParameterElement(string parameterName)
        {
            return new XElement("parameter", new XAttribute("name", parameterName));
        }
    }
}
