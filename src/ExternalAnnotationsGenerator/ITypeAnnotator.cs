using System;
using System.Linq.Expressions;

namespace ExternalAnnotationsGenerator
{
    public interface ITypeAnnotator<TClass> : IFluentInterface
    {
        void Annotate<TResult>(Expression<Func<TClass, TResult>> expression);
        void Annotate(Expression<Action<TClass>> expression);
    }
}