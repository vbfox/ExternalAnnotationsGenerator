using System;
using System.Diagnostics;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AnnotationGenerator.FileGeneration
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
                foreach (var info in annotatedMember)
                {
                    memberElement.Add(info.GetAttributesXml());
                }
                document.Root.Add(memberElement);
            }

            return document;
        }
    }
}