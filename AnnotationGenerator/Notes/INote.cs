using System.Collections.Generic;
using System.Xml.Linq;

namespace AnnotationGenerator.Notes
{
    public interface INote
    {
        IEnumerable<XElement> GetAttributesXml();
    }
}