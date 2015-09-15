using System.Collections.Generic;
using System.Xml.Linq;

namespace AnnotationGenerator.Notes
{
    public interface IAnnotationInfo
    {
        IEnumerable<XElement> GetAttributesXml();
    }
}