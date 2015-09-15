using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public class AssemblyAnnotator<TAsm>
    {
        private readonly Annotator annotator;
        private readonly XElement rootElement;

        internal AssemblyAnnotator([NotNull] Annotator annotator)
        {
            if (annotator == null) throw new ArgumentNullException(nameof(annotator));

            this.annotator = annotator;

            rootElement = CreateDocument();
        }

        private XElement CreateDocument()
        {
            var element = new XElement("assembly",
                new XAttribute("name", typeof (TAsm).Assembly.GetName().Name));

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null), element);

            annotator.AddDocument(document);

            return element;
        }

        internal void AddElement([NotNull] XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            rootElement.Add(element);
        }

        public void AnnotateType<TType>([NotNull] Action<MemberAnnotator<TType,TAsm>> annotationActions)
        {
            if (annotationActions == null) throw new ArgumentNullException(nameof(annotationActions));

            annotationActions(new MemberAnnotator<TType,TAsm>(this));
        }
    }
}