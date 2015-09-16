using System.Collections.Generic;

namespace AnnotationGenerator
{
    public interface IAnnotatorAnnotations
    {
        IEnumerable<AssemblyAnnotations> GetAnnotations();
    }
}