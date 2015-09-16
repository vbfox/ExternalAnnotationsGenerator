using ExternalAnnotationsGenerator.Core.Construction;

namespace ExternalAnnotationsGenerator
{
    public static class Annotator
    {
        public static IAnnotator Create()
        {
            return new AnnotationsBuilder();
        }
    }
}