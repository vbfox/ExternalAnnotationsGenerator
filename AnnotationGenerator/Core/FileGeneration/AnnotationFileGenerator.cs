using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using AnnotationGenerator.Core.Model;
using JetBrains.Annotations;

namespace AnnotationGenerator.Core.FileGeneration
{
    public class AnnotationFileGenerator
    {
        private readonly AssemblyAnnotations annotations;

        public AnnotationFileGenerator([NotNull] AssemblyAnnotations annotations)
        {
            if (annotations == null) throw new ArgumentNullException(nameof(annotations));

            this.annotations = annotations;
        }

        [NotNull]
        public AnnotationFile Generate()
        {
            return new AnnotationFile(GenerateFileNameForAlongDll(), GenerateFileNameForNuget(), CreateDocument());
        }

        private string AssemblyName => annotations.Assembly.GetName().Name;

        private string GenerateFileNameForAlongDll()
        {
            return $"{AssemblyName}.ExternalAnnotations.xml";
        }

        private string GenerateFileNameForNuget()
        {
            return $"{AssemblyName}.xml";
        }

        private XDocument CreateDocument()
        {
            var document = ResharperXmlBuilder.BuildDocument(AssemblyName);
            Debug.Assert(document.Root != null);

            foreach (var annotatedMember in annotations)
            {
                var memberName = ResharperNamesBuilder.GetMemberNameString(annotatedMember.Member);
                var memberElement = new XElement("member", new XAttribute("name", memberName));

                memberElement.Add(annotatedMember.Annotations.Select(AnnotationInfoToXml));
                memberElement.Add(annotatedMember.ParameterAnnotations.Select(AnnotationInfoToXml));
                document.Root.Add(memberElement);
            }

            return document;
        }

        private static IEnumerable<XElement> AnnotationInfoToXml(ParameterAnnotationInfo parameter)
        {
            if (parameter.IsFormatString)
            {
                yield return ResharperXmlBuilder.BuilStringFormatMethodAttribute(parameter.ParameterName);
            }

            if (parameter.IsNotNull || parameter.CanBeNull)
            {
                var element = ResharperXmlBuilder.BuildParameterElement(parameter.ParameterName);
                if (parameter.IsNotNull)
                {
                    element.Add(ResharperXmlBuilder.BuilNotNullAttribute());
                }
                if (parameter.CanBeNull)
                {
                    element.Add(ResharperXmlBuilder.BuilCanBeNullAttribute());
                }
                yield return element;
            }
        }

        private static IEnumerable<XElement> AnnotationInfoToXml(MemberAnnotationInfo member)
        {
            if (member.IsNotNull)
            {
                yield return ResharperXmlBuilder.BuilNotNullAttribute();
            }
            if (member.CanBeNull)
            {
                yield return ResharperXmlBuilder.BuilCanBeNullAttribute();
            }
        }
    }
}