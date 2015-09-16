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
        public class ParsedExpression
        {
            [CanBeNull]
            public MethodCallExpression TargetMethodCall { get; }

            [CanBeNull]
            public MemberExpression TargetMemberAccess { get; }

            [CanBeNull]
            public MethodCallExpression EqualsMethodCall { get; }

            public ParsedExpression([NotNull] MethodCallExpression targetMethodCall,
                [CanBeNull] MethodCallExpression equalsMethodCall)
            {
                if (targetMethodCall == null) throw new ArgumentNullException(nameof(targetMethodCall));

                TargetMethodCall = targetMethodCall;
                EqualsMethodCall = equalsMethodCall;
            }
        }

        public static ParsedExpression Parse(LambdaExpression expression)
        {
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Call:
                    {
                        var call = (MethodCallExpression)expression.Body;
                        return new ParsedExpression(call, null);
                    }

                case ExpressionType.Equal:
                    {
                        var binary = (BinaryExpression) expression.Body;
                        var leftMethodCall = binary.Left as MethodCallExpression;
                        var rightMethodCall = binary.Right as MethodCallExpression;
                        if (leftMethodCall == null || rightMethodCall == null)
                        {
                            throw new ArgumentException("Expected MethodCall() == MethodCall()");
                        }

                        return new ParsedExpression(leftMethodCall, rightMethodCall);
                    }

                default:
                    throw new Exception("boom");
            }
            /*
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return new SupportedExpression(methodCallExpression, null);
            }

            var binaryExpression = expression.Body as BinaryExpression;
            if (binaryExpression != null)
            {
                var methodCallLeft = expression.Body as MethodCallExpression;
                if (methodCallExpression != null)
                {
                    return methodCallExpression.Method;
                }
            }*/
        }

        public static MemberInfo GetMemberInfo(ParsedExpression expression)
        {
            if (expression.TargetMethodCall != null)
            {
                return expression.TargetMethodCall.Method;
            }
            if (expression.TargetMemberAccess != null)
            {
                return expression.TargetMemberAccess.Member;
            }
            throw new ArgumentException("Unexpected expression structure", nameof(expression));
        }

        public static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromMethodCall(MethodCallExpression methodCallExpression)
        {
            return Enumerable.Zip(
                methodCallExpression.Arguments,
                methodCallExpression.Method.GetParameters(),
                ExtractParameter);
        }

        public static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromExpression(ParsedExpression expression)
        {
            if (expression.TargetMethodCall != null)
            {
                return GetAnnotationInfoFromMethodCall(expression.TargetMethodCall);
            }
            if (expression.TargetMemberAccess != null)
            {
                // TODO
            }
            throw new ArgumentException("Unexpected expression structure", nameof(expression));
        }

        /*
        public static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromExpression(Expression expressionBody)
        {
            var methodCallExpression = expressionBody as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException("Expected a method call.", nameof(expressionBody));
            }

            return GetAnnotationInfoFromMethodCall(methodCallExpression);
        }*/

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
                    return new ParameterAnnotationInfo(parameter.Name, isFormatString: true, canBeNull: true);

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