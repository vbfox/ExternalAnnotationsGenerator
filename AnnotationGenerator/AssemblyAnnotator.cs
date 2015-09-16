using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using AnnotationGenerator.Notes;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public class AssemblyAnnotations : IEnumerable<MemberAnnotations>
    {
        public Assembly Assembly { get; }

        private readonly List<MemberAnnotations> membersAnnotations = new List<MemberAnnotations>();

        public AssemblyAnnotations([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Assembly = assembly;
        }

        public void AddRange([NotNull] IEnumerable<MemberAnnotations> memberAnnotations)
        {
            if (memberAnnotations == null) throw new ArgumentNullException(nameof(memberAnnotations));

            membersAnnotations.AddRange(memberAnnotations);
        }

        public IEnumerator<MemberAnnotations> GetEnumerator()
        {
            return membersAnnotations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return membersAnnotations.GetEnumerator();
        }
    }

    public class MemberAnnotations : IEnumerable<IAnnotationInfo>
    {
        public MemberInfo Member { get; }

        private readonly List<IAnnotationInfo> annotationInfos = new List<IAnnotationInfo>();

        public MemberAnnotations(MemberInfo member)
        {
            Member = member;
        }

        public void AddRange([NotNull] IEnumerable<IAnnotationInfo> annotations)
        {
            if (annotations == null) throw new ArgumentNullException(nameof(annotations));

            annotationInfos.AddRange(annotations);
        }

        public IEnumerator<IAnnotationInfo> GetEnumerator()
        {
            return annotationInfos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return annotationInfos.GetEnumerator();
        }
    }

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

        public void AnnotateType<TType>([NotNull] Action<MemberAnnotator<TType,TAssembly>> annotationActions)
        {
            if (annotationActions == null) throw new ArgumentNullException(nameof(annotationActions));

            var memberAnnotator = new MemberAnnotator<TType,TAssembly>();
            annotationActions(memberAnnotator);
            annotations.AddRange(memberAnnotator.GetMembersAnnotations());
        }

        public AnnotationFile GenerateFile()
        {
            return new AnnotationFileGenerator(annotations).Generate();
        }
    }
}