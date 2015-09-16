using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using AnnotationGenerator.Model;
using JetBrains.Annotations;

namespace AnnotationGenerator.FileGeneration
{
    internal class AnnotationFileGenerator
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
            return new AnnotationFile("", CreateDocument());
        }

        private XDocument CreateDocument()
        {
            var document = ResharperXmlBuilder.BuildDocument(annotations.Assembly.GetName().Name);
            Debug.Assert(document.Root != null);

            foreach (var annotatedMember in annotations)
            {
                var memberName = ResharperNamesBuilder.GetMemberNameString(annotatedMember.Member);
                var memberElement = new XElement("member", new XAttribute("name", memberName));
                foreach (var info in annotatedMember.Annotations)
                {
                    memberElement.Add(AnnotationInfoToXml(info));
                }
                foreach (var info in annotatedMember.ParameterAnnotations)
                {
                    memberElement.Add(AnnotationInfoToXml(info));
                }
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