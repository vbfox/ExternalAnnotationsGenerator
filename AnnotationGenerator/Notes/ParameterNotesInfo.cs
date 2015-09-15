using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using static AnnotationGenerator.Notes.ResharperXmlBuilder;

namespace AnnotationGenerator.Notes
{
    public class ParameterNotesInfo : INote
    {
        public string ParameterName { get; }
        public bool IsFormatString { get; }
        public bool IsNotNull { get; }
        public bool CanBeNull { get; }

        public ParameterNotesInfo([NotNull] string parameterName, bool isFormatString = false, bool isNotNull = false, bool canBeNull = false)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            ParameterName = parameterName;
            IsFormatString = isFormatString;
            IsNotNull = isNotNull;
            CanBeNull = canBeNull;
        }

        public IEnumerable<XElement> GetAttributesXml()
        {
            if (IsFormatString)
            {
                yield return BuilStringFormatMethodAttribute(ParameterName);
            }

            if (IsNotNull || CanBeNull)
            {
                var element = BuildParameterElement(ParameterName);
                if (IsNotNull)
                {
                    element.Add(BuilNotNullAttribute());
                }
                if (CanBeNull)
                {
                    element.Add(BuilCanBeNullAttribute());
                }
                yield return element;
            }
        }
    }
}
