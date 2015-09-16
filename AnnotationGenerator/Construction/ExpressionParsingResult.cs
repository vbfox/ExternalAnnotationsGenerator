using System;
using System.Collections.Generic;
using System.Reflection;
using AnnotationGenerator.Model;
using JetBrains.Annotations;

namespace AnnotationGenerator.Construction
{
    internal class ExpressionParsingResult
    {
        [NotNull]
        public MemberInfo Member { get; }

        [NotNull]
        public ICollection<IAnnotationInfo> Annotations { get; }

        public ExpressionParsingResult([NotNull] MemberInfo member, [NotNull] ICollection<IAnnotationInfo> annotations)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (annotations == null) throw new ArgumentNullException(nameof(annotations));

            Member = member;
            Annotations = annotations;
        }
    }
}