using System;
using System.IO;
using AnnotationGenerator.Core;
using AnnotationGenerator.Core.FileGeneration;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public static class AnnotatorExtensions
    {
        public static void SaveToDirectory([NotNull] this IAnnotator annotator, DirectoryInfo directory)
        {
            CoreHelper.GetAnnotations(annotator).GenerateFiles().SaveToDirectory(directory);
        }

        public static void SaveAlongAssemblies([NotNull] this IAnnotator annotator)
        {
            foreach (var annotation in CoreHelper.GetAnnotations(annotator))
            {
                var directoryName = Path.GetDirectoryName(annotation.Assembly.Location);
                if (directoryName == null)
                {
                    throw new InvalidOperationException(
                        $"Can't get assembly directory for {annotation.Assembly.FullName}");
                }

                var dir = new DirectoryInfo(directoryName);
                annotation.GenerateFile().SaveToDirectory(dir);
            }
        }


    }
}