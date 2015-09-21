using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ExternalAnnotationsGenerator.Tests
{
    internal static class XmlTestsHelpers
    {
        public static string Normalize(string xml)
        {
            return Normalize(XDocument.Parse(xml));
        }

        public static string Normalize(XContainer xml)
        {
            using (var stringWriter = new StringWriter())
            {
                var settings = new XmlWriterSettings {Indent = true};

                using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xml.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }
    }
}