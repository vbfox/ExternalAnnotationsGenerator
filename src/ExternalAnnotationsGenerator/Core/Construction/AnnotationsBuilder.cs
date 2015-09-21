using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.Model;

namespace ExternalAnnotationsGenerator.Core.Construction
{
    public class AnnotationsBuilder : IAnnotator
    {
        readonly List<AssemblyAnnotations> annotations = new List<AssemblyAnnotations>();

        public IEnumerable<AssemblyAnnotations> GetAnnotations()
        {
            return annotations;
        }

        private AssemblyAnnotations GetAssemblyAnnotations(Assembly assembly)
        {
            var existing = annotations.FirstOrDefault(m => m.Assembly == assembly);
            if (existing != null)
            {
                return existing;
            }

            var newAnnotations = new AssemblyAnnotations(assembly);
            annotations.Add(newAnnotations);
            return newAnnotations;
        }

        public void AnnotateType<TType>(Action<ITypeAnnotator<TType>> annotationActions)
        {
            if (annotationActions == null) throw new ArgumentNullException(nameof(annotationActions));

            var memberAnnotator = new TypeAnnotationsBuilder<TType>();
            annotationActions(memberAnnotator);

            var assemblyAnnotations = GetAssemblyAnnotations(typeof (TType).Assembly);
            assemblyAnnotations.AddRange(memberAnnotator.GetMembersAnnotations());
        }

        public void AnnotateStatic(Expression<Action> expression)
        {
            AnnotateStaticCore(expression);
        }

        public void AnnotateStatic<TResult>(Expression<Func<TResult>> expression)
        {
            AnnotateStaticCore(expression);
        }

        private MemberAnnotations GetMemberAnnotations(MemberInfo member)
        {
            Debug.Assert(member != null);
            Debug.Assert(member.DeclaringType != null);

            var assemblyAnnotations = GetAssemblyAnnotations(member.DeclaringType.Assembly);
            var existing = assemblyAnnotations.FirstOrDefault(m => m.Member == member);
            if (existing != null)
            {
                return existing;
            }

            var newAnnotations = new MemberAnnotations(member);
            assemblyAnnotations.Add(newAnnotations);
            return newAnnotations;
        }

        private void AnnotateStaticCore(LambdaExpression expression)
        {
            var result = ExpressionParser.Parse(expression);
            Debug.Assert(result.Member.DeclaringType != null,
                "Expression parser shouldn't succeed for members without declaring type");

            GetMemberAnnotations(result.Member).AddAnnotationsFromParsing(result);
        }
    }
}