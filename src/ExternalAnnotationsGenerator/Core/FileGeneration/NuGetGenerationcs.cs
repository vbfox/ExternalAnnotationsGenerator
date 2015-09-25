using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ExternalAnnotationsGenerator.Core.FileGeneration
{
    public static class NuGetGeneration
    {
        public static void CreateNugetPackage(NugetSpec spec, IEnumerable<AnnotationFile> annotationFiles, DirectoryInfo directory)
        {
            var annotationsDir = new DirectoryInfo(Path.Combine(directory.FullName, "DotFiles", "Extensions", spec.Id, "annotations"));

            if (!annotationsDir.Exists)
            {
                annotationsDir.Create();
            }

            WriteSpecFile(spec, directory);
            WriteAnnotationFiles(annotationFiles, annotationsDir);
        }

        private static void WriteAnnotationFiles(IEnumerable<AnnotationFile> annotationFiles, DirectoryInfo annotationsDir)
        {
            foreach (var file in annotationFiles)
            {
                var path = Path.Combine(annotationsDir.FullName, file.FileNameInNuGet);

                using (var writer = new XmlTextWriter(path, Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    file.Content.WriteTo(writer);
                }
            }
        }

        private static void WriteSpecFile(NugetSpec spec, DirectoryInfo packageDir)
        {
            var specFilename = $"{spec.Id}.{spec.Version}.nuspec";
            var specFilePath = Path.Combine(packageDir.FullName, specFilename);

            using (var writer = new XmlTextWriter(specFilePath, Encoding.UTF8) {Formatting = Formatting.Indented})
            {
                spec.GetXml().WriteTo(writer);
            }
        }
    }
}
