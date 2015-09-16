using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public class Annotator : FluentInterface, IAnnotatorAnnotations
    {
        readonly List<AssemblyAnnotations> annotations = new List<AssemblyAnnotations>();

        public IEnumerable<AnnotationFile> GenerateFiles()
        {
            return annotations.Select(a => new AnnotationFileGenerator(a).Generate());
        }

        IEnumerable<AssemblyAnnotations> IAnnotatorAnnotations.GetAnnotations()
        {
            return annotations;
        }

        private AssemblyAnnotations GetAssemblyAnnotations(Assembly assembly)
        {
            var existing = annotations.FirstOrDefault(m => m.Assembly == assembly);
            if (existing != null)
            {
                return existing;
            }

            var newAnnotations = new AssemblyAnnotations(assembly);
            annotations.Add(newAnnotations);
            return newAnnotations;
        }

        public void AnnotateType<TType>([NotNull] Action<MemberAnnotator<TType>> annotationActions)
        {
            if (annotationActions == null) throw new ArgumentNullException(nameof(annotationActions));

            var memberAnnotator = new MemberAnnotator<TType>();
            annotationActions(memberAnnotator);

            var assemblyAnnotations = GetAssemblyAnnotations(typeof (TType).Assembly);
            assemblyAnnotations.AddRange(memberAnnotator.GetMembersAnnotations());
        }

        /*
        public void CreateNugetPackage(NugetSpec spec)
        {
            var directory = Path.Combine(spec.Id, "ReSharper", "vAny", "annotations");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            WriteSpecFile(spec);
            WriteAnnotationFiles(spec);
        }

        private static void WriteSpecFile(NugetSpec spec)
        {
            var specFilename = spec.Id + "." + spec.Version + ".nuspec";
            var specFilePath = Path.Combine(spec.Id, specFilename);

            using (var writer = new XmlTextWriter(specFilePath, Encoding.UTF8) {Formatting = Formatting.Indented})
            {
                spec.GetXml().WriteTo(writer);
            }
        }

        private void WriteAnnotationFiles(NugetSpec spec)
        {
            foreach (var document in documents)
            {
                var assemblyName = document.XPathSelectElement("/assembly").Attribute("name").Value;
                var filename = assemblyName + ".xml";                

                var path = Path.Combine(spec.Id, "ReSharper", "vAny", "annotations", filename);

                using (var writer = new XmlTextWriter(path, Encoding.UTF8) {Formatting = Formatting.Indented})
                {
                    document.WriteTo(writer);
                }
            }
        }
        */
    }
}