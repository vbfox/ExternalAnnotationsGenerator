using System.Collections.Generic;
using System.Xml.Linq;
using static AnnotationGenerator.AnnotationXml.ResharperXmlBuilder;

namespace AnnotationGenerator.Notes
{
    class MemberAnnotationInfo : IAnnotationInfo
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
                yield return BuilNotNullAttribute();
            }
            if (CanBeNull)
            {
                yield return BuilCanBeNullAttribute();
            }
        }
    }
}
