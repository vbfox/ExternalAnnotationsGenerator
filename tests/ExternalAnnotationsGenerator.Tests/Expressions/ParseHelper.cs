using System;
using System.Linq.Expressions;
using ExternalAnnotationsGenerator.Core.Construction;

namespace ExternalAnnotationsGenerator.Tests.Expressions
{
    static class ParseHelper
    {
        public static ExpressionParsingResult Parse(Expression<Action> expression) => ExpressionParser.Parse(expression);
        public static ExpressionParsingResult Parse<TIn>(Expression<Action<TIn>> expression) => ExpressionParser.Parse(expression);
        public static ExpressionParsingResult Parse<TOut>(Expression<Func<TOut>> expression) => ExpressionParser.Parse(expression);
        public static ExpressionParsingResult Parse<TIn, TOut>(Expression<Func<TIn, TOut>> expression) => ExpressionParser.Parse(expression);
    }
}
