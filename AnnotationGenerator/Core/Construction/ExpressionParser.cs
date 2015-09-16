using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Core.Model;
using JetBrains.Annotations;

namespace AnnotationGenerator.Core.Construction
{
    internal static class ExpressionParser
    {
        private class IntermediateExpression
        {
            [CanBeNull]
            public MethodCallExpression TargetMethodCall { get; }

            [CanBeNull]
            public NewExpression TargetNew { get; }

            [CanBeNull]
            public MemberExpression TargetMemberAccess { get; }

            [CanBeNull]
            public MethodCallExpression EqualsMethodCall { get; }

            public IntermediateExpression([CanBeNull] MethodCallExpression targetMethodCall,
                [CanBeNull] MemberExpression targetMemberAccess,
                [CanBeNull] NewExpression targetNew,
                [CanBeNull] MethodCallExpression equalsMethodCall)
            {
                TargetMethodCall = targetMethodCall;
                TargetMemberAccess = targetMemberAccess;
                TargetNew = targetNew;
                EqualsMethodCall = equalsMethodCall;
            }
        }

        public static ExpressionParsingResult Parse(LambdaExpression expression)
        {
            var intermediate = GetIntermediate(expression);
            var annotations = GetAnnotationInfoFromExpression(intermediate).ToList();
            var parameterAnnotations = GetParameterAnnotationInfoFromExpression(intermediate).ToList();

            var methodInfo = GetMemberInfo(intermediate);
            return new ExpressionParsingResult(methodInfo, annotations, parameterAnnotations);
        }

        private static IntermediateExpression GetIntermediate(LambdaExpression expression)
        {
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Call:
                    return GetIntermediateFromCall(expression);

                case ExpressionType.Equal:
                    return GetIntermediateFromEqual(expression);

                case ExpressionType.MemberAccess:
                    return GetIntermediateFromMemberAccess(expression);

                case ExpressionType.New:
                    return GetIntermediateFromNew(expression);

                default:
                    throw new Exception($"Expression type isn't supported : {expression.Body.NodeType}");
            }
        }

        private static IntermediateExpression GetIntermediateFromEqual(LambdaExpression expression)
        {
            var binary = (BinaryExpression) expression.Body;

            var leftIsSpecial = IsMethodCallOnSpecialClass(binary.Left);
            var rightIsSpecial = IsMethodCallOnSpecialClass(binary.Right);

            if (leftIsSpecial && rightIsSpecial)
            {
                throw new ArgumentException("Both sides of '==' are annotations.");
            }

            if (!(leftIsSpecial || rightIsSpecial))
            {
                throw new ArgumentException("No annotation found on any side of '=='.");
            }

            var annotation = (leftIsSpecial ? binary.Left : binary.Right) as MethodCallExpression;
            var target = leftIsSpecial ? binary.Right : binary.Left;

            switch (target.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return new IntermediateExpression(null, (MemberExpression) target, null, annotation);

                case ExpressionType.Call:
                    return new IntermediateExpression((MethodCallExpression)target, null, null, annotation);

                default:
                    throw new ArgumentException("Expected condition on method call or member access");
            }
        }

        private static IntermediateExpression GetIntermediateFromNew(LambdaExpression expression)
        {
            var newExpression = (NewExpression)expression.Body;
            return new IntermediateExpression(null, null, newExpression, null);
        }

        private static IntermediateExpression GetIntermediateFromMemberAccess(LambdaExpression expression)
        {
            var member = (MemberExpression)expression.Body;
            return new IntermediateExpression(null, member, null, null);
        }

        private static IntermediateExpression GetIntermediateFromCall(LambdaExpression expression)
        {
            var call = (MethodCallExpression) expression.Body;
            return new IntermediateExpression(call, null, null, null);
        }

        private static MemberInfo GetMemberInfo(IntermediateExpression expression)
        {
            if (expression.TargetMethodCall != null)
            {
                return expression.TargetMethodCall.Method;
            }
            if (expression.TargetMemberAccess != null)
            {
                return expression.TargetMemberAccess.Member;
            }
            if (expression.TargetNew != null)
            {
                return expression.TargetNew.Constructor;
            }
            throw new ArgumentException("Unexpected expression structure", nameof(expression));
        }

