using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public class AnnotationFile
    {
        [NotNull]
        public string FileName { get; }

        [NotNull]
        public XDocument Content { get; }

        public AnnotationFile([NotNull] string fileName, [NotNull] XDocument content)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (content == null) throw new ArgumentNullException(nameof(content));

            FileName = fileName;
            Content = content;
        }
    }

    public class AnnotationFileGenerator
    {
        private readonly AssemblyAnnotations annotations;

        public AnnotationFileGenerator([NotNull] AssemblyAnnotations annotations)
        {
            if (annotations == null) throw new ArgumentNullException(nameof(annotations));

            this.annotations = annotations;
        }

        [NotNull]
        public AnnotationFile Generate()
        {
            return new AnnotationFile("", new XDocument());
        }
    }

    public class Annotator : FluentInterface
    {
        readonly List<AssemblyAnnotations> annotations = new List<AssemblyAnnotations>();

        public void AnnotateAssemblyContaining<T>(Action<AssemblyAnnotator<T>> annotationActions)
        {
            var annotator = new AssemblyAnnotator<T>();
            annotationActions(annotator);
            annotations.Add(annotator.GetAnnotations());
        }

        public IEnumerable<AnnotationFile> GenerateFiles()
        {
            return annotations.Select(a => new AnnotationFileGenerator(a).Generate());
        }

        /*
        internal IEnumerable<XDocument> GetDocuments()
        {
            return documents;
        }

        internal void AddDocument(XDocument document)
        {
            documents.Add(document);
        }

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