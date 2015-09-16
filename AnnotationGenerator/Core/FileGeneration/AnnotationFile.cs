using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Core.FileGeneration
{
    public class AnnotationFile
    {
        [NotNull]
        public string FileNameAlongDll { get; }

        [NotNull]
        public string FileNameInNuGet { get; }

        [NotNull]
        public XDocument Content { get; }

        public AnnotationFile([NotNull] string fileNameAlongDll, [NotNull] string fileNameInNuGet,
            [NotNull] XDocument content)
        {
            if (fileNameAlongDll == null) throw new ArgumentNullException(nameof(fileNameAlongDll));
            if (fileNameInNuGet == null) throw new ArgumentNullException(nameof(fileNameInNuGet));
            if (content == null) throw new ArgumentNullException(nameof(content));

            FileNameAlongDll = fileNameAlongDll;
            FileNameInNuGet = fileNameInNuGet;
            Content = content;
        }
    }
}