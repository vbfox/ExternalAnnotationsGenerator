using System;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator
{
    public interface IAnnotator : IFluentInterface
    {
        void AnnotateType<TType>([NotNull] Action<ITypeAnnotator<TType>> annotationActions);
    }
}