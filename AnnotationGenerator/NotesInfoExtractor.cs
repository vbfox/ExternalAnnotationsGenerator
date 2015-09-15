using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Notes;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    [SuppressMessage("ReSharper", "RedundantArgumentNameForLiteralExpression")]
    internal static class NotesInfoExtractor
    {
        private static readonly string usageInfo = $"Should be a call on one of the methods of {nameof(ParameterNotes)}.";

        [CanBeNull]
        public static ParameterAnnotationInfo ExtractParameter(Expression expression, ParameterInfo parameter)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(ParameterNotes.FormatString):
                    return new ParameterAnnotationInfo(parameter.Name, isFormatString:true, isNotNull:true);

                case nameof(ParameterNotes.NullableFormatString):
                    return new ParameterAnnotationInfo(parameter.Name, isFormatString: true);

                case nameof(ParameterNotes.Some):
                    return new ParameterAnnotationInfo(parameter.Name);

                case nameof(ParameterNotes.NotNull):
                    return new ParameterAnnotationInfo(parameter.Name, isNotNull: true);

                case nameof(ParameterNotes.CanBeNull):
                    return new ParameterAnnotationInfo(parameter.Name, canBeNull: true);

                default:
                    throw new ArgumentException($"Expression '{expression}' call an unsupported method : {methodName}. {usageInfo}",
                        nameof(expression));
            }
        }

        private static MethodCallExpression AssertCallOnSpecialClass(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException($"Expression '{expression}' isn't a method call. {usageInfo}", nameof(expression));
            }

            if (typeof (ParameterNotes) != methodCallExpression.Method.DeclaringType)
            {
                throw new ArgumentException($"Expression '{expression}' doesn't call a method on the correct type. {usageInfo}",
                    nameof(expression));
            }
            return methodCallExpression;
        }

        [CanBeNull]
        public static MemberAnnotationInfo ExtractMember(Expression expression, MemberInfo member)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(ParameterNotes.Some):
                    return new MemberAnnotationInfo();

                case nameof(ParameterNotes.NotNull):
                    return new MemberAnnotationInfo(isNotNull:true);

                case nameof(ParameterNotes.CanBeNull):
                    return new MemberAnnotationInfo(canBeNull:true);

                default:
                    throw new ArgumentException($"Expression '{expression}' call an unsupported method : {methodName}. {usageInfo}",
                        nameof(expression));
            }
        }
    }
}