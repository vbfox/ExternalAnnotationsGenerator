using System;
using System.Linq.Expressions;

namespace ExternalAnnotationsGenerator
{
    public interface ITypeAnnotator<TType> : IFluentInterface
    {
        void Annotate<TResult>(Expression<Func<TType, TResult>> expression);
        void Annotate(Expression<Action<TType>> expression);
    }
}