        private static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromMethodCall(
            MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression == null)
            {
                return Enumerable.Empty<ParameterAnnotationInfo>();
            }

            return methodCallExpression.Arguments.Zip(methodCallExpression.Method.GetParameters(),
                ExtractParameter);
        }

        private static IEnumerable<ParameterAnnotationInfo> GetAnnotationInfoFromNew(NewExpression newExpression)
        {
            if (newExpression == null)
            {
                return Enumerable.Empty<ParameterAnnotationInfo>();
            }

            return newExpression.Arguments.Zip(newExpression.Constructor.GetParameters(),
                ExtractParameter);
        }

        private static IEnumerable<MemberAnnotationInfo> GetAnnotationInfoFromMemberEquals(
            MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression == null)
            {
                return Enumerable.Empty<MemberAnnotationInfo>();
            }

            return new[] {ExtractMember(methodCallExpression)};
        }

        private static IEnumerable<MemberAnnotationInfo> GetAnnotationInfoFromExpression(IntermediateExpression expression)
        {
            var fromEquals = GetAnnotationInfoFromMemberEquals(expression.EqualsMethodCall);

            return fromEquals;
        }

        private static IEnumerable<ParameterAnnotationInfo> GetParameterAnnotationInfoFromExpression(IntermediateExpression expression)
        {
            var fromCall = GetAnnotationInfoFromMethodCall(expression.TargetMethodCall);
            var fromNew = GetAnnotationInfoFromNew(expression.TargetNew);

            return fromCall.Concat(fromNew);
        }

        private static readonly string usageInfo = $"Should be a call on one of the methods of {nameof(Annotations)}.";

        [CanBeNull]
        [SuppressMessage("ReSharper", "RedundantArgumentNameForLiteralExpression")]
        private static ParameterAnnotationInfo ExtractParameter(Expression expression, ParameterInfo parameter)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(Annotations.FormatString):
                    return new ParameterAnnotationInfo(parameter.Name, isFormatString: true, isNotNull: true);

                case nameof(Annotations.NullableFormatString):
                    return new ParameterAnnotationInfo(parameter.Name, isFormatString: true, canBeNull: true);

                case nameof(Annotations.Some):
                    return new ParameterAnnotationInfo(parameter.Name);

                case nameof(Annotations.NotNull):
                    return new ParameterAnnotationInfo(parameter.Name, isNotNull: true);

                case nameof(Annotations.CanBeNull):
                    return new ParameterAnnotationInfo(parameter.Name, canBeNull: true);

                default:
                    throw new ArgumentException($"Expression '{expression}' call an unsupported method : {methodName}. {usageInfo}",
                        nameof(expression));
            }
        }

        private static bool IsMethodCallOnSpecialClass(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            return methodCallExpression?.Method.DeclaringType == typeof (Annotations);
        }

        private static MethodCallExpression AssertCallOnSpecialClass(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException($"Expression '{expression}' isn't a method call. {usageInfo}", nameof(expression));
            }

            if (typeof(Annotations) != methodCallExpression.Method.DeclaringType)
            {
                throw new ArgumentException($"Expression '{expression}' doesn't call a method on the correct type. {usageInfo}",
                    nameof(expression));
            }
            return methodCallExpression;
        }

        [CanBeNull]
        [SuppressMessage("ReSharper", "RedundantArgumentNameForLiteralExpression")]
        private static MemberAnnotationInfo ExtractMember(MethodCallExpression methodCallExpression)
        {
            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(Annotations.Some):
                    return new MemberAnnotationInfo();

                case nameof(Annotations.NotNull):
                    return new MemberAnnotationInfo(isNotNull: true);

                case nameof(Annotations.CanBeNull):
                    return new MemberAnnotationInfo(canBeNull: true);

                default:
                    throw new ArgumentException($"Expression '{methodCallExpression}' call an unsupported method : {methodName}. {usageInfo}",
                        nameof(methodCallExpression));
            }
        }
    }
}