using System;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public class AssemblyAnnotator<TAssembly> : FluentInterface
    {
        private readonly AssemblyAnnotations annotations;
        //private readonly Annotator annotator;
        //private readonly XElement rootElement;

        internal AssemblyAnnotations GetAnnotations()
        {
            return annotations;
        }

        internal AssemblyAnnotator(/*[NotNull] Annotator annotator*/)
        {/*
            if (annotator == null) throw new ArgumentNullException(nameof(annotator));
            */
            annotations = new AssemblyAnnotations(typeof (TAssembly).Assembly);
            /*
            this.annotator = annotator;

            rootElement = CreateDocument();*/
        }
        /*
        private XElement CreateDocument()
        {
            var document = ResharperXmlBuilder.BuildDocument(typeof (TAsm).Assembly.GetName().Name);
            annotator.AddDocument(document);
            return document.Root;
        }

        internal void AddElement([NotNull] XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            rootElement.Add(element);
        }*/

        public void AnnotateType<TType>([NotNull] Action<MemberAnnotator<TType>> annotationActions)
        {
            if (annotationActions == null) throw new ArgumentNullException(nameof(annotationActions));

            var memberAnnotator = new MemberAnnotator<TType>();
            annotationActions(memberAnnotator);
            annotations.AddRange(memberAnnotator.GetMembersAnnotations());
        }

        public AnnotationFile GenerateFile()
        {
            return new AnnotationFileGenerator(annotations).Generate();
        }
    }
}