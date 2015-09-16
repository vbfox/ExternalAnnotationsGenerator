using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AnnotationGenerator.Notes;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
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
}