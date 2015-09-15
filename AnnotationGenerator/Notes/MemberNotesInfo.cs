using System.Collections.Generic;
using System.Xml.Linq;
using static AnnotationGenerator.Notes.ResharperXmlBuilder;

namespace AnnotationGenerator.Notes
{
    class MemberNotesInfo : INote
    {
        public bool IsNotNull { get; }
        public bool CanBeNull { get; }

        public MemberNotesInfo(bool isNotNull, bool canBeNull)
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
