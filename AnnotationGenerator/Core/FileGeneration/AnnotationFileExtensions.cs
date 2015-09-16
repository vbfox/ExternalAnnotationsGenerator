using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnnotationGenerator.Core.Model;

namespace AnnotationGenerator.Core.FileGeneration
{
    public static class AnnotationFileExtensions
    {
        public static AnnotationFile GenerateFile(this AssemblyAnnotations assembly)
        {
            return new AnnotationFileGenerator(assembly).Generate();
        }

        public static IEnumerable<AnnotationFile> GenerateFiles(this IEnumerable<AssemblyAnnotations> assemblies)
        {
            return assemblies.Select(a => a.GenerateFile());
        }

        public static void SaveToDirectory(this IEnumerable<AnnotationFile> assemblies, DirectoryInfo directory, bool nuget = false)
        {
            foreach (var file in assemblies)
            {
                file.SaveToDirectory(directory, nuget);
            }
        }

        public static void SaveToDirectory(this AnnotationFile annotationFile, DirectoryInfo directory, bool nuget = false)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }

            var fileName = nuget ? annotationFile.FileNameInNuGet : annotationFile.FileNameAlongDll;
            annotationFile.Content.Save(Path.Combine(directory.FullName, fileName));
        }
    }
}
