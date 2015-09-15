using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AnnotationGenerator
{
    public class MemberAnnotator<TClass, TAssembly> : FluentInterface
    {
        private readonly List<MemberAnnotations> membersAnnotations = new List<MemberAnnotations>();

        internal IEnumerable<MemberAnnotations> GetMembersAnnotations()
        {
            return membersAnnotations;
        }

        internal MemberAnnotator()
        {
        }

        public void Annotate<TResult>(Expression<Func<TClass, TResult>> expression)
        {

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

        public void Annotate(Expression<Action<TClass>> expression)
        {
            var methodInfo = ExpressionHelpers.GetMethodInfo(expression);
            var annotationInfos = ExpressionHelpers.GetAnnotationInfoFromExpression(expression).ToList();
            var memberAnnotations = GetMemberAnnotations(methodInfo);
            memberAnnotations.AddRange(annotationInfos);

        }
    }
}