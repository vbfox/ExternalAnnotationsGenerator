using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator
{
    public interface IAnnotator : IFluentInterface
    {
        void AnnotateType<TType>([NotNull] Action<ITypeAnnotator<TType>> annotationActions);
        void AnnotateStatic([NotNull] Expression<Action> expression);
        void AnnotateStatic<TResult>([NotNull] Expression<Func<TResult>> expression);
    }
}