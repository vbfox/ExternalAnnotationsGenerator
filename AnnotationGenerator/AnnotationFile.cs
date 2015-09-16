using System;
using System.Xml.Linq;
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
}