using System;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public interface IAnnotator : IFluentInterface
    {
        void AnnotateType<TType>([NotNull] Action<ITypeAnnotator<TType>> annotationActions);
    }
}