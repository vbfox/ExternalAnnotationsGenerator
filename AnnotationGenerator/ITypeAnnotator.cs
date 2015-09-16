using System;
using System.Linq.Expressions;

namespace AnnotationGenerator
{
    public interface ITypeAnnotator<TClass> : IFluentInterface
    {
        void Annotate<TResult>(Expression<Func<TClass, TResult>> expression);
        void Annotate(Expression<Action<TClass>> expression);
    }
}