using System;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Notes;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    internal static class NotesInfoExtractor
    {
        private static readonly string usageInfo = $"Should be a call on one of the methods of {nameof(ParameterNotes)}.";

        [CanBeNull]
        public static ParameterNotesInfo ExtractParameter(Expression expression, ParameterInfo parameter)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(ParameterNotes.FormatString):
                    return new ParameterNotesInfo(parameter.Name, true, true, false);

                case nameof(ParameterNotes.NullableFormatString):
                    return new ParameterNotesInfo(parameter.Name, true, false, false);

                case nameof(ParameterNotes.Some):
                    return new ParameterNotesInfo(parameter.Name, false, false, false);

                case nameof(ParameterNotes.NotNull):
                    return new ParameterNotesInfo(parameter.Name, false, true, false);

                case nameof(ParameterNotes.CanBeNull):
                    return new ParameterNotesInfo(parameter.Name, false, false, true);

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
        public static MemberNotesInfo ExtractMember(Expression expression, MemberInfo member)
        {
            var methodCallExpression = AssertCallOnSpecialClass(expression);

            var methodName = methodCallExpression.Method.Name;
            switch (methodName)
            {
                case nameof(ParameterNotes.Some):
                    return new MemberNotesInfo(false, false);

                case nameof(ParameterNotes.NotNull):
                    return new MemberNotesInfo(true, false);

                case nameof(ParameterNotes.CanBeNull):
                    return new MemberNotesInfo(false, true);

                default:
                    throw new ArgumentException($"Expression '{expression}' call an unsupported method : {methodName}. {usageInfo}",
                        nameof(expression));
            }
        }
    }
}