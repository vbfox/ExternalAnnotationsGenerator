using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Notes;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    static class ExpressionHelpers
    {
        public static MethodInfo GetMethodInfo<TArg, TResult>(Expression<Func<TArg, TResult>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method;
            }

            throw new ArgumentException("Expression is not a member access", nameof(expression));
        }

        public static MethodInfo GetMethodInfo<TArg>(Expression<Action<TArg>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
                return methodCallExpression.Method;

            throw new ArgumentException("Expression is not a member access", nameof(expression));
        }

        public static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromMethodCall(MethodCallExpression methodCallExpression)
        {
            return Enumerable.Zip(
                methodCallExpression.Arguments,
                methodCallExpression.Method.GetParameters(),
                ExtractParameter);
        }

        public static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromExpression<TClass>(Expression<Func<TClass, bool>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException("Expected a method call.", nameof(expression));
            }

            return GetAnnotationInfoFromMethodCall(methodCallExpression);
        }

        public static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromExpression(Expression expressionBody)
        {
            var methodCallExpression = expressionBody as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException("Expected a method call.", nameof(expressionBody));
            }

            return GetAnnotationInfoFromMethodCall(methodCallExpression);
        }

        private static readonly string usageInfo = $"Should be a call on one of the methods of {nameof(ParameterNotes)}.";

        [CanBeNull]
        [SuppressMessage("ReSharper", "RedundantArgumentNameForLiteralExpression")]
        public static ParameterAnnotationInfo ExtractParameter(Expression expression, ParameterInfo parameter)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(ParameterNotes.FormatString):
                    return new ParameterAnnotationInfo(parameter.Name, isFormatString: true, isNotNull: true);

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

            if (typeof(ParameterNotes) != methodCallExpression.Method.DeclaringType)
            {
                throw new ArgumentException($"Expression '{expression}' doesn't call a method on the correct type. {usageInfo}",
                    nameof(expression));
            }
            return methodCallExpression;
        }

        [CanBeNull]
        [SuppressMessage("ReSharper", "RedundantArgumentNameForLiteralExpression")]
        public static MemberAnnotationInfo ExtractMember(Expression expression, MemberInfo member)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(ParameterNotes.Some):
                    return new MemberAnnotationInfo();

                case nameof(ParameterNotes.NotNull):
                    return new MemberAnnotationInfo(isNotNull: true);

                case nameof(ParameterNotes.CanBeNull):
                    return new MemberAnnotationInfo(canBeNull: true);

                default:
                    throw new ArgumentException($"Expression '{expression}' call an unsupported method : {methodName}. {usageInfo}",
                        nameof(expression));
            }
        }
    }
}