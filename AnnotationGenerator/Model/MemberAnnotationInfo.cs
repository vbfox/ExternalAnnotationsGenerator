using System.Collections.Generic;
using System.Xml.Linq;
using AnnotationGenerator.FileGeneration;

namespace AnnotationGenerator.Model
{
    internal class MemberAnnotationInfo : IAnnotationInfo
    {
        public bool IsNotNull { get; }
        public bool CanBeNull { get; }

        public MemberAnnotationInfo(bool isNotNull = false, bool canBeNull = false)
        {
            IsNotNull = isNotNull;
            CanBeNull = canBeNull;
        }

        public IEnumerable<XElement> GetAttributesXml()
        {
            if (IsNotNull)
            {
                yield return ResharperXmlBuilder.BuilNotNullAttribute();
            }
            if (CanBeNull)
            {
                yield return ResharperXmlBuilder.BuilCanBeNullAttribute();
            }
        }
    }
}
