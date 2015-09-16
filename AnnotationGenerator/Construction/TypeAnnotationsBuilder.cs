using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Model;

namespace AnnotationGenerator.Construction
{
    internal class TypeAnnotationsBuilder<TClass> : ITypeAnnotator<TClass>
    {
        private readonly List<MemberAnnotations> membersAnnotations = new List<MemberAnnotations>();

        internal IEnumerable<MemberAnnotations> GetMembersAnnotations()
        {
            return membersAnnotations;
        }

        private MemberAnnotations GetMemberAnnotations(MemberInfo member)
        {
            var existing = membersAnnotations.FirstOrDefault(m => m.Member == member);
            if (existing != null)
            {
                return existing;
            }

            var newAnnotations = new MemberAnnotations(member);
            membersAnnotations.Add(newAnnotations);
            return newAnnotations;
        }

        public void Annotate<TResult>(Expression<Func<TClass, TResult>> expression)
        {
            AnnotateCore(expression);
        }

        public void Annotate(Expression<Action<TClass>> expression)
        {
            AnnotateCore(expression);
        }

        private void AnnotateCore(LambdaExpression expression)
        {
            var result = ExpressionParser.Parse(expression);
            var memberAnnotations = GetMemberAnnotations(result.Member);

            memberAnnotations.Annotations.AddRange(result.Annotations);
            memberAnnotations.ParameterAnnotations.AddRange(result.ParameterAnnotations);
        }
    }
}