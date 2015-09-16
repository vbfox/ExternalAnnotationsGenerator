using System;
using System.IO;
using ExternalAnnotationsGenerator.Core;
using ExternalAnnotationsGenerator.Core.FileGeneration;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator
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

        public static void CreateNugetPackage([NotNull] this IAnnotator annotator, NugetSpec spec,
            DirectoryInfo directory)
        {
            var annotationFiles = CoreHelper.GetAnnotations(annotator).GenerateFiles();
            NuGetGeneration.CreateNugetPackage(spec, annotationFiles, directory);
        }
    }
}