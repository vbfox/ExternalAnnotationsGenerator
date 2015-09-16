using System;
using System.Collections.Generic;
using System.Linq;
using AnnotationGenerator.Construction;
using AnnotationGenerator.FileGeneration;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public static class AnnotatorExtensions
    {
        public static IEnumerable<AnnotationFile> GenerateFiles([NotNull] this IAnnotator annotator)
        {
            if (annotator == null) throw new ArgumentNullException(nameof(annotator));
            var builder = annotator as AnnotationsBuilder;
            if (builder == null) throw new ArgumentException("Expected a builder", nameof(annotator));

            return builder.GetAnnotations().Select(a => new AnnotationFileGenerator(a).Generate());
        }
    }
}
