using AnnotationGenerator.Construction;

namespace AnnotationGenerator
{
    public static class Annotator
    {
        public static IAnnotator Create()
        {
            return new AnnotationsBuilder();
        }
    }
}