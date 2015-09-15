using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using AnnotationGenerator.Notes;

namespace AnnotationGenerator
{
    public class MemberAnnotator<TClass,TAsm>
    {
        private readonly AssemblyAnnotator<TAsm> assemblyAnnotator;

        internal MemberAnnotator(AssemblyAnnotator<TAsm> assemblyAnnotator)
        {
            this.assemblyAnnotator = assemblyAnnotator;
        }

        public void Annotate(Expression<Func<TClass, bool>> expression)
        {

        }

        public void Annotate(Expression<Action<TClass>> expression)
        {
            var methodInfo = GetMethodInfo(expression);
            var parameterNotes = GetNotesFromExpression(expression.Body).ToList();

            if (parameterNotes.Any())
            {
                var attributesXml = GetAttributesXml(parameterNotes);

                assemblyAnnotator.AddElement(new XElement("member",
                    new XAttribute("name", GetMethodNameString(methodInfo)),
                    attributesXml));
            }
        }

        private string GetMethodNameString(MethodInfo methodInfo)
        {
            var parameters = string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.FullName));

            var declaringType = methodInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentException("The method is required to have a declaring type", nameof(methodInfo));
            }

            return $"M:{declaringType.FullName}.{methodInfo.Name}({parameters})";
        }

        private static IEnumerable<XElement> GetAttributesXml(IEnumerable<INote> notes)
        {
            return notes.SelectMany(n => n.GetAttributesXml());
        }

        public IEnumerable<ParameterNotesInfo> GetNotesFromMethodCall(MethodCallExpression methodCallExpression)
        {
            return Enumerable.Zip(
                methodCallExpression.Arguments,
                methodCallExpression.Method.GetParameters(),
                NotesInfoExtractor.ExtractParameter);
        }

        public IEnumerable<ParameterNotesInfo> GetNotesFromExpression(Expression<Func<TClass, bool>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException("Expected a method call.", nameof(expression));
            }

            return GetNotesFromMethodCall(methodCallExpression);
        }

        public IEnumerable<ParameterNotesInfo> GetNotesFromExpression(Expression expressionBody)
        {
            var methodCallExpression = expressionBody as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException("Expected a method call.", nameof(expressionBody));
            }

            return GetNotesFromMethodCall(methodCallExpression);
        }

        public static MethodInfo GetMethodInfo<TArg, TResult>(Expression<Func<TArg, TResult>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
                return methodCallExpression.Method;

            throw new ArgumentException("Expression is not a member access", nameof(expression));
        }

        public static MethodInfo GetMethodInfo<TArg>(Expression<Action<TArg>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
                return methodCallExpression.Method;

            throw new ArgumentException("Expression is not a member access", nameof(expression));
        }
    }
}