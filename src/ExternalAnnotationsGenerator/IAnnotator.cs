using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator
{
    public interface IAnnotator : IFluentInterface
    {
        void Annotate<TType>([NotNull] Action<ITypeAnnotator<TType>> annotationActions);
        void Annotate([NotNull] Expression<Action> expression);
        void Annotate<TResult>([NotNull] Expression<Func<TResult>> expression);
    }
}