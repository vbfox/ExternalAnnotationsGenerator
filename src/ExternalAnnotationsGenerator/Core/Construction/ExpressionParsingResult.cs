using System;
using System.Collections.Generic;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.Model;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Core.Construction
{
    internal class ExpressionParsingResult
    {
        public ExpressionParsingResult([NotNull] MemberInfo member,
            [NotNull] ICollection<MemberAnnotationInfo> annotations,
            [NotNull] ICollection<ParameterAnnotationInfo> parameterAnnotations)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (annotations == null) throw new ArgumentNullException(nameof(annotations));

            Member = member;
            Annotations = annotations;
            ParameterAnnotations = parameterAnnotations;
        }

        [NotNull]
        public MemberInfo Member { get; }

        [NotNull]
        public ICollection<MemberAnnotationInfo> Annotations { get; }

        [NotNull]
        public ICollection<ParameterAnnotationInfo> ParameterAnnotations { get; }
    }
}