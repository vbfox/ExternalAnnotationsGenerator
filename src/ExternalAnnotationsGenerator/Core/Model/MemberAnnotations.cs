using System;
using System.Collections.Generic;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.Construction;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Core.Model
{
    public class MemberAnnotations
    {
        public MemberInfo Member { get; }
        public List<ParameterAnnotationInfo> ParameterAnnotations { get; } = new List<ParameterAnnotationInfo>();
        public List<MemberAnnotationInfo> Annotations { get; } = new List<MemberAnnotationInfo>();

        public MemberAnnotations([NotNull] MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            Member = member;
        }

        internal void AddAnnotationsFromParsing([NotNull] ExpressionParsingResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            Annotations.AddRange(result.Annotations);
            ParameterAnnotations.AddRange(result.ParameterAnnotations);
        }
    }
}