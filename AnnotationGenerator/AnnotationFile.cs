using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AnnotationGenerator
{
    public class AnnotationFile
    {
        [NotNull]
        public string FileNameAlongDll { get; }

        [NotNull]
        public string FileNameInNuget { get; }

        [NotNull]
        public XDocument Content { get; }

        public AnnotationFile([NotNull] string fileNameAlongDll, [NotNull] string fileNameInNuget,
            [NotNull] XDocument content)
        {
            if (fileNameAlongDll == null) throw new ArgumentNullException(nameof(fileNameAlongDll));
            if (fileNameInNuget == null) throw new ArgumentNullException(nameof(fileNameInNuget));
            if (content == null) throw new ArgumentNullException(nameof(content));

            FileNameAlongDll = fileNameAlongDll;
            FileNameInNuget = fileNameInNuget;
            Content = content;
        }
    }
}