using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.Model;

namespace ExternalAnnotationsGenerator.Core.Construction
{
    internal class TypeAnnotationsBuilder<TType> : ITypeAnnotator<TType>
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

        public void Annotate<TResult>(Expression<Func<TType, TResult>> expression)
        {
            AnnotateCore(expression);
        }

        public void Annotate(Expression<Action<TType>> expression)
        {
            AnnotateCore(expression);
        }

        private void AnnotateCore(LambdaExpression expression)
        {
            var result = ExpressionParser.Parse(expression);
            GetMemberAnnotations(result.Member).AddAnnotationsFromParsing(result);
        }
    }
}