using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace AnnotationGenerator.Model
{
    internal class MemberAnnotations
    {
        public MemberInfo Member { get; }
        public List<ParameterAnnotationInfo> ParameterAnnotations { get; } = new List<ParameterAnnotationInfo>();
        public List<MemberAnnotationInfo> Annotations { get; } = new List<MemberAnnotationInfo>();

        public MemberAnnotations([NotNull] MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            Member = member;
        }
    }
}