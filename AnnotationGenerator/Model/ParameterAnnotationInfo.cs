using System;
using System.Collections.Generic;
using System.Xml.Linq;
using AnnotationGenerator.FileGeneration;
using JetBrains.Annotations;

namespace AnnotationGenerator.Model
{
    internal class ParameterAnnotationInfo : IAnnotationInfo
    {
        public string ParameterName { get; }
        public bool IsFormatString { get; }
        public bool IsNotNull { get; }
        public bool CanBeNull { get; }

        public ParameterAnnotationInfo([NotNull] string parameterName, bool isFormatString = false, bool isNotNull = false, bool canBeNull = false)
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
                yield return ResharperXmlBuilder.BuilStringFormatMethodAttribute(ParameterName);
            }

            if (IsNotNull || CanBeNull)
            {
                var element = ResharperXmlBuilder.BuildParameterElement(ParameterName);
                if (IsNotNull)
                {
                    element.Add(ResharperXmlBuilder.BuilNotNullAttribute());
                }
                if (CanBeNull)
                {
                    element.Add(ResharperXmlBuilder.BuilCanBeNullAttribute());
                }
                yield return element;
            }
        }
    }
}
